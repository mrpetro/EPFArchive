using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EPF
{

    internal struct EPFHeaderBlock
    {
       internal char[] Signature;
       internal UInt32 FATOffset;
       internal byte Unknown;
       internal UInt16 NumberOfFiles;
    };

    internal struct EPFEntryBlock
    {
       internal string Filename;
       internal bool CompressionFlag;
       internal Int32 CompressedSize;
       internal Int32 DecompressedSize;
    };


    public class EPFArchiveReader : IDisposable
    {
        private readonly BinaryReader m_BinReader;

        private LZWDecompressor m_Decompressor = null;

        internal LZWDecompressor Decompressor
        {
            get
            {
                if (m_Decompressor == null)
                    m_Decompressor = new LZWDecompressor(14);

                return m_Decompressor;
            }
        }

        internal BinaryReader BinReader { get { return m_BinReader; } }

        public EPFArchiveReader(Stream stream)
        {
            m_BinReader = new BinaryReader(stream);
        }

        public void Dispose()
        {
            m_BinReader.Dispose();
        }
    }
}
