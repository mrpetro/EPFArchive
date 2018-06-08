using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPF
{
    public delegate void SaveProgressEventHandler(object sender, SaveProgressEventArgs e);

    public enum SaveProgressEventType
    {
        SavingStarted,
        SavingBeforeWriteEntry,
        SavingAfterWriteEntry,
        SavingCompleted,
        SavingEntryBytesRead,
    }

    public class SaveProgressEventArgs : EventArgs
    {
        #region Public Constructors

        public SaveProgressEventArgs()
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public bool Cancel { get; set; }
        public EPFArchiveEntry CurrentEntry { get; internal set; }
        public int EntriesSaved { get; internal set; }
        public int EntriesTotal { get; internal set; }
        public SaveProgressEventType EventType { get; internal set; }

        #endregion Public Properties
    }
}
