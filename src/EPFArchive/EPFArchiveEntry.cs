using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace EPF
{
    public abstract class EPFArchiveEntry
    {
        private EPFArchive m_Archive;
        private string m_Name;
        protected bool m_IsCompressed = true;
        protected int m_CompressedLength = 0;
        protected int m_Length = 0;
        internal Stream OpenedStream;

        public EPFArchive Archive { get { return m_Archive; } }
        public bool IsCompressed { get { return m_IsCompressed; } }
        public int CompressedLength { get { return m_CompressedLength; } }
        public int Lenght { get { return m_Length; } }
        public string Name { get { return m_Name; } }

        protected EPFArchiveEntry(EPFArchive archive)
        {
            m_Archive = archive;
        }

        internal void ReadInfo(BinaryReader reader)
        {
            m_Name = Encoding.ASCII.GetString(reader.ReadBytes(13)).Split(new char[] { '\0' })[0];
            m_IsCompressed = reader.ReadBoolean();
            m_CompressedLength = reader.ReadInt32();
            m_Length= reader.ReadInt32();
        }

        internal void WriteInfo(BinaryWriter writer)
        {
            writer.Write(Encoding.ASCII.GetBytes(m_Name.PadRight(13,'\0')));
            writer.Write(m_IsCompressed);
            writer.Write(m_CompressedLength);
            writer.Write(m_Length);
        }

        protected void ThrowIfInvalidArchive()
        {
            if (m_Archive == null)
                throw new InvalidOperationException("Entry has been deleted");
            m_Archive.ThrowIfDisposed();
        }

        public abstract Stream Open();
        internal abstract void WriteData();
        public abstract void Close();
    }
}
