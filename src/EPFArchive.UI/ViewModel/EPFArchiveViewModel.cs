using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;

namespace EPF.UI.ViewModel
{
    public class EPFArchiveViewModel : BaseViewModel
    {
        #region Private Fields

        private const string APP_NAME = "EPF Archive";
        private string _appLabel = APP_NAME;
        private string _archiveFilePath;
        private EPFArchive _epfArchive;
        private bool _isArchiveModified;
        private bool _isArchiveOpened;
        private bool _isArchiveSaveAllowed;
        private bool _isReadOnly;
        private bool _locked;

        #endregion Private Fields

        #region Public Constructors

        public EPFArchiveViewModel(IDialogProvider dialogProvider)
        {
            if (dialogProvider == null)
                throw new ArgumentNullException(nameof(dialogProvider));

            DialogProvider = dialogProvider;

            Status = new StatusViewModel();
            SelectedEntries = new BindingList<EPFArchiveItemViewModel>();
            Entries = new BindingList<EPFArchiveItemViewModel>();
            Locked = false;
            IsArchiveOpened = false;
            IsReadOnly = true;
            IsArchiveSaveAllowed = false;
            SelectedEntries.ListChanged += (s, e) => { Status.ItemsSelected = SelectedEntries.Count; };
            Entries.ListChanged += (s, e) => { Status.TotalItems = Entries.Count; };
            PropertyChanged += EPFArchiveViewModel_PropertyChanged;
        }

        #endregion Public Constructors

        #region Public Properties

        public string AppLabel
        {
            get { return _appLabel; }
            internal set { SetProperty(ref _appLabel, value); }
        }

        public string ArchiveFilePath
        {
            get { return _archiveFilePath; }
            internal set { SetProperty(ref _archiveFilePath, value); }
        }

        public BindingList<EPFArchiveItemViewModel> Entries { get; private set; }

        public bool IsArchiveModified
        {
            get { return _isArchiveModified; }
            internal set { SetProperty(ref _isArchiveModified, value); }
        }

        public bool IsArchiveOpened
        {
            get { return _isArchiveOpened; }
            internal set { SetProperty(ref _isArchiveOpened, value); }
        }

        /// <summary>
        /// This flag determines if archive is opened in Read-Only mode or Read/Write mode
        /// </summary>
        public bool IsArchiveSaveAllowed
        {
            get { return _isArchiveSaveAllowed; }
            internal set { SetProperty(ref _isArchiveSaveAllowed, value); }
        }

        /// <summary>
        /// This flag determines if archive is opened in Read-Only mode or Read/Write mode
        /// </summary>
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            internal set { SetProperty(ref _isReadOnly, value); }
        }

        /// <summary>
        /// This flag is used for locking UI functionality when performing time consuming tasks
        /// </summary>
        public bool Locked
        {
            get { return _locked; }
            internal set { SetProperty(ref _locked, value); }
        }

        public BindingList<EPFArchiveItemViewModel> SelectedEntries { get; private set; }

        public StatusViewModel Status { get; private set; }

        #endregion Public Properties

        #region Internal Properties

        internal IDialogProvider DialogProvider { get; private set; }

        #endregion Internal Properties

        #region Public Methods

        public void DeselectAll()
        {
            SelectedEntries.RaiseListChangedEvents = false;
            SelectedEntries.Clear();
            SelectedEntries.RaiseListChangedEvents = true;
            SelectedEntries.ResetBindings();
        }

        public void InvertSelection()
        {
            var unselectedEntries = Entries.Where(item => !SelectedEntries.Contains(item)).ToList();

            SelectedEntries.RaiseListChangedEvents = false;

            SelectedEntries.Clear();

            foreach (var entry in unselectedEntries)
                SelectedEntries.Add(entry);

            SelectedEntries.RaiseListChangedEvents = true;
            SelectedEntries.ResetBindings();
        }

        public void RefreshEntries()
        {
            if (_epfArchive == null)
                throw new InvalidOperationException("EPF archive not opened!");

            Entries.RaiseListChangedEvents = false;

            Entries.Clear();

            foreach (var entry in _epfArchive.Entries)
                Entries.Add(new EPFArchiveItemViewModel(entry, EPFArchiveItemStatus.Unchanged));

            Entries.RaiseListChangedEvents = true;
            Entries.ResetBindings();
        }

        public void Save(object argument)
        {
            try
            {
                if (_epfArchive == null)
                    throw new InvalidOperationException("EPF Archive not opened!");

                if (IsArchiveSaveAllowed == false)
                    throw new InvalidOperationException("EPF Archive save not allowed!");

                Locked = true;

                Status.Progress.Value = 0;
                _epfArchive.SaveProgress += _epfArchive_SaveProgress;
                Status.Progress.Visible = true;

                _epfArchive.Save();
            }
            catch (Exception ex)
            {
                Status.Log.Error($"Unable to save entries. Reason: {ex.Message}");
            }
            finally
            {
                _epfArchive.SaveProgress -= _epfArchive_SaveProgress;
                Status.Progress.Visible = false;
                Locked = false;
                IsArchiveModified = false;
            }
        }

        public void SaveAs(string filePath)
        {
            if (_epfArchive == null)
                throw new InvalidOperationException("EPF Archive not opened!");

            if (IsArchiveSaveAllowed == false)
                throw new InvalidOperationException("EPF Archive save not allowed!");

            throw new NotImplementedException("SaveAs");
        }

        public void SelectAll()
        {
            SelectedEntries.RaiseListChangedEvents = false;

            foreach (var entry in Entries)
            {
                if (SelectedEntries.Contains(entry))
                    continue;

                SelectedEntries.Add(entry);
            }

            SelectedEntries.RaiseListChangedEvents = true;
            SelectedEntries.ResetBindings();
        }

        public void TryAddEntries()
        {
            try
            {
                if (!IsArchiveOpened)
                    return;

                if (!IsArchiveSaveAllowed)
                    throw new InvalidOperationException("Archive opened in read-only mode.");

                var fileDialog = DialogProvider.ShowOpenFileDialog("Select files to add to archive...",
                                                                   "All Files (*.*)|*.*",
                                                                   true);
                //Cancel adding files
                if (fileDialog.Answer != DialogAnswer.OK)
                {
                    Status.Log.Info($"Adding files canceled...");
                    return;
                }

                AddEntries(fileDialog.FileNames);
            }
            catch (Exception ex)
            {
                Status.Log.Error($"Unhandled exception: {ex}");
            }
        }

        public bool TryClose()
        {
            if (!IsArchiveOpened)
                return true;

            if (IsArchiveModified)
            {
                var answer = DialogProvider.ShowMessageWithQuestion("Archive have been modified. Do you want to save it before closing?", "Save modified archive before closing?", QuestionDialogButtons.YesNoCancel);

                if (answer == DialogAnswer.Cancel)
                {
                    Status.Log.Info("Archive closing canceled.");
                    return false;
                }
                else if (answer == DialogAnswer.Yes)
                {
                    if (!TrySave())
                        return false;
                }
            }

            Close();
            return true;
        }

        public void TryExtractAll()
        {
            var folderBrowser = DialogProvider.ShowFolderBrowserDialog("Choose folder to extract all entries...", null);

            if (folderBrowser.Answer == DialogAnswer.OK)
                StartWork(ExtractAll, folderBrowser.SelectedDirectory);
        }

        public void TryExtractSelection()
        {
            var folderBrowser = DialogProvider.ShowFolderBrowserDialog("Choose folder to extract selected entries...", null);

            if (folderBrowser.Answer == DialogAnswer.OK)
                StartWork(ExtractSelection, folderBrowser.SelectedDirectory);
        }

        public bool TryOpenArchive()
        {
            var fileDialog = DialogProvider.ShowOpenFileDialog("Select EPF Archive to Open...",
                                                               "East Point Software File (*.EPF)|*.EPF|All Files (*.*)|*.*",
                                                               false);

            //Cancel opening archive
            if (fileDialog.Answer != DialogAnswer.OK)
                return false;

            //Try close any archive that is already opened
            if (!TryClose())
                return false;

            OpenArchive(fileDialog.FileName);
            return true;
        }

        public bool TryOpenArchiveReadOnly()
        {
            var fileDialog = DialogProvider.ShowOpenFileDialog("Select EPF Archive to Open in Read-Only mode...",
                                                               "East Point Software File (*.EPF)|*.EPF|All Files (*.*)|*.*",
                                                               false);

            //Cancel opening archive
            if (fileDialog.Answer != DialogAnswer.OK)
                return false;

            //Try close any archive that is already opened
            if (!TryClose())
                return false;

            OpenArchiveReadOnly(fileDialog.FileName);
            return true;
        }

        public void TryRemoveSelectedEntries()
        {
            try
            {
                if (!IsArchiveOpened)
                    return;

                if (!IsArchiveSaveAllowed)
                    throw new InvalidOperationException("Archive opened in read-only mode.");

                var entriesToDelete = SelectedEntries.ToList();

                foreach (var entry in entriesToDelete)
                    _epfArchive.RemoveEntry(entry.Name);
            }
            catch (InvalidOperationException ex)
            {
                Status.Log.Warning($"Unable to remove any entries. {ex.Message}");
            }
            catch (Exception ex)
            {
                Status.Log.Error($"Unhandled exception: {ex}");
            }
        }

        public bool TrySave()
        {
            try
            {
                StartWork(Save);

                return true;
            }
            catch (Exception ex)
            {
                Status.Log.Error($"Unable to save archive. Reason: {ex.Message}");
            }

            return false;
        }

        public bool TrySaveAs()
        {
            try
            {
                var initialDirectory = Path.GetDirectoryName(ArchiveFilePath);
                var initialFileName = Path.GetFileName(ArchiveFilePath);

                var fileDialog = DialogProvider.ShowSaveFileDialog("Choose file name to save EPF archive...",
                                                                   "East Point Software File (*.EPF)|*.EPF|All Files (*.*)|*.*",
                                                                   initialDirectory,
                                                                   initialFileName);
                //Cancel saving archive
                if (fileDialog.Answer != DialogAnswer.OK)
                    return false;

                var newFilePath = fileDialog.FileName;

                SaveAs(newFilePath);

                return true;
            }
            catch (Exception ex)
            {
                Status.Log.Error($"Unable to save archive. Reason: {ex.Message}");
            }

            return false;
        }

        #endregion Public Methods

        #region Private Methods

        private void EPFArchiveViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IsArchiveModified):

                    break;
                default:
                    break;
            }
        }

        private void _epfArchive_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_epfArchive.IsModified):
                    IsArchiveModified = _epfArchive.IsModified;
                    break;
                default:
                    break;
            }
        }

        private void _epfArchive_EntryChanged(object sender, EntryChangedEventArgs e)
        {
            switch (e.EventType)
            {
                case EntryChangedEventType.Added:
                    {
                        Entries.Add(new EPFArchiveItemViewModel(e.Entry, EPFArchiveItemStatus.Adding));
                        Status.Log.Info($"Entry '{e.Entry.Name}' added.");
                    }
                    break;
                case EntryChangedEventType.Removed:
                    {
                        var entry = Entries.FirstOrDefault(item => item.Name == e.Entry.Name);
                        Entries.Remove(entry);
                        Status.Log.Info($"Entry '{e.Entry.Name}' removed.");
                    }
                    break;
                case EntryChangedEventType.Replaced:
                    {
                        var entry = Entries.FirstOrDefault(item => item.Name == e.Entry.Name);
                        var entryIndex = Entries.IndexOf(entry);
                        Entries[entryIndex] = new EPFArchiveItemViewModel(e.Entry, EPFArchiveItemStatus.Replacing);
                        Status.Log.Info($"Entry '{e.Entry.Name}' replaced.");
                    }
                    break;
                //case EntryChangedEventType.Stored:
                //    {
                //        var entry = Entries.FirstOrDefault(item => item.Name == e.Entry.Name);
                //        var entryIndex = Entries.IndexOf(entry);
                //        Entries[entryIndex] = new EPFArchiveItemViewModel(e.Entry, EPFArchiveItemStatus.Unchanged);
                //    }
                //    break;
                default:
                    break;
            }
        }

        private void _epfArchive_ExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            switch (e.EventType)
            {
                case ExtractProgressEventType.ExtractionStarted:
                    Status.Log.Info("Extraction started.");
                    break;

                case ExtractProgressEventType.ExtractionBeforeReadEntry:
                    Status.Log.Info($"Extracting Entry [{e.EntriesExtracted} of {e.EntriesTotal}] {e.CurrentEntry.Name}...");
                    break;

                case ExtractProgressEventType.ExtractionAfterReadEntry:
                    Status.Progress.Value = (int)(((double)e.EntriesExtracted / (double)e.EntriesTotal) * 100.0);
                    break;

                case ExtractProgressEventType.ExtractionCompleted:
                    Status.Log.Success("Extraction from archive completed.");
                    break;

                case ExtractProgressEventType.ExtractionEntryBytesWrite:
                    break;

                default:
                    break;
            }
        }

        private void _epfArchive_SaveProgress(object sender, SaveProgressEventArgs e)
        {
            switch (e.EventType)
            {
                case SaveProgressEventType.SavingStarted:
                    Status.Log.Info("Saving archive started.");
                    break;

                case SaveProgressEventType.SavingBeforeWriteEntry:
                    Status.Log.Info($"Saving Entry [{e.EntriesSaved} of {e.EntriesTotal}] {e.CurrentEntry.Name}...");
                    break;

                case SaveProgressEventType.SavingAfterWriteEntry:
                    Status.Progress.Value = (int)(((double)e.EntriesSaved / (double)e.EntriesTotal) * 100.0);
                    break;

                case SaveProgressEventType.SavingCompleted:
                    Status.Log.Success("Saving to archive completed.");
                    break;

                case SaveProgressEventType.SavingEntryBytesRead:
                    break;

                default:
                    break;
            }
        }

        private void AddEntries(ICollection<string> filePaths)
        {
            bool alwaysReplace = false;

            foreach (var filePath in filePaths)
            {
                try
                {
                    var entryName = Path.GetFileName(filePath);
                    entryName = EPFArchive.ValidateEntryName(entryName);

                    var entry = Entries.FirstOrDefault(item => item.Name == entryName);

                    if (entry == null)
                    {
                        var epfEntry = _epfArchive.CreateEntry(entryName, filePath);

                    }
                    else
                    {
                        bool replace = false;

                        if (alwaysReplace)
                            replace = true;
                        else
                        {
                            //TODO: Create better WinForms dialog for this purpose
                            var replaceAnswer = DialogProvider.ShowReplaceFileQuestion($"Entry '{entryName}' already exists. Do you want to replace it?", "Entry already exists.");

                            if (replaceAnswer == DialogAnswer.Yes)
                                replace = true;
                            else if (replaceAnswer == DialogAnswer.No)
                                replace = false;
                            else if (replaceAnswer == DialogAnswer.Cancel)
                                break;
                            //else if(replaceAnswer == DialogAnswe.All)
                            //{
                            //    replace = true;
                            //    alwaysReplace = true;
                            //}
                        }

                        if (replace)
                            _epfArchive.ReplaceEntry(entryName, filePath);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    Status.Log.Warning($"Unable to add new entry '{filePath}'. {ex.Message}");
                }
            }
        }


        private void Close()
        {
            if (_epfArchive == null)
                throw new InvalidOperationException("EPF Archive not opened!");

            _epfArchive.Dispose();
            _epfArchive = null;

            Entries.Clear();

            Status.Log.Success($"Archive '{ Path.GetFileName(ArchiveFilePath)}' closed.");
            ArchiveFilePath = null;
            AppLabel = $"{APP_NAME}";

            IsArchiveOpened = false;
            IsArchiveSaveAllowed = false;
            IsReadOnly = true;
            Status.ItemsSelected = 0;
        }

        private void ExtractAll(object argument)
        {
            try
            {
                Locked = true;
                var folderPath = argument as string;

                if (!Directory.Exists(folderPath))
                    throw new Exception("Directory doesn't exists.");

                Status.Progress.Value = 0;
                _epfArchive.ExtractProgress += _epfArchive_ExtractProgress;
                Status.Progress.Visible = true;
                _epfArchive.ExtractAll(folderPath);
            }
            catch (Exception ex)
            {
                Status.Log.Error($"Unable to extract entries. Reason: {ex.Message}");
            }
            finally
            {
                _epfArchive.ExtractProgress -= _epfArchive_ExtractProgress;
                Status.Progress.Visible = false;
                Locked = false;
            }
        }

        private void ExtractSelection(object argument)
        {
            try
            {
                Locked = true;
                var folderPath = argument as string;

                if (!Directory.Exists(folderPath))
                    throw new Exception("Directory doesn't exists.");

                var selectedEntryNames = SelectedEntries.Select(item => item.Name).ToList();

                Status.Progress.Value = 0;
                _epfArchive.ExtractProgress += _epfArchive_ExtractProgress;
                Status.Progress.Visible = true;
                _epfArchive.ExtractEntries(folderPath, selectedEntryNames);
            }
            catch (Exception ex)
            {
                Status.Log.Error($"Unable to extract entries. Reason: {ex.Message}");
            }
            finally
            {
                _epfArchive.ExtractProgress -= _epfArchive_ExtractProgress;
                Status.Progress.Visible = false;
                Locked = false;
            }
        }

        private void OpenArchive(string archiveFilePath)
        {
            try
            {
                var fileStream = File.Open(archiveFilePath, FileMode.Open, FileAccess.ReadWrite);
                _epfArchive = new EPFArchive(fileStream, EPFArchiveMode.Update);
                _epfArchive.EntryChanged += _epfArchive_EntryChanged;
                _epfArchive.PropertyChanged += _epfArchive_PropertyChanged;

                RefreshEntries();

                ArchiveFilePath = archiveFilePath;
                AppLabel = $"{APP_NAME} - {ArchiveFilePath}";
                IsReadOnly = false;
                IsArchiveOpened = true;
                IsArchiveSaveAllowed = true;
                Status.Log.Success($"Archive '{ Path.GetFileName(ArchiveFilePath)}' opened.");
            }
            catch (Exception ex)
            {
                Status.Log.Error($"Unable to open archive. Reason: {ex.Message}");
            }
        }

        private void OpenArchiveReadOnly(string archiveFilePath)
        {
            try
            {
                var fileStream = File.Open(archiveFilePath, FileMode.Open, FileAccess.Read);
                _epfArchive = new EPFArchive(fileStream, EPFArchiveMode.Read);

                RefreshEntries();

                ArchiveFilePath = archiveFilePath;
                AppLabel = $"{APP_NAME} - {ArchiveFilePath} (Read-Only)";
                IsArchiveOpened = true;
                IsReadOnly = true;
                IsArchiveSaveAllowed = false;
                Status.Log.Success($"Archive '{ Path.GetFileName(ArchiveFilePath)}' opened in read-only mode.");
            }
            catch (Exception ex)
            {
                Status.Log.Error($"Unable to open archive in read-only mode. Reason: {ex.Message}");
            }
        }

        private void StartWork(WaitCallback function, object argument)
        {
            ThreadPool.QueueUserWorkItem(function, argument);
        }

        private void StartWork(WaitCallback function)
        {
            ThreadPool.QueueUserWorkItem(function);
        }

        #endregion Private Methods
    }
}