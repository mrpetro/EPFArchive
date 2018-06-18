using System;
using System.IO;

namespace EPF
{
    public class EPFArchiveEntryForRead : EPFArchiveEntry
    {
        #region Private Fields

        private readonly long _ArchiveDataPos;

        #endregion Private Fields

        #region Internal Constructors

        internal EPFArchiveEntryForRead(EPFArchive archive, long dataPos) :
            base(archive)
        {
            _ArchiveDataPos = dataPos;
        }

        #endregion Internal Constructors

        #region Public Properties

        public override bool ToRemove
        {
            get
            {
                return false;
            }

            set
            {
                throw new InvalidOperationException("Entry for reading cannot be marked to remove");
            }
        }

        #endregion Public Properties

        #region Internal Properties

        internal long ArchiveDataPos { get { return _ArchiveDataPos; } }

        #endregion Internal Properties

        #region Public Methods

        public override void Close()
        {
            //TODO: Clean up here if temporary file was created
        }

        /// <summary>
        /// This function will open entry stream in read-only mode
        /// It copies entry data (or decompresses) from EPF archvie to temporary file
        /// Then opens this file and returns it's stream
        /// </summary>
        /// <returns>Stream of entry</returns>
        public override Stream Open()
        {
            ThrowIfInvalidArchive();

            Archive.ArchiveReader.BaseStream.Position = ArchiveDataPos;

            string tempFilePath = Path.GetTempFileName();

            using (FileStream fs = new FileStream(tempFilePath, FileMode.Open, FileAccess.Write, FileShare.None, 4096, FileOptions.None))
            {
                fs.SetLength(Length);

                if (IsCompressed)
                    Archive.Decompressor.Decompress(Archive.ArchiveReader.BaseStream, fs);
                else
                    fs.Write(Archive.ArchiveReader.ReadBytes(Length), 0, Length);
            }

            return new FileStream(tempFilePath, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.DeleteOnClose);
        }

        #endregion Public Methods

        #region Internal Methods

        internal override void WriteData(BinaryWriter writer)
        {
            throw new InvalidOperationException("Unable to write EPFArchiveEntry in Read mode.");
        }

        #endregion Internal Methods
    }
}