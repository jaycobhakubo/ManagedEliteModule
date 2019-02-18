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
    /// The possible status return codes from the Get Staff Module Features 
    /// server message.
    /// </summary>
    public enum GetStaffModuleFeaturesReturnCode
    {
        IncorrectPassword = -10,
        PasswordHasExpired = -8,
        InactiveStaff = -9,
        StaffNotFound = -7,
        StaffLocked = -106 // US1955
    }

    /// <summary>
    /// Represents a Get Staff Module Features server message.
    /// </summary>
    public class GetStaffModuleFeaturesMessage : ServerMessage
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
        protected int m_moduleFeatureId = 0;
        protected List<int> m_moduleFeatureList = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetStaffModuleFeaturesMessage 
        /// class.
        /// </summary>
        public GetStaffModuleFeaturesMessage()
        {
            m_id = 25009; // Get Staff Module Features
            m_moduleFeatureList = new List<int>();
        }

        /// <summary>
        /// Initializes a new instance of the GetStaffModuleFeaturesMessage 
        /// class with the specified parameters.
        /// </summary>
        /// <param name="staffId">The id of the staff to get 
        /// features for.</param>
        /// <param name="moduleId">The id of the module to get features 
        /// for.</param>
        /// <param name="moduleFeatureId">The id of the module feature to 
        /// return or 0 for all feature for the specified module.</param>
        public GetStaffModuleFeaturesMessage(int staffId, int moduleId, int moduleFeatureId)
            : this()
        {
            StaffId = staffId;
            m_moduleId = moduleId;
            m_moduleFeatureId = moduleFeatureId;
        }

        /// <summary>
        /// Initializes a new instance of the GetStaffModuleFeaturesMessage 
        /// class with the specified parameters
        /// </summary>
        /// <param name="loginNumber">The login number of the staff to get 
        /// features for.</param>
        /// <param name="passwordHash">The password hash of the staff.</param>
        /// <param name="moduleId">The id of the module to get features 
        /// for.</param>
        /// <param name="moduleFeatureId">The id of the module feature to 
        /// return or 0 for all feature for the specified module.</param>
        public GetStaffModuleFeaturesMessage(int loginNumber, byte[] passwordHash, int moduleId, int moduleFeatureId)
            : this()
        {
            LoginNumber = loginNumber;
            PasswordHash = passwordHash;
            m_moduleId = moduleId;
            m_moduleFeatureId = moduleFeatureId;
        }

        /// <summary>
        /// Initializes a new instance of the GetStaffModuleFeaturesMessage 
        /// class with the specified parameters
        /// </summary>
        /// <param name="magCardNumber">The magnetic card number of the staff 
        /// to get features for.</param>
        /// <param name="passwordHash">The password hash of the staff.</param>
        /// <param name="moduleId">The id of the module to get features 
        /// for.</param>
        /// <param name="moduleFeatureId">The id of the module feature to 
        /// return or 0 for all features for the specified module.</param>
        public GetStaffModuleFeaturesMessage(string magCardNumber, byte[] passwordHash, int moduleId, int moduleFeatureId)
            : this()
        {
            MagneticCardNumber = magCardNumber;
            PasswordHash = passwordHash;
            m_moduleId = moduleId;
            m_moduleFeatureId = moduleFeatureId;
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

            // Module Feature Id
            requestWriter.Write(m_moduleFeatureId);

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
                GetStaffModuleFeaturesReturnCode code = (GetStaffModuleFeaturesReturnCode)e.ReturnCode;

                if(code == GetStaffModuleFeaturesReturnCode.InactiveStaff || code == GetStaffModuleFeaturesReturnCode.IncorrectPassword ||
                   code == GetStaffModuleFeaturesReturnCode.PasswordHasExpired || code == GetStaffModuleFeaturesReturnCode.StaffNotFound)
                    return;
                else
                    throw e; // Rethrow
            }

            // Create the streams we will be reading from.
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Check the response length.
            if(responseStream.Length < MinResponseMessageLength)
                throw new MessageWrongSizeException("Get Staff Module Features");

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Get the count of features.
                ushort featureCount = responseReader.ReadUInt16();

                // Clear the feature list.
                m_moduleFeatureList.Clear();

                // Get all the features.
                for(ushort x = 0; x < featureCount; x++)
                {
                    m_moduleFeatureList.Add(responseReader.ReadInt32());
                }

                m_staffId = responseReader.ReadInt32();
            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Staff Module Features", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Get Staff Module Features", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the id of the staff to get features for or 0 if 
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
        /// Gets or sets the login number of the staff to get features for or 
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
        /// Gets or sets the magnetic card number of the staff to get features 
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
        /// Gets or sets the id of the module to get features for.
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
        /// Gets or sets the id of the module feature to return or 0 for all 
        /// features for the specified module.
        /// </summary>
        public int ModuleFeatureId
        {
            get
            {
                return m_moduleFeatureId;
            }
            set
            {
                m_moduleFeatureId = value;
            }
        }
        
        /// <summary>
        /// Gets the list of modules features the staff has access to.
        /// </summary>
        public int[] ModuleFeatureList
        {
            get
            {
                return m_moduleFeatureList.ToArray();
            }
        }
        #endregion
    }
}
