using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeneralPlayerDrawingEvent = GTI.Modules.Shared.Business.GeneralPlayerDrawingEvent;

namespace GTI.Modules.Shared.Data
{
    public class ExecuteGeneralDrawingEventMessage : ServerMessage
    {
        #region Constructors

        private ExecuteGeneralDrawingEventMessage(int eventId, bool retrieveEntries, bool retrieveResults)
        {
            m_id = (int)GTIServerMessageId.MGMT_EXECUTE_GENERAL_DRAWING_EVENT;
            EventId = eventId;
            RetrievingEntries = retrieveEntries;
            RetrievingResults = retrieveResults;
            Executed = false;
            DrawingEvent = null;
        }

        #endregion

        #region Member Variables

        public int EventId { get; private set; }
        public bool RetrievingEntries { get; private set; }
        public bool RetrievingResults { get; private set; }

        public bool Executed { get; private set; }
        public GeneralPlayerDrawingEvent DrawingEvent { get; protected set; }

        #endregion

        #region Member Methods

        public static Tuple<bool, GeneralPlayerDrawingEvent> ExecuteEvent(int eventId, bool retrieveEntries = false, bool retrieveResults = true)
        {
            var msg = new ExecuteGeneralDrawingEventMessage(eventId, retrieveEntries, retrieveResults);
            try
            {
                msg.Send();
            }
            catch(ServerCommException ex)
            {
                throw new Exception("ExecuteEvent: " + ex.Message);
            }
            return new Tuple<bool,GeneralPlayerDrawingEvent>(msg.Executed, msg.DrawingEvent);
        }

        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            var requestStream = new MemoryStream();
            var requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Parameters
            requestWriter.Write(EventId);
            requestWriter.Write(RetrievingEntries);
            requestWriter.Write(RetrievingResults);

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

                    // Clear prior results
                    DrawingEvent = null;

                    Executed = reader.ReadBoolean();

                    // Get all the events retrieved
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

                    DrawingEvent = drawingEvent;

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
