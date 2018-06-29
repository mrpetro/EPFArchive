using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace EPF
{
    public enum EPFArchiveMode
    {
        Create,
        Read,
        Update
    }

    internal struct EPFEntryBlock
    {
        #region Internal Properties

        internal Int32 CompressedSize { get; set; }
        internal bool CompressionFlag { get; set; }
        internal Int32 DecompressedSize { get; set; }
        internal string Filename { get; set; }

        #endregion Internal Properties
    };

    internal struct EPFHeaderBlock
    {
        #region Internal Properties

        internal UInt32 FATOffset { get; set; }
        internal UInt16 NumberOfFiles { get; set; }
        internal char[] Signature { get; set; }
        internal byte Unknown { get; set; }

        #endregion Internal Properties
    };

    public class EPFArchive : INotifyPropertyChanged, IDisposable
    {
        #region Private Fields

        private static readonly char[] SIGNATURE = { 'E', 'P', 'F', 'S' };
        private Stream _BackStream;
        private LZWCompressor _compressor = null;
        private LZWDecompressor _decompressor = null;
        private List<EPFArchiveEntry> _entries;
        private int _modifiedEntryiesNo = 0;
        private Dictionary<string, EPFArchiveEntry> _entryDictionary;
        private ExtractProgressEventArgs _extractProgressEventArgs;
        private byte[] _hiddenData;

        private bool _IsDisposed;

        private bool _isModified;

        private Stream _MainStream;

        private SaveProgressEventArgs _saveProgressEventArgs;

        #endregion Private Fields

        #region Public Constructors

        public EPFArchive(Stream stream, EPFArchiveMode mode = EPFArchiveMode.Read)
        {
            PropertyChanged += EPFArchive_PropertyChanged;

            Init(stream, mode);
        }

        #endregion Public Constructors

        #region Public Events

        public event EntryChangedEventHandler EntryChanged;

        public event ExtractProgressEventHandler ExtractProgress;

        public event PropertyChangedEventHandler PropertyChanged;

        public event SaveProgressEventHandler SaveProgress;

        #endregion Public Events

        #region Public Properties

        public ReadOnlyCollection<EPFArchiveEntry> Entries { get; private set; }

        internal int ModifiedEntryiesNo
        {
            get
            {
                return _modifiedEntryiesNo;
            }

            set
            {
                if (_modifiedEntryiesNo == value)
                    return;

                _modifiedEntryiesNo = value;

                PropertyChanged(this, new PropertyChangedEventArgs(nameof(ModifiedEntryiesNo)));
            }
        }

        public bool IsModified
        {
            get
            {
                return _isModified;
            }

            protected set
            {
                if (_isModified == value)
                    return;

                _isModified = value;

                PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsModified)));
            }
        }

        public EPFArchiveMode Mode { get; private set; }

        #endregion Public Properties

        #region Internal Properties

        internal BinaryReader ArchiveReader { get; private set; }

        internal LZWCompressor Compressor
        {
            get
            {
                if (_compressor == null)
                    _compressor = new LZWCompressor(14);

                return _compressor;
            }
        }

        internal LZWDecompressor Decompressor
        {
            get
            {
                if (_decompressor == null)
                    _decompressor = new LZWDecompressor(14);

                return _decompressor;
            }
        }

        #endregion Internal Properties

        #region Public Methods

        public static string ValidateEntryName(string name)
        {
            if (name == null)
                throw new Exception("Name is not set");

            if (!IsASCII(name))
                throw new InvalidOperationException("Name must contain only ASCII characters");

            name = name.Trim();

            if (name.Length > 12)
                throw new InvalidOperationException("Name length must not exceed 12 characters");

            return name.ToUpper();
        }

        public void Close(bool saveChanges)
        {
            try
            {
                switch (Mode)
                {
                    case EPFArchiveMode.Read:
                        break;

                    case EPFArchiveMode.Create:
                    case EPFArchiveMode.Update:
                    default:
                        Debug.Assert(Mode == EPFArchiveMode.Update || Mode == EPFArchiveMode.Create);
                        if (saveChanges)
                            Save();
                        break;
                }
            }
            finally
            {
                CloseStreams();
            }
        }

        public EPFArchiveEntry CreateEntry(string entryName, string filePath)
        {
            if (_entryDictionary.ContainsKey(entryName))
                throw new InvalidOperationException($"Entry '{entryName}' already exists.");

            var newEntry = new EPFArchiveEntryForCreate(this, entryName, filePath);
            AddEntry(newEntry);

            ModifiedEntryiesNo++;

            return newEntry;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void ExtractAll(string folderPath)
        {
            ExtractEntries(folderPath, _entries);
        }

        public void ExtractEntries(string folderPath, ICollection<string> entryNames)
        {
            var entries = _entries.Where(item => entryNames.Contains(item.Name)).ToList();

            ExtractEntries(folderPath, entries);
        }

        public EPFArchiveEntry FindEntry(string entryName)
        {
            if (entryName == null)
                throw new ArgumentNullException("entryName");

            if (Mode == EPFArchiveMode.Create)
                throw new NotSupportedException("Cannot access entries in Create mode.");

            EPFArchiveEntry entry = null;
            _entryDictionary.TryGetValue(entryName, out entry);
            return entry;
        }

        public bool RemoveEntry(string entryName)
        {
            var entry = FindEntry(entryName);

            if (entry == null)
                return false;

            _entries.Remove(entry);
            _entryDictionary.Remove(entryName);

            ModifiedEntryiesNo++;

            RaiseEntryChanged(entry, EntryChangedEventType.Removed);
            return true;
        }

        public EPFArchiveEntry ReplaceEntry(string entryName, string filePath)
        {
            var oldEntry = FindEntry(entryName);

            if (oldEntry == null)
                throw new InvalidOperationException($"Entry {entryName} doesn't exists.");

            var entryIndex = _entries.IndexOf(oldEntry);

            var newEntry = new EPFArchiveEntryForCreate(this, entryName, filePath);
            _entries[entryIndex] = newEntry;
            _entryDictionary[newEntry.Name] = newEntry;

            ModifiedEntryiesNo++;

            RaiseEntryChanged(newEntry, EntryChangedEventType.Replaced);
            return newEntry;
        }

        public void Save()
        {
            try
            {
                using (var binWriter = new BinaryWriter(_MainStream, System.Text.Encoding.UTF8, true))
                {
                    if (Mode == EPFArchiveMode.Read)
                        throw new InvalidOperationException("Trying to save in read-only mode");

                    _saveProgressEventArgs = new SaveProgressEventArgs();

                    _saveProgressEventArgs.EventType = SaveProgressEventType.SavingStarted;
                    OnSaveProgress(_saveProgressEventArgs);

                    _saveProgressEventArgs.EntriesTotal = _entries.Count;

                    //Write remaining entries to archive
                    WriteEntries(binWriter);

                    //Update BackStream with new MainStream content
                    UpdateBackStream();

                    _saveProgressEventArgs.EventType = SaveProgressEventType.SavingCompleted;
                    OnSaveProgress(_saveProgressEventArgs);
                }
            }
            finally
            {
                _saveProgressEventArgs = null;
                ModifiedEntryiesNo = 0;
            }
        }

        #endregion Public Methods

        #region Internal Methods

        internal void RaiseEntryChanged(EPFArchiveEntry entry, EntryChangedEventType eventType)
        {
            OnEntryChanged(new EntryChangedEventArgs(entry, eventType));
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
                    Close(false);
                }
                catch (Exception)
                {
                    _IsDisposed = true;
                }
            }
        }

        protected void OnEntryChanged(EntryChangedEventArgs eventArgs)
        {
            if (EntryChanged != null)
                EntryChanged(this, eventArgs);
        }

        protected void OnExtractProgress(ExtractProgressEventArgs eventArgs)
        {
            if (ExtractProgress != null)
                ExtractProgress(this, eventArgs);
        }

        protected void OnSaveProgress(SaveProgressEventArgs eventArgs)
        {
            if (SaveProgress != null)
                SaveProgress(this, eventArgs);
        }

        #endregion Protected Methods

        #region Private Methods

        private static bool IsASCII(string value)
        {
            return Encoding.UTF8.GetByteCount(value) == value.Length;
        }

        private void AddEntry(EPFArchiveEntry entry)
        {
            _entries.Add(entry);
            _entryDictionary.Add(entry.Name, entry);
            RaiseEntryChanged(entry, EntryChangedEventType.Added);
        }

        private void CloseStreams()
        {
            if (ArchiveReader != null)
            {
                ArchiveReader.Dispose();
                ArchiveReader = null;
            }

            if (_MainStream != null)
            {
                _MainStream.Dispose();
                _MainStream = null;
            }
        }

        private void ExtractEntries(string folderPath, ICollection<EPFArchiveEntry> entries)
        {
            try
            {
                _extractProgressEventArgs = new ExtractProgressEventArgs();

                _extractProgressEventArgs.EventType = ExtractProgressEventType.ExtractionStarted;
                _extractProgressEventArgs.EntriesTotal = entries.Count;
                OnExtractProgress(_extractProgressEventArgs);

                var count = 0;

                foreach (var entry in entries)
                {
                    if (entry.Archive != this)
                        throw new InvalidOperationException("At least one of enties is invalid.");

                    count++;

                    _extractProgressEventArgs.CurrentEntry = entry;
                    _extractProgressEventArgs.EventType = ExtractProgressEventType.ExtractionBeforeReadEntry;
                    OnExtractProgress(_extractProgressEventArgs);

                    entry.ExtractTo(folderPath);

                    _extractProgressEventArgs.EventType = ExtractProgressEventType.ExtractionAfterReadEntry;
                    _extractProgressEventArgs.EntriesExtracted = count;
                    OnExtractProgress(_extractProgressEventArgs);
                }

                _extractProgressEventArgs.EventType = ExtractProgressEventType.ExtractionCompleted;
                OnExtractProgress(_extractProgressEventArgs);
            }
            finally
            {
                _extractProgressEventArgs = null;
            }
        }

        private void Init(Stream stream, EPFArchiveMode mode)
        {
            _entries = new List<EPFArchiveEntry>();
            Entries = new ReadOnlyCollection<EPFArchiveEntry>(_entries);
            _entryDictionary = new Dictionary<string, EPFArchiveEntry>();

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

            Mode = EPFArchiveMode.Create;
            _MainStream = stream;
            _BackStream = null;
            ArchiveReader = null;
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

            Mode = EPFArchiveMode.Read;
            //This is the main data stream
            _MainStream = stream;
            //There is no back stream necesary in read mode
            _BackStream = null;
            ArchiveReader = new BinaryReader(_MainStream);

            ReadEntries();
        }

        private void OpenForUpdate(Stream stream)
        {
            if (!stream.CanRead || !stream.CanWrite || !stream.CanSeek)
                throw new ArgumentException("Incorrect input stream capabilities in archive update mode");

            Mode = EPFArchiveMode.Update;

            _MainStream = stream;

            //Create BackStream from temporary file
            _BackStream = File.Open(Path.GetTempFileName(), FileMode.Open);

            //Backup archive stream (MainStream) to BackStream
            UpdateBackStream();

            //Reading will be done from BackStream and writing will be done to MainStream
            ArchiveReader = new BinaryReader(_BackStream);

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
                    throw new InvalidOperationException("Reading archive entries only possible in Read or Update mode.");

                epfArchiveEntry.ReadInfo(ArchiveReader);

                if (_entryDictionary.ContainsKey(epfArchiveEntry.Name))
                    throw new InvalidOperationException($"Entry with name '{epfArchiveEntry.Name}' already exists.");

                AddEntry(epfArchiveEntry);
                dataPos += epfArchiveEntry.CompressedLength;
            }
        }

        private void UpdateBackStream()
        {
            _MainStream.Seek(0, SeekOrigin.Begin);
            _BackStream.Seek(0, SeekOrigin.Begin);
            _MainStream.CopyTo(_BackStream);
            _MainStream.Seek(0, SeekOrigin.Begin);
            _BackStream.Seek(0, SeekOrigin.Begin);
        }

        private void WriteEntries(BinaryWriter writer)
        {
            writer.BaseStream.SetLength(0);
            writer.BaseStream.Seek(0, SeekOrigin.Begin);

            UInt32 fatOffset = 0;
            long fatOffsetDataPos = 0;

            //Write archive header
            writer.Write(SIGNATURE);
            //Remeber fat offset data position
            fatOffsetDataPos = writer.BaseStream.Position;
            //Reserve fat offset data space
            writer.Write(fatOffset);
            //Write alignment(unknown purpose) byte
            writer.Write((byte)0);
            //Write number of entries
            writer.Write((UInt16)Entries.Count);

            //Write all entries data
            for (int i = 0; i < _entries.Count; i++)
            {
                var entry = _entries[i];

                _saveProgressEventArgs.EventType = SaveProgressEventType.SavingBeforeWriteEntry;
                _saveProgressEventArgs.CurrentEntry = entry;
                OnSaveProgress(_saveProgressEventArgs);

                entry.WriteData(writer);

                //If entry is EntryForCrate (new or to replace), then convert it to EntryForUpdate
                if (entry is EPFArchiveEntryForCreate)
                {
                    _entries[i] = ((EPFArchiveEntryForCreate)entry).Convert();
                    RaiseEntryChanged(_entries[i], EntryChangedEventType.Stored);
                }

                _saveProgressEventArgs.EventType = SaveProgressEventType.SavingAfterWriteEntry;
                _saveProgressEventArgs.EntriesSaved = i + 1;
                OnSaveProgress(_saveProgressEventArgs);
            }

            fatOffset = (UInt32)writer.BaseStream.Position;
            //Get back to fatOffset information and write it
            writer.BaseStream.Seek(fatOffsetDataPos, SeekOrigin.Begin);
            writer.Write(fatOffset);
            //Get back to FATOffet
            writer.BaseStream.Seek((long)fatOffset, SeekOrigin.Begin);
            //Write all entries info
            for (int i = 0; i < Entries.Count; i++)
            {
                var entry = Entries[i];
                entry.WriteInfo(writer);
            }
        }

        private void EPFArchive_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ModifiedEntryiesNo):
                    IsModified = ModifiedEntryiesNo > 0;
                    break;
                default:
                    break;
            }
        }

        #endregion Private Methods
    }
}