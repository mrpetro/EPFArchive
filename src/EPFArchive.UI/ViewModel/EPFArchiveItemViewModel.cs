namespace EPF.UI.ViewModel
{
    public enum EPFArchiveItemStatus
    {
        Unchanged,
        Modifying,
        Adding,
        Removing,
        Replacing
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

        #endregion Private Fields

        #region Public Constructors

        public EPFArchiveItemViewModel(EPFArchiveEntry entry, EPFArchiveItemStatus status)
        {
            _entry = entry;

            _entry.PropertyChanged += _entry_PropertyChanged;
            PropertyChanged += EPFArchiveItemViewModel_PropertyChanged;

            Name = entry.Name;
            Status = status;

            IsCompressed = entry.ToCompress;
            Length = entry.Length;
            CompressedLength = entry.CompressedLength;

            RecalculateCompressionRatio();


        }

        #endregion Public Constructors

        #region Public Properties

        public int CompressedLength
        {
            get { return _compressedLength; }
            set { SetProperty(ref _compressedLength, value); }
        }

        public float CompressionRatio
        {
            get { return _compressionRatio; }
            set { SetProperty(ref _compressionRatio, value); }
        }

        public bool IsCompressed
        {
            get { return _isCompressed; }
            set { SetProperty(ref _isCompressed, value); }
        }

        public int Length
        {
            get { return _length; }
            set { SetProperty(ref _length, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public EPFArchiveItemStatus Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        #endregion Public Properties

        #region Private Methods

        private void _entry_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_entry.IsModified):
                    Status = _entry.IsModified ? EPFArchiveItemStatus.Modifying : EPFArchiveItemStatus.Unchanged;
                    Length = _entry.Length;
                    CompressedLength = _entry.CompressedLength;
                    break;

                default:
                    break;
            }
        }

        private void RecalculateCompressionRatio()
        {
            CompressionRatio = (float)CompressedLength / (float)Length;
        }

        private void EPFArchiveItemViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IsCompressed):
                    _entry.ToCompress = IsCompressed;
                    break;
                case nameof(CompressedLength):
                case nameof(Length):
                    RecalculateCompressionRatio();
                    break;
                default:
                    break;
            }
        }

        #endregion Private Methods
    }
}