using System;
using System.IO;
using System.Text;

namespace EPF
{
    public class EPFArchiveEntryForCreate : EPFArchiveEntry
    {
        #region Private Fields

        private string _filePath;

        private long _archiveDataPos;

        #endregion Private Fields

        #region Internal Constructors

        internal EPFArchiveEntryForCreate(EPFArchive archive, string name, string filePath) :
            base(archive)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
            var fileInfo = new FileInfo(filePath);
            Length = (int)fileInfo.Length;
            CompressedLength = Length;
            Action = EPFEntryAction.Add;
            _filePath = filePath;
            _archiveDataPos = -1;
        }

        #endregion Internal Constructors

        #region Public Methods

        public override void Close()
        {
        }

        public override Stream Open()
        {
            throw new InvalidOperationException("New entry cannot be opened");
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// This method will create equivalent archive entry suitable for updating from current entry.
        /// It should be used in stage of saving archive, which has new entries created.
        /// </summary>
        /// <returns>Archive entry suitable for updating</returns>
        internal EPFArchiveEntryForUpdate Convert()
        {
            return new EPFArchiveEntryForUpdate(Archive, Name, Length, CompressedLength, _archiveDataPos);
        }

        /// <summary>
        /// This method will write file which has been passes by file path in the constructor
        /// into current Archive. Optionally it will be compressed in the process.
        /// </summary>
        /// <param name="writer">Reference to archive binary writer stream</param>
        internal override void WriteData(BinaryWriter writer)
        {
            if (Action == EPFEntryAction.Remove)
                throw new InvalidOperationException("Writing entry marked for removing");

            using (var openedStream = File.OpenRead(_filePath))
            {
                //Remember position of entry data start in archive file
                _archiveDataPos = writer.BaseStream.Position;

                if (Action.HasFlag(EPFEntryAction.Compress))
                {
                    //Compress entry data while storing it into archive
                    Archive.Compressor.Compress(openedStream, writer.BaseStream);
                    //Update entry normal and compressed data lengths
                    Length = (int)openedStream.Length;
                    CompressedLength = (int)writer.BaseStream.Position - (int)_archiveDataPos;
                }
                else
                {
                    int newLength = (int)openedStream.Length;

                    using (var reader = new BinaryReader(openedStream, Encoding.UTF8, true))
                        writer.Write(reader.ReadBytes(newLength), 0, newLength);

                    Length = newLength;
                    CompressedLength = Length;
                }
            }
        }

        #endregion Internal Methods

    }
}