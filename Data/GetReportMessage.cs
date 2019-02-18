// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace GTI.Modules.Shared
{
    public enum ReportIDs
    {
        NotImplemented = 0,
        Player_CreditBalances = 1,
        // TTP 50114
        Sales_CrystalBallSales = 2,
        Bingo_GameBallCallReportByGame = 3,
        Exceptions_ExceptionReport = 5,
	    Electronics_MachineInventory = 6,
	    Electronics_MachineStatus = 7,
	    Misc_StaffList = 8,
	    Player_PlayerListLastName = 9,
	    Player_PlayerListDetailsLastName = 10,
	    Player_PlayerLoyaltyListByTier = 11,
        // Rally US1648
        POS_RegisterSalesReport = 12,
	    Player_PointLiability = 13,
	    Bingo_POSMenuReport = 14,
	    Bingo_ProgramReportActive = 15,
	    Sales_BingoCardSalesDetail = 16,
	    Sales_SalesByDeviceTotals = 17,
	    Sales_SalesTotals = 18,
	    Player_PlayerMailingLabels = 19,
	    Sales_RegisterDetail = 20,
	    Player_PlayerSales = 21,
	    Bingo_GameWinners = 22,
	    Sales_UnplayedPackReport = 23,
	    Bingo_BingoCardSummary = 24,
        TaxForms_1042S = 33,
	    TaxForms_W2GForm = 34,
        Gaming_KenoPlayerPicks = 36,
	    Gaming_KenoSummary = 37,
	    Gaming_LinkedBingoCallsByGame = 38,
	    Gaming_LinkedBingoCardsInPlayByGame = 39,
	    Gaming_LinkedBingoGameByGame = 40,
	    Gaming_LinkedBingoPlayerSalesByGame = 41,
	    Gaming_LinkedBingoGameSummary = 42,
	    Gaming_LinkedBingoWinners = 43,
	    Player_CreditUsageByPlayer = 44,
	    Player_PlayerSpend = 45,
	    Player_PlayerPointsEarned = 46,
	    Gaming_PokerReport = 47,
	    Gaming_PokerSummary = 48,
	    Sales_SalesByItemTotals = 49,
	    Sales_SalesByPackageTotals = 50,
	    Gaming_SlotConfigurationReport = 51,
	    Gaming_SlotGamePaytables = 52,
	    Gaming_SlotGame = 53,
	    Gaming_SlotGameSummary = 54,
	    Gaming_SlotGameWinnerReport = 55,
	    Exceptions_SystemEvents = 56,
	    Sales_ProductReport = 57,
	    TaxForms_TaxFormSummary = 60,
	    Misc_CashReport = 61,
	    Sales_CashierDailyTotals = 62,
        // PDTS 584
        POS_MiniRegisterSalesReport = 63,
        // Rally US505
        CrystalBallPlayItSheetCards = 66,
        CrystalBallPlayItSheetLines = 67,
        // Rally TA8688
        CrystalBallPlayItSheetCardsThermal = 119,
        CrystalBallPlayItSheetLinesThermal = 120,
        PaperTransactionDetail = 121,
        InventoryTransaction = 122,
        DailyInventoryMovement = 123,
        POS_RegisterCloseReport = 124,
        POS_MiniRegisterCloseReport = 125,
        DoorSalesReportMichigan = 126
        // END: US1648

        // US1622
        , AccrualsActivityByAccount = 128
        , AccrualsBalancesReport = 129
        , AccrualsDetailsReport = 130,
        // END: US1622
        // US2150
        CrystalBallPlayItSheetVerticleLinesThermal = 175
    }

    public class GetReportMessage : ServerMessage
    {
         private int mReportID=0;
         private byte[] mReportFile;
         public GetReportMessage(int reportID)
         {
             m_id = 18102;
             mReportID = reportID;
         }

         #region Member Methods
         /// <summary>
         /// Prepares the request to be sent to the server.
         /// </summary>
         protected override void PackRequest()
         {
             // Create the streams we will be writing to.
             MemoryStream requestStream = new MemoryStream();
             BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

             // report type Id
             requestWriter.Write(mReportID);

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

             // Create the streams we will be reading from.
             MemoryStream responseStream = new MemoryStream(m_responsePayload);
             BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

             // Check the response length.
             if (responseStream.Length < 8)
                 throw new MessageWrongSizeException("GetReportMessage");

             // Try to unpack the data.

             // Seek past return code.
             responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

             // Get length
            int length = responseReader.ReadInt32();

             // Get all 
            mReportFile = new byte[length];
            responseReader.ReadBytes(length).CopyTo (mReportFile,0);
             // Close the streams.
             responseReader.Close();
         }
         #endregion


        
        public byte[] ReportFile
        { get { return mReportFile; } }
    }
}
