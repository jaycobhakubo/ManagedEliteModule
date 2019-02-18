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
    /// Represents a GetStaffOperators server message.
    /// </summary>
    public class GetStaffOperatorsMessage : ServerMessage
    {
        #region Constants and Data Types
        protected const int MinResponseMessageLength = 6;
        #endregion

        #region Member Variables
        protected int m_staffId = 0;
        protected List<Operator> m_operators = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetStaffOperatorsMessage class.
        /// </summary>
        public GetStaffOperatorsMessage()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetStaffOperatorsMessage class
        /// with the specified staff id.
        /// </summary>
        /// <param name="staffId">The id of the staff member to get the 
        /// operators for.</param>
        public GetStaffOperatorsMessage(int staffId)
        {
            m_id = 18022; // Get Staff Operator List
            m_staffId = staffId;
            m_operators = new List<Operator>();
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
                throw new MessageWrongSizeException("Get Staff Operator List");

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Get the count of operators.
                ushort operatorCount = responseReader.ReadUInt16();

                // Clear the operator array.
                m_operators.Clear();

                // Get all the operators.
                for(ushort x = 0; x < operatorCount; x++)
                {
                    Operator op = new Operator();

                    // Operator Id
                    op.Id = responseReader.ReadInt32();

                    // Operator Name
                    ushort stringLen = responseReader.ReadUInt16();
                    op.Name = new string(responseReader.ReadChars(stringLen));

                    m_operators.Add(op);
                }
            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Staff Operator List", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Get Staff Operator List", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the id of the staff member to get operators for.
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
        /// Gets a list of all operators recieived from the server.
        /// </summary>
        public Operator[] Operators
        {
            get
            {
                return m_operators.ToArray();
            }
        }
        #endregion
    }
}
