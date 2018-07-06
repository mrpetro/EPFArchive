using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private string _backupArchivePath;
        private LZWCompressor _compressor = null;
        private LZWDecompressor _decompressor = null;
        private List<EPFArchiveEntry> _entries;
        private Dictionary<string, EPFArchiveEntry> _entryDictionary;
        private ExtractProgressEventArgs _extractProgressEventArgs;
        private byte[] _hiddenData;
        private bool _IsDisposed;
        private bool _isModified;
        private bool _leaveOpen;
        private Stream _MainStream;
        private int _modifiedEntryiesNo = 0;
        private SaveProgressEventArgs _saveProgressEventArgs;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Opens EPF archive file given in stream
        /// By default it opens it in read-only mode and closes the stream on dispose
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="mode"></param>
        /// <param name="leaveOpen"></param>
        public EPFArchive(Stream stream, EPFArchiveMode mode = EPFArchiveMode.Read, bool leaveOpen = false)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            _leaveOpen = leaveOpen;

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

                OnPropertyChanged(nameof(IsModified));
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

        internal int ModifiedEntriesNo
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

                OnPropertyChanged(nameof(ModifiedEntriesNo));
            }
        }

        #endregion Internal Properties

        #region Public Methods

        public static string ValidateEntryName(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (!IsASCII(name))
                throw new InvalidOperationException("Name must contain only ASCII characters");

            name = name.Trim();

            if (name.Length > 12)
                throw new InvalidOperationException("Name length must not exceed 12 characters");

            return name.ToUpper();
        }

        public EPFArchiveEntry CreateEntry(string entryName, string filePath)
        {
            if (Mode == EPFArchiveMode.Read)
                throw new InvalidOperationException("Unable to create any entry in read-only mode");

            if (_entryDictionary.ContainsKey(entryName))
                throw new ArgumentException($"Entry '{entryName}' already exists.");

            var newEntry = new EPFArchiveEntryForCreate(this, entryName, filePath);
            AddEntry(newEntry);

            ModifiedEntriesNo++;

            return newEntry;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This method extracts all entries (and decompresses if needed) data into folder
        /// given as parameter. Given folder must exist.
        /// </summary>
        /// <param name="folderPath">Entries data destination folder</param>
        public void ExtractAll(string folderPath)
        {
            ExtractEntries(folderPath, _entries);
        }

        /// <summary>
        /// This method extracts given entries (and decompresses if needed) data into folder
        /// given as parameter. Given folder must exist.
        /// </summary>
        /// <param name="folderPath">Entries data destination folder</param>
        /// <param name="entryNames">Collection of entry names to extract</param>
        public void ExtractEntries(string folderPath, ICollection<string> entryNames)
        {
            var entries = _entries.Where(item => entryNames.Contains(item.Name, StringComparer.OrdinalIgnoreCase)).ToList();

            ExtractEntries(folderPath, entries);
        }

        /// <summary>
        /// Find entry with given name (case insensitive search) and returns entry object controller
        /// </summary>
        /// <param name="entryName"></param>
        /// <returns></returns>
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
            if (Mode == EPFArchiveMode.Read)
                throw new InvalidOperationException("Unable to remove any entry in read-only mode");

            var entry = FindEntry(entryName);

            if (entry == null)
                return false;

            _entries.Remove(entry);
            _entryDictionary.Remove(entryName);

            ModifiedEntriesNo++;

            RaiseEntryChanged(entry, EntryChangedEventType.Removed);
            return true;
        }

        public EPFArchiveEntry ReplaceEntry(string entryName, string filePath)
        {
            if (Mode == EPFArchiveMode.Read)
                throw new InvalidOperationException("Unable to replace any entry in read-only mode");

            var oldEntry = FindEntry(entryName);

            if (oldEntry == null)
                throw new ArgumentException($"Entry {entryName} doesn't exists.");

            oldEntry.Dispose();

            var entryIndex = _entries.IndexOf(oldEntry);

            var newEntry = new EPFArchiveEntryForCreate(this, entryName, filePath);
            _entries[entryIndex] = newEntry;
            _entryDictionary[newEntry.Name] = newEntry;

            ModifiedEntriesNo++;

            RaiseEntryChanged(newEntry, EntryChangedEventType.Replaced);
            return newEntry;
        }

        public void Save()
        {
            if (Mode == EPFArchiveMode.Read)
                throw new InvalidOperationException("Unable to save in read-only mode");

            try
            {
                using (var binWriter = new BinaryWriter(_MainStream, System.Text.Encoding.UTF8, true))
                {
                    _saveProgressEventArgs = new SaveProgressEventArgs();

                    _saveProgressEventArgs.EventType = SaveProgressEventType.SavingStarted;
                    OnSaveProgress(_saveProgressEventArgs);

                    _saveProgressEventArgs.EntriesTotal = _entries.Count;

                    //Write remaining entries to archive
                    WriteEntries(binWriter);

                    if (Mode == EPFArchiveMode.Create)
                        CreateBackupStream();

                    //Update BackStream with new MainStream content
                    UpdateBackStream();

                    _saveProgressEventArgs.EventType = SaveProgressEventType.SavingCompleted;
                    OnSaveProgress(_saveProgressEventArgs);
                }

                if (Mode == EPFArchiveMode.Create)
                {
                    //Reading will be done from BackStream and writing will be done to MainStream
                    ArchiveReader = new BinaryReader(_BackStream);

                    Mode = EPFArchiveMode.Update;
                }
            }
            finally
            {
                _saveProgressEventArgs = null;
                ModifiedEntriesNo = 0;
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
                    CloseStreams();
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

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
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
            if (_BackStream != null)
            {
                _BackStream.Dispose();
                _BackStream = null;
                File.Delete(_backupArchivePath);
            }

            if (!_leaveOpen && _MainStream != null)
            {
                _MainStream.Dispose();
                _MainStream = null;
            }
        }

        private void CreateBackupStream()
        {
            _backupArchivePath = Path.GetTempFileName();
            _BackStream = File.Open(_backupArchivePath, FileMode.Open);
        }

        private void EPFArchive_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ModifiedEntriesNo):
                    IsModified = ModifiedEntriesNo > 0;
                    break;

                default:
                    break;
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
            _entryDictionary = new Dictionary<string, EPFArchiveEntry>(StringComparer.OrdinalIgnoreCase);

            // check stream against mode
            switch (mode)
            {
                case EPFArchiveMode.Create:
                    OpenForCreate(stream);
                    break;

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
                throw new InvalidOperationException("Incorrect input stream capabilities in archive update mode");

            Mode = EPFArchiveMode.Update;

            _MainStream = stream;

            //Create BackStream from temporary file
            CreateBackupStream();

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

        #endregion Private Methods
    }
}