using System;
using System.IO;
using System.Text;

namespace EPF
{

    public enum EPFEntryAction
    {
        Nothing = 0,
        Add = 1,
        Remove = 2,
        Compress = 4,
        Decompress = 8
    }

    public abstract class EPFArchiveEntry
    {
        #region Private Fields

        #endregion Private Fields

        #region Protected Constructors

        protected EPFArchiveEntry(EPFArchive archive)
        {
            if (archive == null)
                throw new ArgumentNullException(nameof(archive));

            Archive = archive;
        }

        #endregion Protected Constructors

        #region Public Properties

        public EPFArchive Archive { get; private set; }
        public int CompressedLength { get; protected set; }
        public bool IsCompressed { get; private set; }

        public int Length { get; protected set; }

        public EPFEntryAction Action { get; set; }

        public string Name { get; protected set; }

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
            IsCompressed = reader.ReadBoolean();
            CompressedLength = reader.ReadInt32();
            Length = reader.ReadInt32();
        }

        internal abstract void WriteData(BinaryWriter writer);

        internal void WriteInfo(BinaryWriter writer)
        {
            if (Action.HasFlag(EPFEntryAction.Compress))
                IsCompressed = true;
            else if (Action.HasFlag(EPFEntryAction.Decompress))
                IsCompressed = false;

            writer.Write(Encoding.ASCII.GetBytes(Name.PadRight(13, '\0')));
            writer.Write(IsCompressed);
            writer.Write(CompressedLength);
            writer.Write(Length);

            Action = EPFEntryAction.Nothing;
        }

        #endregion Internal Methods

        #region Protected Methods

        protected void ThrowIfInvalidArchive()
        {
            if (Archive == null)
                throw new InvalidOperationException("Entry has been deleted");
            Archive.ThrowIfDisposed();
        }

        public EPFArchiveEntry Replace(string filePath)
        {
            return Archive.ReplaceEntry(this, filePath);
        }

        #endregion Protected Methods
    }
}