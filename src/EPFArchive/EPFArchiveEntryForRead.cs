using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EPF
{
    public class EPFArchiveEntryForRead : EPFArchiveEntry
    {
        private readonly long m_ArchiveDataPos;

        internal long ArchiveDataPos { get { return m_ArchiveDataPos; } }

        internal EPFArchiveEntryForRead(EPFArchive archive, long dataPos) :
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

            return new FileStream(tempFilePath, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.DeleteOnClose);
        }

        internal override void WriteData()
        {
            throw new InvalidOperationException("Unable to write EPFArchiveEntry in Read mode.");
        }

        public override void Close()
        {
        }
    }
}
