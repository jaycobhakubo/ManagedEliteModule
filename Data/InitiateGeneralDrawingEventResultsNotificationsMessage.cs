using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeneralPlayerDrawingEvent = GTI.Modules.Shared.Business.GeneralPlayerDrawingEvent;

namespace GTI.Modules.Shared.Data
{
    public class InitiateGeneralDrawingEventResultsNotificationsMessage : ServerMessage
    {
        #region Constructors

        private InitiateGeneralDrawingEventResultsNotificationsMessage(int eventId)
        {
            m_id = (int)GTIServerMessageId.MGMT_INITIATE_GENERAL_DRAWING_EVENT_RESULTS_NOTIFICATIONS;
            EventId = eventId;
            Initiated = false;
        }

        #endregion

        #region Member Variables

        public int EventId { get; private set; }
        public bool Initiated { get; private set; }

        #endregion

        #region Member Methods

        public static bool InitiateResultsNotifications(int eventId)
        {
            var msg = new InitiateGeneralDrawingEventResultsNotificationsMessage(eventId);
            try
            {
                msg.Send();
            }
            catch(ServerCommException ex)
            {
                throw new Exception("InitiateResultsNotifications: " + ex.Message);
            }
            return msg.Initiated;
        }

        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            var requestStream = new MemoryStream();
            var requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Parameters
            requestWriter.Write(EventId);

            // Set the bytes to be sent.
            m_requestPayload = requestStream.ToArray();

            // Close the streams.
            requestWriter.Close();
        }

        protected override void UnpackResponse()
        {
            base.UnpackResponse();

            // Create the streams we will be reading from.
            using(var responseStream = new MemoryStream(m_responsePayload))
            using(var reader = new BinaryReader(responseStream, Encoding.Unicode))
            {
                // Try to unpack the data.
                try
                {
                    // Seek past return code.
                    reader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                    Initiated = reader.ReadByte() != 0;

                }
                catch(EndOfStreamException e)
                {
                    throw new MessageWrongSizeException(m_strMessageName, e);
                }
                catch(Exception e)
                {
                    throw new ServerException(m_strMessageName, e);
                }
            }
        }

        #endregion
    }
}
