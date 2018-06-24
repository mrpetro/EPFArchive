namespace EPF.UI.ViewModel
{
    public enum EPFArchiveItemStatus
    {
        Unchanged,
        Modifying,
        Adding,
        Removing
    }

    public class EPFArchiveItemViewModel : BaseViewModel
    {
        #region Private Fields

        private int _compressedLength;
        private float _compressionRatio;
        private EPFArchiveEntry _entry;
        private bool _isCompressed;
        private int _length;
        private string _name;
        private EPFArchiveItemStatus _status;
        private bool _toRemove;

        #endregion Private Fields

        #region Public Constructors

        public EPFArchiveItemViewModel(EPFArchiveEntry entry)
        {
            _entry = entry;

            Name = entry.Name;

            if (entry is EPFArchiveEntryForCreate)
                Status = EPFArchiveItemStatus.Adding;
            else
                Status = EPFArchiveItemStatus.Unchanged;

            IsCompressed = entry.IsCompressed;
            Length = entry.Length;
            CompressedLength = entry.CompressedLength;
            CompressionRatio = (float)CompressedLength / (float)Length;

            PropertyChanged += EPFArchiveItemViewModel_PropertyChanged;
        }

        #endregion Public Constructors

        #region Public Properties

        public int CompressedLength
        {
            get
            {
                return _compressedLength;
            }

            set
            {
                if (_compressedLength == value)
                    return;

                _compressedLength = value;
                OnPropertyChanged(nameof(CompressedLength));
            }
        }

        public float CompressionRatio
        {
            get
            {
                return _compressionRatio;
            }

            set
            {
                if (_compressionRatio == value)
                    return;

                _compressionRatio = value;
                OnPropertyChanged(nameof(CompressionRatio));
            }
        }

        public bool IsCompressed
        {
            get
            {
                return _isCompressed;
            }

            set
            {
                if (_isCompressed == value)
                    return;

                _isCompressed = value;
                OnPropertyChanged(nameof(IsCompressed));
            }
        }

        public int Length
        {
            get
            {
                return _length;
            }

            set
            {
                if (_length == value)
                    return;

                _length = value;
                OnPropertyChanged(nameof(Length));
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name == value)
                    return;

                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public EPFArchiveItemStatus Status
        {
            get
            {
                return _status;
            }

            set
            {
                if (_status == value)
                    return;

                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public bool ToRemove
        {
            get
            {
                return _toRemove;
            }

            set
            {
                if (_toRemove == value)
                    return;

                _toRemove = value;
                OnPropertyChanged(nameof(ToRemove));
            }
        }

        #endregion Public Properties

        #region Internal Methods

        internal void ReplaceWith(string filePath)
        {
            _entry = _entry.Replace(filePath);
            Status = EPFArchiveItemStatus.Modifying;
        }

        #endregion Internal Methods

        #region Private Methods

        private void EPFArchiveItemViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IsCompressed):
                    _entry.Action = IsCompressed ? EPFEntryAction.Compress : EPFEntryAction.Decompress;
                    Status = _entry.Action != EPFEntryAction.Nothing ? EPFArchiveItemStatus.Modifying : EPFArchiveItemStatus.Unchanged;
                    break;

                case nameof(ToRemove):
                    _entry.Action = EPFEntryAction.Remove;
                    Status = EPFArchiveItemStatus.Removing;
                    break;

                default:
                    break;
            }
        }

        #endregion Private Methods
    }
}