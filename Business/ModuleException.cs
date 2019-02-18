// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// The exception that is thrown when a non-fatal module error occurs.
    /// </summary>
    public class ModuleException : ApplicationException
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ModuleException class.
        /// </summary>
        public ModuleException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ModuleException class with a 
        /// specified error message.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public ModuleException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ModuleException class with a 
        /// specified error message and a reference to the inner exception 
        /// that is the cause of this exception.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of 
        /// the current exception. If the innerException parameter is not a 
        /// null reference, the current exception is raised in a catch block 
        /// that handles the inner exception.</param>
        public ModuleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        #endregion
    }    
}
