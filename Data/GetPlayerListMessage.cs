// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  � 2008 GameTech
// International, Inc.

// TTP 50114

using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using GTI.Modules.Shared.Properties;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// A helper class that represents a player in a list.
    /// </summary>
    public class PlayerListItem
    {
        #region Member Variables
        private int m_id;
        private string m_firstName;
        private string m_middleInitial;
        private string m_lastName;
        private string m_magCard;
        private string m_playerIdentity;
        private DateTime m_birthDate;
        private DateTime m_lastVisitDate;
        #endregion

        #region Member Methods
        /// <summary>
        /// Returns a string that represents the current PlayerListItem.
        /// </summary>
        /// <returns>A string that represents the current 
        /// PlayerListItem.</returns>
        public override string ToString()
        {
            string returnVal = string.Empty;

            if(!string.IsNullOrEmpty(m_lastName))
                returnVal = m_lastName;

            if(!string.IsNullOrEmpty(m_firstName))
            {
                if(!string.IsNullOrEmpty(returnVal))
                    returnVal += ", ";

                returnVal += m_firstName;
            }

            if(!string.IsNullOrEmpty(m_middleInitial))
            {
                if(!string.IsNullOrEmpty(returnVal))
                    returnVal += " ";

                returnVal += m_middleInitial;
            }

            if(string.IsNullOrEmpty(returnVal.Trim()))
                returnVal = string.Format(CultureInfo.CurrentCulture, Resources.PlayerNoName, m_id);

            return returnVal;
        }

        /// <summary>
        /// Determines whether two PlayerListItem instances are equal.
        /// </summary>
        /// <param name="obj">The PlayerListItem to compare with the 
        /// current PlayerListItem.</param>
        /// <returns>true if the specified PlayerListItem is equal 
        /// to the current PlayerListItem; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            if(!(obj is PlayerListItem)) return false;

            PlayerListItem item = (PlayerListItem)obj;

            return (m_id == item.m_id);
        }

        /// <summary>
        /// Serves as a hash function for a PlayerListItem. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current PlayerListItem.</returns>
        public override int GetHashCode()
        {
            return m_id.GetHashCode();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the player's id.
        /// </summary>
        public int Id
        {
            get
            {
                return m_id;
            }
            set
            {
                m_id = value;
            }
        }

      

        /// <summary>
        /// Gets or sets the player's first name.
        /// </summary>
        public string FirstName
        {
            get
            {
                return m_firstName;
            }
            set
            {
                m_firstName = value;
            }
        }

                /// <summary>
        /// Gets or sets the player's middle initial.
        /// </summary>
        public string MiddleInitial
        {
            get
            {
                return m_middleInitial;
            }
            set
            {
                m_middleInitial = value;
            }
        }

          /// <summary>
        /// Gets or sets the player's last name.
        /// </summary>
        public string LastName
        {
            get
            {
                return m_lastName;
            }
            set
            {
                m_lastName = value;
            }
        }





        /// <summary>
        /// Gets the name of the player in the form [First Name] [Last Name].
        /// </summary>
        public string FullName
        {
            get
            {
                string returnVal = null;

                returnVal = m_firstName;

                if(!string.IsNullOrEmpty(m_firstName))
                    returnVal += " ";

                returnVal += m_lastName;

                return returnVal;
            }
        }

          /// <summary>
        /// Gets or sets the player's mag card number
        /// </summary>
        public string MagCard
        {
            get
            {
                return m_magCard;
            }
            set
            {
                m_magCard = value;
            }
        }

                  /// <summary>
        /// Gets or sets the player's player's identity
        /// </summary>
        public string PlayerIdentity
        {
            get
            {
                return m_playerIdentity;
            }
            set
            {
                m_playerIdentity = value;
            }
        }

        public DateTime BirthDate
        {
            get
            {
                return m_birthDate;
            }
            set
            {
                m_birthDate = value;
            }
        }

        public DateTime LastVisitDate
        {
            get
            {
                return m_lastVisitDate;
            }
            set
            {
                m_lastVisitDate = value;
            }
        }


        #endregion
    }

    /// <summary>
    /// Represents the Get Player List server message.
    /// </summary>
    public class GetPlayerListMessage : ServerMessage
    {
        #region Constants and Data Types
        protected const int MinResponseMessageLength = 6;
        #endregion

        #region Member Variables
        protected string m_firstName = "";
        protected string m_lastName = "";
        protected string m_searchCategory = "";
        protected List<PlayerListItem> m_players = new List<PlayerListItem>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetPlayerListMessage class.
        /// </summary>
        public GetPlayerListMessage()
            //: this(null, null)
             : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetPlayerListMessage class 
        /// with the specified operator id and names.
        /// </summary>
        /// <param name="operatorId">The id of the operator to find 
        /// players for.</param>
        /// <param name="firstName">The first name to search for.  
        /// Partial matches will be returned.  A blank first name means any 
        /// first name.</param>
        /// <param name="lastName">The last name to search for.  
        /// Partial matches will be returned.  A blank last name means any 
        /// last name.</param>
       // public GetPlayerListMessage(string firstName, string lastName)
        public GetPlayerListMessage(string searchCategory)
        {
            m_id = 8014; // Get Player List
            m_strMessageName = "Get Player List";
            SearchCategory = searchCategory;
        }

        public GetPlayerListMessage(string firstName, string lastName)
        {
            m_id = 8014; // Get Player List
            m_strMessageName = "Get Player List";
            FirstName = firstName;
            LastName = lastName;
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


            if(FirstName == "" && LastName == "") //no names, send the category (even if it is empty)
            {
                requestWriter.Write((ushort)m_searchCategory.Length);

                if(m_searchCategory.Length > 0)
                    requestWriter.Write(m_searchCategory.ToCharArray());
            }
            else //must be at least one name
            {
                string tmp = FirstName + (FirstName != "" && LastName != "" ? " " : "") + LastName;

                requestWriter.Write((ushort)tmp.Length);
                requestWriter.Write(tmp.ToCharArray());
            }

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

            // Check the response length.
            if(responseStream.Length < MinResponseMessageLength)
                throw new MessageWrongSizeException(m_strMessageName);

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Get the count of players.
                ushort playerCount = responseReader.ReadUInt16();

                // Clear the player array.
                m_players.Clear();

                // Get all the players
                for(ushort x = 0; x < playerCount; x++)
                {
                    PlayerListItem item = new PlayerListItem();

                    // Player Id
                    item.Id = responseReader.ReadInt32();

                    // First Name
                    ushort stringLen = responseReader.ReadUInt16();
                    item.FirstName = new string(responseReader.ReadChars(stringLen));

                    // Middle Initial
                    stringLen = responseReader.ReadUInt16();
                    item.MiddleInitial = new string(responseReader.ReadChars(stringLen));

                    // Last Name
                    stringLen = responseReader.ReadUInt16();
                    item.LastName = new string(responseReader.ReadChars(stringLen));

                      // Mag Card
                    stringLen = responseReader.ReadUInt16();
                    item.MagCard = new string(responseReader.ReadChars(stringLen));

                       // Player Identity
                    stringLen = responseReader.ReadUInt16();
                    item.PlayerIdentity = new string(responseReader.ReadChars(stringLen));

                    // BirthDate
                    string dateTempValueString;
                    stringLen = responseReader.ReadUInt16();
                    dateTempValueString = new string(responseReader.ReadChars(stringLen));

                    try
                    {
                        item.BirthDate = Convert.ToDateTime(dateTempValueString);
                    }
                    catch
                    {
                        item.BirthDate = DateTime.MinValue;
                    }

                    // LatVisitDate
                    stringLen = responseReader.ReadUInt16();
                    dateTempValueString = new string(responseReader.ReadChars(stringLen));

                    try
                    {
                        item.LastVisitDate = Convert.ToDateTime(dateTempValueString);
                    }
                    catch
                    {
                        item.LastVisitDate = DateTime.MinValue;
                    }

                    m_players.Add(item);
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

        public string SearchCategory
        {
            get
            {
                return m_searchCategory;
            }

            set
            {
                if (value == null)
                {
                    m_searchCategory = "";
                }
                else if (value.Length <= StringSizes.MaxNameLength)
                {
                    if (LastName != "" || FirstName != "")
                        throw new Exception("Can't have a category and name.");
                    else
                        m_searchCategory = value;
                }
                else
                {
                    throw new ArgumentException("Category search is too big.");
                }
            }
        }
      
        /// <summary>
        /// Gets or sets the first name to search for.
        /// </summary>
         public string FirstName
         {
             get
             {
                 return m_firstName;
             }
    
             set
             {
                 if (value == null)
                 {
                     m_firstName = "";
                 }
                 else if (value.Length <= StringSizes.MaxNameLength)
                 {
                     if (SearchCategory != "")
                         throw new Exception("Can't have a name and a category.");
                     else
                         m_firstName = value;
                 }
                 else
                 {
                     throw new ArgumentException("FirstName is too big.");
                 }
             }
         }

        /// <summary>
        /// Gets or sets the last name to search for.  A blank last name means 
        /// any last name.
        /// </summary>
         public string LastName
         {
             get
             {
                 return m_lastName;
             }
             
             set
             {
                 if (value == null)
                 {
                     m_lastName = "";
                 }
                 else if (value.Length <= StringSizes.MaxNameLength)
                 {
                     if (SearchCategory != "")
                         throw new Exception("Can't have a name and a category.");
                     else
                         m_lastName = value;
                 }
                 else
                 {
                     throw new ArgumentException("LastName is too big.");
                 }
             }
         }

        /// <summary>
        /// Gets a list of all players received from the server.
        /// </summary>
        public PlayerListItem[] Players
        {
            get
            {
                return m_players.ToArray();
            }
        }
        #endregion
    }
}
