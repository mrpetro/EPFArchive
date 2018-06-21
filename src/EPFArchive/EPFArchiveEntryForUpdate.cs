using System;
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

        /// <summary>
        /// This function will open entry stream in read-write mode
        /// It copies entry data (or decompresses) from EPF archvie to temporary file
        /// Then opens this file and returns it's stream
        /// </summary>
        /// <returns>Stream of entry</returns>
        public override Stream Open()
        {
            ThrowIfInvalidArchive();

            Archive.ArchiveReader.BaseStream.Position = _archiveDataPos;

            string tempFilePath = Path.GetTempFileName();

            using (var fs = new FileStream(tempFilePath, FileMode.Open, FileAccess.Write, FileShare.None, 4096, FileOptions.None))
            {
                fs.SetLength(Length);

                if (IsCompressed)
                    Archive.Decompressor.Decompress(Archive.ArchiveReader.BaseStream, fs);
                else
                {
                    int times = Length / 4096;
                    byte[] read = null;

                    for (int i = 0; i < times; i++)
                    {
                        read = Archive.ArchiveReader.ReadBytes(4096);
                        fs.Write(read, 0, 4096);
                    }

                    read = Archive.ArchiveReader.ReadBytes(Length % 4096);
                    fs.Write(read, 0, Length % 4096);
                }
            }

            _openedStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);

            return _openedStream;
        }

        #endregion Public Methods

        #region Internal Methods

        internal override void WriteData(BinaryWriter writer)
        {
            if (Action == EPFEntryAction.Remove)
                throw new InvalidOperationException("Writing entry marked for removing");

            if (_openedStream == null && Action != EPFEntryAction.Nothing)
                Open();

            Archive.ArchiveReader.BaseStream.Position = _archiveDataPos;

            _archiveDataPos = writer.BaseStream.Position;

            //Entry was never opened so it will be copied from original
            if (_openedStream == null)
            {
                var bytes = Archive.ArchiveReader.ReadBytes(CompressedLength);

                writer.Write(bytes, 0, CompressedLength);
            }
            else
            {
                _openedStream.Seek(0, SeekOrigin.Begin);

                if (Action.HasFlag(EPFEntryAction.Compress))
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