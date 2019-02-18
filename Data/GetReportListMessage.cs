using System;
using System.Collections.Generic;
using System.Text;
using System.IO;



namespace GTI.Modules.Shared
{
    public enum ReportTypes
    {
        All=0,
        Sales=1,
        Paper=2,
        Player=3,
        Misc=4,
        Staff=5,
        POS=6,
        Bingo=7,
        Electronics=8,
        Exceptions=9,
        Custom=10,
        // TTP 50114
        TaxForms=11,
        Gaming=12,
        Inventory = 13 // Rally US1492
        , Accruals = 14 // US1831
        , Payouts = 15  // US1844
        , Texas = 16
        , Coupon = 17
        , SessionSummary = 19
        , Presales = 20
    }
   
    
   

    /// <summary>
    /// Represents the Get Report List Ex server message.
    /// </summary>
    public class GetReportListExMessage : ServerMessage
    {
        #region Constants and Data Types
        protected const int MinResponseSize = 6;
        #endregion

        #region Member Variables
        protected int m_typeId;
        protected int m_localeId;
        protected string m_cultureName;
        protected Dictionary<int, ReportInfo> m_reports;
        public int m_raffledisplaytextsetting;
        //public static int SystemSettingDrawingOrRaffle {get;set;}
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetReportListExMessage class.
        /// </summary>
        /// <param name="type">The type of reports to retrieve.</param>
        /// <param name="localeId">The LCID of the culture to use.</param>
        public GetReportListExMessage(ReportTypes type, int localeId)
        {
            m_id = 18129;
            m_strMessageName = "Get Report List Ex";
            m_typeId = (int)type;
            m_localeId = localeId;
            m_reports = new Dictionary<int, ReportInfo>();
        }

        /// <summary>
        /// Initializes a new instance of the GetReportListExMessage class.
        /// </summary>
        /// <param name="type">The type of reports to retrieve.</param>
        /// <param name="cultureName">The name of the culture to use.</param>
        public GetReportListExMessage(ReportTypes type, string cultureName)
            : this(type, 0)
        {
            m_cultureName = cultureName;
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Prepares the request to be sent to the server.
        /// </summary>
        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Report Type Id
            requestWriter.Write(m_typeId);

            //TA9032
            requestWriter.Write((Int32)0);

            // Locale Id
            requestWriter.Write(m_localeId);

            // Locale String
            if(!string.IsNullOrEmpty(m_cultureName))
            {
                requestWriter.Write((ushort)m_cultureName.Length);
                requestWriter.Write(m_cultureName.ToCharArray());
            }
            else
                requestWriter.Write((ushort)0);

            // Set the bytes to be sent.
            m_requestPayload = requestStream.ToArray();

            // Close the streams.
            requestWriter.Close();
        }

        /// <summary>
        /// Parses the response received from the server.
        /// </summary>
        protected override void UnpackResponse()
        {
            // Clear the previous results.
            m_reports.Clear();

            base.UnpackResponse();

            // Create the streams we will be reading from.
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Check the response length.
            if(responseStream.Length < MinResponseSize)
                throw new MessageWrongSizeException(m_strMessageName);

            //For drawing or raffle.



            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Get the count of reports.
                ushort count = responseReader.ReadUInt16();

                // Get all the reports.
                for(ushort x = 0; x < count; x++)
                {
                    ReportInfo tempReport = new ReportInfo();

                    // Report Id
                    tempReport.ID = responseReader.ReadInt32();

                    // Report Type
                    tempReport.TypeID = responseReader.ReadInt32();

                    // Report Display Name
                    tempReport.DisplayName = " " + (ReadString(responseReader) ?? "");
                
                    //Override the report display name per raffle or drawing.
                    //Get the reportID
                    if (tempReport.ID == 168 || tempReport.ID == 221 || tempReport.ID == 68  //Raffle Entrants ;Raffle Entrants by Date ;Raffle Winners
                        || tempReport.DisplayName.Contains("Raffle"))
                    //Since this is a fix name from db I just have to strip this out.
                    {
                        if (m_raffledisplaytextsetting == 2)//Its always going to be raffle so check if its 2 "Drawing"
                        {
                            //Replace the text.
                            tempReport.DisplayName = tempReport.DisplayName.Replace("Raffle", "Drawing");
                        }                          
                    }


                    // Report MD5
                    ushort byteLen = responseReader.ReadUInt16();

                    if(byteLen > 0)
                        tempReport.Hash = responseReader.ReadBytes(byteLen);

                    // Report File Name
                    tempReport.FileName = ReadString(responseReader);

                    tempReport.Parameters = new Dictionary<int, string>();

                    // Parameter Count
                    ushort paramCount = responseReader.ReadUInt16();

                    for(int y = 0; y < paramCount; y++)
                    {
                        // Parameter Id
                        int paramId = responseReader.ReadInt32();

                        // Parameter String
                        string paramName = ReadString(responseReader);

                        if(!tempReport.Parameters.ContainsKey(paramId)) // in case of bad data, just keep going
                            tempReport.Parameters.Add(paramId, paramName);
                    }

                    m_reports.Add(tempReport.ID, tempReport);
                }
            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException(m_strMessageName, e);
            }
            catch(Exception e)
            {
                throw new ServerException(m_strMessageName, e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the type of reports to retrieve.
        /// </summary>
        public ReportTypes Type
        {
            get
            {
                return (ReportTypes)m_typeId;
            }
            set
            {
                m_typeId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the LCID of the culture to use.
        /// </summary>
        /// <remarks>Setting this value will set the CultureName property to 
        /// nothing.</remarks>
        public int LocaleId
        {
            get
            {
                return m_localeId;
            }
            set
            {
                m_localeId = value;
                m_cultureName = null;
            }
        }

        /// <summary>
        /// Gets or sets name of the culture to use.
        /// </summary>
        /// <remarks>Setting this value will set the locale id to 0.</remarks>
        public string CultureName
        {
            get
            {
                return m_cultureName;
            }
            set
            {
                m_cultureName = value;
                m_localeId = 0;
            }
        }

        /// <summary>
        /// Gets all the reports retrieved from the server.
        /// </summary>
        public Dictionary<int, ReportInfo> Reports
        {
            get
            {
                return m_reports;
            }
        }
        #endregion
    }
}
