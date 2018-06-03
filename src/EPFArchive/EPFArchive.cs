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

        private BinaryReader m_ArchiveReader = null;
        private EPFArchiveWriter _ArchiveWriter = null;
        private Stream _BackStream;
        private LZWCompressor _Compressor = null;
        private LZWDecompressor _Decompressor = null;
        private List<EPFArchiveEntry> _Entries;
        private Dictionary<string, EPFArchiveEntry> _EntryDictionary;
        private bool _IsDisposed;
        private Stream _MainStream;
        private EPFArchiveMode _Mode;
        private ReadOnlyCollection<EPFArchiveEntry> _ReadOnlyEntries;

        #endregion Private Fields

        #region Public Constructors

        public EPFArchive(Stream stream, EPFArchiveMode mode = EPFArchiveMode.Read)
        {
            Init(stream, mode);
        }

        #endregion Public Constructors

        #region Public Properties

        public ReadOnlyCollection<EPFArchiveEntry> Entries { get { return _ReadOnlyEntries; } }

        public EPFArchiveMode Mode { get { return _Mode; } }

        #endregion Public Properties

        #region Internal Properties

        //public Stream ArchiveStream { get { return m_MainStream; } }
        internal BinaryReader ArchiveReader { get { return m_ArchiveReader; } }

        internal EPFArchiveWriter ArchiveWriter
        {
            get
            {
                if (_ArchiveWriter == null)
                    _ArchiveWriter = new EPFArchiveWriter(_MainStream);

                return _ArchiveWriter;
            }
        }

        internal LZWCompressor Compressor
        {
            get
            {
                if (_Compressor == null)
                    _Compressor = new LZWCompressor(14);

                return _Compressor;
            }
        }

        internal LZWDecompressor Decompressor
        {
            get
            {
                if (_Decompressor == null)
                    _Decompressor = new LZWDecompressor(14);

                return _Decompressor;
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
            _EntryDictionary.TryGetValue(entryName, out entry);
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
            if (_IsDisposed)
                throw new ObjectDisposedException(GetType().ToString());
        }

        #endregion Internal Methods

        #region Protected Methods

        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing && !_IsDisposed)
            {
                try
                {
                    switch (_Mode)
                    {
                        case EPFArchiveMode.Read:
                            break;

                        case EPFArchiveMode.Create:
                        case EPFArchiveMode.Update:
                        default:
                            Debug.Assert(_Mode == EPFArchiveMode.Update || _Mode == EPFArchiveMode.Create);
                            Close(true);
                            break;
                    }
                }
                finally
                {
                    CloseStreams();
                    _IsDisposed = true;
                }
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void AddEntry(EPFArchiveEntry entry)
        {
            _Entries.Add(entry);

            string entryName = entry.Name;
            if (!_EntryDictionary.ContainsKey(entryName))
            {
                _EntryDictionary.Add(entryName, entry);
            }
        }

        private void CloseStreams()
        {
            if (_ArchiveWriter != null)
                _ArchiveWriter.Dispose();

            if (m_ArchiveReader != null)
                m_ArchiveReader.Dispose();
        }

        private void Init(Stream stream, EPFArchiveMode mode)
        {
            _Entries = new List<EPFArchiveEntry>();
            _ReadOnlyEntries = new ReadOnlyCollection<EPFArchiveEntry>(_Entries);
            _EntryDictionary = new Dictionary<string, EPFArchiveEntry>();

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

            _Mode = EPFArchiveMode.Create;
            _MainStream = stream;
            _BackStream = null;
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

            _Mode = EPFArchiveMode.Read;
            //This is the main data stream
            _MainStream = stream;
            //There is no back stream necesary in read mode
            _BackStream = null;
            m_ArchiveReader = new BinaryReader(_MainStream);

            ReadEntries();
        }

        private void OpenForUpdate(Stream stream)
        {
            if (!stream.CanRead || !stream.CanWrite || !stream.CanSeek)
                throw new ArgumentException("Incorrect input stream capabilities in archive update mode");

            _Mode = EPFArchiveMode.Update;

            //Backup archive stream (MainStream) to temporary file and open this file for read (as BackStream)
            _MainStream = stream;
            _BackStream = File.Open(Path.GetTempFileName(), FileMode.Open);
            _MainStream.CopyTo(_BackStream);
            _MainStream.Seek(0, SeekOrigin.Begin);
            _BackStream.Seek(0, SeekOrigin.Begin);
            //Reading will be done from BackStream and writing will be done to MainStream
            m_ArchiveReader = new BinaryReader(_BackStream);
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
                entry.WriteInfo(_ArchiveWriter.BinWriter);
            }
        }

        #endregion Private Methods
    }
}