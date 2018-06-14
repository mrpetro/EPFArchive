using System;
using System.IO;

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
        private EPFArchiveEntry _entry;
        private EPFArchiveItemStatus _status;
        private bool _isCompressed;
        private int _length;
        private string _name;

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


            PropertyChanged += EPFArchiveItemViewModel_PropertyChanged;
        }

        private void EPFArchiveItemViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsCompressed))
            {
                if (Status == EPFArchiveItemStatus.Unchanged || Status == EPFArchiveItemStatus.Modifying)
                    Status = _entry.IsCompressed == IsCompressed ? EPFArchiveItemStatus.Unchanged : EPFArchiveItemStatus.Modifying;
            }
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

        internal void TryRemove()
        {
            _entry.ToRemove = true;
        }

        internal void TryModify()
        {
            _entry.IsCompressed = IsCompressed;
            _entry.Modify = true;
        }

        #endregion Public Properties
    }
}