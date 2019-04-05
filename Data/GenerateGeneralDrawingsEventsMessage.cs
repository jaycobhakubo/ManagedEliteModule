using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeneralPlayerDrawingEvent = GTI.Modules.Shared.Business.GeneralPlayerDrawingEvent;

namespace GTI.Modules.Shared.Data
{
    public class GenerateGeneralDrawingsEventsMessage : ServerMessage
    {
        #region Constructors

        private GenerateGeneralDrawingsEventsMessage(DateTime entryFrom, DateTime entryTo, int drawingId)
        {
            m_id = (int)GTIServerMessageId.MGMT_GENERATE_GENERAL_DRAWING_EVENTS;
            EntryFrom = entryFrom;
            EntryTo = entryTo;
            DrawingId = drawingId;
            Events = new List<GeneralPlayerDrawingEvent>();
        }

        #endregion

        #region Member Variables

        public int DrawingId { get; private set; }
        public DateTime EntryFrom { get; private set; }
        public DateTime EntryTo { get; private set; }

        public List<GeneralPlayerDrawingEvent> Events { get; protected set; }

        #endregion

        #region Member Methods

        public static List<GeneralPlayerDrawingEvent> GenerateDrawingEvents(DateTime entryOn, int drawingId = 0)
        {
            return GenerateDrawingEvents(entryOn, entryOn, drawingId);
        }

        public static List<GeneralPlayerDrawingEvent> GenerateDrawingEvents(DateTime entryFrom, DateTime entryTo, int drawingId = 0)
        {
            var msg = new GenerateGeneralDrawingsEventsMessage(entryFrom, entryTo, drawingId);
            try
            {
                msg.Send();
            }
            catch(ServerCommException ex)
            {
                throw new Exception("GenerateDrawingEvents: " + ex.Message);
            }
            return msg.Events;
        }

        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            var requestStream = new MemoryStream();
            var requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            WriteDateTime(requestWriter, EntryFrom);
            WriteDateTime(requestWriter, EntryTo);
            requestWriter.Write(DrawingId);

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

                    // Get the count of events
                    var eventCount = reader.ReadInt32();

                    // Clear prior results
                    Events.Clear();

                    // Get all the events generated
                    for(int event_i = 0; event_i < eventCount; event_i++)
                    {
                        var drawingEvent = new GeneralPlayerDrawingEvent();

                        drawingEvent.EventId = reader.ReadInt32();
                        drawingEvent.DrawingId = reader.ReadInt32();
                        drawingEvent.EntryPeriodBegin = ReadDateTime(reader) ?? DateTime.MinValue;
                        drawingEvent.EntryPeriodEnd = ReadDateTime(reader) ?? DateTime.MinValue;
                        drawingEvent.ScheduledForWhen = ReadDateTime(reader);
                        drawingEvent.HeldWhen = ReadDateTime(reader);
                        drawingEvent.CancelledWhen = ReadDateTime(reader);
                        drawingEvent.CreatedWhen = ReadDateTime(reader);

                        var entryCount = reader.ReadInt32();
                        for(int r = 0; r < entryCount; r++)
                        {
                            var entry = new GeneralPlayerDrawingEvent.DrawingEventEntry();
                            entry.PlayerId = reader.ReadInt32();
                            entry.EntryCount = reader.ReadInt32();
                            drawingEvent.Entries.Add(entry);
                        }

                        var resultCount = reader.ReadInt32();
                        for(int r = 0; r < resultCount; r++)
                        {
                            var result = new GeneralPlayerDrawingEvent.DrawingEventResult();
                            result.PlayerId = reader.ReadInt32();
                            result.DrawingPosition = reader.ReadInt32();
                            drawingEvent.Results.Add(result);
                        }

                        Events.Add(drawingEvent);
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
            }
        }

        #endregion
    }
}
