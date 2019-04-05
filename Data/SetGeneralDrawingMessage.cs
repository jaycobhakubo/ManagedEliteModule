using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeneralPlayerDrawing = GTI.Modules.Shared.Business.GeneralPlayerDrawing;
using System.Globalization;

namespace GTI.Modules.Shared.Data
{
    public class SetGeneralDrawingMessage : ServerMessage
    {
        #region Constructors

        private SetGeneralDrawingMessage(GeneralPlayerDrawing submittedDrawing)
        {
            m_id = (int)GTIServerMessageId.MGMT_SET_GENERAL_DRAWING_DATA;
            Submitted = submittedDrawing;
            Resulting = null;
        }

        #endregion

        #region Member Variables

        public GeneralPlayerDrawing Submitted { get; private set; }
        public GeneralPlayerDrawing Resulting { get; private set; }

        #endregion

        #region Member Methods

        public static GeneralPlayerDrawing SetDrawing(GeneralPlayerDrawing submitted)
        {
            var msg = new SetGeneralDrawingMessage(submitted);
            try
            {
                msg.Send();
            }
            catch(ServerCommException ex)
            {
                throw new Exception("SetDrawing: " + ex.Message);
            }
            return msg.Resulting;
        }

        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            var requestStream = new MemoryStream();
            var requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Drawing
            requestWriter.Write(Submitted.Id ?? 0);
            WriteString(requestWriter, Submitted.Name);
            WriteString(requestWriter, Submitted.Description);
            requestWriter.Write(Submitted.Active);
            requestWriter.Write(Submitted.EntriesDrawn);
            requestWriter.Write(Submitted.MinimumEntries);
            requestWriter.Write(Submitted.PlayerPresenceRequired);
            requestWriter.Write(Submitted.ShowEntriesOnReceipts);
            requestWriter.Write(Submitted.MaximumDrawsPerPlayer);

            WriteDateTime(requestWriter, Submitted.InitialEventEntryPeriodBegin);
            WriteDateTime(requestWriter, Submitted.InitialEventEntryPeriodEnd);
            WriteDateTime(requestWriter, Submitted.InitialEventScheduledForWhen);
            WriteString(requestWriter, Submitted.EventRepeatInterval);
            requestWriter.Write(Submitted.EventRepeatIncrement);
            WriteDateTime(requestWriter, Submitted.EventRepeatUntil);

            requestWriter.Write((byte)Submitted.EntrySessionNumbers.Count);
            foreach(var s in Submitted.EntrySessionNumbers)
                requestWriter.Write(s);

            requestWriter.Write((byte)Submitted.EntrySpendGrouping);
            requestWriter.Write((Int16)Submitted.EntrySpendTiers.Count);
            foreach(var t in Submitted.EntrySpendTiers)
            {
                WriteDecimal(requestWriter, t.TierBegin);
                WriteDecimal(requestWriter, t.TierEnd);
                requestWriter.Write(t.Entries);
            }

            requestWriter.Write((byte)Submitted.EntryVisitType);
            requestWriter.Write((Int16)Submitted.EntryVisitTiers.Count);
            foreach(var t in Submitted.EntryVisitTiers)
            {
                requestWriter.Write(t.TierBegin);
                requestWriter.Write(t.TierEnd);
                requestWriter.Write(t.Entries);
            }

            requestWriter.Write((byte)Submitted.EntryPurchaseType);
            requestWriter.Write((byte)Submitted.EntryPurchaseGrouping);
            requestWriter.Write((Int16)Submitted.EntryPurchaseTiers.Count);
            foreach(var t in Submitted.EntryPurchaseTiers)
            {
                requestWriter.Write(t.TierBegin);
                requestWriter.Write(t.TierEnd);
                requestWriter.Write(t.Entries);
            }

            requestWriter.Write((Int16)Submitted.EntryPurchasePackageIds.Count);
            foreach(var id in Submitted.EntryPurchasePackageIds)
                requestWriter.Write(id);

            requestWriter.Write((Int16)Submitted.EntryPurchaseProductIds.Count);
            foreach(var id in Submitted.EntryPurchaseProductIds)
                requestWriter.Write(id);

            requestWriter.Write(Submitted.PlayerEntryMaximum ?? 0);

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
                        var moneyStr = ReadString(reader);
                        var tierBegin = decimal.Parse(moneyStr);
                        moneyStr = ReadString(reader);
                        var tierEnd = decimal.Parse(moneyStr);
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

                    Resulting = drawing;

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
