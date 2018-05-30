using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace EPF
{
    public enum EPFArchiveMode
    {
        Create,
        Read,
        Update
    }

    public class EPFArchive : IDisposable
    {
        #region Private Fields

        private static readonly char[] SIGNATURE = { 'E', 'P', 'F', 'S' };

        //private readonly EPFArchiveHelper m_Helper;

        //private EPFArchiveReader m_ArchiveReader = null;
        private BinaryReader m_ArchiveReader = null;

        private EPFArchiveWriter m_ArchiveWriter = null;
        private Stream m_BackStream;
        private LZWCompressor m_Compressor = null;
        private LZWDecompressor m_Decompressor = null;
        private List<EPFArchiveEntry> m_Entries;
        private Dictionary<string, EPFArchiveEntry> m_EntryDictionary;
        private bool m_IsDisposed;
        private Stream m_MainStream;
        private EPFArchiveMode m_Mode;
        private ReadOnlyCollection<EPFArchiveEntry> m_ReadOnlyEntries;

        #endregion Private Fields

        #region Public Constructors

        public EPFArchive(Stream stream, EPFArchiveMode mode = EPFArchiveMode.Read)
        {
            Init(stream, mode);
        }

        #endregion Public Constructors

        #region Public Properties

        public ReadOnlyCollection<EPFArchiveEntry> Entries { get { return m_ReadOnlyEntries; } }

        public EPFArchiveMode Mode { get { return m_Mode; } }

        #endregion Public Properties

        #region Internal Properties

        //public Stream ArchiveStream { get { return m_MainStream; } }
        internal BinaryReader ArchiveReader { get { return m_ArchiveReader; } }

        internal EPFArchiveWriter ArchiveWriter
        {
            get
            {
                if (m_ArchiveWriter == null)
                    m_ArchiveWriter = new EPFArchiveWriter(m_MainStream);

                return m_ArchiveWriter;
            }
        }

        internal LZWCompressor Compressor
        {
            get
            {
                if (m_Compressor == null)
                    m_Compressor = new LZWCompressor(14);

                return m_Compressor;
            }
        }

        internal LZWDecompressor Decompressor
        {
            get
            {
                if (m_Decompressor == null)
                    m_Decompressor = new LZWDecompressor(14);

                return m_Decompressor;
            }
        }

        #endregion Internal Properties

        #region Public Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public EPFArchiveEntry GetEntry(string entryName)
        {
            if (entryName == null)
                throw new ArgumentNullException("entryName");

            if (Mode == EPFArchiveMode.Create)
                throw new NotSupportedException("Cannot access entries in Create mode.");

            EPFArchiveEntry entry = null;
            m_EntryDictionary.TryGetValue(entryName, out entry);
            return entry;
        }

        #endregion Public Methods

        #region Internal Methods

        internal void Close(bool writeChanges)
        {
            WriteEntries();
        }

        internal void ThrowIfDisposed()
        {
            if (m_IsDisposed)
                throw new ObjectDisposedException(GetType().ToString());
        }

        #endregion Internal Methods

        #region Protected Methods

        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing && !m_IsDisposed)
            {
                try
                {
                    switch (m_Mode)
                    {
                        case EPFArchiveMode.Read:
                            break;

                        case EPFArchiveMode.Create:
                        case EPFArchiveMode.Update:
                        default:
                            Debug.Assert(m_Mode == EPFArchiveMode.Update || m_Mode == EPFArchiveMode.Create);
                            Close(true);
                            break;
                    }
                }
                finally
                {
                    CloseStreams();
                    m_IsDisposed = true;
                }
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void AddEntry(EPFArchiveEntry entry)
        {
            m_Entries.Add(entry);

            string entryName = entry.Name;
            if (!m_EntryDictionary.ContainsKey(entryName))
            {
                m_EntryDictionary.Add(entryName, entry);
            }
        }

        private void CloseStreams()
        {
            if (m_ArchiveWriter != null)
                m_ArchiveWriter.Dispose();

            if (m_ArchiveReader != null)
                m_ArchiveReader.Dispose();
        }

        private void Init(Stream stream, EPFArchiveMode mode)
        {
            m_Entries = new List<EPFArchiveEntry>();
            m_ReadOnlyEntries = new ReadOnlyCollection<EPFArchiveEntry>(m_Entries);
            m_EntryDictionary = new Dictionary<string, EPFArchiveEntry>();

            // check stream against mode
            switch (mode)
            {
                case EPFArchiveMode.Create:
                    throw new NotImplementedException("Create mode is not implemented yet");
                //OpenForCreate(stream);
                //break;
                case EPFArchiveMode.Read:
                    OpenForRead(stream);
                    break;

                case EPFArchiveMode.Update:
                    OpenForUpdate(stream);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("Unknown archive mode");
            }
        }

        private void OpenForCreate(Stream stream)
        {
            if (!stream.CanWrite)
                throw new ArgumentException("Incorrect input stream capabilities in archive create mode");

            m_Mode = EPFArchiveMode.Create;
            m_MainStream = stream;
            m_BackStream = null;
            m_ArchiveReader = null;
        }

        /// <summary>
        /// Open EPF file in read mode.
        /// Only reading entries will be possible
        /// </summary>
        /// <param name="stream"></param>
        private void OpenForRead(Stream stream)
        {
            if (!stream.CanRead)
                throw new ArgumentException("Can't read from input stream");

            m_Mode = EPFArchiveMode.Read;
            //This is the main data stream
            m_MainStream = stream;
            //There is no back stream necesary in read mode
            m_BackStream = null;
            m_ArchiveReader = new BinaryReader(m_MainStream);

            ReadEntries();
        }

        private void OpenForUpdate(Stream stream)
        {
            if (!stream.CanRead || !stream.CanWrite || !stream.CanSeek)
                throw new ArgumentException("Incorrect input stream capabilities in archive update mode");

            m_Mode = EPFArchiveMode.Update;

            //Backup archive stream (MainStream) to temporary file and open this file for read (as BackStream)
            m_MainStream = stream;
            m_BackStream = File.Open(Path.GetTempFileName(), FileMode.Open);
            m_MainStream.CopyTo(m_BackStream);
            m_MainStream.Seek(0, SeekOrigin.Begin);
            m_BackStream.Seek(0, SeekOrigin.Begin);
            //Reading will be done from BackStream and writing will be done to MainStream
            m_ArchiveReader = new BinaryReader(m_BackStream);
            ReadEntries();
        }

        private void ReadEntries()
        {
            char[] signature = ArchiveReader.ReadChars(4);

            if (!signature.SequenceEqual(SIGNATURE))
                throw new InvalidDataException("Missing EPFS signature.");

            UInt32 fatOffset = ArchiveReader.ReadUInt32();
            byte unknown = ArchiveReader.ReadByte();
            UInt16 numberOfFiles = ArchiveReader.ReadUInt16();

            long dataPos = ArchiveReader.BaseStream.Position;

            //Jump to file table at the end of the file
            if (fatOffset > ArchiveReader.BaseStream.Length)
                throw new FormatException("FAT offset is higher than archive file length.");

            ArchiveReader.BaseStream.Position = fatOffset;

            //Read file table entries
            for (int i = 0; i < numberOfFiles; i++)
            {
                EPFArchiveEntry epfArchiveEntry = null;

                if (Mode == EPFArchiveMode.Update)
                    epfArchiveEntry = new EPFArchiveEntryForUpdate(this, dataPos);
                else if (Mode == EPFArchiveMode.Read)
                    epfArchiveEntry = new EPFArchiveEntryForRead(this, dataPos);
                else
                    throw new InvalidOperationException("Read archive entries only possible in Read or Update mode.");

                epfArchiveEntry.ReadInfo(ArchiveReader);
                AddEntry(epfArchiveEntry);
                dataPos += epfArchiveEntry.CompressedLength;
            }
        }

        private void WriteEntries()
        {
            ArchiveWriter.BinWriter.BaseStream.SetLength(0);
            ArchiveWriter.BinWriter.BaseStream.Seek(0, SeekOrigin.Begin);

            UInt32 fatOffset = 0;
            long fatOffsetDataPos = 0;

            ArchiveWriter.BinWriter.Write(SIGNATURE);
            fatOffsetDataPos = ArchiveWriter.BinWriter.BaseStream.Position;
            ArchiveWriter.BinWriter.Write(fatOffset);
            ArchiveWriter.BinWriter.Write((byte)0);
            ArchiveWriter.BinWriter.Write((UInt16)Entries.Count);

            for (int i = 0; i < Entries.Count; i++)
            {
                var entry = Entries[i];
                entry.WriteData();
            }

            fatOffset = (UInt32)ArchiveWriter.BinWriter.BaseStream.Position;
            //Get back to fatOffset information and write it
            ArchiveWriter.BinWriter.BaseStream.Seek(fatOffsetDataPos, SeekOrigin.Begin);
            ArchiveWriter.BinWriter.Write(fatOffset);
            //Get back to FATOffet
            ArchiveWriter.BinWriter.BaseStream.Seek((long)fatOffset, SeekOrigin.Begin);
            //Write all entries info
            for (int i = 0; i < Entries.Count; i++)
            {
                var entry = Entries[i];
                entry.WriteInfo(m_ArchiveWriter.BinWriter);
            }
        }

        #endregion Private Methods
    }
}