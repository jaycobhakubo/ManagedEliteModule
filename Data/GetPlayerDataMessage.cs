// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  � 2007 GameTech
// International, Inc.

using System;
using System.IO;
using System.Text;
using System.Globalization;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents the Get Player Data server message.
    /// </summary>
    public class GetPlayerDataMessage : ServerMessage
    {
        #region Member Variables
        protected int m_playerId = 0;
        protected string m_firstName = string.Empty;
        protected string m_middleInitial = string.Empty;
        protected string m_lastName = string.Empty;
        protected string m_govIssuedIdNum = string.Empty;
        DateTime? m_birthDate = null;
        protected string m_email = string.Empty;
        protected string m_playerIdent = string.Empty;
        protected string m_phoneNum = string.Empty;
        protected string m_gender = string.Empty;
        protected byte[] m_pinNum; // Rally TA1583
        protected string m_address1 = string.Empty;
        protected string m_address2 = string.Empty;
        protected string m_city = string.Empty;
        protected string m_state = string.Empty;
        protected string m_zip = string.Empty;
        protected string m_country = string.Empty;
        protected DateTime m_joinDate;
        protected DateTime m_lastVisit;
        protected decimal m_pointsBalance = 0M;
        protected bool m_pointBalanceInvalid = false;
        protected int m_visitCount = 0;
        protected string m_comment = string.Empty;
        protected string m_magCardNum = string.Empty;
        protected int m_playerTierId = 0;
        protected decimal m_totalSpend = 0M;
        protected bool m_isLoggedIn = false;
        protected int m_PIN = 0;
        protected bool m_PINError = false;
        protected bool m_thirdPartyInterfaceDown = false;
        protected string m_errorMessage = string.Empty;
        protected bool m_thirdPartyPlayerSync = true;
        protected bool m_noThirdPartySyncPointsAreGood = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetPlayerDataMessage class.
        /// </summary>
        public GetPlayerDataMessage()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetPlayerDataMessage class 
        /// with the specified player id.
        /// </summary>
        public GetPlayerDataMessage(int playerId)
        {
            m_id = 8009; // Get Player Data
            m_playerId = playerId;
        }

        /// <summary>
        /// Initializes a new instance of the GetPlayerDataMessage class 
        /// with the specified player id.
        /// </summary>
        public GetPlayerDataMessage(int playerId, int PIN)
        {
            m_id = 8009; // Get Player Data
            m_playerId = playerId;
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

            // Player Id
            requestWriter.Write(m_playerId);

            //Player card PIN
            requestWriter.Write(m_PIN);

            // Sync
            requestWriter.Write(m_thirdPartyPlayerSync ? 1 : 0);

            // DB points OK
            requestWriter.Write(m_noThirdPartySyncPointsAreGood ? 1 : 0);

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

                // First Name
                ushort stringLen = responseReader.ReadUInt16();
                m_firstName = new string(responseReader.ReadChars(stringLen));

                // Middle Initial
                stringLen = responseReader.ReadUInt16();
                m_middleInitial = new string(responseReader.ReadChars(stringLen));

                // Last Name
                stringLen = responseReader.ReadUInt16();
                m_lastName = new string(responseReader.ReadChars(stringLen));

                // Gov. Issued Id Num
                stringLen = responseReader.ReadUInt16();
                m_govIssuedIdNum = new string(responseReader.ReadChars(stringLen));

                // Birth Date
                stringLen = responseReader.ReadUInt16();
                string tempDate = new string(responseReader.ReadChars(stringLen));

                if (tempDate != string.Empty)
                    m_birthDate = DateTime.Parse(tempDate, CultureInfo.InvariantCulture);
                else
                    m_birthDate = null;

                // Email
                stringLen = responseReader.ReadUInt16();
                m_email = new string(responseReader.ReadChars(stringLen));

                // Player Ident
                stringLen = responseReader.ReadUInt16();
                m_playerIdent = new string(responseReader.ReadChars(stringLen));

                // Phone #
                stringLen = responseReader.ReadUInt16();
                m_phoneNum = new string(responseReader.ReadChars(stringLen));

                // Gender
                stringLen = responseReader.ReadUInt16();
                m_gender = new string(responseReader.ReadChars(stringLen));

                // Rally TA1583
                // Pin #
                m_pinNum = responseReader.ReadBytes(DataSizes.PasswordHash);

                // Address 1
                stringLen = responseReader.ReadUInt16();
                m_address1 = new string(responseReader.ReadChars(stringLen));

                // Address 2
                stringLen = responseReader.ReadUInt16();
                m_address2 = new string(responseReader.ReadChars(stringLen));

                // City
                stringLen = responseReader.ReadUInt16();
                m_city = new string(responseReader.ReadChars(stringLen));

                // State
                stringLen = responseReader.ReadUInt16();
                m_state = new string(responseReader.ReadChars(stringLen));

                // Zip
                stringLen = responseReader.ReadUInt16();
                m_zip = new string(responseReader.ReadChars(stringLen));

                // Country
                stringLen = responseReader.ReadUInt16();
                m_country = new string(responseReader.ReadChars(stringLen));

                // Join Date
                stringLen = responseReader.ReadUInt16();
                tempDate = new string(responseReader.ReadChars(stringLen));

                if(tempDate != string.Empty)
                    m_joinDate = DateTime.Parse(tempDate, CultureInfo.InvariantCulture);

                // Last Visit
                stringLen = responseReader.ReadUInt16();
                tempDate = new string(responseReader.ReadChars(stringLen));

                if(tempDate != string.Empty)
                    m_lastVisit = DateTime.Parse(tempDate, CultureInfo.InvariantCulture);

                // Points Balance
                stringLen = responseReader.ReadUInt16();
                string tempDec = new string(responseReader.ReadChars(stringLen));

                if(tempDec != string.Empty)
                    m_pointsBalance = decimal.Parse(tempDec, CultureInfo.InvariantCulture);

                // Visit Count
                m_visitCount = responseReader.ReadInt32();

                // Comment
                stringLen = responseReader.ReadUInt16();
                m_comment = new string(responseReader.ReadChars(stringLen));

                // Mag. Card
                stringLen = responseReader.ReadUInt16();
                m_magCardNum = new string(responseReader.ReadChars(stringLen));

                // Player Loyalty Tier Id
                m_playerTierId = responseReader.ReadInt32();

                // Total Spend
                stringLen = responseReader.ReadUInt16();
                tempDec = new string(responseReader.ReadChars(stringLen));

                if(tempDec != string.Empty)
                    m_totalSpend = decimal.Parse(tempDec, CultureInfo.InvariantCulture);

                // Is Logged In
                m_isLoggedIn = responseReader.ReadBoolean();
                
                m_thirdPartyInterfaceDown = responseReader.ReadBoolean();

                stringLen = responseReader.ReadUInt16();
                m_errorMessage = new string(responseReader.ReadChars(stringLen));

            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Player Data", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Get Player Data", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the id of the player.
        /// </summary>
        public int PlayerId
        {
            get
            {
                return m_playerId;
            }
            set
            {
                m_playerId = value;
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
        /// Gets the player's government issued id number received from the 
        /// server.
        /// </summary>
        public string GovIssuedIdNumber
        {
            get
            {
                return m_govIssuedIdNum;
            }
        }

        /// Gets the player's birth date received from the server.
        /// </summary>
        public DateTime BirthDate
        {
            get
            {
                return m_birthDate == null ? DateTime.MinValue : (DateTime)m_birthDate;
            }
        }

        /// <summary>
        /// Gets the player's age in years.  Returns 0 if no birth date has been given.
        /// </summary>
        public int Age
        {
            get
            {
                int age = 0;

                if (m_birthDate != null)
                {
                    DateTime now = DateTime.Now, bday = (DateTime)m_birthDate;
                    age = DateTime.Now.Year - bday.Year;

                    if (now.Month < bday.Month || (now.Month == bday.Month && now.Day < bday.Day))
                        age--;
                }

                return age;
            }
        }

        /// <summary>
        /// Gets the player's email address received from the server.
        /// </summary>
        public string Email
        {
            get
            {
                return m_email;
            }
        }

        /// <summary>
        /// Gets the custom player id string received from the server.
        /// </summary>
        public string PlayerIdentity
        {
            get
            {
                return m_playerIdent;
            }
        }

        /// <summary>
        /// Gets the player's phone number received from the server.
        /// </summary>
        public string PhoneNumber
        {
            get
            {
                return m_phoneNum;
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

        // Rally TA1583
        /// <summary>
        /// Gets the player's pin number received from the server.
        /// </summary>
        public byte[] PinNumber
        {
            get
            {
                return m_pinNum;
            }
        }

        /// <summary>
        /// Gets the player's address line 1 received from the server.
        /// </summary>
        public string Address1
        {
            get
            {
                return m_address1;
            }
        }

        /// <summary>
        /// Gets the player's address line 2 received from the server.
        /// </summary>
        public string Address2
        {
            get
            {
                return m_address2;
            }
        }

        /// <summary>
        /// Gets the player's city received from the server.
        /// </summary>
        public string City
        {
            get
            {
                return m_city;
            }
        }

        /// <summary>
        /// Gets the player's state received from the server.
        /// </summary>
        public string State
        {
            get
            {
                return m_state;
            }
        }

        /// <summary>
        /// Gets the player's zip received from the server.
        /// </summary>
        public string Zip
        {
            get
            {
                return m_zip;
            }
        }

        /// <summary>
        /// Gets the player's country received from the server.
        /// </summary>
        public string Country
        {
            get
            {
                return m_country;
            }
        }

        /// <summary>
        /// Gets the player's join date received from the server.
        /// </summary>
        public DateTime JoinDate
        {
            get
            {
                return m_joinDate;
            }
        }

        /// <summary>
        /// Gets the player's last visit received from the server.
        /// </summary>
        public DateTime LastVisit
        {
            get
            {
                return m_lastVisit;
            }
        }

        /// <summary>
        /// Gets v player's point balance received from the server.
        /// </summary>
        public decimal PointsBalance
        {
            get
            {
                return m_pointsBalance;
            }
        }

        /// <summary>
        /// Gets the player's visit count received from the server.
        /// </summary>
        public int VisitCount
        {
            get
            {
                return m_visitCount;
            }         
        }

        /// <summary>
        /// Gets the player's comment received from the server.
        /// </summary>
        public string Comment
        {
            get
            {
                return m_comment;
            }
        }

        /// <summary>
        /// Gets the player's mag. card number received from the server.
        /// </summary>
        public string MagCardNumber
        {
            get
            {
                return m_magCardNum;
            }
        }

        /// <summary>
        /// Gets the player's tier's id received from the server.
        /// </summary>
        public int PlayerTierId
        {
            get
            {
                return m_playerTierId;
            }
        }

        /// <summary>
        /// Gets the total amount spent by the player received from the server.
        /// </summary>
        public decimal TotalSpend
        {
            get
            {
                return m_totalSpend;
            }
        }

        /// <summary>
        /// Gets the whether the player is logged in recieved from the server.
        /// </summary>
        public bool IsLoggedIn
        {
            get
            {
                return m_isLoggedIn;
            }
        }

        /// <summary>
        /// Sets the third party player card PIN. 0=none & -1=use saved PIN.
        /// </summary>
        public int PlayerCardPIN
        {
            set
            {
                m_PIN = value;
            }
        }

        /// <summary>
        /// Gets or sets if the third party system (if there is one) should
        /// be used to update the player information in our database.
        /// </summary>
        public bool ThirdPartyPlayerSync
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

        /// <summary>
        /// Gets or sets whether to assume player points in DB are
        /// OK if no third party sync was performed.
        /// </summary>
        public bool PointsAreGood
        {
            get
            {
                return m_noThirdPartySyncPointsAreGood;
            }

            set
            {
                m_noThirdPartySyncPointsAreGood = value;
            }
        }

        /// <summary>
        /// Gets if there was an error with the third party player card PIN.
        /// </summary>
        public bool PlayerCardPINError
        {
            get
            {
                return m_PINError;
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
        /// Gets if the points were loaded from the third party system.
        /// </summary>
        public bool PointsUpToDate
        {
            get
            {
                return !m_pointBalanceInvalid;
            }
        }

        /// <summary>
        /// Error message passed back (usually from third party
        /// player tracking system).
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return m_errorMessage;
            }
        }
        #endregion
    }
}
