using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTI.Modules.Shared.Business
{
    public class GeneralPlayerDrawing
    {
        public enum SpendGrouping : byte
        {
            NONE = 0,
            BY_TRANSACTION = 1,
            BY_SESSION = 2,
            BY_DAY = 3,
            WITHIN_ENTRY_WINDOW = 4,
        }

        public enum PurchaseGrouping : byte
        {
            NONE = 0,
            BY_TRANSACTION = 1,
            BY_SESSION = 2,
            BY_DAY = 3,
            WITHIN_ENTRY_WINDOW = 4,
        }

        public enum VisitType : byte
        {
            NONE = 0,
            SESSIONS_PER_DAY = 1,
            DAYS_IN_ENTRY_PERIOD = 2,
            SESSIONS_IN_ENTRY_PERIOD = 3,
        }

        public enum PurchaseType : byte
        {
            NONE = 0,
            PACKAGE = 1,
            PRODUCT = 2,
        }

        public class EntryTier<T>
            where T : IComparable<T>
        {
            #region Events
            public event EventHandler<EventArgs> TierBeginChanged;
            protected virtual void OnTierBeginChanged(EventArgs e = null)
            {
                var h = TierBeginChanged;
                if(h != null)
                    h(this, e);
            }

            public event EventHandler<EventArgs> TierEndChanged;
            protected virtual void OnTierEndChanged(EventArgs e = null)
            {
                var h = TierEndChanged;
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

            #endregion Events

            #region Member Variables
            T m_tierBegin;
            T m_tierEnd;
            int m_entries;
            #endregion Member Variables

            #region Constructor(s)
            public EntryTier(T tierBegin, T tierEnd, int entries)
            {
                m_tierBegin = tierBegin;
                m_tierEnd = tierEnd;
                m_entries = entries;
            }
            #endregion Constructor(s)

            #region Properties
            public T TierBegin
            {
                get { return m_tierBegin; }
                set
                {
                    if(!m_tierBegin.Equals(value))
                    {
                        m_tierBegin = value;
                        OnTierBeginChanged();
                    }
                }
            }

            public T TierEnd
            {
                get { return m_tierEnd; }
                set
                {
                    if(!m_tierEnd.Equals(value))
                    {
                        m_tierEnd = value;
                        OnTierEndChanged();
                    }
                }
            }

            public int Entries
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

            #endregion Properties

            public static readonly OverlapEntryTierComparer OverlapComparer = new OverlapEntryTierComparer();
            public static readonly SortEntryTierComparer SortComparer = new SortEntryTierComparer();

            public class OverlapEntryTierComparer : IComparer<EntryTier<T>>
            {
                public int Compare(EntryTier<T> x, EntryTier<T> y)
                {
                    if(x.TierEnd.CompareTo(y.TierBegin) < 0)
                        return -1;
                    if(x.TierBegin.CompareTo(y.TierEnd) > 0)
                        return 1;
                    return 0;
                }
            }

            public class SortEntryTierComparer : IComparer<EntryTier<T>>
            {
                public int Compare(EntryTier<T> x, EntryTier<T> y)
                {
                    var c = x.TierBegin.CompareTo(y.TierBegin);
                    if(c != 0)
                        return c;
                    c = x.TierEnd.CompareTo(y.TierEnd);
                    if(c != 0)
                        return c;

                    return 0;
                }
            }
        }

        #region Events
        public event EventHandler IdChanged;
        protected virtual void OnIdChanged(EventArgs e = null)
        {
            var h = IdChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler NameChanged;
        protected virtual void OnNameChanged(EventArgs e = null)
        {
            var h = NameChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> ActiveChanged;
        protected virtual void OnActiveChanged(EventArgs e = null)
        {
            var h = ActiveChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> DescriptionChanged;
        protected virtual void OnDescriptionChanged(EventArgs e = null)
        {
            var h = DescriptionChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> EntriesDrawnChanged;
        protected virtual void OnEntriesDrawnChanged(EventArgs e = null)
        {
            var h = EntriesDrawnChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> MinimumEntriesChanged;
        protected virtual void OnMinimumEntriesChanged(EventArgs e = null)
        {
            var h = MinimumEntriesChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> MaximumDrawsPerPlayerChanged;
        protected virtual void OnMaximumDrawsPerPlayerChanged(EventArgs e = null)
        {
            var h = MaximumDrawsPerPlayerChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> ShowEntriesOnReceiptsChanged;
        protected virtual void OnShowEntriesOnReceiptsChanged(EventArgs e = null)
        {
            var h = ShowEntriesOnReceiptsChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> PlayerPresenceRequiredChanged;
        protected virtual void OnPlayerPresenceRequiredChanged(EventArgs e = null)
        {
            var h = PlayerPresenceRequiredChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> InitialEventEntryPeriodBeginChanged;
        protected virtual void OnInitialEventEntryPeriodBeginChanged(EventArgs e = null)
        {
            var h = InitialEventEntryPeriodBeginChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> InitialEventEntryPeriodEndChanged;
        protected virtual void OnInitialEventEntryPeriodEndChanged(EventArgs e = null)
        {
            var h = InitialEventEntryPeriodEndChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> InitialEventScheduledForWhenChanged;
        protected virtual void OnInitialEventScheduledForWhenChanged(EventArgs e = null)
        {
            var h = InitialEventScheduledForWhenChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> EventRepeatIncrementChanged;
        protected virtual void OnEventRepeatIncrementChanged(EventArgs e = null)
        {
            var h = EventRepeatIncrementChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> EventRepeatIntervalChanged;
        protected virtual void OnEventRepeatIntervalChanged(EventArgs e = null)
        {
            var h = EventRepeatIntervalChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> EventRepeatUntilChanged;
        protected virtual void OnEventRepeatUntilChanged(EventArgs e = null)
        {
            var h = EventRepeatUntilChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> EntrySessionsChanged;
        protected virtual void OnEntrySessionsChanged(EventArgs e = null)
        {
            var h = EntrySessionsChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> PlayerEntryMaximumChanged;
        protected virtual void OnPlayerEntryMaximumChanged(EventArgs e = null)
        {
            var h = PlayerEntryMaximumChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> EntrySpendGroupingChanged;
        protected virtual void OnEntrySpendGroupingChanged(EventArgs e = null)
        {
            var h = EntrySpendGroupingChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> EntrySpendTiersChanged;
        protected virtual void OnEntrySpendTiersChanged(EventArgs e = null)
        {
            var h = EntrySpendTiersChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> EntryVisitTypeChanged;
        protected virtual void OnEntryVisitTypeChanged(EventArgs e = null)
        {
            var h = EntryVisitTypeChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> EntryVisitTiersChanged;
        protected virtual void OnEntryVisitTiersChanged(EventArgs e = null)
        {
            var h = EntryVisitTiersChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> EntryPurchaseTypeChanged;
        protected virtual void OnEntryPurchaseTypeChanged(EventArgs e = null)
        {
            var h = EntryPurchaseTypeChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> EntryPurchaseGroupingChanged;
        protected virtual void OnEntryPurchaseGroupingChanged(EventArgs e = null)
        {
            var h = EntryPurchaseGroupingChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> EntryPurchaseTiersChanged;
        protected virtual void OnEntryPurchaseTiersChanged(EventArgs e = null)
        {
            var h = EntryPurchaseTiersChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> EntryPurchasePackageIdsChanged;
        protected virtual void OnEntryPurchasePackageIdsChanged(EventArgs e = null)
        {
            var h = EntryPurchasePackageIdsChanged;
            if(h != null)
                h(this, e);
        }

        public event EventHandler<EventArgs> EntryPurchaseProductIdsChanged;
        protected virtual void OnEntryPurchaseProductIdsChanged(EventArgs e = null)
        {
            var h = EntryPurchaseProductIdsChanged;
            if(h != null)
                h(this, e);
        }

        #endregion

        #region Member Variables
        private int? m_id = null;

        private string m_name = "New Drawing";
        bool m_active = false;
        String m_description = "";

        int m_entriesDrawn = 1;
        int m_minimumEntries = 10;
        int m_maximumDrawsPerPlayer = 1;
        bool m_showEntriesOnReceipts = false;
        bool m_playerPresenceRequired = true;

        DateTime m_initialEventEntryPeriodBegin = DateTime.Now.Date;
        DateTime m_initialEventEntryPeriodEnd = DateTime.Now.Date;
        DateTime? m_initialEventScheduledForWhen = null;
        Int16 m_eventRepeatIncrement = 0;
        String m_eventRepeatInterval;
        DateTime? m_eventRepeatUntil = null;
        List<Byte> m_entrySessions;

        int? m_playerEntryMaximum = null;

        SpendGrouping m_entrySpendGrouping = SpendGrouping.NONE;
        SortedSet<EntryTier<decimal>> m_entrySpendTiers;

        VisitType m_entryVisitType = VisitType.NONE;
        SortedSet<EntryTier<int>> m_entryVisitTiers;

        PurchaseType m_entryPurchaseType = PurchaseType.NONE;

        PurchaseGrouping m_entryPurchaseGrouping = PurchaseGrouping.BY_SESSION;
        SortedSet<EntryTier<int>> m_entryPurchaseTiers;
        List<int> m_entryPurchasePackageIds;
        List<int> m_entryPurchaseProductIds;

        #endregion

        #region Constructors
        public GeneralPlayerDrawing()
        {
            m_entrySessions = new List<byte>();

            m_entrySpendTiers = new SortedSet<EntryTier<decimal>>(EntryTier<decimal>.OverlapComparer);
            m_entryVisitTiers = new SortedSet<EntryTier<int>>(EntryTier<int>.OverlapComparer);
            m_entryPurchaseTiers = new SortedSet<EntryTier<int>>(EntryTier<int>.OverlapComparer);
            m_entryPurchasePackageIds = new List<int>();
            m_entryPurchaseProductIds = new List<int>();
        }

        public GeneralPlayerDrawing(string name, bool active, String description
            , int entriesDrawn, int minimumEntries, int maximumDrawsPerPlayer, bool showEntriesOnReceipts, bool playerPresenceRequired
            , DateTime entryWindowBegins, DateTime entryWindowEnds, DateTime? initialEventScheduledForWhen
            , Int16 eventRepeatIncrement, String eventRepeatInterval, DateTime? eventRepeatUntil, IEnumerable<Byte> entrySessions
            , int? playerEntryMaximum
            , SpendGrouping entryQualificationSpendGrouping, IEnumerable<EntryTier<decimal>> entrySpendTiers
            , VisitType entryQualificationVisitType, IEnumerable<EntryTier<int>> entryVisitTiers
            , PurchaseType entryQualificationPurchaseType, PurchaseGrouping entryQualificationPurchaseGrouping, IEnumerable<EntryTier<int>> entryPurchaseTiers, List<int> entryPurchasePackageIds, List<int> entryPurchaseProductIds
            , int? id = null)
        {
            m_id = id;

            m_name = name;
            m_active = active;
            m_description = description;

            m_entriesDrawn = entriesDrawn;
            m_minimumEntries = minimumEntries;
            m_maximumDrawsPerPlayer = maximumDrawsPerPlayer;
            m_showEntriesOnReceipts = showEntriesOnReceipts;
            m_playerPresenceRequired = playerPresenceRequired;

            m_initialEventEntryPeriodBegin = entryWindowBegins;
            m_initialEventEntryPeriodEnd = entryWindowEnds;
            m_initialEventScheduledForWhen = initialEventScheduledForWhen;
            m_eventRepeatIncrement = eventRepeatIncrement;
            m_eventRepeatInterval = eventRepeatInterval;
            m_eventRepeatUntil = eventRepeatUntil;
            m_entrySessions = new List<byte>(entrySessions);

            m_playerEntryMaximum = playerEntryMaximum;

            m_entrySpendGrouping = entryQualificationSpendGrouping;
            if(m_entrySpendGrouping == SpendGrouping.NONE)
                m_entrySpendTiers = new SortedSet<EntryTier<decimal>>();
            else
                m_entrySpendTiers = new SortedSet<EntryTier<decimal>>(entrySpendTiers, EntryTier<decimal>.OverlapComparer);

            m_entryVisitType = entryQualificationVisitType;
            if(m_entryVisitType == VisitType.NONE)
                m_entryVisitTiers = new SortedSet<EntryTier<int>>();
            else
                m_entryVisitTiers = new SortedSet<EntryTier<int>>(entryVisitTiers, EntryTier<int>.OverlapComparer);

            m_entryPurchaseType = entryQualificationPurchaseType;
            m_entryPurchaseGrouping = entryQualificationPurchaseGrouping;
            if(m_entryPurchaseType == PurchaseType.NONE)
                m_entryPurchaseTiers = new SortedSet<EntryTier<int>>();
            else
                m_entryPurchaseTiers = new SortedSet<EntryTier<int>>(entryPurchaseTiers, EntryTier<int>.OverlapComparer);

            if(m_entryPurchaseType == PurchaseType.PACKAGE && entryPurchasePackageIds != null)
                m_entryPurchasePackageIds = new List<int>(entryPurchasePackageIds);
            else
                m_entryPurchasePackageIds = new List<int>();

            if(m_entryPurchaseType == PurchaseType.PRODUCT && entryPurchaseProductIds != null)
                m_entryPurchaseProductIds = new List<int>(entryPurchaseProductIds);
            else
                m_entryPurchaseProductIds = new List<int>();

        }

        public GeneralPlayerDrawing(GeneralPlayerDrawing source, bool asNew = false)
        {
            if(asNew)
                Id = null;
            else
                m_id = source.m_id;

            m_name = source.m_name;
            m_active = source.m_active;
            m_description = source.m_description;

            m_entriesDrawn = source.m_entriesDrawn;
            m_minimumEntries = source.m_minimumEntries;
            m_maximumDrawsPerPlayer = source.m_maximumDrawsPerPlayer;
            m_showEntriesOnReceipts = source.m_showEntriesOnReceipts;
            m_playerPresenceRequired = source.m_playerPresenceRequired;

            m_initialEventEntryPeriodBegin = source.m_initialEventEntryPeriodBegin;
            m_initialEventEntryPeriodEnd = source.m_initialEventEntryPeriodEnd;
            m_initialEventScheduledForWhen = source.m_initialEventScheduledForWhen;
            m_eventRepeatIncrement = source.m_eventRepeatIncrement;
            m_eventRepeatInterval = source.m_eventRepeatInterval;
            m_eventRepeatUntil = source.m_eventRepeatUntil;

            m_entrySessions = new List<byte>();
            m_entrySessions.AddRange(source.m_entrySessions);

            m_playerEntryMaximum = source.m_playerEntryMaximum;

            m_entrySpendGrouping = source.m_entrySpendGrouping;
            m_entrySpendTiers = new SortedSet<EntryTier<decimal>>(source.m_entrySpendTiers, EntryTier<decimal>.OverlapComparer);

            m_entryVisitType = source.m_entryVisitType;
            m_entryVisitTiers = new SortedSet<EntryTier<int>>(source.m_entryVisitTiers, EntryTier<int>.OverlapComparer);

            m_entryPurchaseType = source.m_entryPurchaseType;
            m_entryPurchaseGrouping = source.m_entryPurchaseGrouping;
            m_entryPurchaseTiers = new SortedSet<EntryTier<int>>(source.m_entryPurchaseTiers, EntryTier<int>.OverlapComparer);
            m_entryPurchasePackageIds = new List<int>(source.m_entryPurchasePackageIds);
            m_entryPurchaseProductIds = new List<int>(source.m_entryPurchaseProductIds);

        }
        #endregion

        #region Properties
        public int? Id
        {
            get { return m_id; }
            internal set
            {
                if(m_id.HasValue != value.HasValue || (m_id.HasValue && value.HasValue && m_id.Value != value.Value))
                {
                    m_id = value;
                    OnIdChanged();
                }
            }
        }

        public string Name
        {
            get { return m_name; }
            set
            {
                if(m_name != value)
                {
                    m_name = value;
                    OnNameChanged();
                }
            }
        }
        public bool Active
        {
            get { return m_active; }
            set
            {
                if(m_active != value)
                {
                    m_active = value;
                    OnActiveChanged();
                }
            }
        }

        #region General/Common detail properties
        public String Description
        {
            get { return m_description; }
            set
            {
                if(m_description != value)
                {
                    m_description = value;
                    OnDescriptionChanged();
                }
            }
        }

        public int EntriesDrawn
        {
            get { return m_entriesDrawn; }
            set
            {
                if(m_entriesDrawn != value)
                {
                    m_entriesDrawn = value;
                    OnEntriesDrawnChanged();
                }
            }
        }

        public int MinimumEntries
        {
            get { return m_minimumEntries; }
            set
            {
                if(m_minimumEntries != value)
                {
                    m_minimumEntries = value;
                    OnMinimumEntriesChanged();
                }
            }
        }

        public int MaximumDrawsPerPlayer
        {
            get { return m_maximumDrawsPerPlayer; }
            set
            {
                if(m_maximumDrawsPerPlayer != value)
                {
                    m_maximumDrawsPerPlayer = value;
                    OnMaximumDrawsPerPlayerChanged();
                }
            }
        }

        public bool ShowEntriesOnReceipts
        {
            get { return m_showEntriesOnReceipts; }
            set
            {
                if(m_showEntriesOnReceipts != value)
                {
                    m_showEntriesOnReceipts = value;
                    OnShowEntriesOnReceiptsChanged();
                }
            }
        }

        public bool PlayerPresenceRequired
        {
            get { return m_playerPresenceRequired; }
            set
            {
                if(m_playerPresenceRequired != value)
                {
                    m_playerPresenceRequired = value;
                    OnPlayerPresenceRequiredChanged();
                }
            }
        }
        #endregion

        #region Entry Window Properties
        public DateTime InitialEventEntryPeriodBegin
        {
            get { return m_initialEventEntryPeriodBegin; }
            set
            {
                if(m_initialEventEntryPeriodBegin != value)
                {
                    m_initialEventEntryPeriodBegin = value;
                    OnInitialEventEntryPeriodBeginChanged();
                }
            }
        }

        public DateTime InitialEventEntryPeriodEnd
        {
            get { return m_initialEventEntryPeriodEnd; }
            set
            {
                if(m_initialEventEntryPeriodEnd != value)
                {
                    m_initialEventEntryPeriodEnd = value;
                    OnInitialEventEntryPeriodEndChanged();
                }
            }
        }

        public DateTime? InitialEventScheduledForWhen
        {
            get { return m_initialEventScheduledForWhen; }
            set
            {
                if(m_initialEventScheduledForWhen.HasValue != value.HasValue || (m_initialEventScheduledForWhen.HasValue && value.HasValue && m_initialEventScheduledForWhen.Value != value.Value))
                {
                    m_initialEventScheduledForWhen = value;
                    OnInitialEventScheduledForWhenChanged();
                }
            }
        }

        public Int16 EventRepeatIncrement
        {
            get { return m_eventRepeatIncrement; }
            set
            {
                if(m_eventRepeatIncrement != value)
                {
                    m_eventRepeatIncrement = value;
                    OnEventRepeatIncrementChanged();
                }
            }
        }

        public String EventRepeatInterval
        {
            get { return m_eventRepeatInterval; }
            set
            {
                if(m_eventRepeatInterval != value)
                {
                    m_eventRepeatInterval = value;
                    OnEventRepeatIntervalChanged();
                }
            }
        }

        public DateTime? EventRepeatUntil
        {
            get { return m_eventRepeatUntil; }
            set
            {
                if(m_eventRepeatUntil.HasValue != value.HasValue || (m_eventRepeatUntil.HasValue && value.HasValue && m_eventRepeatUntil.Value != value.Value))
                {
                    m_eventRepeatUntil = value;
                    OnEventRepeatUntilChanged();
                }
            }
        }

        public List<Byte> EntrySessionNumbers
        {
            get { return m_entrySessions; }
            set
            {
                if(m_entrySessions != value)
                {
                    m_entrySessions = value;
                    OnEntrySessionsChanged();
                }
            }
        }
        #endregion

        #region Entry Limit Properties
        public int? PlayerEntryMaximum
        {
            get { return m_playerEntryMaximum; }
            set
            {
                if(m_playerEntryMaximum != value)
                {
                    m_playerEntryMaximum = value;
                    OnPlayerEntryMaximumChanged();
                }
            }
        }
        #endregion

        #region Entry Qualification Properties

        public SpendGrouping EntrySpendGrouping
        {
            get { return m_entrySpendGrouping; }
            set
            {
                if(m_entrySpendGrouping != value)
                {
                    m_entrySpendGrouping = value;
                    OnEntrySpendGroupingChanged();
                }
            }
        }

        public SortedSet<EntryTier<decimal>> EntrySpendTiers
        {
            get { return m_entrySpendTiers; }
            set
            {
                if(m_entrySpendTiers != value)
                {
                    m_entrySpendTiers = value;
                    OnEntrySpendTiersChanged();
                }
            }
        }

        public VisitType EntryVisitType
        {
            get { return m_entryVisitType; }
            set
            {
                if(m_entryVisitType != value)
                {
                    m_entryVisitType = value;
                    OnEntryVisitTypeChanged();
                }
            }
        }

        public SortedSet<EntryTier<int>> EntryVisitTiers
        {
            get { return m_entryVisitTiers; }
            set
            {
                if(m_entryVisitTiers != value)
                {
                    m_entryVisitTiers = value;
                    OnEntryVisitTiersChanged();
                }
            }
        }

        public PurchaseType EntryPurchaseType
        {
            get { return m_entryPurchaseType; }
            set
            {
                if(m_entryPurchaseType != value)
                {
                    m_entryPurchaseType = value;
                    OnEntryPurchaseTypeChanged();
                }
            }
        }

        public PurchaseGrouping EntryPurchaseGrouping
        {
            get { return m_entryPurchaseGrouping; }
            set
            {
                if(m_entryPurchaseGrouping != value)
                {
                    m_entryPurchaseGrouping = value;
                    OnEntryPurchaseGroupingChanged();
                }
            }
        }

        public SortedSet<EntryTier<int>> EntryPurchaseTiers
        {
            get { return m_entryPurchaseTiers; }
            set
            {
                if(m_entryPurchaseTiers != value)
                {
                    m_entryPurchaseTiers = value;
                    OnEntryPurchaseTiersChanged();
                }
            }
        }

        public List<int> EntryPurchasePackageIds
        {
            get { return m_entryPurchasePackageIds; }
            set
            {
                if(m_entryPurchasePackageIds != value)
                {
                    m_entryPurchasePackageIds = value;
                    OnEntryPurchasePackageIdsChanged();
                }
            }
        }

        public List<int> EntryPurchaseProductIds
        {
            get { return m_entryPurchaseProductIds; }
            set
            {
                if(m_entryPurchaseProductIds != value)
                {
                    m_entryPurchaseProductIds = value;
                    OnEntryPurchaseProductIdsChanged();
                }
            }
        }
        #endregion

        #endregion

        #region Methods
        #endregion


    }
}
