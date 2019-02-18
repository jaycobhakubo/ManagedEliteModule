// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.IO;
using System.Text;
using System.Globalization;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a Get Operator Data server message.
    /// </summary>
    public class GetOperatorDataMessage : ServerMessage
    {
        #region Member Variables
        protected int m_operatorId = 0;
        protected string m_name = string.Empty;
        protected string m_receiptHeaderLine1 = string.Empty;
        protected string m_receiptHeaderLine2 = string.Empty;
        protected string m_receiptHeaderLine3 = string.Empty;
        protected string m_receiptFooterLine1 = string.Empty;
        protected string m_receiptFooterLine2 = string.Empty;
        protected string m_receiptFooterLine3 = string.Empty;
        protected short m_maxCardLimit = 0;
        protected bool m_sameCards = false;
        protected bool m_consecutiveCards = false;
        protected bool m_printBarCode = false;
        protected bool m_playerTrackingEnabled = false;
        protected bool m_printCardFaces = false;
        protected decimal m_salesTax = 0M;
        protected bool m_printCardNumbers = false;
        protected DateTime m_endOfDay = DateTime.MinValue;
        protected decimal m_fixedFee = 0M;
        protected decimal m_travelerFee = 0M;
        protected decimal m_trackerFee = 0M;
        protected decimal m_miniFee = 0M;
        protected decimal m_traveler2Fee; // PDTS 964, Rally US765
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetOperatorDataMessage class.
        /// </summary>
        public GetOperatorDataMessage()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetOperatorDataMessage class
        /// with the specified operator id.
        /// </summary>
        /// <param name="operatorId">The id of the operator to get 
        /// data for.</param>
        public GetOperatorDataMessage(int operatorId)
        {
            m_id = 18021; // Get Operator Data
            m_operatorId = operatorId;
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

            // Operator Id
            requestWriter.Write(m_operatorId);

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
            base.UnpackResponse();

            // Create the streams we will be reading from.
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Try to unpack the data.
            try
            {
                // Seek past return code and operator id.
                responseReader.BaseStream.Seek(sizeof(int) + sizeof(int), SeekOrigin.Begin);

                // Operator Name
                ushort stringLen = responseReader.ReadUInt16();
                m_name = new string(responseReader.ReadChars(stringLen));

                // Receipt Header Line 1
                stringLen = responseReader.ReadUInt16();
                m_receiptHeaderLine1 = new string(responseReader.ReadChars(stringLen));

                // Receipt Header Line 2
                stringLen = responseReader.ReadUInt16();
                m_receiptHeaderLine2 = new string(responseReader.ReadChars(stringLen));

                // Receipt Header Line 3
                stringLen = responseReader.ReadUInt16();
                m_receiptHeaderLine3 = new string(responseReader.ReadChars(stringLen));

                // Receipt Footer Line 1
                stringLen = responseReader.ReadUInt16();
                m_receiptFooterLine1 = new string(responseReader.ReadChars(stringLen));

                // Receipt Footer Line 2
                stringLen = responseReader.ReadUInt16();
                m_receiptFooterLine2 = new string(responseReader.ReadChars(stringLen));

                // Receipt Footer Line 3
                stringLen = responseReader.ReadUInt16();
                m_receiptFooterLine3 = new string(responseReader.ReadChars(stringLen));

                // Max. Card Limit
                m_maxCardLimit = responseReader.ReadInt16();

                // Same Cards
                m_sameCards = responseReader.ReadBoolean();

                // Consecutive Cards
                m_consecutiveCards = responseReader.ReadBoolean();

                // Print Bar Code
                m_printBarCode = responseReader.ReadBoolean();

                // Player Tracking Enabled
                m_playerTrackingEnabled = responseReader.ReadBoolean();

                // Print Card Faces
                m_printCardFaces = responseReader.ReadBoolean();

                // Sales Tax
                stringLen = responseReader.ReadUInt16();
                string tempDec = new string(responseReader.ReadChars(stringLen));

                if(tempDec != string.Empty)
                    m_salesTax = decimal.Parse(tempDec, CultureInfo.InvariantCulture);

                // Print Card Numbers
                m_printCardNumbers = responseReader.ReadBoolean();

                // End of Day
                stringLen = responseReader.ReadUInt16();
                string tempDate = new string(responseReader.ReadChars(stringLen));

                if(tempDate != string.Empty)
                    m_endOfDay = DateTime.Parse("1/1/1 " + tempDate, CultureInfo.InvariantCulture);

                // Operator Fee Count
                ushort feeCount = responseReader.ReadUInt16();

                // Operator Fees
                for(ushort x = 0; x < feeCount; x++)
                {
                    // Device Id
                    int deviceId = responseReader.ReadInt32();

                    // Fee
                    stringLen = responseReader.ReadUInt16();
                    tempDec = new string(responseReader.ReadChars(stringLen));

                    if(tempDec != string.Empty)
                    {
                        if(Device.Fixed.Id == deviceId)
                            m_fixedFee = decimal.Parse(tempDec, CultureInfo.InvariantCulture);
                        else if(Device.Traveler.Id == deviceId)
                            m_travelerFee = decimal.Parse(tempDec, CultureInfo.InvariantCulture);
                        else if(Device.Tracker.Id == deviceId)
                            m_trackerFee = decimal.Parse(tempDec, CultureInfo.InvariantCulture);
                        else if(Device.Mini.Id == deviceId)
                            m_miniFee = decimal.Parse(tempDec, CultureInfo.InvariantCulture);
                        else if(Device.Traveler2.Id == deviceId) // PDTS 964, Rally US765
                            m_traveler2Fee = decimal.Parse(tempDec, CultureInfo.InvariantCulture);
                    }
                }
            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Operator Data", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Get Operator Data", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the id of the operator to get data for.
        /// </summary>
        public int OperatorId
        {
            get
            {
                return m_operatorId;
            }
            set
            {
                m_operatorId = value;
            }
        }

        /// <summary>
        /// Gets the name of the operator received from the server.
        /// </summary>
        public string Name
        {
            get
            {
                return m_name;
            }
        }

        /// <summary>
        /// Gets the operator's first receipt line received from the server.
        /// </summary>
        public string ReceiptHeaderLine1
        {
            get
            {
                return m_receiptHeaderLine1;
            }
        }

        /// <summary>
        /// Gets the operator's second receipt line received from the server.
        /// </summary>
        public string ReceiptHeaderLine2
        {
            get
            {
                return m_receiptHeaderLine2;
            }
        }

        /// <summary>
        /// Gets the operator's third receipt line received from the server.
        /// </summary>
        public string ReceiptHeaderLine3
        {
            get
            {
                return m_receiptHeaderLine3;
            }
        }

        /// <summary>
        /// Gets the operator's first receipt footer line received from the 
        /// server.
        /// </summary>
        public string ReceiptFooterLine1
        {
            get
            {
                return m_receiptFooterLine1;
            }
        }

        /// <summary>
        /// Gets the operator's second receipt footer line received from the 
        /// server.
        /// </summary>
        public string ReceiptFooterLine2
        {
            get
            {
                return m_receiptFooterLine2;
            }
        }

        /// <summary>
        /// Gets the operator's third receipt footer line received from the 
        /// server.
        /// </summary>
        public string ReceiptFooterLine3
        {
            get
            {
                return m_receiptFooterLine3;
            }
        }

        /// <summary>
        /// Gets the operator's max card limit received from the server.
        /// </summary>
        public short MaxCardLimit
        {
            get
            {
                return m_maxCardLimit;
            }
        }

        /// <summary>
        /// Gets the use same cards value received from the server.
        /// </summary>
        public bool UseSameCards
        {
            get
            {
                return m_sameCards;
            }
        }

        /// <summary>
        /// Gets the use consecutive cards value received from the server.
        /// </summary>
        public bool UseConsecutiveCards
        {
            get
            {
                return m_consecutiveCards;
            }
        }

        /// <summary>
        /// Gets the print bar code value received from the server.
        /// </summary>
        public bool PrintBarCode
        {
            get
            {
                return m_printBarCode;
            }
        }

        /// <summary>
        /// Gets the player tracking enabled value received from the server.
        /// </summary>
        public bool PlayerTrackingEnabled
        {
            get
            {
                return m_playerTrackingEnabled;
            }
        }

        /// <summary>
        /// Gets the print card faces value received from the server.
        /// </summary>
        public bool PrintCardFaces
        {
            get
            {
                return m_printCardFaces;
            }
        }

        /// <summary>
        /// Gets the operator's sales tax rate received from the server.
        /// </summary>
        public decimal SalesTax
        {
            get
            {
                return m_salesTax;
            }
        }

        /// <summary>
        /// Gets the print card numbers value received from the server.
        /// </summary>
        public bool PrintCardNumbers
        {
            get
            {
                return m_printCardNumbers;
            }
        }

        /// <summary>
        /// Gets the time that is considered the end of day from the server.
        /// The date portion of this value should be ignored.
        /// </summary>
        public DateTime EndOfDay
        {
            get
            {
                return m_endOfDay;
            }
        }

        /// <summary>
        /// Gets the fixed base device fee received from the server.
        /// </summary>
        public decimal FixedDeviceFee
        {
            get
            {
                return m_fixedFee;
            }
        }

        /// <summary>
        /// Gets the Traveler device fee received from the server.
        /// </summary>
        public decimal TravelerDeviceFee
        {
            get
            {
                return m_travelerFee;
            }
        }

        /// <summary>
        /// Gets the Tracker device fee received from the server.
        /// </summary>
        public decimal TrackerDeviceFee
        {
            get
            {
                return m_trackerFee;
            }
        }

        /// <summary>
        /// Gets the Mini device fee received from the server.
        /// </summary>
        public decimal MiniDeviceFee
        {
            get
            {
                return m_miniFee;
            }
        }

        // PDTS 964
        // Rally US765
        /// <summary>
        /// Gets the Traveler II device fee received from the server.
        /// </summary>
        public decimal Traveler2DeviceFee
        {
            get
            {
                return m_traveler2Fee;
            }
        }
        #endregion
    }
}
