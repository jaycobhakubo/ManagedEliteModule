using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeneralPlayerDrawingEvent = GTI.Modules.Shared.Business.GeneralPlayerDrawingEvent;

namespace GTI.Modules.Shared.Data
{
    public class SetGeneralDrawingEventCancelledMessage : ServerMessage
    {
        #region Constructors

        private SetGeneralDrawingEventCancelledMessage(int eventId, bool cancel)
        {
            m_id = (int)GTIServerMessageId.MGMT_SET_GENERAL_DRAWING_EVENT_CANCELLED;
            EventId = eventId;
            Cancel = cancel;
            CancelledWhen = null;
        }

        #endregion

        #region Member Variables

        public int EventId { get; private set; }
        public bool Cancel { get; private set; }
        public DateTime? CancelledWhen { get; private set; }

        #endregion

        #region Member Methods

        public static DateTime? CancelEvent(int eventId)
        {
            var msg = new SetGeneralDrawingEventCancelledMessage(eventId, true);
            try
            {
                msg.Send();
            }
            catch(ServerCommException ex)
            {
                throw new Exception("CancelEvent: " + ex.Message);
            }
            return msg.CancelledWhen;
        }

        public static DateTime? ReinstateEvent(int eventId)
        {
            var msg = new SetGeneralDrawingEventCancelledMessage(eventId, false);
            try
            {
                msg.Send();
            }
            catch(ServerCommException ex)
            {
                throw new Exception("ReinstateEvent: " + ex.Message);
            }
            return msg.CancelledWhen;
        }

        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            var requestStream = new MemoryStream();
            var requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Parameters
            requestWriter.Write(EventId);
            requestWriter.Write((byte)(Cancel ? 1 : 0));

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

                    CancelledWhen = ReadDateTime(reader);

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
