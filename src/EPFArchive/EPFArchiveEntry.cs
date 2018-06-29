using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace EPF
{
    public abstract class EPFArchiveEntry : INotifyPropertyChanged
    {
        #region Protected Fields

        protected bool isCompressed;

        #endregion Protected Fields

        #region Private Fields

        private bool _isModified;
        private bool _toCompress;

        #endregion Private Fields

        #region Protected Constructors

        protected EPFArchiveEntry(EPFArchive archive)
        {
            if (archive == null)
                throw new ArgumentNullException(nameof(archive));

            Archive = archive;

            PropertyChanged += EPFArchiveEntry_PropertyChanged;
        }

        #endregion Protected Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public EPFArchive Archive { get; private set; }

        public int CompressedLength { get; protected set; }

        public bool IsModified
        {
            get
            {
                return _isModified;
            }

            protected set
            {
                if (_isModified == value)
                    return;

                _isModified = value;

                PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsModified)));
            }
        }

        public int Length { get; protected set; }
        public string Name { get; protected set; }

        public virtual bool ToCompress
        {
            get
            {
                return _toCompress;
            }

            set
            {
                if (_toCompress == value)
                    return;

                _toCompress = value;

                PropertyChanged(this, new PropertyChangedEventArgs(nameof(ToCompress)));
            }
        }

        #endregion Public Properties

        #region Public Methods

        public abstract void Close();

        public void ExtractTo(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                throw new InvalidOperationException($"Directory '{folderPath}' doesn't exists.");

            using (var entryStream = Open())
            {
                var outFilePath = Path.Combine(folderPath, Name);
                using (var outFile = File.Create(outFilePath))
                    entryStream.CopyTo(outFile);
            }
        }

        public abstract Stream Open();

        #endregion Public Methods

        #region Internal Methods

        internal void ReadInfo(BinaryReader reader)
        {
            Name = Encoding.ASCII.GetString(reader.ReadBytes(13)).Split(new char[] { '\0' })[0];
            isCompressed = _toCompress = reader.ReadBoolean();
            CompressedLength = reader.ReadInt32();
            Length = reader.ReadInt32();
        }

        internal abstract void WriteData(BinaryWriter writer);

        internal void WriteInfo(BinaryWriter writer)
        {
            writer.Write(Encoding.ASCII.GetBytes(Name.PadRight(13, '\0')));
            writer.Write(ToCompress);
            writer.Write(CompressedLength);
            writer.Write(Length);

            isCompressed = ToCompress;
            IsModified = false;
            Archive.RaiseEntryChanged(this, EntryChangedEventType.Stored);
        }

        #endregion Internal Methods

        #region Protected Methods

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        protected void ThrowIfInvalidArchive()
        {
            if (Archive == null)
                throw new InvalidOperationException("Entry has been deleted");
            Archive.ThrowIfDisposed();
        }

        #endregion Protected Methods

        #region Private Methods

        private void EPFArchiveEntry_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ToCompress):
                    IsModified = (isCompressed != ToCompress);
                    break;
                case nameof(IsModified):
                    Archive.ModifiedEntryiesNo += IsModified ? 1 : -1;
                    break;
                default:
                    break;
            }
        }

        #endregion Private Methods
    }
}