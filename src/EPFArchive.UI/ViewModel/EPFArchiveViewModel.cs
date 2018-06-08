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
        private bool _locked;

        #endregion Private Fields

        #region Public Constructors

        public EPFArchiveViewModel(IDialogProvider dialogProvider)
        {
            if (dialogProvider == null)
                throw new ArgumentNullException(nameof(dialogProvider));

            DialogProvider = dialogProvider;

            Status = new StatusViewModel();
            Entries = new BindingList<EPFArchiveItemViewModel>();
            Locked = false;
            IsArchiveOpened = false;
            IsArchiveSaveAllowed = false;
            Entries.ListChanged += Entries_ListChanged;


        }

        private void _epfArchive_SaveProgress(object sender, SaveProgressEventArgs e)
        {
            switch (e.EventType)
            {
                case SaveProgressEventType.SavingStarted:
                    Status.Log.Info("Saving archive started.");
                    break;
                case SaveProgressEventType.SavingBeforeWriteEntry:
                    break;
                case SaveProgressEventType.SavingAfterWriteEntry:
                    Status.Log.Info($"Entry {e.CurrentEntry.Name} [{e.EntriesSaved} of {e.EntriesTotal}] saved.");
                    break;
                case SaveProgressEventType.SavingCompleted:
                    Status.Log.Success("Saving archive completed.");
                    break;
                case SaveProgressEventType.SavingEntryBytesRead:
                    break;
                default:
                    break;
            }
        }

        #endregion Public Constructors

        #region Public Properties

        public string AppLabel
        {
            get
            {
                return _appLabel;
            }

            internal set
            {
                if (_appLabel == value)
                    return;

                _appLabel = value;
                OnPropertyChanged(nameof(AppLabel));
            }
        }

        public string ArchiveFilePath
        {
            get
            {
                return _archiveFilePath;
            }

            internal set
            {
                if (_archiveFilePath == value)
                    return;

                _archiveFilePath = value;
                OnPropertyChanged(nameof(ArchiveFilePath));
            }
        }

        public BindingList<EPFArchiveItemViewModel> Entries { get; private set; }

        public bool IsArchiveModified
        {
            get
            {
                return _isArchiveModified;
            }

            internal set
            {
                if (_isArchiveModified == value)
                    return;

                _isArchiveModified = value;
                OnPropertyChanged(nameof(IsArchiveModified));
            }
        }

        public bool IsArchiveOpened
        {
            get
            {
                return _isArchiveOpened;
            }

            internal set
            {
                if (_isArchiveOpened == value)
                    return;

                _isArchiveOpened = value;
                OnPropertyChanged(nameof(IsArchiveOpened));
            }
        }

        /// <summary>
        /// This flag determines if archive is opene in Read-Only mode or Read/Write mode
        /// </summary>
        public bool IsArchiveSaveAllowed
        {
            get
            {
                return _isArchiveSaveAllowed;
            }

            internal set
            {
                if (_isArchiveSaveAllowed == value)
                    return;

                _isArchiveSaveAllowed = value;
                OnPropertyChanged(nameof(IsArchiveSaveAllowed));
            }
        }

        /// <summary>
        /// This flag is used for locking UI functionality when performing time consuming tasks
        /// </summary>
        public bool Locked
        {
            get
            {
                return _locked;
            }

            internal set
            {
                if (_locked == value)
                    return;

                _locked = value;
                OnPropertyChanged(nameof(Locked));
            }
        }

        public StatusViewModel Status { get; private set; }

        #endregion Public Properties

        #region Internal Properties

        internal IDialogProvider DialogProvider { get; private set; }

        #endregion Internal Properties

        #region Public Methods

        public void Save()
        {
            if (_epfArchive == null)
                throw new InvalidOperationException("EPF Archive not opened!");

            if (IsArchiveSaveAllowed == false)
                throw new InvalidOperationException("EPF Archive save not allowed!");

            foreach (var entry in Entries)
            {
                if(entry.Status == EPFArchiveItemStatus.Removing)
                    entry.TryRemove();
            }

            _epfArchive.Save();


            foreach (var entry in Entries)
                entry.Status = EPFArchiveItemStatus.Unchanged;
        }

        public void SaveAs(string filePath)
        {
            if (_epfArchive == null)
                throw new InvalidOperationException("EPF Archive not opened!");

            if (IsArchiveSaveAllowed == false)
                throw new InvalidOperationException("EPF Archive save not allowed!");

            throw new NotImplementedException("SaveAs");
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

            Close(false);
            return true;
        }

        public void TryExtractAll()
        {
            var folderBrowser = DialogProvider.ShowFolderBrowserDialog("Select folder to extract all entries...", null);

            if (folderBrowser.Answer == DialogAnswer.OK)
                StartWork(ExtractAll, folderBrowser.SelectedDirectory);
        }

        public void TryExtractSelection()
        {
            var folderBrowser = DialogProvider.ShowFolderBrowserDialog("Select folder to extract selected entries...", null);

            if (folderBrowser.Answer == DialogAnswer.OK)
                StartWork(ExtractSelection, folderBrowser.SelectedDirectory);
        }

        public void TryRemoveSelectedEntries()
        {
            try
            {
                if (!IsArchiveOpened)
                    return;

                if (!IsArchiveSaveAllowed)
                    throw new InvalidOperationException("Archive opened in read-only mode.");

                foreach (var item in Entries.Where(item => item.IsSelected))
                {
                    if (item.Status != EPFArchiveItemStatus.Removing)
                        item.Status = EPFArchiveItemStatus.Removing;
                }

                CheckIfArchiveModified();
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

        public bool TrySave()
        {
            try
            {
                Save();
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

        private void AddEntries(ICollection<string> filePaths)
        {
            foreach (var filePath in filePaths)
            {
                try
                {
                    var newEntry = _epfArchive.CreateEntry(filePath);
                    Entries.Add(new EPFArchiveItemViewModel(newEntry));
                }
                catch (InvalidOperationException ex)
                {
                    Status.Log.Warning($"Unable to add new entry '{filePath}'. {ex.Message}");
                }
            }

            CheckIfArchiveModified();
        }

        private void CheckIfArchiveModified()
        {
            IsArchiveModified = Entries.Any(item => item.Status != EPFArchiveItemStatus.Unchanged);
        }

        private void Close(bool saveChanges)
        {
            if (_epfArchive == null)
                throw new InvalidOperationException("EPF Archive not opened!");

            if (saveChanges)
                _epfArchive.Save();

            _epfArchive.SaveProgress -= _epfArchive_SaveProgress;
            _epfArchive.Dispose();
            _epfArchive = null;

            Entries.Clear();

            Status.Log.Success($"Archive '{ Path.GetFileName(ArchiveFilePath)}' closed.");
            ArchiveFilePath = null;
            AppLabel = $"{APP_NAME}";

            IsArchiveOpened = false;
            IsArchiveSaveAllowed = false;
            Status.TotalItems = 0;
            Status.ItemsSelected = 0;
        }

        private void Entries_ListChanged(object sender, ListChangedEventArgs e)
        {
        }

        private void ExtractAll(object argument)
        {
            try
            {
                Locked = true;
                var folderPath = argument as string;

                if (!Directory.Exists(folderPath))
                    throw new Exception("Directory doesn't exist.");

                Status.Progress.Value = 0;
                Status.Progress.Visible = true;
                int count = 0;
                foreach (var entry in Entries)
                {
                    Status.Log.Info($"Extracting [{count} of {Entries.Count}] {entry.Name}...");
                    entry.ExtractTo(folderPath);
                    count++;
                    Status.Progress.Value = (int)(((double)count / (double)Entries.Count) * 100.0);
                }

                Status.Log.Success($"Extraction finished.");

                Status.Progress.Visible = false;
            }
            catch (Exception ex)
            {
                Status.Log.Error($"Unable to extract entries. Reason: {ex.Message}");
            }
            finally
            {
                Locked = false;
            }
        }

        private void ExtractSelection(object argument)
        {
            try
            {
                Locked = true;
                var folderPath = argument as string;
                Status.Progress.Value = 0;
                Status.Progress.Visible = true;
                int count = 0;

                var selectedEntries = Entries.Where(item => item.IsSelected).ToList();

                foreach (var entry in selectedEntries)
                {
                    Status.Log.Info($"Extracting [{count} of {selectedEntries.Count}] {entry.Name}...");

                    entry.ExtractTo(folderPath);
                    count++;
                    Status.Progress.Value = (int)(((double)count / (double)selectedEntries.Count) * 100.0);
                }

                Status.Log.Success($"Extraction finished.");

                Status.Progress.Visible = false;
            }
            catch (Exception ex)
            {
                Status.Log.Error($"Unable to extract entries. Reason: {ex.Message}");
            }
            finally
            {
                Locked = false;
            }
        }

        private void OpenArchive(string archiveFilePath)
        {
            try
            {
                var fileStream = File.Open(archiveFilePath, FileMode.Open, FileAccess.ReadWrite);
                _epfArchive = new EPFArchive(fileStream, EPFArchiveMode.Update);
                _epfArchive.SaveProgress += _epfArchive_SaveProgress;

                ReadEntries();

                ArchiveFilePath = archiveFilePath;
                AppLabel = $"{APP_NAME} - {ArchiveFilePath}";
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

                ReadEntries();

                ArchiveFilePath = archiveFilePath;
                AppLabel = $"{APP_NAME} - {ArchiveFilePath}";
                IsArchiveOpened = true;
                IsArchiveSaveAllowed = false;
                Status.Log.Success($"Archive '{ Path.GetFileName(ArchiveFilePath)}' opened in read-only mode.");
            }
            catch (Exception ex)
            {
                Status.Log.Error($"Unable to open archive in read-only mode. Reason: {ex.Message}");
            }
        }

        private void ReadEntries()
        {
            if (_epfArchive == null)
                throw new InvalidOperationException("EPF archive not opened!");

            Entries.Clear();

            foreach (var entry in _epfArchive.Entries)
            {
                Entries.Add(new EPFArchiveItemViewModel(entry));
            }

            Status.TotalItems = _epfArchive.Entries.Count;
        }

        private void StartWork(WaitCallback function, object argument)
        {
            ThreadPool.QueueUserWorkItem(function, argument);
        }

        #endregion Private Methods
    }
}