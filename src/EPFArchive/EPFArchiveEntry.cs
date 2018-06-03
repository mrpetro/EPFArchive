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
        internal Stream OpenedStream;

        public EPFArchive Archive { get; private set; }
        public bool IsCompressed { get; protected set; }
        public int CompressedLength { get; protected set; }
        public int Length { get; protected set; }
        public string Name { get; protected set; }

        protected EPFArchiveEntry(EPFArchive archive)
        {
            Archive = archive;
            IsCompressed = true;
        }

        internal void ReadInfo(BinaryReader reader)
        {
            Name = Encoding.ASCII.GetString(reader.ReadBytes(13)).Split(new char[] { '\0' })[0];
            IsCompressed = reader.ReadBoolean();
            CompressedLength = reader.ReadInt32();
            Length = reader.ReadInt32();
        }

        internal void WriteInfo(BinaryWriter writer)
        {
            writer.Write(Encoding.ASCII.GetBytes(Name.PadRight(13,'\0')));
            writer.Write(IsCompressed);
            writer.Write(CompressedLength);
            writer.Write(Length);
        }

        protected void ThrowIfInvalidArchive()
        {
            if (Archive == null)
                throw new InvalidOperationException("Entry has been deleted");
            Archive.ThrowIfDisposed();
        }

        public abstract Stream Open();
        internal abstract void WriteData();
        public abstract void Close();
    }
}
