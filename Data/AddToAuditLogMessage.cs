// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2013-2019 FortuNet


using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace GTI.Modules.Shared.Data
{
    public enum AuditType
    {
        Progressive = 1,
        Inventory = 2,
        Payouts = 3,
        Staff = 4,
        StaffFailedLogins = 5,
        ProgressiveSettingChanges = 6,
        SystemSettingChanges = 7,
        PointsStructureChanges = 8,
        AccountsAndPermissions = 9,
        TexasPayoutChanges = 10,
        Receipts = 11,
        BanksPriorDayModifications = 12,
        PayoutsPriorDayChanges = 13,
        General = 14,
        B3Settings = 15,
        Blower = 16,
        Reports = 17,
        Program = 18,
        Authorizations = 19
    }

    public class AddToAuditLogMessage : ServerMessage
    {
        #region Member variables and classes

        protected AuditType m_auditType = AuditType.General;
        protected int m_authStaffID = 0;
        protected string m_description = "*No description set*";

        #endregion

        #region Member Properties
        
        /// <summary>
        /// Get/set the type code for the audit log entry.
        /// </summary>
        public AuditType AuditEntryType
        {
            get
            {
                return m_auditType;
            }
            
            set
            {
                m_auditType = value;
            }
        }

        /// <summary>
        /// Get/set the ID of the staff member who authorized this event.
        /// 0 will use the ID of the staff member who is logged in to this machine.
        /// </summary>
        public int AuthorizedStaffID
        {
            get
            {
                return m_authStaffID;
            }
            
            set
            {
                m_authStaffID = value;
            }
        }

        /// <summary>
        /// Get/set the description of the event.
        /// </summary>
        public string Description
        {
            get
            {
                return m_description;
            }
            
            set
            {
                m_description = value;
            }
        }

        #endregion
        
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the Set Promo Text message
        /// </summary>
        AddToAuditLogMessage()
        {
            m_id = 18264; // Modify Channel Data Message
        }

        public AddToAuditLogMessage(AuditType auditType, string description, int authStaffID = 0)
        {
            m_id = 18264; // Modify Channel Data Message
            AuditEntryType = auditType;
            Description = description;
            AuthorizedStaffID = authStaffID;
        }

        #endregion

        #region Member Methods

        /// <summary>
        /// Prepares the request to be sent to the server
        /// </summary>
        protected override void PackRequest()
        {
            // Create the streams that will be written to
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            requestWriter.Write((int)m_auditType);
            requestWriter.Write(m_authStaffID);
            requestWriter.Write((ushort)m_description.Length);
            requestWriter.Write(m_description.ToCharArray());

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
        }

        #endregion
    }
}