// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// The possible status return codes from the Get Staff Modules server 
    /// message.
    /// </summary>
    public enum GetStaffModulesReturnCode
    {
        IncorrectPassword = -10,
        PasswordHasExpired = -8,
        InactiveStaff = -9,
        StaffNotFound = -7
    }

    /// <summary>
    /// Represents a Get Staff Modules server message.
    /// </summary>
    public class GetStaffModulesMessage : ServerMessage
    {
        #region Constants And Data Types
        protected const int MinResponseMessageLength = 6;
        #endregion

        #region Member Variables
        protected int m_staffId = 0;
        protected int m_loginNum = 0;
        protected string m_magCardNum = string.Empty;
        protected byte[] m_password = null;
        protected int m_moduleId = 0;
        protected List<int> m_moduleList = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetStaffModulesMessage 
        /// class.
        /// </summary>
        public GetStaffModulesMessage()
        {
            m_id = 25010; // Get Staff Modules
            m_moduleList = new List<int>();
        }

        /// <summary>
        /// Initializes a new instance of the GetStaffModulesMessage 
        /// class with the specified parameters.
        /// </summary>
        /// <param name="staffId">The id of the staff to get 
        /// modules for.</param>
        /// <param name="moduleId">The id of the module to return or 0 for all 
        /// modules.</param>
        public GetStaffModulesMessage(int staffId, int moduleId)
            : this()
        {
            StaffId = staffId;
            m_moduleId = moduleId;
        }

        /// <summary>
        /// Initializes a new instance of the GetStaffModulesMessage 
        /// class with the specified parameters
        /// </summary>
        /// <param name="loginNumber">The login number of the staff to get 
        /// modules for.</param>
        /// <param name="passwordHash">The password hash of the staff.</param>
        /// <param name="moduleId">The id of the module to return or 0 for all 
        /// modules.</param>
        public GetStaffModulesMessage(int loginNumber, byte[] passwordHash, int moduleId)
            : this()
        {
            LoginNumber = loginNumber;
            PasswordHash = passwordHash;
            m_moduleId = moduleId;
        }

        /// <summary>
        /// Initializes a new instance of the GetStaffModulesMessage 
        /// class with the specified parameters
        /// </summary>
        /// <param name="magCardNumber">The magnetic card number of the staff 
        /// to get modules for.</param>
        /// <param name="passwordHash">The password hash of the staff.</param>
        /// <param name="moduleId">The id of the module to return or 0 for all 
        /// modules.</param>
        public GetStaffModulesMessage(string magCardNumber, byte[] passwordHash, int moduleId)
            : this()
        {
            MagneticCardNumber = magCardNumber;
            PasswordHash = passwordHash;
            m_moduleId = moduleId;
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

            // Login Number
            requestWriter.Write(m_loginNum);

            // Mag. Card Number
            requestWriter.Write((ushort)m_magCardNum.Length);
            requestWriter.Write(m_magCardNum.ToCharArray());

            // Password Hash
            byte[] hashBuffer = new byte[DataSizes.PasswordHash];

            if(m_password != null)
                Array.Copy(m_password, hashBuffer, DataSizes.PasswordHash);

            requestWriter.Write(hashBuffer);

            // Module Id
            requestWriter.Write(m_moduleId);

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
            try
            {
                base.UnpackResponse();
            }
            catch(ServerException e)
            {
                // If it's a known return code, then it's okay.
                GetStaffModulesReturnCode code = (GetStaffModulesReturnCode)e.ReturnCode;

                if(code == GetStaffModulesReturnCode.InactiveStaff || code == GetStaffModulesReturnCode.IncorrectPassword ||
                   code == GetStaffModulesReturnCode.PasswordHasExpired || code == GetStaffModulesReturnCode.StaffNotFound)
                    return;
                else
                    throw e; // Rethrow
            }

            // Create the streams we will be reading from.
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Check the response length.
            if(responseStream.Length < MinResponseMessageLength)
                throw new MessageWrongSizeException("Get Staff Modules");

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Get the count of modules.
                ushort moduleCount = responseReader.ReadUInt16();

                // Clear the module list.
                m_moduleList.Clear();

                // Get all the modules.
                for(ushort x = 0; x < moduleCount; x++)
                {
                    m_moduleList.Add(responseReader.ReadInt32());
                }
            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Staff Modules", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Get Staff Modules", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the id of the staff to get modules for or 0 if 
        /// LoginNumber or MagneticCardNumber is being used instead.
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

                if(m_staffId != 0)
                {
                    m_loginNum = 0;
                    m_magCardNum = string.Empty;
                    m_password = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the login number of the staff to get modules for or 
        /// 0 if StaffId or MagneticCardNumber is being used instead.
        /// </summary>
        public int LoginNumber
        {
            get
            {
                return m_loginNum;
            }
            set
            {
                m_loginNum = value;

                if(m_loginNum != 0)
                {
                    m_staffId = 0;
                    m_magCardNum = string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets or sets the magnetic card number of the staff to get modules 
        /// for or blank if StaffId or LoginNumber is being used instead.
        /// </summary>
        public string MagneticCardNumber
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
                    throw new ArgumentException("MagneticCardNumber is too big.");

                if(m_magCardNum != null && m_magCardNum != string.Empty)
                {
                    m_staffId = 0;
                    m_loginNum = 0;
                }
            }
        }

        /// <summary>
        /// Gets or sets the password hash of the staff or null if StaffId is 
        /// being used.
        /// </summary>
        public byte[] PasswordHash
        {
            get
            {
                return m_password;
            }
            set
            {
                if(value != null && value.Length != DataSizes.PasswordHash)
                    throw new ArgumentException("PasswordHash is wrong size.");

                m_password = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the module to return or 0 for all modules.
        /// </summary>
        public int ModuleId
        {
            get
            {
                return m_moduleId;
            }
            set
            {
                m_moduleId = value;
            }
        }

        /// <summary>
        /// Gets the list of modules the staff has access to.
        /// </summary>
        public int[] ModuleList
        {
            get
            {
                return m_moduleList.ToArray();
            }
        }
        #endregion
    }
}