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
    /// Represents the Find Player by Player Card server message.
    /// </summary>
    public class FindPlayerByCardMessage : ServerMessage
    {
        #region Member Variables
        protected string m_magCardNum = string.Empty;
        protected int m_PIN = 0;
        protected int m_playerId = 0;
        protected bool m_pinRequired = false;
        protected string m_firstName = string.Empty;
        protected string m_middleInitial = string.Empty;
        protected string m_lastName = string.Empty;
        protected string m_gender = string.Empty;
        protected bool m_PINError = false;
        protected bool m_pointsUpToDate = false;
        protected bool m_thirdPartyInterfaceDown = false;
        protected bool m_thirdPartyPlayerSync = true;
        protected string m_thirdPartyErrorMessage = string.Empty;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the FindPlayerByCardMessage class.
        /// </summary>
        public FindPlayerByCardMessage()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the FindPlayerByCardMessage class 
        /// with the specified mag. card number.
        /// </summary>
        /// <param name="magCardNumber">The player's mag. card number.</param>
        /// <param name="PIN">The mag card PIN (0=none & -1=use saved).</param>
        public FindPlayerByCardMessage(string magCardNumber, int PIN = 0)
        {
            m_id = 8012; // Find Player by Player Card       
            MagCardNumber = magCardNumber;
            m_PIN = PIN;
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

            // Mag. Card #
            requestWriter.Write((ushort)m_magCardNum.Length);
            requestWriter.Write(m_magCardNum.ToCharArray());

            // PIN
            requestWriter.Write(m_PIN);

            // Sync
            requestWriter.Write(m_thirdPartyPlayerSync?1:0);

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
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Player Id
                m_playerId = responseReader.ReadInt32();

                // Pin Required
                m_pinRequired = responseReader.ReadBoolean();

                // Third party player card PIN error
                m_PINError = responseReader.ReadBoolean();

                // Third party player points correct
                m_pointsUpToDate = !responseReader.ReadBoolean();

                // Third party interface down
                m_thirdPartyInterfaceDown = responseReader.ReadBoolean();

                // Third party interface error
                ushort stringLen = responseReader.ReadUInt16();
                m_thirdPartyErrorMessage = new string(responseReader.ReadChars(stringLen));

                // First Name
                stringLen = responseReader.ReadUInt16();
                m_firstName = new string(responseReader.ReadChars(stringLen));

                // Middle Initial
                stringLen = responseReader.ReadUInt16();
                m_middleInitial = new string(responseReader.ReadChars(stringLen));

                // Last Name
                stringLen = responseReader.ReadUInt16();
                m_lastName = new string(responseReader.ReadChars(stringLen));

                // Gender
                stringLen = responseReader.ReadUInt16();
                m_gender = new string(responseReader.ReadChars(stringLen));
            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Find Player by Player Card", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Find Player by Player Card", e);
            }           

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the magnetic card number to search with.
        /// </summary>
        public string MagCardNumber
        {
            get
            {
                return m_magCardNum;
            }
            set
            {
                if(value.Length <= StringSizes.MaxMagneticCardLength)
                    m_magCardNum = value;
                else
                    throw new ArgumentException("MagCardNumber is too big.");
            }
        }

        /// <summary>
        /// Gets the player's id received from the server or 0 if 
        /// none was found.
        /// </summary>
        public int PlayerId
        {
            get
            {
                return m_playerId;
            }
        }

        /// <summary>
        /// Sets the PIN if needed for player lookup.
        /// </summary>
        public int PIN
        {
            set
            {
                m_PIN = value;
            }
        }

        /// <summary>
        /// Gets the pin required value received from the server.
        /// </summary>
        public bool PinRequired
        {
            get
            {
                return m_pinRequired;
            }
        }

        /// <summary>
        /// Gets third party player card PIN error status.
        /// </summary>
        public bool PINError
        {
            get
            {
                return m_PINError;
            }
        }

        /// <summary>
        /// Gets if the player points are correct.
        /// </summary>
        public bool PointsUpToDate
        {
            get
            {
                return m_pointsUpToDate;
            }
        }

        /// <summary>
        /// Gets if the third party interface is down.
        /// </summary>
        public bool ThirdPartyInterfaceDown
        {
            get
            {
                return m_thirdPartyInterfaceDown;
            }
        }

        /// <summary>
        /// Gets a string with a returned error message.
        /// </summary>
        public string SyncError
        {
            get
            {
                return m_thirdPartyErrorMessage;
            }
        }

        /// <summary>
        /// Gets the player's first name received from the server.
        /// </summary>
        public string FirstName
        {
            get
            {
                return m_firstName;
            }
        }

        /// <summary>
        /// Gets the player's middle initial received from the server.
        /// </summary>
        public string MiddleInitial
        {
            get
            {
                return m_middleInitial;
            }
        }

        /// <summary>
        /// Gets the player's last name received from the server.
        /// </summary>
        public string LastName
        {
            get
            {
                return m_lastName;
            }
        }

        /// <summary>
        /// Gets the player's gender received from the server.
        /// </summary>
        public string Gender
        {
            get
            {
                return m_gender;
            }
        }

        //Get or set if the player information should be updated from the third party system.
        public bool SyncPlayerWithThirdParty
        {
            get
            {
                return m_thirdPartyPlayerSync;
            }
            set
            {
                m_thirdPartyPlayerSync = value;
            }
        }
        #endregion
    }
}
