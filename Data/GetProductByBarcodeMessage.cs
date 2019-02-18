// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2016 Fortunet

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace GTI.Modules.Shared
{
    public class GetProductByBarcodeMessage : ServerMessage
    {
        #region Constants and Data Types

        private readonly string m_barcode;

        #endregion


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetProductItemMessage class.
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        private GetProductByBarcodeMessage(string barcode)
        {
            m_id = 18202;
            m_barcode = barcode;
            SessionKeyItems = new List<SessionKeyItem>();
        }
        #endregion
        
        #region Member Properties
        public string Name { get; set; }

        public string Serial { get; set; }

        public int Audit { get; set; }

        public int CardCount { get; set; }

        public int Status {get; set; }

        public BarcodeType BarcodeType { get; private set; }

        public List<SessionKeyItem> SessionKeyItems { get; private set; } 
        #endregion

        #region Member Methods
        public static GetProductByBarcodeMessage GetProductByBarcode(string barcode)
        {
            var msg = new GetProductByBarcodeMessage(barcode);
            try
            {
                msg.Send();
            }
            catch (ServerCommException ex)
            {
                throw new Exception("GetPackageItemMessage: " + ex.Message);
            }
            return msg;
        }

        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            var requestStream = new MemoryStream();
            var requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            //barcode
            requestWriter.Write((ushort)m_barcode.Length);
            requestWriter.Write(m_barcode.ToCharArray());

            // Set the bytes to be sent.
            m_requestPayload = requestStream.ToArray();

            // Close the streams.
            requestWriter.Close();
        }

        protected override void UnpackResponse()
        {
            base.UnpackResponse();

            // Create the streams we will be reading from.
            var responseStream = new MemoryStream(m_responsePayload);
            var responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Determine which barcode type we are dealing with
                BarcodeType = (BarcodeType)responseReader.ReadInt32();

                if (BarcodeType == BarcodeType.PaperScanCode)
                {
                    Status = responseReader.ReadInt32();
                    Audit = responseReader.ReadInt32();

                    ushort stringLen = responseReader.ReadUInt16();
                    Serial = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    Name = new string(responseReader.ReadChars(stringLen));

                    CardCount = responseReader.ReadInt32(); //US3509

                    ushort itemCount = responseReader.ReadUInt16();
                    for (ushort n = 0; n < itemCount; ++n)
                    {
                        SessionKeyItem keyItem = new SessionKeyItem
                        {
                            Session = responseReader.ReadInt32(),
                            Page = responseReader.ReadInt32(),
                            Key = responseReader.ReadInt32()
                        };

                        SessionKeyItems.Add(keyItem);
                    }
                }
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException(m_strMessageName, e);
            }
            catch (Exception e)
            {
                throw new ServerException(m_strMessageName, e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

    }

    public class SessionKeyItem : IEquatable<SessionKeyItem>
    {
        #region Member Variables
        protected int m_sessionId;
        protected int m_pageNumber;
        protected int m_keyNumber;
        protected int m_packageID;
        #endregion

        #region Constructors

        #endregion

        #region Member Methods

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SessionKeyItem keyObj = obj as SessionKeyItem;
            if (keyObj == null)
                return false;
            return Equals(keyObj);
        }

        public bool Equals(SessionKeyItem other)
        {
            if (other == null)
                return false;

            if (Session == other.Session &&
                Key == other.Key &&
                Page == other.Page &&
                PackageID == other.PackageID)
                return true;
            
                return false;
        }

        public override int GetHashCode()
        {
            return (base.GetHashCode() ^ m_sessionId.GetHashCode() ^
                    m_pageNumber.GetHashCode() ^ m_keyNumber.GetHashCode() ^ m_packageID.GetHashCode());
        }
        #endregion

        #region Member Properties

        /// <summary>
        /// Gets or sets the Session Id for this key item
        /// </summary>
        public int Session
        {
            get { return m_sessionId; }
            set { m_sessionId = value; }
        }

        /// <summary>
        /// Gets or sets that page that the key is on
        /// </summary>
        public int Page
        {
            get { return m_pageNumber; }
            set { m_pageNumber = value; }
        }

        /// <summary>
        /// Gets or sets the key number
        /// </summary>
        public int Key
        {
            get { return m_keyNumber; }
            set { m_keyNumber = value; }
        }

        /// <summary>
        /// Gets or sets the package ID for this item.
        /// </summary>
        public int PackageID
        {
            get
            {
                return m_packageID;
            }
            set
            {
                m_packageID = value;
            }
        }
        #endregion
    }
}
