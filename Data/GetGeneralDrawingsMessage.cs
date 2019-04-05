using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeneralPlayerDrawing = GTI.Modules.Shared.Business.GeneralPlayerDrawing;

namespace GTI.Modules.Shared.Data
{
    public class GetGeneralDrawingsMessage : ServerMessage
    {
        #region Constructors

        private GetGeneralDrawingsMessage(int drawingId)
        {
            m_id = (int)GTIServerMessageId.MGMT_GET_GENERAL_DRAWING_DATA;
            DrawingId = drawingId;
            Drawings = new List<GeneralPlayerDrawing>();
        }

        #endregion

        #region Member Variables

        public int DrawingId { get; private set; }

        public List<GeneralPlayerDrawing> Drawings { get; protected set; }

        #endregion

        #region Member Methods

        public static List<GeneralPlayerDrawing> GetDrawings(int drawingId = 0)
        {
            var msg = new GetGeneralDrawingsMessage(drawingId);
            try
            {
                msg.Send();
            }
            catch(ServerCommException ex)
            {
                throw new Exception("GetDrawings: " + ex.Message);
            }
            return msg.Drawings;
        }

        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            var requestStream = new MemoryStream();
            var requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Drawing Id
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

                    // Get the count of Product Types.
                    var drawingCount = reader.ReadInt32();

                    // Clear prior results
                    Drawings.Clear();

                    // Get all the drawings
                    for(int dI = 0; dI < drawingCount; dI++)
                    {
                        var drawing = new GeneralPlayerDrawing();

                        drawing.Id = reader.ReadInt32();
                        drawing.Name = ReadString(reader);
                        drawing.Description = ReadString(reader);
                        drawing.Active = reader.ReadBoolean();
                        drawing.EntriesDrawn = reader.ReadInt32();
                        drawing.MinimumEntries = reader.ReadInt32();
                        drawing.PlayerPresenceRequired = reader.ReadBoolean();
                        drawing.ShowEntriesOnReceipts = reader.ReadBoolean();
                        drawing.MaximumDrawsPerPlayer = reader.ReadInt32();

                        drawing.InitialEventEntryPeriodBegin = ReadDateTime(reader) ?? DateTime.MinValue;
                        drawing.InitialEventEntryPeriodEnd = ReadDateTime(reader) ?? DateTime.MinValue;
                        drawing.InitialEventScheduledForWhen = ReadDateTime(reader);
                        drawing.EventRepeatInterval = ReadString(reader);
                        drawing.EventRepeatIncrement = reader.ReadInt16();
                        drawing.EventRepeatUntil = ReadDateTime(reader);

                        var sessionNumbersCount = reader.ReadByte();
                        for(Byte b = 0; b < sessionNumbersCount; ++b)
                            drawing.EntrySessionNumbers.Add(reader.ReadByte());

                        Int16 shortCount;
                        drawing.EntrySpendGrouping = (GeneralPlayerDrawing.SpendGrouping)reader.ReadByte();
                        shortCount = reader.ReadInt16();
                        for(Int16 tier = 0; tier < shortCount; ++tier)
                        {
                            var tierBegin = ReadDecimal(reader) ?? 0m;
                            var tierEnd = ReadDecimal(reader) ?? 0m;
                            var entries = reader.ReadInt32();

                            var t = new GeneralPlayerDrawing.EntryTier<decimal>(tierBegin, tierEnd, entries);
                            drawing.EntrySpendTiers.Add(t);
                        }

                        drawing.EntryVisitType = (GeneralPlayerDrawing.VisitType)reader.ReadByte();
                        shortCount = reader.ReadInt16();
                        for(Int16 tier = 0; tier < shortCount; ++tier)
                        {
                            var visitCountFrom = reader.ReadInt32();
                            var visitCountTo = reader.ReadInt32();
                            var entries = reader.ReadInt32();

                            var t = new GeneralPlayerDrawing.EntryTier<int>(visitCountFrom, visitCountTo, entries);
                            drawing.EntryVisitTiers.Add(t);
                        }

                        drawing.EntryPurchaseType = (GeneralPlayerDrawing.PurchaseType)reader.ReadByte();
                        drawing.EntryPurchaseGrouping = (GeneralPlayerDrawing.PurchaseGrouping)reader.ReadByte();
                        shortCount = reader.ReadInt16();
                        for(Int16 tier = 0; tier < shortCount; ++tier)
                        {
                            var purchaseCountFrom = reader.ReadInt32();
                            var purchaseCountTo = reader.ReadInt32();
                            var entries = reader.ReadInt32();

                            var t = new GeneralPlayerDrawing.EntryTier<int>(purchaseCountFrom, purchaseCountTo, entries);
                            drawing.EntryPurchaseTiers.Add(t);
                        }

                        shortCount = reader.ReadInt16();
                        for(int i = 0; i < shortCount; ++i)
                            drawing.EntryPurchasePackageIds.Add(reader.ReadInt32());

                        shortCount = reader.ReadInt16();
                        for(int i = 0; i < shortCount; ++i)
                            drawing.EntryPurchaseProductIds.Add(reader.ReadInt32());

                        drawing.PlayerEntryMaximum = reader.ReadInt32();
                        if(drawing.PlayerEntryMaximum == 0)
                            drawing.PlayerEntryMaximum = null;

                        Drawings.Add(drawing);
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
