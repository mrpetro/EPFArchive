using System;
using System.IO;
using System.Text;

namespace EPF
{
    public struct EPFEntryState
    {
        #region Public Properties

        public bool IsCompressed { get; set; }
        public bool IsRemoved { get; set; }
        public string Name { get; set; }

        #endregion Public Properties
    }

    public abstract class EPFArchiveEntry
    {
        #region Private Fields

        #endregion Private Fields

        #region Protected Constructors

        protected EPFArchiveEntry(EPFArchive archive)
        {
            Archive = archive;
        }

        #endregion Protected Constructors

        #region Public Properties

        private bool _toRemove;
        internal bool _isCompressed;
        private bool _ToCompress;

        public EPFArchive Archive { get; private set; }
        public int CompressedLength { get; protected set; }
        public bool IsCompressed
        {
            get
            {
                return _ToCompress;
            }

            set
            {
                _ToCompress = value;

                Modify = (_isCompressed != _ToCompress);
            }
        }
        public int Length { get; protected set; }
        public bool Modify { get; set; }
        public string Name { get; protected set; }

        public virtual bool ToRemove
        {
            get
            {
                return _toRemove;
            }

            set
            {
                _toRemove = value;

                if(_toRemove)
                    Modify = true;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public abstract void Close();

        public void ExtractTo(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                throw new InvalidOperationException($"Directory '{folderPath}' doesn't exist.");

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
            _isCompressed = _ToCompress = reader.ReadBoolean();
            CompressedLength = reader.ReadInt32();
            Length = reader.ReadInt32();
        }

        internal abstract void WriteData(BinaryWriter writer);

        internal void WriteInfo(BinaryWriter writer)
        {
            writer.Write(Encoding.ASCII.GetBytes(Name.PadRight(13, '\0')));
            writer.Write(IsCompressed);
            writer.Write(CompressedLength);
            writer.Write(Length);

            _isCompressed = _ToCompress;
            Modify = false;
        }

        #endregion Internal Methods

        #region Protected Methods

        protected void ThrowIfInvalidArchive()
        {
            if (Archive == null)
                throw new InvalidOperationException("Entry has been deleted");
            Archive.ThrowIfDisposed();
        }

        #endregion Protected Methods
    }
}