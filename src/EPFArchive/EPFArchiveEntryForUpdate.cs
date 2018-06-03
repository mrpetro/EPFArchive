using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EPF
{
    class EPFArchiveEntryForUpdate : EPFArchiveEntry
    {
        private long _ArchiveDataPos;

        internal long ArchiveDataPos { get { return _ArchiveDataPos; } }

        internal EPFArchiveEntryForUpdate(EPFArchive archive, long dataPos) :
            base(archive)
        {
            _ArchiveDataPos = dataPos;
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

        internal override void WriteData()
        {
            Archive.ArchiveReader.BaseStream.Position = _ArchiveDataPos;

            _ArchiveDataPos = Archive.ArchiveWriter.BinWriter.BaseStream.Position;

            //Entry was never opened so it will be copied from original
            if (OpenedStream == null)
                Archive.ArchiveWriter.BinWriter.Write(Archive.ArchiveReader.ReadBytes(CompressedLength), 0, CompressedLength);
            else
            {
                OpenedStream.Seek(0, SeekOrigin.Begin);

                if (IsCompressed)
                {
                    //Compress entry data while storing it in to archive
                    Archive.Compressor.Compress(OpenedStream, Archive.ArchiveWriter.BinWriter.BaseStream);
                    //Update entry normal and compressed data lengths
                    Length = (int)OpenedStream.Length;
                    CompressedLength = (int)Archive.ArchiveWriter.BinWriter.BaseStream.Position - (int)_ArchiveDataPos;
                }
                else
                {
                    int newLength = (int)OpenedStream.Length;

                    using (var reader = new BinaryReader(OpenedStream, Encoding.UTF8, true))
                        Archive.ArchiveWriter.BinWriter.Write(reader.ReadBytes(newLength), 0, newLength);

                    Length = newLength;
                    CompressedLength = Length;
                }
            }

        }

        public override void Close()
        {

        }
    }
}
