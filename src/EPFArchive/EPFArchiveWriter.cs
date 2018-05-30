using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EPF
{
    class EPFArchiveWriter : IDisposable
    {
        private readonly BinaryWriter m_BinWriter;

        private LZWCompressor m_Compressor = null;

        internal LZWCompressor Compressor
        {
            get
            {
                if (m_Compressor == null)
                    m_Compressor = new LZWCompressor(14);

                return m_Compressor;
            }
        }

        internal BinaryWriter BinWriter { get { return m_BinWriter; } }

        public EPFArchiveWriter(Stream stream)
        {
            m_BinWriter = new BinaryWriter(stream);
        }

        internal void WriterHeaderBlock(EPFHeaderBlock headerBlock)
        {
            BinWriter.Write(headerBlock.Signature);
            BinWriter.Write(headerBlock.FATOffset);
            BinWriter.Write(headerBlock.Unknown);
            BinWriter.Write(headerBlock.NumberOfFiles);
        }

        internal void WriteEntryBlock(EPFEntryBlock entryBlock)
        {
            BinWriter.Write(entryBlock.Filename.PadRight(13, '\0').ToArray());
            BinWriter.Write(entryBlock.CompressionFlag);
            BinWriter.Write(entryBlock.CompressedSize);
            BinWriter.Write(entryBlock.DecompressedSize);
        }

        internal void WriteEntryData(Stream entryDataStream, bool compress)
        {
            if (compress)
                Compressor.Compress(entryDataStream, BinWriter.BaseStream);
            else
            {
                entryDataStream.Position = 0;
                BinaryReader binReader = new BinaryReader(entryDataStream);
                BinWriter.Write(binReader.ReadBytes((int)entryDataStream.Length));
            }
        }

        public void Dispose()
        {
            m_BinWriter.Dispose();
        }
    }
}
