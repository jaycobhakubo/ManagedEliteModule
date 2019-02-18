// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.Globalization;
using GTI.Modules.Shared.Properties;

namespace GTI.Modules.Shared
{
    // 50114
    /// <summary>
    /// This class translates server exceptions into human readable strings.
    /// </summary>
    public class ServerExceptionTranslator
    {
        #region Static Methods
        /// <summary>
        /// Translates a server error code into a localized string.
        /// </summary>
        /// <param name="returnCode">The return code to translate.</param>
        /// <returns>A localized error message.</returns>
        public static string GetServerErrorMessage(GTIServerReturnCode returnCode)
        {
            string message;

            switch(returnCode)
            {
                case GTIServerReturnCode.Success:
                    message = Resources.GTIServerErrorSuccess;
                    break;

                case GTIServerReturnCode.GeneralError:
                    message = Resources.GTIServerErrorGeneral;
                    break;

                case GTIServerReturnCode.ServerBusy:
                    message = Resources.GTIServerErrorBusy;
                    break;

                case GTIServerReturnCode.ParamError:
                    message = Resources.GTIServerErrorParam;
                    break;

                case GTIServerReturnCode.SQLError:
                    message = Resources.GTIServerErrorSQL;
                    break;

                case GTIServerReturnCode.UnauthorizedAction:
                    message = Resources.GTIServerErrorUnauth;
                    break;

                case GTIServerReturnCode.StringLengthError:
                    message = Resources.GTIServerErrorString;
                    break;

                case GTIServerReturnCode.ErrorStartingTrans:
                    message = Resources.GTIServerErrorStartTrans;
                    break;

                case GTIServerReturnCode.ErrorCommitingTrans:
                    message = Resources.GTIServerErrorCommitTrans;
                    break;

                case GTIServerReturnCode.ErrorRollbackTrans:
                    message = Resources.GTIServerErrorRollbackTrans;
                    break;

                case GTIServerReturnCode.ErrorUpdatingTable:
                    message = Resources.GTIServerErrorUpdateTable;
                    break;

                case GTIServerReturnCode.ErrorWritingFile:
                    message = Resources.GTIServerErrorWriteFile;
                    break;

                case GTIServerReturnCode.MulticastError:
                    message = Resources.GTIServerErrorMulticast;
                    break;

                case GTIServerReturnCode.ConnectFailure:
                    message = Resources.GTIServerErrorConnect;
                    break;

                case GTIServerReturnCode.ResultsPending:
                    message = Resources.GTIServerErrorRequestPend;
                    break;

                case GTIServerReturnCode.NoPendingRequest:
                    message = Resources.GTIServerErrorNoRequests;
                    break;

                case GTIServerReturnCode.DuplicateTableEntry:
                    message = Resources.GTIServerErrorDupTable;
                    break;

                case GTIServerReturnCode.MissingTableEntry:
                    message = Resources.GTIServerErrorMissingTable;
                    break;

                case GTIServerReturnCode.UnableToAccessDatabase:
                    message = Resources.GTIServerErrorDbAccess;
                    break;

                case GTIServerReturnCode.MsgHandlerNotFound:
                    message = Resources.GTIServerErrorMsgHandler;
                    break;

                case GTIServerReturnCode.InsufficientRights:
                    message = Resources.GTIServerErrorNoRights;
                    break;

                case GTIServerReturnCode.MissingStoredProcedure:
                    message = Resources.GTIServerErrorNoSProc;
                    break;

                case GTIServerReturnCode.AllocMemoryFailure:
                    message = Resources.GTIServerErrorMemAlloc;
                    break;

                case GTIServerReturnCode.CentralServerCommError:
                    message = Resources.GTIServerErrorCentralComm;
                    break;

                // Rally DE4225 - Nondescript error code for card sale problems.
                case GTIServerReturnCode.CardSalesUnavailable:
                    message = Resources.GTIServerErrorCardSalesUnavailable;
                    break;

                case GTIServerReturnCode.InsufficientInventory: // Rally US1283
                    message = Resources.GTIServerErrorInsufficientInventory;
                    break;

                //START RALLY DE8439
                case GTIServerReturnCode.InUse:
                    message = Resources.GTIServerErrorInUse;
                    break;
                //END RALLY DE8439

                default:
                    message = Resources.GTIServerErrorUnknown;
                    break;
            }

            return message;
        }

        /// <summary>
        /// Translates a server error code into a localized string.
        /// </summary>
        /// <param name="returnCode">The return code to translate.</param>
        /// <returns>A localized error message.</returns>
        public static string GetServerErrorMessage(int returnCode)
        {
            GTIServerReturnCode code = (GTIServerReturnCode)returnCode;

            return GetServerErrorMessage(code);
        }

        /// <summary>
        /// Based on the exception passed in, this method will translate
        /// the error message to localized text and return the value.
        /// </summary>
        /// <param name="ex">The exception to format.</param>
        /// <returns>The exception's localized message.</returns>
        public static string FormatExceptionMessage(Exception ex)
        {
            if(ex is MessageWrongSizeException)
                return string.Format(CultureInfo.CurrentCulture, Resources.MessagePayloadWrongSize, ex.Message);
            else if(ex is ServerCommException)
                return Resources.ServerCommFailed;
            else if(ex is ServerException && ex.InnerException != null)
                return string.Format(CultureInfo.CurrentCulture, Resources.InvalidMessageResponse, ex.Message);
            else if(ex is ServerException)
            {
                int errorCode = (int)((ServerException)ex).ReturnCode;
                return string.Format(CultureInfo.CurrentCulture, Resources.ServerErrorCode, GetServerErrorMessage(errorCode), errorCode);
            }
            else
                return ex.Message;
        }
        #endregion
    }

    /// <summary>
    /// The exception that is thrown when a non-fatal server error occurs.
    /// </summary>
    public class ServerException : ModuleException
    {
        #region Member Variables
        protected GTIServerReturnCode m_returnCode = GTIServerReturnCode.Success;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ServerException class.
        /// </summary>
        public ServerException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ServerException class with a 
        /// specified error message.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public ServerException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ServerException class with a 
        /// specified error message and a reference to the inner exception 
        /// that is the cause of this exception.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of 
        /// the current exception. If the innerException parameter is not a 
        /// null reference, the current exception is raised in a catch block 
        /// that handles the inner exception.</param>
        public ServerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ServerException class with a 
        /// specified return code and error message.
        /// </summary>
        /// <param name="returnCode">The return code from the server.</param>
        /// <param name="message">A message that describes the error.</param>
        public ServerException(GTIServerReturnCode returnCode, string message)
            : base(message)
        {
            m_returnCode = returnCode;
        }

        /// <summary>
        /// Initializes a new instance of the ServerException class with a 
        /// specified return code, error message, and a reference to the 
        /// inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="returnCode">The return code from the server.</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of 
        /// the current exception. If the innerException parameter is not a 
        /// null reference, the current exception is raised in a catch block 
        /// that handles the inner exception.</param>
        public ServerException(GTIServerReturnCode returnCode, string message, Exception innerException)
            : base(message, innerException)
        {
            m_returnCode = returnCode;
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets the return code received from the server.
        /// </summary>
        public GTIServerReturnCode ReturnCode
        {
            get
            {
                return m_returnCode;
            }
        }
        #endregion
    }

    /// <summary>
    /// The exception that is thrown when a server communication error occurs.
    /// </summary>
    public class ServerCommException : ModuleException
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ServerCommException class.
        /// </summary>
        public ServerCommException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ServerCommException class with a 
        /// specified error message.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public ServerCommException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ServerCommException class with a 
        /// specified error message and a reference to the inner exception 
        /// that is the cause of this exception.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of 
        /// the current exception. If the innerException parameter is not a 
        /// null reference, the current exception is raised in a catch block 
        /// that handles the inner exception.</param>
        public ServerCommException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        #endregion
    }

    /// <summary>
    /// The exception that is thrown when a server message is an 
    /// unexpected size.
    /// </summary>
    public class MessageWrongSizeException : ServerCommException
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MessageWrongSizeException
        /// class.
        /// </summary>
        public MessageWrongSizeException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MessageWrongSizeException 
        /// class with a specified error message.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public MessageWrongSizeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MessageWrongSizeException class 
        /// with a specified error message and a reference to the inner 
        /// exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of 
        /// the current exception. If the innerException parameter is not a 
        /// null reference, the current exception is raised in a catch block 
        /// that handles the inner exception.</param>
        public MessageWrongSizeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        #endregion
    }
}
