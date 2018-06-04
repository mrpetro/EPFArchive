using System;
using System.IO;
using System.Text;

namespace EPF
{
    public abstract class EPFArchiveEntry
    {
        #region Internal Fields

        internal Stream OpenedStream;

        #endregion Internal Fields

        #region Protected Constructors

        protected EPFArchiveEntry(EPFArchive archive)
        {
            Archive = archive;
            IsCompressed = true;
        }

        #endregion Protected Constructors

        #region Public Properties

        public EPFArchive Archive { get; private set; }
        public int CompressedLength { get; protected set; }
        public bool IsCompressed { get; protected set; }
        public int Length { get; protected set; }
        public string Name { get; protected set; }

        #endregion Public Properties

        #region Public Methods

        public abstract void Close();

        public abstract Stream Open();

        #endregion Public Methods

        #region Internal Methods

        internal void ReadInfo(BinaryReader reader)
        {
            Name = Encoding.ASCII.GetString(reader.ReadBytes(13)).Split(new char[] { '\0' })[0];
            IsCompressed = reader.ReadBoolean();
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