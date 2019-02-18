// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using EliteMCPLib;
using GTI.Modules.Shared.Properties;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Handles all server communications for modules.
    /// </summary>
    public class ModuleComm
    {
        #region Member Variables
        IEliteMCP m_mcp = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ModuleComm class.
        /// </summary>
        /// <exception cref="GTI.Modules.Shared.ModuleException">Elite MCP 
        /// communication failed.</exception>
        public ModuleComm()
        {
            m_mcp = new EliteMCPClass();

            if(m_mcp.IsActive() == 0)
                throw new ModuleException(Resources.MCPNotActive);
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Returns the operator id for the module.
        /// </summary>
        /// <returns>The operator id.</returns>
        public int GetOperatorId()
        {
            return m_mcp.GetOperatorID();
        }

        /// <summary>
        /// Returns the device id for the module.
        /// </summary>
        /// <returns>The device id.</returns>
        public int GetDeviceId()
        {
            return m_mcp.GetDeviceID();
        }

        /// <summary>
        /// Returns the machine id for the module.
        /// </summary>
        /// <returns>The machine id.</returns>
        public int GetMachineId()
        {
            return m_mcp.GetMachineID();
        }

        public string GetMachineDescription()
        {
            return m_mcp.GetMachineDesc();
        }

        /// <summary>
        /// Returns the staff id for the module.
        /// </summary>
        /// <returns>The staff id.</returns>
        public int GetStaffId()
        {
            return m_mcp.GetStaffID();
        }

        /// <summary>
        /// Returns whether the specified module is install on the 
        /// local machine.
        /// </summary>
        /// <param name="moduleId">The id of the module to check.</param>
        /// <returns>true if the module is present; otherwise false.</returns>
        public bool IsModuleInstalled(int moduleId)
        {
            return (1 == m_mcp.IsModuleInstalled(moduleId));
        }

        /// <summary>
        /// Establishes a connection to the specified computer using the GTI 
        /// protocol.
        /// </summary>
        /// <param name="serverName">The server's name or IP address.</param>
        /// <param name="port">The TCP port to connect to.</param>
        /// <returns>The id of the new connection or 0 if the call 
        /// failed.</returns>
        public int Connect(string serverName, int port)
        {
            return m_mcp.Connect(serverName, port);
        }

        /// <summary>
        /// Sends an array of bytes, synchronously, to the server.
        /// </summary>
        /// <param name="commandId">The id of the server command 
        /// to send.</param>
        /// <param name="request">The request payload of the command.</param>
        /// <param name="response">The response payload from the 
        /// server.</param>
        /// <param name="connectionId">The id of the connection to send the 
        /// message through (0 is always GTI Server).</param>
        public void SendMessageSync(int commandId, object request, out object response, int connectionId)
        {
            m_mcp.SendMessageWaitReply(commandId, request, out response, connectionId);
        }

        /// <summary>
        /// Closes a connection previously opened with Connect.
        /// </summary>
        /// <param name="connectionId">The id of the connection to 
        /// terminate.</param>
        public void TerminateConnection(int connectionId)
        {
            m_mcp.TerminateConnection(connectionId);
        }

        // PDTS 966
        /// <summary>
        /// Adds a module to the list to be notified when the specified message 
        /// is receieved.
        /// </summary>
        /// <param name="moduleId">The id that the EliteMCP assigned when 
        /// StartModule was called.</param>
        /// <param name="commandId">The id of the server command to subscribe 
        /// to.</param>
        public void SubscribeToMessage(int moduleId, int commandId)
        {
            if(moduleId > -1)
                m_mcp.SubscribeToMessage(moduleId, commandId);
        }

        /// <summary>
        /// Removes a module from the list to be notified when the specified 
        /// message is receieved.
        /// </summary>
        /// <param name="moduleId">The id that the EliteMCP assigned when 
        /// StartModule was called.</param>
        /// <param name="commandId">The id of the server command to 
        /// unsubscribe from.</param>
        public void UnsubscribeFromMessage(int moduleId, int commandId)
        {
            if(moduleId > -1)
                m_mcp.UnsubscribeFromMessage(moduleId, commandId);
        }
        #endregion
    }

    // PDTS 966
    /// <summary>
    /// Provides data for when the EliteMCP reports a server initiated message 
    /// to a module.
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        #region Member Variables
        protected int m_commandId;
        protected object m_messageData;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MessageReceivedEventArgs class.
        /// </summary>
        public MessageReceivedEventArgs()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MessageReceivedEventArgs class.
        /// </summary>
        /// <param name="commandId">The id of the message received.</param>
        /// <param name="messageData">The payload data of the message or null 
        /// if the message has no data.</param>
        public MessageReceivedEventArgs(int commandId, object messageData)
            : base()
        {
            m_commandId = commandId;
            m_messageData = messageData;
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets the id of the message received.
        /// </summary>
        public int CommandId
        {
            get
            {
                return m_commandId;
            }
        }

        /// <summary>
        /// Gets the payload data of the message or null if the message has no 
        /// data.
        /// </summary>
        public object MessageData
        {
            get
            {
                return m_messageData;
            }
        }
        #endregion
    }

    // PDTS 966
    /// <summary>
    /// Represents the method that handles a message received event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A MessageReceivedEventArgs that contains the event 
    /// data.</param>
    public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);
}
