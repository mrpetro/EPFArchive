using System.IO;
using System.Text;

namespace EPF
{
    internal class EPFArchiveEntryForUpdate : EPFArchiveEntry
    {
        #region Private Fields

        private long _ArchiveDataPos;

        #endregion Private Fields

        #region Internal Constructors

        internal EPFArchiveEntryForUpdate(EPFArchive archive, long dataPos) :
            base(archive)
        {
            _ArchiveDataPos = dataPos;
        }

        #endregion Internal Constructors

        #region Internal Properties

        internal long ArchiveDataPos { get { return _ArchiveDataPos; } }

        #endregion Internal Properties

        #region Public Methods

        public override void Close()
        {
        }

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

            OpenedStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);

            return OpenedStream;
        }

        #endregion Public Methods

        #region Internal Methods

        internal override void WriteData(BinaryWriter writer)
        {
            Archive.ArchiveReader.BaseStream.Position = _ArchiveDataPos;

            _ArchiveDataPos = writer.BaseStream.Position;

            //Entry was never opened so it will be copied from original
            if (OpenedStream == null)
                writer.Write(Archive.ArchiveReader.ReadBytes(CompressedLength), 0, CompressedLength);
            else
            {
                OpenedStream.Seek(0, SeekOrigin.Begin);

                if (IsCompressed)
                {
                    //Compress entry data while storing it in to archive
                    Archive.Compressor.Compress(OpenedStream, writer.BaseStream);
                    //Update entry normal and compressed data lengths
                    Length = (int)OpenedStream.Length;
                    CompressedLength = (int)writer.BaseStream.Position - (int)_ArchiveDataPos;
                }
                else
                {
                    int newLength = (int)OpenedStream.Length;

                    using (var reader = new BinaryReader(OpenedStream, Encoding.UTF8, true))
                        writer.Write(reader.ReadBytes(newLength), 0, newLength);

                    Length = newLength;
                    CompressedLength = Length;
                }
            }
        }

        #endregion Internal Methods
    }
}