// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2016 FortuNet, Inc

using System;

namespace GTI.Modules.Shared
{
    /// <summary>
    ///  Represents a tv channel for use by player units with tv tuners
    /// </summary>
    public class Channel
    {
        #region Member Variables
        protected int m_ChannelID = 0;
        protected int m_ChannelNumber = 0;
        protected string m_ChannelName = string.Empty;
        protected int m_Frequency = 0;
        protected int m_MajorChannel = 0;
        protected int m_MinorChannel = 0;
        protected bool m_Enabled = true;
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the channel ID
        /// </summary>
        public int ChannelID
        {
            get
            {
                return m_ChannelID;
            }
            set
            {
                m_ChannelID = value;
            }
        }


        /// <summary>
        /// Gets or sets the channel number
        /// </summary>
        public int ChannelNumber
        {
            get 
            {
                return m_ChannelNumber;
            }
            set
            {
                m_ChannelNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the channel name
        /// </summary>
        public string ChannelName
        {
            get
            {
                return m_ChannelName;
            }
            set
            {
                m_ChannelName = value;
            }
        }

        /// <summary>
        /// Gets or sets the channel frequency
        /// </summary>
        public int ChannelFrequency
        {
            get
            {
                return m_Frequency;
            }
            set
            {
                m_Frequency = value;
            }
        }

        /// <summary>
        /// Gets or sets the major channel
        /// </summary>
        public int MajorChannel
        {
            get
            {
                return m_MajorChannel;
            }
            set
            {
                m_MajorChannel = value;
            }
        }

        /// <summary>
        /// Gets or sets the minor channel
        /// </summary>
        public int MinorChannel
        {
            get
            {
                return m_MinorChannel;
            }
            set
            {
                m_MinorChannel = value;
            }
        }

        /// <summary>
        /// Gets or sets the Active flag.
        /// </summary>
        public bool Enabled 
        {
            get
            {
                return m_Enabled;
            }
            set
            {
                m_Enabled = value;
            }
        }
        #endregion
    }
}
