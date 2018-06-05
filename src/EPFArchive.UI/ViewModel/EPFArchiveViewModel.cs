using System;
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
            Locked = true;
            IsArchiveOpened = false;
            IsArchiveSaveAllowed = false;
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

        public void ExtractAll(string folderPath)
        {
            StartWork(InternalExtractAll, folderPath);
        }

        public void ExtractSelected(string folderPath)
        {
            StartWork(InternalExtractSelected, folderPath);
        }

        public void Save()
        {
            SaveAs(ArchiveFilePath);
        }

        public void SaveAs(string filePath)
        {
            if (_epfArchive == null)
                throw new InvalidOperationException("EPF Archive not opened!");

            if (IsArchiveSaveAllowed == false)
                throw new InvalidOperationException("EPF Archive save not allowed!");

            throw new NotImplementedException("Save");
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
            SaveAs(ArchiveFilePath);
            return true;
        }

        public bool TrySaveAs()
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

        #endregion Public Methods

        #region Private Methods

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
            Status.TotalItems = 0;
            Status.ItemsSelected = 0;
        }

        private void InternalExtractAll(object argument)
        {
            try
            {
                Locked = false;
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
                Locked = true;
            }
        }

        public void TryExtractAll()
        {
            throw new NotImplementedException();
        }

        public void TryExtractSelection()
        {
            throw new NotImplementedException();
        }

        public void TryAddEntries()
        {
            try
            {
                if (!IsArchiveOpened)
                    return;

                if (!IsArchiveSaveAllowed)
                    throw new InvalidOperationException("Archive opened in read-only mode.");

                //TODO: Fill that
            }
            catch (InvalidOperationException ex)
            {
                Status.Log.Warning($"Unable to add new entries. {ex.Message}");
            }
            catch (Exception ex)
            {
                Status.Log.Error($"Unhandled exception: {ex}");
            }
        }

        public void TryMarkSelectedEntriesToRemove()
        {
            try
            {
                if (!IsArchiveOpened)
                    return;

                if (!IsArchiveSaveAllowed)
                    throw new InvalidOperationException("Archive opened in read-only mode.");

                foreach (var item in Entries.Where(item => item.IsSelected))
                {
                    if (item.Status == EPFArchiveItemStatus.Unchanged || item.Status == EPFArchiveItemStatus.Modifying)
                        item.Status = EPFArchiveItemStatus.Removing;
                }

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

        private void InternalExtractSelected(object argument)
        {
            try
            {
                Locked = false;
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
                Locked = true;
            }
        }

        private void OpenArchive(string archiveFilePath)
        {
            try
            {
                var fileStream = File.Open(archiveFilePath, FileMode.Open, FileAccess.ReadWrite);
                _epfArchive = new EPFArchive(fileStream, EPFArchiveMode.Update);

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