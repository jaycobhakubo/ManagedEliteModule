// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.IO;
using System.Globalization;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// An enumeration of the possible generic error codes returned from 
    /// GTIServer.
    /// </summary>
    public enum GTIServerReturnCode
    {
        Success                     = 0,
        GeneralError                = -1,
        ServerBusy                  = -2,
        ParamError                  = -3,
        SQLError                    = -4,
        UnauthorizedAction          = -5,
		StringLengthError		    = -6,
        ErrorStartingTrans          = -8,
        ErrorCommitingTrans         = -9,
        ErrorRollbackTrans          = -10,
        ErrorUpdatingTable          = -11,
        ErrorWritingFile            = -12,
        MulticastError              = -13,
        ConnectFailure              = -14,
        ResultsPending              = -15,
        NoPendingRequest            = -16,
        DuplicateTableEntry         = -17,
        MissingTableEntry           = -18,
        UnableToAccessDatabase      = -19,
        MsgHandlerNotFound          = -20,
        InsufficientRights          = -21,
        MissingStoredProcedure      = -22,
        AllocMemoryFailure          = -25,
        CentralServerCommError      = -32,
        CardSalesUnavailable        = -33, // Rally DE4225
        InUse                       = -86, // FIX: DE3188 support new error code
        InsufficientPoints          = -88,
        InsufficientInventory       = -94, // Rally US1283

		// I added these to handle message response errors
		InvalidResonseSize		= -100,
		ServerCommError			= -101,
        AccountLocked           = -106,
        PasswordReuseError      = -107
	}

    /// <summary>
    /// The abstract base class from which all message to the 
    /// GTIServer should derive.  Inheritors should set the id
    /// field of the message before calling Send.  This class sends
    /// "raw" type GTIServer messages.
    /// </summary>
    public abstract class ServerMessage
    {
        #region Member Variables
        protected int m_id = 0;
		protected string m_strMessageName = "";
		protected byte[] m_requestPayload = null;
        protected byte[] m_responsePayload = null;
        protected int m_returnCode = (int)GTIServerReturnCode.GeneralError;
        #endregion

        #region Member Methods
        /// <summary>
        /// Prepares the request to be sent to the server.  All subclasses must
        /// implement this method.
        /// </summary>
        protected abstract void PackRequest();

        /// <summary>
        /// Parses the response received from the server.  Subclasses should
        /// override this method if more than the return code is send from the
        /// server.  When overriding UnpackResponse in a derived class, you can
        /// call the base class's UnpackResponse method to parse the 
        /// return code.
        /// </summary>
        /// <exception cref="GTI.Modules.Shared.ServerCommException">Thrown when
        /// the server failed to respond.</exception>
        /// <exception cref="GTI.Modules.Shared.ServerException">Thrown in case 
        /// of any error or an unsuccessful return code.</exception>
        protected virtual void UnpackResponse()
        {
            // Check to see if we got the payload correctly.
            if(m_responsePayload == null)
                throw new ServerCommException("Server communication lost.");
                
            if(m_responsePayload.Length < sizeof(int))
                throw new MessageWrongSizeException("Message payload size is too small.");

            // Check the return code.
            m_returnCode = BitConverter.ToInt32(m_responsePayload, 0);

            if(m_returnCode != (int)GTIServerReturnCode.Success)
                throw new ServerException((GTIServerReturnCode)m_returnCode, "Server Error Code: " + m_returnCode.ToString());
        }

        /// <summary>
        /// Packs the request, sends the message to the server, then
        /// unpacks the response.
        /// </summary>
        public virtual void Send()
        {
            // Prepare the request.
            PackRequest();

            // Box the response.
            object response = (object)m_responsePayload;

            // Create an instance of the comm. interface.
            ModuleComm comm = new ModuleComm();
            comm.SendMessageSync(m_id, (object)m_requestPayload, out response, 0);

            // Unbox the response.
            m_responsePayload = (byte[])response;

            // Parse the response.
            UnpackResponse();
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Writes a string to the specified BinaryWriter in a format the
        /// server expects.
        /// </summary>
        /// <param name="writer">The BinaryWriter to write to.</param>
        /// <param name="data">The string to write.</param>
        /// <exception cref="System.ArgumentNullException">writer is a null
        /// reference.</exception>
        /// <exception cref="System.IO.IOException">An I/O error
        /// occurred.</exception>
        /// <exception cref="System.ObjectDisposedException">The stream is
        /// closed.</exception>
        protected static void WriteString(BinaryWriter writer, string data)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (!string.IsNullOrEmpty(data))
            {
                writer.Write((ushort)data.Length);
                writer.Write(data.ToCharArray());
            }
            else
            {
                writer.Write((ushort)0);
            }
        }

        /// <summary>
        /// Writes a date/time to the specified BinaryWriter in a format the
        /// server expects.
        /// </summary>
        /// <param name="writer">The BinaryWriter to write to.</param>
        /// <param name="data">The DateTime to write.</param>
        /// <exception cref="System.ArgumentNullException">writer is a null
        /// reference.</exception>
        /// <exception cref="System.IO.IOException">An I/O error
        /// occurred.</exception>
        /// <exception cref="System.ObjectDisposedException">The stream is
        /// closed.</exception>
        protected static void WriteDateTime(BinaryWriter writer, DateTime data)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            string tempDate = data.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            writer.Write((ushort)tempDate.Length);
            writer.Write(tempDate.ToCharArray());
        }

        /// <summary>
        /// Writes a decimal to the specified BinaryWriter in a format the
        /// server expects.
        /// </summary>
        /// <param name="writer">The BinaryWriter to write to.</param>
        /// <param name="data">The decimal to write.</param>
        /// <param name="format">A standard or custom numeric format
        /// string.</param>
        /// <exception cref="System.ArgumentNullException">writer is a null
        /// reference.</exception>
        /// <exception cref="System.IO.IOException">An I/O error
        /// occurred.</exception>
        /// <exception cref="System.ObjectDisposedException">The stream is
        /// closed.</exception>
        /// <remarks>If format is null or empty, a default of F2 is
        /// used.</remarks>
        protected static void WriteDecimal(BinaryWriter writer, decimal data, string format)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            string tempDec = data.ToString(string.IsNullOrWhiteSpace(format) ? "F2" : format, CultureInfo.InvariantCulture);
            writer.Write((ushort)tempDec.Length);
            writer.Write(tempDec.ToCharArray());
        }

        /// <summary>
        /// Reads a string from the BinaryReader.
        /// </summary>
        /// <param name="reader">The BinaryReader to read from.</param>
        /// <returns>A string instance or null.</returns>
        /// <exception cref="System.ArgumentNullException">reader is a null
        /// reference.</exception>
        /// <exception cref="System.IO.EndOfStreamException">The end of the
        /// stream was reached.</exception>
        /// <exception cref="System.IO.IOException">An I/O error
        /// occurred.</exception>
        /// <exception cref="System.ObjectDisposedException">The stream is
        /// closed.</exception>
        protected static string ReadString(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            ushort stringLen = reader.ReadUInt16();

            if (stringLen > 0)
                return new string(reader.ReadChars(stringLen));
            else
                return null;
        }

        /// <summary>
        /// Reads a DateTime from the BinaryReader.
        /// </summary>
        /// <param name="reader">The BinaryReader to read from.</param>
        /// <returns>A valid DateTime or null if there was no date.</returns>
        /// <exception cref="System.ArgumentNullException">reader is a null
        /// reference.</exception>
        /// <exception cref="System.IO.EndOfStreamException">The end of the
        /// stream was reached.</exception>
        /// <exception cref="System.IO.IOException">An I/O error
        /// occurred.</exception>
        /// <exception cref="System.ObjectDisposedException">The stream is
        /// closed.</exception>
        protected static DateTime? ReadDateTime(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            ushort stringLen = reader.ReadUInt16();

            if (stringLen > 0)
                return DateTime.Parse(new string(reader.ReadChars(stringLen)), CultureInfo.InvariantCulture);
            else
                return null;
        }

        /// <summary>
        /// Reads a decimal from the BinaryReader.
        /// </summary>
        /// <param name="reader">The BinaryReader to read from.</param>
        /// <returns>A valid decimal or null if there was no data.</returns>
        /// <exception cref="System.ArgumentNullException">reader is a null
        /// reference.</exception>
        /// <exception cref="System.IO.EndOfStreamException">The end of the
        /// stream was reached.</exception>
        /// <exception cref="System.IO.IOException">An I/O error
        /// occurred.</exception>
        /// <exception cref="System.ObjectDisposedException">The stream is
        /// closed.</exception>
        protected static decimal? ReadDecimal(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            ushort stringLen = reader.ReadUInt16();

            if (stringLen > 0)
                return decimal.Parse(new string(reader.ReadChars(stringLen)), CultureInfo.InvariantCulture);
            else
                return null;
        }

        /// <summary>
        /// Writes a decimal value to the BinaryWriter
        /// </summary>
        /// <param name="writer">The BinaryWriter to write to.</param>
        /// <param name="data"></param>
        protected static void WriteDecimal(BinaryWriter writer, decimal data)
        {
            WriteString(writer, data.ToString());
        }
        
        /// <summary>
        /// Writes a nullable int to the output stream
        /// </summary>
        /// <param name="value"></param>
        /// <param name="stream"></param>
        public static void WriteNullableInt(BinaryWriter writer, int? value)
        {
            writer.Write((bool)value.HasValue);
            if (value.HasValue)
                writer.Write((int)value.Value);
        }

        /// <summary>
        /// reads a nullable int from the input stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static int? ReadNullableInt(BinaryReader reader)
        {
            if (reader.ReadBoolean())
                return reader.ReadInt32();
            return null;
        }

        /// <summary>
        /// Writes a nullable int to the output stream
        /// </summary>
        /// <param name="value"></param>
        /// <param name="stream"></param>
        public static void WriteNullableByte(BinaryWriter writer, byte? value)
        {
            writer.Write((bool)value.HasValue);
            if (value.HasValue)
                writer.Write((byte)value.Value);
        }

        /// <summary>
        /// reads a nullable int from the input stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte? ReadNullableByte(BinaryReader reader)
        {
            if (reader.ReadBoolean())
                return reader.ReadByte();
            return null;
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets the id of the server message.
        /// </summary>
        public int Id
        {
            get
            {
                return m_id;
            }
        }

        /// <summary>
        /// Gets the return code received from the server.
        /// </summary>
        public int ReturnCode
        {
            get
            {
                return m_returnCode;
            }
        }

		/// <summary>
		/// Used to return descriptive error messages
		/// </summary>
		public string MessageName
		{
			get
			{
				if (m_strMessageName.Length > 0)
					return m_strMessageName;
				else
					return Convert.ToString(m_id);
			}
		}

		/// <summary>
		/// Used to eliminate the need for (int) casts
		/// </summary>
		public GTIServerReturnCode ServerReturnCode
		{
			get { return (GTIServerReturnCode)m_returnCode; }
		}
		#endregion
    }
}
