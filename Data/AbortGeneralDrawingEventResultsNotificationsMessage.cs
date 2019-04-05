using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeneralPlayerDrawingEvent = GTI.Modules.Shared.Business.GeneralPlayerDrawingEvent;

namespace GTI.Modules.Shared.Data
{
    public class AbortGeneralDrawingEventResultsNotificationsMessage : ServerMessage
    {
        #region Constructors

        private AbortGeneralDrawingEventResultsNotificationsMessage()
        {
            m_id = (int)GTIServerMessageId.MGMT_ABORT_GENERAL_DRAWING_EVENT_RESULTS_NOTIFICATIONS;
        }

        #endregion

        #region Member Variables

        #endregion

        #region Member Methods

        public static bool AbortResultsNotifications()
        {
            var msg = new AbortGeneralDrawingEventResultsNotificationsMessage();
            try
            {
                msg.Send();
            }
            catch(ServerCommException ex)
            {
                throw new Exception("AbortResultsNotifications: " + ex.Message);
            }
            return true;
        }

        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            var requestStream = new MemoryStream();
            var requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Parameters
            //requestWriter.Write(EventId);

            // Set the bytes to be sent.
            m_requestPayload = requestStream.ToArray();

            // Close the streams.
            requestWriter.Close();
        }

        #endregion
    }
}
