using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EPF
{
    class EPFArchiveEntryForUpdate : EPFArchiveEntry
    {
        private long m_ArchiveDataPos;

        internal long ArchiveDataPos { get { return m_ArchiveDataPos; } }

        internal EPFArchiveEntryForUpdate(EPFArchive archive, long dataPos) :
            base(archive)
        {
            m_ArchiveDataPos = dataPos;
        }

        public override Stream Open()
        {
            ThrowIfInvalidArchive();

            Archive.ArchiveReader.BaseStream.Position = ArchiveDataPos;

            string tempFilePath = Path.GetTempFileName();

            using (FileStream fs = new FileStream(tempFilePath, FileMode.Open, FileAccess.Write, FileShare.None, 4096, FileOptions.None))
            {
                fs.SetLength(Lenght);

                if (IsCompressed)
                    Archive.Decompressor.Decompress(Archive.ArchiveReader.BaseStream, fs);
                else
                    fs.Write(Archive.ArchiveReader.ReadBytes(Lenght), 0, Lenght);
            }

            OpenedStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);

            return OpenedStream;

        }

        internal override void WriteData()
        {
            Archive.ArchiveReader.BaseStream.Position = m_ArchiveDataPos;

            m_ArchiveDataPos = Archive.ArchiveWriter.BinWriter.BaseStream.Position;

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
                    m_Length = (int)OpenedStream.Length;
                    m_CompressedLength = (int)Archive.ArchiveWriter.BinWriter.BaseStream.Position - (int)m_ArchiveDataPos;
                }
                else
                {
                    //Don't dispose reader because it will also close the stream.
                    BinaryReader reader = new BinaryReader(OpenedStream);
                    int newLength = (int)OpenedStream.Length;
                    Archive.ArchiveWriter.BinWriter.Write(reader.ReadBytes(newLength), 0, newLength);
                    m_Length = newLength;
                    m_CompressedLength = m_Length;
                }
            }

        }

        public override void Close()
        {

        }
    }
}
