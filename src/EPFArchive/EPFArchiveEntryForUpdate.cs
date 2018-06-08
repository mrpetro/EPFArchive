using System.IO;
using System.Text;

namespace EPF
{
    internal class EPFArchiveEntryForUpdate : EPFArchiveEntry
    {
        #region Private Fields

        private long _archiveDataPos;
        private Stream _openedStream;

        #endregion Private Fields

        #region Internal Constructors

        internal EPFArchiveEntryForUpdate(EPFArchive archive, long dataPos) :
            base(archive)
        {
            _archiveDataPos = dataPos;
        }

        #endregion Internal Constructors

        #region Public Methods

        public override void Close()
        {
            if (_openedStream != null)
            {
                _openedStream.Dispose();
                _openedStream = null;
            }
        }

        public override Stream Open()
        {
            ThrowIfInvalidArchive();

            Archive.ArchiveReader.BaseStream.Position = _archiveDataPos;

            string tempFilePath = Path.GetTempFileName();

            using (FileStream fs = new FileStream(tempFilePath, FileMode.Open, FileAccess.Write, FileShare.None, 4096, FileOptions.None))
            {
                fs.SetLength(Length);

                if (IsCompressed)
                    Archive.Decompressor.Decompress(Archive.ArchiveReader.BaseStream, fs);
                else
                    fs.Write(Archive.ArchiveReader.ReadBytes(Length), 0, Length);
            }

            _openedStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);

            return _openedStream;
        }

        #endregion Public Methods

        #region Internal Methods

        internal override void WriteData(BinaryWriter writer)
        {
            Archive.ArchiveReader.BaseStream.Position = _archiveDataPos;

            _archiveDataPos = writer.BaseStream.Position;

            //Entry was never opened so it will be copied from original
            if (_openedStream == null)
                writer.Write(Archive.ArchiveReader.ReadBytes(CompressedLength), 0, CompressedLength);
            else
            {
                _openedStream.Seek(0, SeekOrigin.Begin);

                if (IsCompressed)
                {
                    //Compress entry data while storing it in to archive
                    Archive.Compressor.Compress(_openedStream, writer.BaseStream);
                    //Update entry normal and compressed data lengths
                    Length = (int)_openedStream.Length;
                    CompressedLength = (int)writer.BaseStream.Position - (int)_archiveDataPos;
                }
                else
                {
                    int newLength = (int)_openedStream.Length;

                    using (var reader = new BinaryReader(_openedStream, Encoding.UTF8, true))
                        writer.Write(reader.ReadBytes(newLength), 0, newLength);

                    Length = newLength;
                    CompressedLength = Length;
                }
            }
        }

        #endregion Internal Methods
    }
}