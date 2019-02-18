// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2008 GameTech
// International, Inc.

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Globalization;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using GameTech.Elite.Base;
using System.IO.Ports;

namespace GTI.Modules.Shared
{
    public class MSRSettings
    {
        public string MSRStart = string.Empty;
        public string MSREnd = string.Empty;
        public List<string> MSRFilters = new List<string>();
        public bool alwaysReturnAfterCardRead = false;

        public static string StringToPrintable(string text)
        {
            //convert to printable
            StringBuilder sb = new StringBuilder();

            foreach (char c in text)
            {
                if (c < ' ' || c > '~')
                    sb.Append("\\x" + Convert.ToByte(c).ToString("X2"));
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }

        public static string PrintableStringToString(string text)
        {
            //convert from printable
            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(@"^.*(\\x[0-9a-fA-F]{2})");
            System.Text.RegularExpressions.Match match;
            string hex = "0123456789ABCDEF";
            StringBuilder sb = new StringBuilder(text);

            while((match = rex.Match(sb.ToString())).Success)
            {
                string tmp = match.Groups[1].Value.ToUpper();
                byte b1 = (byte)(hex.IndexOf(tmp[2]) * 16);
                byte b2 = (byte)(hex.IndexOf(tmp[3]));
                char t = Convert.ToChar(b1 + b2);

                sb.Replace(match.Groups[1].Value, t.ToString());
            }

            return sb.ToString();
        }

        public void setFilters(string filtersData)
        {
            string[] filters = filtersData.Split(new char[] { '\xFF' });

            foreach (string f in filters)
            {
                if (f != string.Empty)
                    this.MSRFilters.Add(f);
            }
        }

        public void setReadTriggers(string triggers)
        {
            this.MSRStart = string.Empty;
            this.MSREnd = string.Empty;

            if (triggers == string.Empty)
                return;

            try
            {
                byte numberOfStartTriggers = Convert.ToByte(triggers.Substring(0, 2));
                byte numberOfStopTriggers = Convert.ToByte(triggers.Substring(numberOfStartTriggers + 2, 2));

                this.MSRStart = PrintableStringToString(triggers.Substring(2, numberOfStartTriggers));
                this.MSREnd = PrintableStringToString(triggers.Substring(4 + numberOfStartTriggers, numberOfStopTriggers));
            }
            catch (Exception)
            {
                this.MSRStart = string.Empty;
                this.MSREnd = string.Empty;
            }
        }
    }

    // PDTS 1064
    /// <summary>
    /// Provides data for CardSwiped events.
    /// </summary>
    public class MagneticCardSwipeArgs : EventArgs
    {
        #region Member Variables
        protected short m_track;
        protected string m_cardData;
        protected short m_MatchedOn;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MagneticCardSwipeArgs class.
        /// </summary>
        /// <param name="track">The track the data was on.</param>
        /// <param name="cardData">The data read in from the card 
        /// reader.</param>
        /// <param name="matched">The filter number the match was made on. -1=first track track 1, -2=first track track 2, -10=manual entry</param>/>
        public MagneticCardSwipeArgs(short track, string cardData, short matched)
        {
            m_track = track;
            m_cardData = cardData;
            m_MatchedOn = matched;
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets the track that the CardData was on or 0 if the track is 
        /// unknown.
        /// </summary>
        public short Track
        {
            get
            {
                return m_track;
            }
        }

        /// <summary>
        /// Gets the data read in from the card reader.
        /// </summary>
        public string CardData
        {
            get
            {
                return m_cardData;
            }
        }

        /// <summary>
        /// Gets the filter the match was made on. 
        /// -1=No filters track 1, -2=No filters track 2
        /// -10=Manual entry
        /// 0= No match
        /// </summary>
        public short MatchFilter
        {
            get
            {
                return m_MatchedOn;
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents the method that handles the CardSwiped event of the 
    /// MagneticCardReader class.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The MagneticCardSwipeArgs object that contains the event 
    /// data.</param>
    public delegate void MagneticCardSwipedHandler(object sender, MagneticCardSwipeArgs e);

    /// <summary>
    /// Represents which IMagneticCardReaderSources are to be loaded.
    /// </summary>
    public enum MagneticCardReaderMode
    {
        KeyboardOnly = 1,
        KeyboardAndCPCLTCP = 2
    }

    /// <summary>
    /// This class parses and signals the presence of magnetic card data from 
    /// the keyboard and, potentially, other sources.
    /// </summary>
    public class MagneticCardReader
    {
        #region Constants and Data Types
        protected const int MaxBufferSize = 1024;
        protected const string ErrorData = "e";
        #endregion

        #region Member Variables
        protected MSRSettings m_MSRSettings = null;
        protected List<IMagneticCardReaderSource> m_otherSources = new List<IMagneticCardReaderSource>();
        protected object m_readingSync = new object();
        protected bool m_readingCards;
        protected StringBuilder m_cardData = new StringBuilder();
        protected ISynchronizeInvoke m_syncObject;
        protected bool m_MSRInputInProgress = false;
        private SynchronizationContext m_syncContext = null;
        private Timer m_terminationTimer = null;
        private bool m_haveReadTerminator = false;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a magnetic card swipe has been detected via either 
        /// ProcessCharacter or other sources.
        /// </summary>
        public event MagneticCardSwipedHandler CardSwiped;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MagCardReader class.
        /// </summary>
        /// <param name="settings">The settings structure defining the characters
        /// used to determine card data in the keyboard stream and the regular 
        /// expression filters for the data.</param>
        /// <remarks>If settings contains empty elements, then 
        /// parsing card data with ProcessCharacter will not function.</remarks>
        public MagneticCardReader(MSRSettings settings)
        {
            m_MSRSettings = new MSRSettings();
            
            m_MSRSettings.MSRStart = settings.MSRStart;
            m_MSRSettings.MSREnd = settings.MSREnd;

            foreach (string f in settings.MSRFilters)
                m_MSRSettings.MSRFilters.Add(string.Copy(f));

            m_MSRSettings.alwaysReturnAfterCardRead = settings.alwaysReturnAfterCardRead;
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Adds a new card reader source that will be managed by 
        /// MagneticCardReader.
        /// </summary>
        /// <param name="source">The instance of the source to add.</param>
        /// <remarks>This method will call IMagneticCardReaderSource.Initialize 
        /// and, potentially, IMagneticCardReaderSource.BeginReading on the 
        /// specified source.</remarks>
        public void AddSource(IMagneticCardReaderSource source)
        {
            if(!m_otherSources.Contains(source))
            {
                m_otherSources.Add(source);
                source.Initialize(this);

                if(m_readingCards)
                    source.BeginReading();
            }
        }

        /// <summary>
        /// Removes a source that is currently managed by MagneticCardReader.
        /// </summary>
        /// <param name="source">The instance of the source to remove.</param>
        /// <remarks>This method will call IMagneticCardReaderSource.EndReading 
        /// and IMagneticCardReaderSource.Shutdown before the source is 
        /// removed.</remarks>
        public void RemoveSource(IMagneticCardReaderSource source)
        {
            int index = m_otherSources.IndexOf(source);

            if(index > -1)
            {
                m_otherSources[index].EndReading();
                m_otherSources[index].Shutdown();
                m_otherSources.RemoveAt(index);
            }
        }

        /// <summary>
        /// Removes all sources that are currently managed by 
        /// MagneticCardReader.
        /// </summary>
        /// <remarks>This method will call IMagneticCardReaderSource.EndReading 
        /// and IMagneticCardReaderSource.Shutdown for each source 
        /// removed.</remarks>
        public void RemoveAllSources()
        {
            foreach(IMagneticCardReaderSource source in m_otherSources)
            {
                source.EndReading();
                source.Shutdown();
            }

            m_otherSources.Clear();
        }

        /// <summary>
        /// Processes a string of characters as a card swipe. If the first character is an MSR
        /// starting character, the string will be processed (appending a terminating character if needed)
        /// and OnCardSwiped will be called to determine if the string matches any filters for the MSR; if so, 
        /// the owner will be notified.
        /// </summary>
        /// <param name="text">String to process as a card swipe.</param>
        /// <returns>True if processing was attempted and False if the string was empty, too long, or
        /// did not start with an MSR starting character.</returns>
        public bool ProcessString(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            if (m_MSRSettings.MSRStart.Contains(text.Substring(0, 1)))
            {
                if (!m_MSRSettings.MSREnd.Contains(text.Substring(text.Length - 1, 1)))
                    text = text + m_MSRSettings.MSREnd[0];

                m_cardData.Clear();
                m_cardData.Append(text);

                // Make sure we don't go over the max size.
                if (m_cardData.Length == MaxBufferSize) //there is no way this could be valid data, just stop the read
                {
                    Reset();
                    return false;
                }

                m_syncContext = SynchronizationContext.Current; //needed so timer thread can make a call on the main thread
                m_terminationTimer = new Timer(new TimerCallback(TerminationTimer_Tick));
                m_terminationTimer.Change(250, 0); //set timer to report card swiped in 1/4 second.

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Evaluates a character to see if it should be added to the card 
        /// buffer.  If this character is not a start of swipe character or the
        /// reader currently isn't scanning for cards, then this method 
        /// does nothing.
        /// </summary>
        /// <param name="character">The character to be evaluated.</param>
        /// <returns>true if the reader is scanning for characters and this was 
        /// a valid character; otherwise false.</returns>
        public bool ProcessCharacter(char character)
        {
            if (m_terminationTimer != null)
            {
                try
                {
                    m_terminationTimer.Dispose();
                    m_terminationTimer = null;
                }
                catch (Exception)
                {
                }
            }

            bool validChar = false;

            if(ReadingCards && m_MSRSettings.MSRStart != string.Empty && m_MSRSettings.MSREnd != string.Empty) 
            {
                bool isMSRStartCharacter = false;

                if (!m_MSRInputInProgress)
                {
                    // First check to see if this is the start of a card swipe.
                    if (m_MSRSettings.MSRStart.Contains(new string(character, 1)))
                        isMSRStartCharacter = true;
                }

                if(isMSRStartCharacter) // Clear the buffer and start reading.
                {
                    m_cardData.Length = 0;
                    m_haveReadTerminator = false;
                    
                    // Append the char to the card number buffer (the buffer contains the start and stop characters).
                    m_cardData.Append(character);

                    m_MSRInputInProgress = true;
                    validChar = true;
                }
                else if(m_MSRInputInProgress)
                {
                    // We are currently reading a card, should we stop?
                    if (m_MSRSettings.MSREnd.Contains(new string(character, 1)))
                    {
                        // Append the char to the card number buffer (the buffer contains the start and stop characters).
                        m_cardData.Append(character);
                        m_haveReadTerminator = true;

                        //this is our terminator if we don't get another character within 1/4 second
                        m_syncContext = SynchronizationContext.Current; //needed so timer thread can make a call on the main thread
                        m_terminationTimer = new Timer(new TimerCallback(TerminationTimer_Tick));
                        m_terminationTimer.Change(250, 0); //set timer to report card swiped in 1/4 second.

                        validChar = true;
                    }
                    else //if(!char.IsControl(character))
                    {
                        // Append the next char to the mag. card number buffer.
                        m_cardData.Append(character);

                        // Make sure we don't go over the max size.
                        if(m_cardData.Length == MaxBufferSize) //there is no way this could be valid data, just stop the read
                            Reset();

                        if (m_haveReadTerminator)
                        {
                            //this could be junk after our terminator (someone set up a terminator that isn't really out terminator). If we don't get another character within 1/4 second then assume swip finished.
                            m_syncContext = SynchronizationContext.Current; //needed so timer thread can make a call on the main thread
                            m_terminationTimer = new Timer(new TimerCallback(TerminationTimer_Tick));
                            m_terminationTimer.Change(250, 0); //set timer to report card swiped in 1/4 second.
                        }

                        validChar = true;
                    }
                }
            }

            return validChar;
        }

        private void CardSwipeComplete(object state)
        {
            // Record the swipe and reset.
            OnCardSwiped(this, 0, m_cardData.ToString());

            Reset();
        }

        /// <summary>
        /// Timer callback for 1/4 second with no characters to process after a terminating character.
        /// </summary>
        /// <param name="state">The timer object.</param>
        private void TerminationTimer_Tick(object state)
        {
            try
            {
                Timer t = (Timer)state;

                t.Dispose();

                m_syncContext.Send(CardSwipeComplete, null); //call CardSwipeComplete on main thread

                m_terminationTimer = null;
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Clears any current card data found with the ProcessCharacter method.
        /// </summary>
        public void Reset()
        {
            m_cardData.Length = 0;
            m_MSRInputInProgress = false;
            m_haveReadTerminator = false;
        }

        /// <summary>
        /// Tells the ProcessCharacter method and all sources to begin 
        /// allowing card swipes.
        /// </summary>
        public void BeginReading()
        {
            lock(m_readingSync)
            {
                m_readingCards = true;

                Reset();

                // Notify all sources.
                foreach(IMagneticCardReaderSource source in m_otherSources)
                    source.BeginReading();
            }
        }

        /// <summary>
        /// Tells the ProcessCharacter method and all sources to stop allowing 
        /// card swipes.
        /// </summary>
        public void EndReading()
        {
            lock(m_readingSync)
            {
                m_readingCards = false;

                Reset();

                // Notify all sources.
                foreach(IMagneticCardReaderSource source in m_otherSources)
                    source.EndReading();
            }
        }

        /// <summary>
        /// This method is for sources to tell the MagneticCardReader that it 
        /// has detected a swipe.
        /// </summary>
        /// <param name="sender">The caller of this method.</param>
        /// <param name="track">The track that the card data was on or 0 if the 
        /// track is unknown.</param>
        /// <param name="data">The data read in from the card reader.</param>
        public void CardSwipeDetected(object sender, short track, string data)
        {
            OnCardSwiped(sender, track, data);
        }

        /// <summary>
        /// Raises the CardSwiped event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="track">The track that the card data was on or 0 if the 
        /// track is unknown.</param>
        /// <param name="data">The data read in from the card reader.</param>
        protected virtual void OnCardSwiped(object sender, short track, string data)
        {
            MagneticCardSwipedHandler handler = CardSwiped;

            if(ReadingCards && handler != null && data.ToLower(CultureInfo.InvariantCulture) != ErrorData)
            {
                string filteredData = data;
                short filterMatch = 0;
                bool ignored = false;

                //filter out the account number
                if (!data.Contains("?")) //no track end, assume manual entry (% or ; followed by the account number terminated by a CR)
                {
                    //Get first number
                    filteredData = EnhancedRegularExpression.Match(@"^([0123456789a-zA-Z]*)", data.Substring(1));
                    filterMatch = -10; //manual
                }
                else //not manual entry
                {
                    //clean up the card data
                    int end = data.Length - 1;

                    while (end >= 0 && m_MSRSettings.MSREnd.IndexOf(data[end]) == -1)
                        end--;

                    if (end < data.Length - 1) //junk on end
                        data = data.Substring(0, end + 1);

                    if (m_MSRSettings.MSRFilters.Count == 0) //no filters, just try for the number
                    {
                        //try track #1
                        filteredData = EnhancedRegularExpression.Match(@"^[^%]*%[a-zA-Z](\d*)", data);

                        if (filteredData != "")
                            filterMatch = -1;
                        else //try track #2
                        {
                            filteredData = EnhancedRegularExpression.Match(@"^[^;]*;(\d*)", data);
                            
                            if (filteredData != "")
                                filterMatch = -2;
                        }
                    }
                    else //try filters until we match
                    {
                        filterMatch = 0;

                        foreach (string filter in m_MSRSettings.MSRFilters)
                        {
                            filteredData = EnhancedRegularExpression.Match(filter, data);
                            filterMatch++;

                            if (filteredData.ToLower() == "ignore")
                            {
                                ignored = true;
                                filteredData = string.Empty;
                                break;
                            }

                            if (filteredData != string.Empty)
                                break;
                        }
                    }
                }

                if (filteredData == string.Empty) //no matches, forget about it
                {
                    if (!m_MSRSettings.alwaysReturnAfterCardRead)
                        return;

                    if(!ignored)
                        filterMatch = 0;

                    track = 0;
                }
                                             
                if(m_syncObject == null || !m_syncObject.InvokeRequired)
                    handler(sender, new MagneticCardSwipeArgs(track, filteredData, filterMatch));
                else
                    m_syncObject.Invoke(handler, new object[] { sender, new MagneticCardSwipeArgs(track, filteredData, filterMatch) });
            }
        }

        /// <summary>
        /// Destroys the instance of this class.
        /// </summary>
        ~MagneticCardReader()
        {
            RemoveAllSources();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets whether the reader is currently scanning for card swipes.
        /// </summary>
        public bool ReadingCards
        {
            get
            {
                lock(m_readingSync)
                {
                    return m_readingCards;
                }
            }
        }

        public bool MSRInputInProgress
        {
            get
            {
                lock(m_readingSync)
                {
                    return m_MSRInputInProgress;
                }
            }
        }

        public string MSRStartCharacters
        {
            get
            {
                return m_MSRSettings.MSRStart;
            }
        }

        /// <summary>
        /// Gets or sets the object used to marshal event-handler calls that 
        /// are issued when a card has been swiped.
        /// </summary>
        /// <remarks>
        /// When SynchronizingObject is null, the method that handles the 
        /// CardSwiped event might be called on a background thread. When the 
        /// CardSwiped event is handled by a visual Windows Forms component, 
        /// such as a button, accessing the component on another thread might 
        /// result in an exception or just might not work. Avoid this effect by 
        /// setting SynchronizingObject to a Windows Forms component, which 
        /// causes the method that handles the CardSwiped event to be called on 
        /// the same thread that the component was created on.
        /// </remarks>
        public ISynchronizeInvoke SynchronizingObject
        {
            get
            {
                return m_syncObject;
            }
            set
            {
                m_syncObject = value;
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents a printer with a magnetic card swipe that can communicate 
    /// those swipes on a TCP port with the Comtec Printer Control Language.
    /// </summary>
    public class CPCLPrinterTCPSource : IMagneticCardReaderSource, IDisposable
    {
        #region Constants and Data Types
        protected const int DefaultPort = 6101;
        protected const short DefaultTrack = 2;
        protected const int ThreadPause = 1000; // Milliseconds
        protected const int TCPTimeout = 1500; // Milliseconds
        protected const string CPCLVersion = "! U1 VERSION";
        protected const int VersionLength = 4;
        protected const string CPCLMagInit = "! U1 MCR 0 T{0} MULTIPLE QUERY PREFIX {1} POSTFIX ? NTN DEL   ";
        protected const string CPCLMagQuery = "! U1 MCR-QUERY";
        protected const string CPCLMagEnd = "! U1 MCR-CAN";
        #endregion

        #region Member Variables
        protected bool m_disposed;
        protected bool m_initialized;
        protected MagneticCardReader m_parent;
        protected object m_settingSync = new object();
        protected string m_serverName;
        protected int m_port = DefaultPort;
        protected short m_track = DefaultTrack;
        protected object m_readingSync = new object();
        protected bool m_reading;
        protected TcpClient m_tcpClient;
        protected Thread m_listenThread;
        protected object m_terminateSync = new object();
        protected bool m_terminateThread;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the CPCLPrinterTCPSource class.
        /// </summary>
        /// <param name="serverName">The name or IP address of the 
        /// printer.</param>
        /// <param name="port">The port on the printer to send commands 
        /// to.</param>
        /// <param name="track">The card track to read data from.</param>
        public CPCLPrinterTCPSource(string serverName, int port, short track)
        {
            m_serverName = serverName;
            m_port = port;
            m_track = track;
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Attempts to read the source's ServerName, Port, and Track from 
        /// the specified string.
        /// </summary>
        /// <param name="settings">The string that contains ServerName, 
        /// Port, and Track.</param>
        /// <exception cref="System.FormatException">The string was in an unexpected 
        /// format.</exception>
        public void SetSettingsFromString(string settings)
        {
            string server;
            int port;
            short track;

            SettingsFromString(settings, out server, out port, out track);

            ServerName = server;
            Port = port;
            Track = track;
        }

        /// <summary>
        /// Performs any initialization needed for the card reader.
        /// </summary>
        /// <param name="parent">The MagneticCardReader instance to which this 
        /// reader belongs.</param>
        public void Initialize(MagneticCardReader parent)
        {
            if(!m_initialized)
            {
                try
                {
                    m_parent = parent;

                    // Start the listening thread for the card swipes.
                    lock(m_terminateSync)
                    {
                        m_terminateThread = false;

                        m_listenThread = new Thread(ThreadWorker);
                        m_listenThread.IsBackground = true;

                        m_listenThread.Start();
                    }

                    m_initialized = true;
                }
                catch
                {
                    Shutdown();
                }
            }
        }

        /// <summary>
        /// Starts the card reading process.
        /// </summary>
        public void BeginReading()
        {
            if(m_initialized)
            {
                lock(m_readingSync)
                {
                    m_reading = true;
                }
            }
        }

        /// <summary>
        /// The method that reads for cards.
        /// </summary>
        private void ThreadWorker()
        {
            while(!TerminateThread)
            {
                try
                {
                    NetworkStream stream = null;
                    StreamWriter writer = null;
                    StreamReader reader = null;

                    // Are we reading for cards?
                    if(ReadingCards)
                    {
                        char[] buffer;

                        // Check our TCP connection to make sure we are open and 
                        // ready to go.
                        if(CheckConnection())
                        {
                            // We had the restablish the connection, so make
                            // sure we are talking to a CPCL printer.
                            stream = m_tcpClient.GetStream();
                            writer = new StreamWriter(stream);
                            reader = new StreamReader(stream);

                            writer.WriteLine(CPCLVersion);
                            writer.Flush();

                            // Attempt to read the version.
                            buffer = new char[VersionLength];
                            reader.Read(buffer, 0, buffer.Length);

                            // Check to make sure it returned valid values.
                            for(int x = 0; x < buffer.Length; x++)
                            {
                                if(!char.IsLetterOrDigit(buffer[x]))
                                    throw new ApplicationException();
                            }

                            // Everything seems okay, so send the mag. init function.
                            short track;

                            lock(m_settingSync)
                            {
                                track = m_track;
                            }

                            writer.WriteLine(string.Format(CPCLMagInit, track, (track == 1?'%':';')));
                            writer.Flush();
                        }

                        // Is there any data to read?
                        if(m_tcpClient != null && m_tcpClient.Connected)
                        {
                            if(writer == null)
                            {
                                stream = m_tcpClient.GetStream();
                                writer = new StreamWriter(stream);
                            }

                            if(reader == null)
                            {
                                stream = m_tcpClient.GetStream();
                                reader = new StreamReader(stream);
                            }

                            writer.WriteLine(CPCLMagQuery);
                            writer.Flush();

                            if(m_tcpClient.Available > 0)
                            {
                                buffer = new char[m_tcpClient.Available];
                                reader.Read(buffer, 0, buffer.Length);

                                // Send the data to the parent class.
                                m_parent.CardSwipeDetected(this, m_track, new string(buffer).Trim());
                            }
                        }
                    }
                    else
                    {
                        // We are no longer reading, should we disconnect?
                        if(m_tcpClient != null && m_tcpClient.Connected)
                        {
                            stream = m_tcpClient.GetStream();
                            writer = new StreamWriter(stream);

                            writer.WriteLine(CPCLMagEnd);
                            writer.Flush();
                        }

                        CloseConnection();
                    }
                }
                catch
                {
                    CloseConnection();
                }

                Thread.Sleep(ThreadPause);
            }

            // Clean up the connection.
            CloseConnection();
        }

        /// <summary>
        /// Checks to see if the TCP client is valid and open.  If it is not
        /// then it will attempt to establish one.
        /// </summary>
        /// <returns>true if the connection was opened; otherwise 
        /// false.</returns>
        protected bool CheckConnection()
        {
            if(m_tcpClient == null)
            {
                m_tcpClient = new TcpClient();
                m_tcpClient.ReceiveTimeout = TCPTimeout;
                m_tcpClient.SendTimeout = TCPTimeout;
            }

            if(!m_tcpClient.Connected)
            {
                // Attempt to connect.
                string address;
                int port;

                lock(m_settingSync)
                {
                    address = m_serverName;
                    port = m_port;
                }

                m_tcpClient.Connect(address, port);

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// This method will close the TCP connection if it exists.
        /// </summary>
        protected void CloseConnection()
        {
            if(m_tcpClient != null)
            {
                m_tcpClient.Close();
                m_tcpClient = null;
            }
        }

        /// <summary>
        /// Stops the card reading process.
        /// </summary>
        public void EndReading()
        {
            if(m_initialized)
            {
                lock(m_readingSync)
                {
                    m_reading = false;
                }
            }
        }

        /// <summary>
        /// Performs any cleanup needed for the card reader.
        /// </summary>
        public void Shutdown()
        {
            // Tell the thread to shut down if it is running.
            if(m_listenThread != null && m_listenThread.IsAlive)
            {
                lock(m_terminateSync)
                {
                    m_terminateThread = true;
                }

                m_listenThread.Join();
            }
        }

        /// <summary>
        /// Releases all resources used by CPCLPrinterTCPSource.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all resources used by CPCLPrinterTCPSource.
        /// </summary>
        /// <param name="disposing">Whether this function is being called from 
        /// user code.</param>
        private void Dispose(bool disposing)
        {
            if(!m_disposed)
            {
                if(disposing)
                    Shutdown();
            }

            m_disposed = true;
        }

        /// <summary>
        /// Destroys the instance of this class.
        /// </summary>
        ~CPCLPrinterTCPSource()
        {
            Dispose(false);
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Takes the specified settings and creates a string which contains 
        /// those settings for later use with the SettingsFromString method.
        /// </summary>
        /// <param name="serverName">The name or IP address of the 
        /// printer.</param>
        /// <param name="port">The port on the printer to send commands 
        /// to.</param>
        /// <param name="track">The card track to read data from.</param>
        /// <returns>A string which contains all the settings.</returns>
        public static string SettingsToString(string serverName, int port, short track)
        {
            return serverName + "," + port.ToString(CultureInfo.InvariantCulture) + "," + track.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Attempts to read the a ServerName, Port, and Track from the 
        /// specified string.
        /// </summary>
        /// <param name="settings">The string that contains ServerName, 
        /// Port, and Track.</param>
        /// <exception cref="System.FormatException">The string was in an unexpected 
        /// format.</exception>
        public static void SettingsFromString(string settings, out string serverName, out int port, out short track)
        {
            if(string.IsNullOrEmpty(settings))
                throw new FormatException("settings");

            try
            {
                string[] values = settings.Split(new char[] { ',' });

                serverName = values[0];
                port = Convert.ToInt32(values[1], CultureInfo.InvariantCulture);
                track = Convert.ToInt16(values[2], CultureInfo.InvariantCulture);
            }
            catch(Exception e)
            {
                throw new FormatException("settings", e);
            }
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets whether the source is currently reading for cards.
        /// </summary>
        public bool ReadingCards
        {
            get
            {
                lock(m_readingSync)
                {
                    return m_reading;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name or IP address of the printer.</param>
        /// </summary>
        /// <remarks>A change in this value will not take effect until 
        /// BeginReading is called.</remarks>
        public string ServerName
        {
            get
            {
                lock(m_settingSync)
                {
                    return m_serverName;
                }
            }
            set
            {
                lock(m_settingSync)
                {
                    m_serverName = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the port on the printer to send commands to.
        /// </summary>
        /// <remarks>A change in this value will not take effect until 
        /// BeginReading is called.</remarks>
        public int Port
        {
            get
            {
                lock(m_settingSync)
                {
                    return m_port;
                }
            }
            set
            {
                lock(m_settingSync)
                {
                    m_port = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the card track to read data from.
        /// </summary>
        /// <remarks>A change in this value will not take effect until 
        /// BeginReading is called.</remarks>
        public short Track
        {
            get
            {
                lock(m_settingSync)
                {
                    return m_track;
                }
            }
            set
            {
                lock(m_settingSync)
                {
                    m_track = value;
                }
            }
        }

        /// <summary>
        /// Gets whether the worker thread should be terminated.
        /// </summary>
        protected bool TerminateThread
        {
            get
            {
                lock(m_terminateSync)
                {
                    return m_terminateThread;
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents a COM port (or virtual COM port) that is hooked to a magnetic card reader
    /// </summary>
    public class SerialMagneticCardReaderSource : IMagneticCardReaderSource, IDisposable
    {
        #region Member Variables

        protected bool m_disposed;
        protected MagneticCardReader m_parent;
        protected SerialPort serial;

        #endregion

        #region Public Properties

        /// <summary>
        /// Whether or not the Serial port is open
        /// </summary>
        public bool IsOpen
        {
            get { return serial != null && serial.IsOpen; }
        }

        /// <summary>
        /// The name of the serial port this class encapsulates
        /// </summary>
        public string PortName
        {
            get { return serial.PortName; }
            set { serial.PortName = value; }
        }

        /// <summary>
        /// Gets whether the source is currently reading for cards.
        /// </summary>
        public bool ReadingCards
        {
            get
            {
                return IsOpen;
            }
        }
        #endregion

        public SerialMagneticCardReaderSource(string portName = null, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One)
        {
            if (!String.IsNullOrWhiteSpace(portName)) //otherwise use it as a pretend serial with other classes sending in the data and triggering the event
            {
                serial = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
                serial.DataReceived += new SerialDataReceivedEventHandler(serial_DataReceived);
                serial.ErrorReceived += new SerialErrorReceivedEventHandler(serial_ErrorReceived);
            }
        }

        public void Dispose()
        {
            if (serial != null)
            {
                if (serial.IsOpen)
                    serial.Close();
                serial.Dispose();
            }
        }

        /// <summary>
        /// Tries to open the serial port. If unsuccessful, logs the error and continues
        /// </summary>
        public void Open()
        {
            try
            {
                if (serial != null && !serial.IsOpen)
                {
                    serial.Open();
                    //LoggerSingleton.Log(com.Fortunet.FortunetLogger.Interfaces.LogLevel.Message, "Started Serial Port: " + serial.PortName);
                }
            }
            catch (Exception)
            {
                //LoggerSingleton.Log(com.Fortunet.FortunetLogger.Interfaces.LogLevel.Error, Resources.SERIAL_OPEN_ERROR + " " + serial.PortName + "\n" + ex);
            }
        }

        /// <summary>
        /// Closes and disposes of the serial port
        /// </summary>
        public void Close()
        {
            try
            {
                if (serial.IsOpen)
                    serial.Close();
                serial.DataReceived -= new SerialDataReceivedEventHandler(serial_DataReceived);
                serial.Dispose();
            }
            catch (Exception ex)
            {
                //LoggerSingleton.Log(com.Fortunet.FortunetLogger.Interfaces.LogLevel.Error, Resources.SERIAL_CLOSE_ERROR + " " + serial.PortName + "\n" + ex);
            }
        }

        /// <summary>
        /// Data receieved event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serial.IsOpen)
            {
                List<char> readBuffer = new List<char>();
                while (serial.BytesToRead > 0)
                {
                    readBuffer.Add((char)serial.ReadByte());
                }

                if (readBuffer.Count > 0)
                    m_parent.CardSwipeDetected(this, 1, new string(readBuffer.ToArray()).Trim());
            }
        }

        /// <summary>
        /// Logs any errors that are received from the serial
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void serial_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            //LoggerSingleton.Log(com.Fortunet.FortunetLogger.Interfaces.LogLevel.Error, Resources.GENERAL_ERROR + " " + serial.PortName + "\n" + e);
        }

        #region IMagneticCardReaderSource Implementation

        /// <summary>
        /// Performs any initialization needed for the card reader.
        /// </summary>
        /// <param name="parent">The MagneticCardReader instance to which this 
        /// reader belongs.</param>
        public void Initialize(MagneticCardReader parent)
        {
            m_parent = parent;
        }

        /// <summary>
        /// Starts the card reading process.
        /// </summary>
        public void BeginReading()
        {
            Open();
        }

        /// <summary>
        /// Stops the card reading process.
        /// </summary>
        public void EndReading()
        {
            if (serial.IsOpen)
                serial.Close();
        }

        /// <summary>
        /// Performs any cleanup needed for the card reader.
        /// </summary>
        public void Shutdown()
        {
            Close();
        }

        #endregion
    }

    /// <summary>
    /// Defines methods and properties for magnetic card readers.
    /// </summary>
    public interface IMagneticCardReaderSource
    {
        #region Member Methods
        /// <summary>
        /// Performs any initialization needed for the card reader.
        /// </summary>
        /// <param name="parent">The MagneticCardReader instance to which this 
        /// reader belongs.</param>
        void Initialize(MagneticCardReader parent);

        /// <summary>
        /// Starts the card reading process.
        /// </summary>
        void BeginReading();

        /// <summary>
        /// Stops the card reading process.
        /// </summary>
        void EndReading();

        /// <summary>
        /// Performs any cleanup needed for the card reader.
        /// </summary>
        void Shutdown();
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets whether the source is currently reading for cards.
        /// </summary>
        bool ReadingCards
        {
            get;
        }
        #endregion
    }
}
