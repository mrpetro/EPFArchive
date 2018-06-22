using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPF
{
    public delegate void ExtractProgressEventHandler(object sender, ExtractProgressEventArgs e);

    public enum ExtractProgressEventType
    {
        ExtractionStarted,
        ExtractionBeforeReadEntry,
        ExtractionAfterReadEntry,
        ExtractionCompleted,
        ExtractionEntryBytesWrite,
    }

    public class ExtractProgressEventArgs : EventArgs
    {
        #region Public Constructors

        public ExtractProgressEventArgs()
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public bool Cancel { get; set; }
        public EPFArchiveEntry CurrentEntry { get; internal set; }
        public int EntriesExtracted { get; internal set; }
        public int EntriesTotal { get; internal set; }
        public ExtractProgressEventType EventType { get; internal set; }

        #endregion Public Properties
    }
}
