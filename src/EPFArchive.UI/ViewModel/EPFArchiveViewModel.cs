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
        private bool _locked;
        private EPFArchive _epfArchive;
        private bool _isArchiveOpened;
        private bool _isArchiveSaveAllowed;
        private int _itemsSelected;
        private string _message;
        private int _totalItems;

        #endregion Private Fields

        #region Public Constructors

        public EPFArchiveViewModel()
        {
            Entries = new BindingList<EPFArchiveItemViewModel>();
            Progress = new ProgressViewModel();
            Locked = true;
            IsArchiveOpened = false;
            IsArchiveSaveAllowed = false;

            Progress.Visible = false;
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

        public BindingList<EPFArchiveItemViewModel> Entries { get; private set; }

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

        public int ItemsSelected
        {
            get
            {
                return _itemsSelected;
            }

            set
            {
                if (_itemsSelected == value)
                    return;

                _itemsSelected = value;
                OnPropertyChanged(nameof(ItemsSelected));
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }

            internal set
            {
                if (_message == value)
                    return;

                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public ProgressViewModel Progress { get; private set; }

        public int TotalItems
        {
            get
            {
                return _totalItems;
            }

            internal set
            {
                if (_totalItems == value)
                    return;

                _totalItems = value;
                OnPropertyChanged(nameof(TotalItems));
            }
        }

        #endregion Public Properties

        #region Public Methods

        public void Close()
        {
            if (_epfArchive == null)
                throw new InvalidOperationException("EPF Archive not opened!");

            _epfArchive.Dispose();
            _epfArchive = null;

            Entries.Clear();

            Message = $"Archive '{ Path.GetFileName(ArchiveFilePath)}' closed.";
            ArchiveFilePath = null;
            AppLabel = $"{APP_NAME}";

            IsArchiveOpened = false;
            TotalItems = 0;
            ItemsSelected = 0;
        }

        public void ExtractAll(string folderPath)
        {
            StartWork(InternalExtractAll, folderPath);
        }

        public void ExtractSelected(string folderPath)
        {
            StartWork(InternalExtractSelected, folderPath);
        }

        public void OpenReadOnly(string archiveFilePath)
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
                Message = $"Archive '{ Path.GetFileName(ArchiveFilePath)}' opened.";
            }
            catch (Exception ex)
            {
                Message = $"Unable to open archive. Reason: {ex.Message}";
            }
        }

        public void Save()
        {
            throw new NotImplementedException("Save");
        }

        public void SaveAs(string filePath)
        {
            throw new NotImplementedException("SaveAs");
        }

        #endregion Public Methods

        #region Private Methods

        private void InternalExtractAll(object argument)
        {
            try
            {
                Locked = false;
                var folderPath = argument as string;

                if (!Directory.Exists(folderPath))
                    throw new Exception("Directory doesn't exist.");

                Progress.Value = 0;
                Progress.Visible = true;
                int count = 0;
                foreach (var entry in Entries)
                {
                    Message = $"Extracting [{count} of {Entries.Count}] {entry.Name}...";
                    entry.ExtractTo(folderPath);
                    count++;
                    Progress.Value = (int)(((double)count / (double)Entries.Count) * 100.0);
                }

                Message = $"Extraction finished.";
                Progress.Visible = false;
            }
            finally
            {
                Locked = true;
            }
        }

        private void InternalExtractSelected(object argument)
        {
            try
            {
                Locked = false;
                var folderPath = argument as string;
                Progress.Value = 0;
                Progress.Visible = true;
                int count = 0;
                foreach (var entry in Entries.Where(item => item.IsSelected))
                {
                    Message = $"Extracting [{count} of {Entries.Count}] {entry.Name}...";
                    entry.ExtractTo(folderPath);
                    count++;
                    Progress.Value = (int)(((double)count / (double)Entries.Count) * 100.0);
                }

                Message = $"Extraction finished.";
                Progress.Visible = false;
            }
            finally
            {
                Locked = true;
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

            TotalItems = _epfArchive.Entries.Count;
        }

        private void StartWork(WaitCallback function, object argument)
        {
            ThreadPool.QueueUserWorkItem(function, argument);
        }

        #endregion Private Methods
    }
}