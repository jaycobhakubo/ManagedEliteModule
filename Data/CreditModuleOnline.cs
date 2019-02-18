using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;

// TTP 50061

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Server Messages Info:
    ///  2.6.1 – Credit Module Online
    ///      This is a request message from the client to the 
    ///  GTIS app. The purpose of this message is to 
    ///  determine if the Credit Module is currently 
    ///  running
    ///  CommandID: 20000
    /// 
    ///  2.6.1.1	Request
    ///  Fields:
    ///     •	Nothing
    /// 
    /// 2.6.1.2	Response
    /// Fields:
    /// •	ReturnCode (int [4-bytes]): A standard GTI 
    ///     return code
    /// •	VersionStringLength (WORD  [2-bytes]): The 
    ///     length of the version string, excluding NULL 
    ///     termination
    /// •	VersionString (CString [??-bytes]):  
    ///     The version string representing the current 
    ///     version of the module.
    /// </summary>
    public class CreditModuleOnline : ServerMessage
    {
        #region Variables Declarations
        protected int mintCommandID = 20000;

        protected bool mbolIsCreditModuleOnline = false;
        protected string mstrCreditModuleVersion = "";
        #endregion

        #region Properties
        public bool IsCreditModuleOnline
        {
            get
            {
                return mbolIsCreditModuleOnline;
            }
        }

        public string CreditModuleVersion
        {
            get
            {
                return mstrCreditModuleVersion;
            }
        }
        #endregion


        #region Constructor
        /// <summary>
        /// Initializes a new instance of the CreditModuleOnline class.
        /// </summary>
        /// <remarks>This message is sent when the class is created.</remarks>
        public CreditModuleOnline()
        {
            m_id = mintCommandID;
            m_strMessageName = "Credit Module Online";
            Send();
        }
        #endregion

        #region Base Methods override
        /// <summary>
        /// Prepares the request to be sent to the server.
        /// </summary>
        protected override void PackRequest()
        {
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
                if(e.ReturnCode != GTIServerReturnCode.MsgHandlerNotFound)
                    throw;
            }

            // Create the streams we will be reading from.
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                if(ServerReturnCode != GTIServerReturnCode.MsgHandlerNotFound)
                {
                    // Version of Credit Module
                    ushort stringLen = responseReader.ReadUInt16();
                    string tempStr = new string(responseReader.ReadChars(stringLen));

                    if(tempStr != string.Empty)
                    {
                        mbolIsCreditModuleOnline = true;
                        mstrCreditModuleVersion = tempStr;
                    }
                }
                else
                {
                    mbolIsCreditModuleOnline = false;
                    mstrCreditModuleVersion = string.Empty;
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

        public override string ToString()
        {
            return mstrCreditModuleVersion;
        }
        #endregion






    }
}
