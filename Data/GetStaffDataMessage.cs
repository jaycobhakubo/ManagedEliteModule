// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a Get Staff Data server message.
    /// </summary>
    public class GetStaffDataMessage : ServerMessage
    {
        #region Constants And Data Types
        protected const int MinResponseMessageLength = 6;
        #endregion

        #region Member Variables
        protected int m_staffId = 0;
        protected List<Staff> m_staffList = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetStaffDataMessage class.
        /// </summary>
        public GetStaffDataMessage()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetStaffDataMessage class
        /// with the specified staff and operator id.
        /// </summary>
        /// <param name="staffId">The id of the staff member to get 
        /// data for (or 0 for all staff).</param>
        public GetStaffDataMessage(int staffId)
        {
            m_id = 18023; // Get Staff Data
            m_staffId = staffId;
            m_staffList = new List<Staff>();
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

            // Staff Id
            requestWriter.Write(m_staffId);

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
                throw new MessageWrongSizeException("Get Staff Data");

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Get the count of staff.
                ushort staffCount = responseReader.ReadUInt16();

                // Clear the staff list.
                m_staffList.Clear();

                for(ushort x = 0; x < staffCount; x++)
                {
                    Staff staff = new Staff();

                    // Staff Id
                    staff.Id = responseReader.ReadInt32();

                    // Last Name
                    ushort stringLen = responseReader.ReadUInt16();
                    staff.LastName = new string(responseReader.ReadChars(stringLen));

                    // First Name
                    stringLen = responseReader.ReadUInt16();
                    staff.FirstName = new string(responseReader.ReadChars(stringLen));

                    // Birth Date
                    stringLen = responseReader.ReadUInt16();
                    string tempDate = new string(responseReader.ReadChars(stringLen));

                    if(tempDate != string.Empty)
                        staff.BirthDate = DateTime.Parse(tempDate, CultureInfo.InvariantCulture);

                    // Hire Date
                    stringLen = responseReader.ReadUInt16();
                    tempDate = new string(responseReader.ReadChars(stringLen));

                    if(tempDate != string.Empty)
                        staff.HireDate = DateTime.Parse(tempDate, CultureInfo.InvariantCulture);

                    // Login Number
                    staff.LoginNumber = responseReader.ReadInt32();

                    // Is Active
                    staff.IsActive = responseReader.ReadBoolean();

                    // Home Phone
                    stringLen = responseReader.ReadUInt16();
                    staff.HomePhone = new string(responseReader.ReadChars(stringLen));

                    // Other Phone
                    stringLen = responseReader.ReadUInt16();
                    staff.OtherPhone = new string(responseReader.ReadChars(stringLen));

                    // Gov. Issued Id Num
                    stringLen = responseReader.ReadUInt16();
                    staff.GovIssuedIdNumber = new string(responseReader.ReadChars(stringLen));

                    // Mag. Card Number
                    stringLen = responseReader.ReadUInt16();
                    staff.MagneticCardNumber = new string(responseReader.ReadChars(stringLen));

                    // Address Id
                    Address addr = new Address();
                    addr.AddressID = responseReader.ReadInt32();

                    // Address 1
                    stringLen = responseReader.ReadUInt16();
                    addr.Address1 = new string(responseReader.ReadChars(stringLen));

                    // Address 2
                    stringLen = responseReader.ReadUInt16();
                    addr.Address2 = new string(responseReader.ReadChars(stringLen));

                    // City
                    stringLen = responseReader.ReadUInt16();
                    addr.City = new string(responseReader.ReadChars(stringLen));

                    // State
                    stringLen = responseReader.ReadUInt16();
                    addr.State = new string(responseReader.ReadChars(stringLen));

                    // Zip
                    stringLen = responseReader.ReadUInt16();
                    addr.Zipcode = new string(responseReader.ReadChars(stringLen));

                    // Country
                    stringLen = responseReader.ReadUInt16();
                    addr.Country = new string(responseReader.ReadChars(stringLen));

                    staff.Address = addr;

                    staff.LeftHanded = responseReader.ReadBoolean();

                    //AcctLocked
                    staff.AcctLocked = responseReader.ReadBoolean();

                    m_staffList.Add(staff);
                }
            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Staff Data", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Get Staff Data", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the id of the staff member to get data for (or 0 for 
        /// all staff).
        /// </summary>
        public int StaffId
        {
            get
            {
                return m_staffId;
            }
            set
            {
                m_staffId = value;
            }
        }

        /// <summary>
        /// Returns the list of Staff retrieved from the server.
        /// </summary>
        public Staff[] StaffList
        {
            get
            {
                return m_staffList.ToArray();
            }
        }
        #endregion
    }
}
