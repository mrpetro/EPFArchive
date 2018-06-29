using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPF
{
    public enum EntryChangedEventType
    {
        Added,
        Removed,
        Replaced,
        Unchanged,
        Stored
    }

    public delegate void EntryChangedEventHandler(object sender, EntryChangedEventArgs e);

    public class EntryChangedEventArgs : EventArgs
    {
        #region Public Constructors

        public EntryChangedEventArgs(EPFArchiveEntry entry, EntryChangedEventType eventType)
        {
            Entry = entry;
            EventType = eventType;
        }

        #endregion Public Constructors

        #region Public Properties
        public EPFArchiveEntry Entry { get; internal set; }
        public EntryChangedEventType EventType { get; internal set; }
        #endregion Public Properties
    }


}
