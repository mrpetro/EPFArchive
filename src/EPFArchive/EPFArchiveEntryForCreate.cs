using System;
using System.IO;
using System.Text;

namespace EPF
{
    public class EPFArchiveEntryForCreate : EPFArchiveEntry
    {
        #region Private Fields

        private string _filePath;

        #endregion Private Fields

        #region Internal Constructors

        internal EPFArchiveEntryForCreate(EPFArchive archive, string filePath) :
            base(archive)
        {
            var fileInfo = new FileInfo(filePath);
            Name = ValidateName(fileInfo.Name);
            Length = (int)fileInfo.Length;
            CompressedLength = Length;
            Action = EPFEntryAction.Add;
            _filePath = filePath;
        }

        #endregion Internal Constructors

        #region Internal Properties

        internal string FilePath { get { return _filePath; } }

        #endregion Internal Properties

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

        internal override void WriteData(BinaryWriter writer)
        {
            if (Action == EPFEntryAction.Remove)
                throw new InvalidOperationException("Writing entry marked for removing");

            using (var openedStream = File.OpenRead(_filePath))
            {
                if (Action.HasFlag(EPFEntryAction.Compress))
                {
                    //Remember last position before writing compressed data
                    var lastPosition = writer.BaseStream.Position;
                    //Compress entry data while storing it into archive
                    Archive.Compressor.Compress(openedStream, writer.BaseStream);
                    //Update entry normal and compressed data lengths
                    Length = (int)openedStream.Length;
                    CompressedLength = (int)writer.BaseStream.Position - (int)lastPosition;
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

        #region Private Methods

        private static bool IsASCII(string value)
        {
            return Encoding.UTF8.GetByteCount(value) == value.Length;
        }

        private static string ValidateName(string name)
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

        #endregion Private Methods
    }
}