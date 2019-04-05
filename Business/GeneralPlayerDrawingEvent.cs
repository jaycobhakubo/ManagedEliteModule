using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTI.Modules.Shared.Business
{
    public class GeneralPlayerDrawingEvent
    {
        public class DrawingEventEntry
        {
            public int PlayerId;
            public int EntryCount;
        }

        public class DrawingEventResult
        {
            public int PlayerId;
            public int DrawingPosition;
        }

        #region Events
        public event EventHandler<EventArgs> EventIdChanged;
        protected virtual void OnEventIdChanged(EventArgs e = null)
        {
            var h = EventIdChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> DrawingIdChanged;
        protected virtual void OnDrawingIdChanged(EventArgs e = null)
        {
            var h = DrawingIdChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> EntryPeriodBeginChanged;
        protected virtual void OnEntryPeriodBeginChanged(EventArgs e = null)
        {
            var h = EntryPeriodBeginChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> EntryPeriodEndChanged;
        protected virtual void OnEntryPeriodEndChanged(EventArgs e = null)
        {
            var h = EntryPeriodEndChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> ScheduledForWhenChanged;
        protected virtual void OnScheduledForWhenChanged(EventArgs e = null)
        {
            var h = ScheduledForWhenChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> HeldWhenChanged;
        protected virtual void OnHeldWhenChanged(EventArgs e = null)
        {
            var h = HeldWhenChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> CancelledWhenChanged;
        protected virtual void OnCancelledWhenChanged(EventArgs e = null)
        {
            var h = CancelledWhenChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> CreatedWhenChanged;
        protected virtual void OnCreatedWhenChanged(EventArgs e = null)
        {
            var h = CreatedWhenChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> EntriesChanged;
        protected virtual void OnEntriesChanged(EventArgs e = null)
        {
            var h = EntriesChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> ResultsChanged;
        protected virtual void OnResultsChanged(EventArgs e = null)
        {
            var h = ResultsChanged;
            if(h != null)
                h(this, e);
        }

        #endregion Events

        #region Member Variables
        int m_eventId;
        int m_drawingId;
        DateTime m_entryPeriodBegin;
        DateTime m_entryPeriodEnd;
        DateTime? m_scheduleForWhen;
        DateTime? m_heldWhen;
        DateTime? m_cancelledWhen;
        DateTime? m_createdWhen;
        List<DrawingEventEntry> m_entries;
        List<DrawingEventResult> m_results;
        #endregion Member Variables

        #region Constructor(s)
        public GeneralPlayerDrawingEvent()
        {
            m_eventId = 0;
            m_drawingId = 0;
            m_entryPeriodBegin = DateTime.MinValue;
            m_entryPeriodEnd = DateTime.MinValue;
            m_scheduleForWhen = null;
            m_heldWhen = null;
            m_cancelledWhen = null;
            m_createdWhen = null;
            m_entries = new List<DrawingEventEntry>();
            m_results = new List<DrawingEventResult>();
        }

        public GeneralPlayerDrawingEvent(int eventId, int drawingId
            , DateTime entryWindowBegin, DateTime entryWindowEnd
            , DateTime? scheduledForWhen, DateTime? heldWhen, DateTime? cancelledWhen, DateTime? createdWhen, List<DrawingEventEntry> entries, List<DrawingEventResult> results)
        {
            m_eventId = eventId;
            m_drawingId = drawingId;
            m_entryPeriodBegin = entryWindowBegin;
            m_entryPeriodEnd = entryWindowEnd;
            m_scheduleForWhen = scheduledForWhen;
            m_heldWhen = heldWhen;
            m_cancelledWhen = cancelledWhen;
            m_createdWhen = createdWhen;
            m_entries = new List<DrawingEventEntry>(entries);
            m_results = new List<DrawingEventResult>(results);
        }
        #endregion Constructor(s)

        #region Properties
        public int EventId
        {
            get { return m_eventId; }
            internal set
            {
                if(m_eventId != value)
                {
                    m_eventId = value;
                    OnEventIdChanged();
                }
            }
        }

        public int DrawingId
        {
            get { return m_drawingId; }
            set
            {
                if(m_drawingId != value)
                {
                    m_drawingId = value;
                    OnDrawingIdChanged();
                }
            }
        }

        public DateTime EntryPeriodBegin
        {
            get { return m_entryPeriodBegin; }
            set
            {
                if(m_entryPeriodBegin != value)
                {
                    m_entryPeriodBegin = value;
                    OnEntryPeriodBeginChanged();
                }
            }
        }

        public DateTime EntryPeriodEnd
        {
            get { return m_entryPeriodEnd; }
            set
            {
                if(m_entryPeriodEnd != value)
                {
                    m_entryPeriodEnd = value;
                    OnEntryPeriodEndChanged();
                }
            }
        }

        public DateTime? ScheduledForWhen
        {
            get { return m_scheduleForWhen; }
            set
            {
                if(m_scheduleForWhen != value)
                {
                    m_scheduleForWhen = value;
                    OnScheduledForWhenChanged();
                }
            }
        }

        public DateTime? HeldWhen
        {
            get { return m_heldWhen; }
            set
            {
                if(m_heldWhen != value)
                {
                    m_heldWhen = value;
                    OnHeldWhenChanged();
                }
            }
        }

        public DateTime? CancelledWhen
        {
            get { return m_cancelledWhen; }
            set
            {
                if(m_cancelledWhen != value)
                {
                    m_cancelledWhen = value;
                    OnCancelledWhenChanged();
                }
            }
        }

        public DateTime? CreatedWhen
        {
            get { return m_createdWhen; }
            set
            {
                if(m_createdWhen != value)
                {
                    m_createdWhen = value;
                    OnCreatedWhenChanged();
                }
            }
        }

        public List<DrawingEventEntry> Entries
        {
            get { return m_entries; }
            set
            {
                if(m_entries != value)
                {
                    m_entries = value;
                    OnEntriesChanged();
                }
            }
        }

        public List<DrawingEventResult> Results
        {
            get { return m_results; }
            set
            {
                if(m_results != value)
                {
                    m_results = value;
                    OnResultsChanged();
                }
            }
        }

        #endregion Properties
    }
}
