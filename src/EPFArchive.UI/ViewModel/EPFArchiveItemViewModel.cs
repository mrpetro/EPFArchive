using System;
using System.IO;

namespace EPF.UI.ViewModel
{
    public enum EPFArchiveItemStatus
    {
        Unchanged,
        Modified,
        Deleted,
        Added
    }

    public class EPFArchiveItemViewModel : BaseViewModel
    {
        #region Private Fields

        private int _compressedLength;
        private EPFArchiveEntry _entry;
        private EPFArchiveItemStatus _status;
        private bool _isCompressed;
        private bool _isSelected;
        private int _length;
        private string _name;

        #endregion Private Fields

        #region Public Constructors

        public EPFArchiveItemViewModel(EPFArchiveEntry entry)
        {
            _entry = entry;

            Name = entry.Name;
            Status = EPFArchiveItemStatus.Unchanged;
            IsCompressed = entry.IsCompressed;
            Length = entry.Lenght;
            CompressedLength = entry.CompressedLength;
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

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                if (_isSelected == value)
                    return;

                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
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

        internal void ExtractTo(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                throw new InvalidOperationException($"Directory {folderPath} doesn't exist.");

            using (var entryStream = _entry.Open())
            {
                var outFilePath = Path.Combine(folderPath, Name);
                using (var outFile = File.Create(outFilePath))
                    entryStream.CopyTo(outFile);
            }
        }

        #endregion Public Properties
    }
}