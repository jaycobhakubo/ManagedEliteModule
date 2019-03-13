#region Copyright

// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2015 All rights reserved

// US3989 Adding setting for displaying player pictures
// US3987 Adding setting for counting the number of verified winners
// US4010 Adding setting for displaying the verified card on a player unit
// US4039 Adding improved support for checking for duplicates when issuing
// US4147 Adding support for being able to specify the length of a players PIN
// US4848 Add signature lines to void receipts
#endregion


using System;
using System.ComponentModel;

namespace GTI.Modules.Shared
{
    // This file contains common data values that are shared between all 
    // modules.

    /// <summary>
    /// Contains static constants that represent the sizes of strings on 
    /// the server.
    /// </summary>
    public static class StringSizes
    {
        #region Static Properties
        /// <summary>
        /// Gets the maximum string length of names.
        /// </summary>
        public static int MaxNameLength
        {
            get
            {
                return 32;
            }
        }

        /// <summary>
        /// Gets the maximum string length of middle names.
        /// </summary>
        public static int MaxMiddleNameLength
        {
            get
            {
                return 4;
            }
        }
        /// <summary>
        /// Gets the maximum string length of a player's gov. issued id number.
        /// </summary>
        public static int MaxGovIssuedIdNumLength
        {
            get
            {
                return 24;
            }
        }

        /// <summary>
        /// Gets the maximum string length for dates.
        /// </summary>
        public static int MaxDateLength
        {
            get
            {
                return 24;
            }
        }
       
        /// <summary>
        /// Gets the maximum string length of email addresses.
        /// </summary>
        public static int MaxEmailLength
        {
            get
            {
                return 200;
            }
        }

        /// <summary>
        /// Gets the maximum string length of player identities.
        /// </summary>
        public static int MaxPlayerIdentLength
        {
            get
            {
                return 32;
            }
        }

        /// <summary>
        /// Gets the maximum string length of phone numbers.
        /// </summary>
        public static int MaxPhoneNumberLength
        {
            get
            {
                return 32;
            }
        }

        /// <summary>
        /// Gets the maximum string length of gender abbrivations.
        /// </summary>
        public static int MaxGenderLength
        {
            get
            {
                return 4;
            }
        }

        /// <summary>
        /// Gets the maximum string length of pin numbers.
        /// </summary>
        public static int MaxPinNumLength
        {
            get
            {
                return 255; // Rally DE1569 - Security Center data validation.
            }
        }

        /// <summary>
        /// Gets the maximum string length of addresses.
        /// </summary>
        public static int MaxAddressLength
        {
            get
            {
                return 64;
            }
        }

        /// <summary>
        /// Gets the maximum string length of city, states, zip/postal codes, 
        /// and countries.
        /// </summary>
        public static int MaxCityStateZipCountryLength
        {
            get
            {
                return 32;
            }
        }

        /// <summary>
        /// Gets the maximum string length of player comments.
        /// </summary>
        public static int MaxPlayerCommentLength
        {
            get
            {
                return 255;
            }
        }

        /// <summary>
        /// Gets the maximum string length for magnetic card numbers.
        /// </summary>
        public static int MaxMagneticCardLength
        {
            get
            {
                return 32;
            }
        }

        /// <summary>
        /// Gets the maximum string length of decimal values.
        /// </summary>
        public static int MaxDecimalLength
        {
            get
            {
                return 16;
            }
        }

        /// <summary>
        /// Gets the maximum string length of serial numbers.
        /// </summary>
        public static int MaxSerialNumberLength
        {
            get
            {
                return 15;
            }
        }

        // Rally TA5748 - Support play with paper.
        /// <summary>
        /// Gets the maximum string length of card start numbers.
        /// </summary>
        public static int MaxStartNumberLength
        {
            get
            {
                return 9;
            }
        }
        #endregion
    }

    /// <summary>
    /// Contains the static constants that represent the sizes of fixed-length 
    /// data on the server.
    /// </summary>
    public static class DataSizes
    {
        #region Static Properties
        /// <summary>
        /// Gets the size of player and staff password hashes.
        /// </summary>
        public static int PasswordHash
        {
            get
            {
                return 20;
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents a feature of a hardware device.
    /// </summary>
    public enum HardwareAttribute
    {
        MaxCards = 1,
        WiFiNetworking = 2
    }

    /// <summary>
    /// Represents the defined method of voiding a receipt from POS.
    /// </summary>
    public enum POSVoid
    {
        Allowed = 0,
        RequiresAuthorization = 1,
        NotAllowed = 2
    }

    /// <summary>
    /// Represets a type of system setting.
    /// </summary>
    public enum SettingsCategory
    {
        AllCategories = 0,
        GlobalSystemSettings = 1,
        POSSettings = 2,
        KioskSettings = 3,
        RemoteDisplaySettings = 4,
        CrateModuleSettings = 5, 
        LinkedBingoSettings = 6,
        BingoCallerSettings = 7,
        ReceiptMgmtSettings = 8,
        PlayerCenterSettings = 9,
        UnitMgmtSettings = 10,
        MoneyCenterSettings = 11,
        PlayerUnitFixedBaseSettings = 12,    //ATD
        Security = 14,
        ThirdPartyPlayerTrackingSettings = 15
    }

    public enum ThirdPartyVoidMode
    {
        NormalVoid = 0,
        AutoForce = 1,
        NeverVoid = 2
    }

    // Rally US1274 - Remove global settings that are license file only.
    /// <summary>
    /// Represents a setting in the system.
    /// </summary>
    public enum Setting
    {
        // Temp settings
        DoPatternSearchOldWay = -17,
        DoPointQualifyingAmountCalculationOldWay = -21,

        // Normal Settings
        MagneticCardFilters = 1,
        ShowMouseCursor = 2,
        POSReceiptPrinterName = 3,
        CashDrawerEjectCode = 4,
        CrateServerAddress = 5,
        TenderSales = 6,
        DatabaseServer = 7,
        DatabaseName = 8,
        DatabaseUser = 9,
        DatabasePassword = 10,
        ForceEnglish = 11,
        MaxCardLimit = 12, // FIX: TA4779
        VideoAdapterNumber = 13,
        UseHardwareAcceleration = 14,
        ScreenWidth = 15,
        ScreenHeight = 16,
        RefreshRate = 17,
        ColorDepth = 18,
        SwipeType = 19,
        IntervalPeriod=20,
        PointsPerSwipe = 21,
        SwipeEntersRaffle = 22,
        TitleMessage = 23,
        TextLine1 = 24,
        AllowManualWildCalls = 25, //RALLY TA 9169
        MinPoints =26,
        ActivityTimeout = 27,
        ShowPlayersWinningInfo=28,
        WinnerPollInterval =29,
        PreviousWinnerDisplayInterval=30,
        CurrencyRegion = 31,
        BillAcceptor = 32,
        CoinAcceptor = 33,
        PreviousWinnerDisplayTime=34,
        EnableLogging = 35,
        KioskReceiptPrinterName = 36,
        MinPlayersForRaffle = 37,
        RemoveWinnerFromNextRaffle = 38,
        CBBScannerPort = 39,
        CBBSheetDefinition = 40,
        //Operator Settings - POS
        FlashboardInterfacePort = 41, //Rally TA1714
        LatchBallCalls = 42,
        OpAllowNoSale = 43,
        OpAllowReturns = 44,
        PrintNonElectronicReceipts = 45,
        RFMode = 46,
        PromptForPlayerCreation = 47, // PDTS 1044
        CBBPlayItSheetType = 48, // Rally US505 - Create the ability to sell CBB cards
        DefaultReduceAtRegister = 49,//RALLY TA 9264       
        UseExchangeRateOnSale = 50, // Rally US1658
        EnableRegisterSalesReport = 51, // Rally US1650
        //START RALLY US1562
        PrintAccrualTransferReceipt = 52,
        PrintAccrualResetReceipt = 54,
        PrintAccrualReseedReceipt = 55,
        PrintAccrualIncreaseReceipt = 56,
        //END RALLY US1562
        OpVipCardRequired = 53,     
        OpPaperTrackingType = 57,
        //START RALLY US1572
        PrizeFeeAmount = 58,
        PrizeFeeAmountType = 59,
        //END RALLY US1572
        AllowAutoGameAdvance = 60, //RALLY TA 9089 allow auto game advance
        OpIssueBySerial = 61,
        AllowUnitCrossTransfers = 62,
        PrintAllPayoutWinners = 63,//RALLY US1571
        AllowNegativeInventory = 64,//RALLY US 1283 changed name to allow negative inventory
        OpRetireOnZeroInv = 65,
        RestartNumbersOnReset = 66,
		SilentPollingInterval = 67,
		BalanceSales = 68,
		ProgramPacketDelay = 69,
        DTRDelay = 70,
        CentralServerName = 71,
		TravStart = 72,
		TravEnd = 73,
		TrackStart = 74,
		TrackEnd = 75,
        PrintFacesToGlobalPrinter = 76,
        GlobalPrinterName = 77,
        CardFacePointSize = 78,
        PrizeFeeMinAmount =79,//RALLY US1572
        PrintPointInfo =80,
        PrintPlayerSignatureLine=81,
        PrintCBBCardsToPlayItSheet = 82, // Rally US505
        PurgeCreditsAtEOD=83,
        SellElectronics=84,
        PrintRegisterReceiptsNumber=85,
        DisclaimerLine1=86,
        DisclaimerLine2=87,
        DisclaimerLine3=88,
        FlashBoardInterfaceType=89,
        LoggingLevel=90,
        LogRecycleDays=91,
        MaxBetValue=92,
        ServerInstallDrive=93,
        ServerInstallRootDirectory=94,
        ClientInstallDrive=95,
        ClientInstallRootDirectory=96,
        RFSerialPort = 97,
        PrintWinners = 98,
        VIPRequiresPIN = 99,   
        ShowWinnersOnly = 100,
        // TTP 50114
        // Army Release
        DefaultSceneID = 101,
        ForcePlayerOnPayout = 102,//RALLY US1570
        MaxAssignableUnits = 103,
        AllowableSceneIDs = 104,
        ConfirmUnitAssignment = 105,
        EnableUnitAssignment = 106,
        MaxCreditWinAmount = 107,
        TaxFormMinWinAmount = 108,
		MaxPurgeInactivityPeriod = 109,		
        MinPurgeAmount = 110,
		DownloadRecycleDays = 111,
		PingTimeoutMilliseconds = 112,
        MinimumSaleAllowed = 113, // Rally US1854
		PrintStaffFirstNameOnly = 114,
        NumberOfPayoutReceipts = 115,
        WiFiOutOfRange = 116,
        ExportDataToCentral = 117,        // PDTS 571
	    AllowQuantitySale = 118,
	    PrintQuantitySaleReceipts = 119,
        MaxPlayerQuantitySale = 120,
        PrintProductNames = 121, // PDTS 964
        ShowPayoutAmount = 122,//RALLY US1897
        MagneticCardReaderMode = 123,        // PDTS 1064
        MagneticCardReaderParameters = 124,
        ClientBroadcastRate = 125,
        MSRReadTriggers = 126,
        MaxStaffMachines = 127,        // Rally DE147
        UnitAssignmentReceipt = 128,
        DefaultCardColor = 129,      
        DefaultCardBackgroundColor = 130,
        UnassignUnitsOnSessionChange = 131,
        NumberOfBankIssueReceipts = 132,        //START RALLY US1906
        NumberOfBankCloseReceipts = 133,
        NumberOfBankDropReceipts = 135,
        //END RALLY US1906
        PrintZeroAmountBankReciept = 136,//RALLY US1930
        PlayType = 134,
        MaxPayoutPerSession = 137,
        WinNotificationToCaller = 138,
        CBBAutoLock = 139, // Rally TA1941 (LockNGames)       
        PrintReconcileRecipt = 140,//RALLY US1926
        MinimumPasswordLength = 141,//RALLY US1940
        AutomaticUnlockTime = 142,//RALLY US1944
        ForcePackToPlayer = 143,     //RALLY TA6042
        AllowCardStatusOverride = 144,        //Rally 2560
        ShareOperatorCredits = 145,        //Multiple Operator
        AllowPlayTypeToggle = 146,        //Rally TA1726
        PlayDaubLocation = 147,
        PlayModeCatchUpEnabled = 148,
        PlayModePreDaubEnabled = 149,
        PlayModePreDaubErrorsEnabled = 150,
        PlayModeDaubOnImageEnabled = 151,
        PlayModeGreenDaubEnabled = 152,
        PlayAllSoundEnabled = 153,
        PlayModeOneAwaySound = 154,
        PlayWinningSoundEnabled = 155,
        PlayBallCallSoundEnabled = 156,
        PlayKeyClickEnabled = 157,
        PlayWinAnimationDuration = 158,
        KioskFont = 161,        //Rally TA1703
        KioskTitleColor = 162,
        KioskLineColor = 163,
        MaxVolume = 175,         // Rallys US510
        RandomRaffleParticipants = 176,
        UseVirtualFlashboardCamera = 177,
        ScreenSaverTimeout = 178,        //Rally TA1732
        BallCallCameraChannel = 179,
        ShareOperatorPoints = 180,
        AllowMultipleOperators = 181,
        RaffleDrawingSetting = 182,        //Rally TA4714
        MultipleReceiptVoiding = 183,        //Rally TA4712
        UseSameCards = 184,
        UseConsecutiveCards = 185,
        PinExpireDays = 186,
        PrintCardFaces = 187,
        PrintCardNumbers = 188,
        ReceiptHeaderLine1 = 189,
        ReceiptHeaderLine2 = 190,
        ReceiptHeaderLine3 = 191,
        ReceiptFooterLine1 = 192,
        ReceiptFooterLine2 = 193,
        ReceiptFooterLine3 = 194,
        MaxtvVolume = 195,// Rally TA5749 - Support play with paper.
        WiredNetworkLossThreshold = 196, //RALLY TA 9171
        WirelessNetworkLossThreshold = 197,//RALLY TA 9171
        PreviousPasswordNumber = 198,//RALLY US1941
        PasswordLockoutAttempts = 199,//   
        PasswordComplexitySetting = 200,//RALLY US1940
        // END: TA5749
        LockSessionVoids = 201, //RALLY DE 1937
        PrintBallCalls = 202, // RALLY TA 6877
        CoolDownTimer = 203, //RALLY TA 9092
        ScreenSaverEnabled = 206,  //RALLY TA 10508
        ScreenSaverWait = 207,  //RALLY TA 10508
        MasterBankUsePreviousClose = 208, //RALLY DE 6756
        BankUsePreviousClose = 209,//RALLY DE 6756
        PrintPointsRedeemed = 210, // US2139
        EnableOneTouchVerify = 211, //RALLY TA 8743
        PrintRegisterReportByPackage = 212, // US1808
        CrateServerPortNumber = 213, //RALLY TA 9123
        RaffleDisplayDuration = 214, //RALLY DE 6611
        UsePasswordKeypad = 215, // US2057
        EnablePeekMode = 216,//US1998
        UseManagedRouting = 217,
        QuickDrawExpireCount = 218,
        PrintWinnersAddress = 219, //US2723
        EnableCBBFavorites = 220, //US2418        
        EnableActiveSalesSession = 221, //US2828
        ThirdPartyTimeout = 222,
        ThirdPartyLocation = 223,
        ThirdPartyPointScaleNumerator = 224,
        ThirdPartyPointScaleDenominator = 225,
        ThirdPartyRedeemNumerator = 226,
        ThirdPartyRedeemDenominator = 227, //continued at 295
        EnableLockScreenButton = 228,
        EnableAutoModeButton = 229,
        VoidLockAtGameCount = 230, //US3298 Adding support for locking voids
        GameSoundsAdapterId = 231,
        TVSoundsAdapterId = 232,
        BallImageMinDisplayTime = 264,
        BlowerEnabled = 265,//US2797
        BlowerAddress = 266,//US2797
        ClearWinnersScreen = 268, //US4716
        TVWithoutPurchase = 269, //US3860
        RaffleRunFromLocation = 271, //US3914
        EnableCouponManagement = 273, // US1848 Adding support for comps (coupons)
        AreCouponsTaxable = 274, // US1848 Adding support for comps (coupons)
        DisplayGameWinnerCount = 276, //US3987
        ShowPlayerPictures = 277, //US3989
        DisplayVerifiedCard = 278, //US4010
        CheckProductCardCount = 279, //US4028
        CheckPaperForDuplicates = 280, //US4039
        PlayerPinLength = 281, //US4147
        EnableValidation = 283, //VALIDATION 
        ProductValidationCardCount = 284,
        DisplayFunGamesOnLogin = 285, //US4526
        MaxValidationPerTransaction = 286,
        RFTransmitterType =             288, // US4490 
        BlowerScanner1Port =            289, // RALLY US4468
        BlowerScanner2Port =            290,
        FlashboardNumericDisplayMode =  291, // RALLY US4487
        LogoutPacksOnSessionEnd =       292, // RALLY US4539
        AutoIssueBank =                 293, //US4434
        LEDFlashboardEnabled =          294, // RALLY US4487
        ThirdPartyPlayerInterfaceID = 295, //US4405
        ThirdPartyPlayerInterfaceTimeoutOption = 296, //US4405
        ThirdPartyPlayerInterfaceExternalRating = 297, //US4405
        ThirdPartyPlayerInterfacePINLength = 298, //US4405
        ThirdPartyPlayerInterfaceGetPINWhenCardSwiped = 299, //US4405
        ThirdPartyPlayerInterfaceNeedPINForPlayerInfo = 300, //US4405
        ThirdPartyPlayerInterfaceNeedPINForPoints = 301, //US4405
        ThirdPartyPlayerInterfaceNeedPINForRating = 302, //US4405
        ThirdPartyPlayerInterfaceNeedPINForRedemption = 303, //US4405
        ThirdPartyPlayerInterfaceNeedPINForRatingVoid = 304, //US4405
        ThirdPartyPlayerInterfaceNeedPINForRedemptionVoid = 305, //US4405      
        EnableFlexTendering = 233,
        AllowCreditCards = 234,
        AllowDebitCards = 235,
        AllowGiftCards = 236,
        AllowChecks = 237,
        AllowCash = 238,
        SellPreviouslySoldItemOption = 239,
        AllowSplitTendering = 240,
        PaymentProcessorCommunicationsTimeout = 241,
        PinPadDisplayColumnCount = 242,
        AllowManualCardEntry = 243,
        PinPadGreeting = 244,
        PinPadEnabled = 245,
        PinPadCardFailMessage = 246,
        MaximumTotalNotRequiringSignature = 247,
        PinPadAfterSaleMessage = 248,
        DisplayAmountDueOnPinPad = 249,
        DisplayItemDetailOnPinPad = 250,
        PinPadStationClosedMessage = 251,
        DisplaySubtotalOnPinPad = 252, 
        PrintCustomerAndHallReceiptsForNonCashSales = 253,
        PINPadDisplayLineCount = 254,
        PaymentProcessingEnabled = 255,
        Shift4AuthToken = 256,
        ProcessFundsTransferForChecks = 257,
        PaymentDeviceAddress = 258,
        PaymentDevicePort = 259,
        TransnetAddress = 260,
        TransnetPort = 261,
        PaymentProcessingAppAddress = 262,
        PaymentProcessingAppPort = 263,
        PlayerUnitRoverPackOnReboot = 287, // US5137
        AllowFunMultiplayerGames = 307, //US4611
        PatternShadingEnabled = 308, //US4538
        ThirdPartyPlayerInterfacePointsTransferedAsDollars = 310,
        ThirdPartyPlayerInterfacePointsTransferedAsDollarsForRedemptions = 311,
        ThirdPartyPlayerInterfacePointsTransferedAsDollarsForRatings = 312,
        ThirdPartyPlayerInterfacePlayerInfoHasPoints = 313,      
        DisplayNextBall = 315,
        PrintPayoutText = 316,
        PrintDenominationReceipt = 317,
        RFPacketDelay = 318, // US4753
        Use00ForCurrencyEntry = 319,
        AutoIgnoreUnusedBalls = 320,  //US4748
        BonusBallRange = 321,  // US4793
        NotifyUserIfPackInUseInsteadOfRipping = 322, //US4810
        UseLinearGameNumbering = 323, // US4804
        CreditCardProcessor = 324,
        PrintVoidSignatureLines = 325, //US4848
        POSDefaultElectronicUnit = 328, //US4884
        PayoutReceiptSignatureLineCount = 329, //US5107      
        PrintReceiptSortedByPackageType = 330, //US4884
        ThirdPartyPlayerSyncMode = 331,
        EnablePaperUsage = 332,
        ShowPaperUsageAtLogin = 333,
        QuickDrawElecPermFile = 334,
        CompactPaperPacksSold = 335,
        RepeatSaleRemoveCouponPackages = 336,
        RepeatSaleRemovePaper = 337,
        UseLongDescriptionsOnPOSScreen = 338,
        ReturnToPageOneAfterSale = 339,
        RepeatSaleRemoveDiscounts = 340,
        UseSystemMenuForDeviceSelection = 341,
        PrintOperatorInfoOnReceipt = 342,
        DisplayProgressiveOnPlayerUnit = 343, //US5123
        CbbScannerType = 344, //US4511
        EnableRegisterClosingReport = 345, //US5115: POS: Add Register Closing report button
        PrintRegisterClosingOnBankClose = 346, //US5108: POS > Bank Close: Print register closing report
        PlayerUnitsCacheSettings = 347, //US5139
        PlayerUnitRebootUpperThreshold = 348, //US5171
        ResetTedeRadioOnWifiInterruptions = 349,//US5175
        SessionSummaryBankUsePreviousClose = 350, //US4846
        AddInternalPlayerTrackingToPlayerClubPointsTable = 352,
        SessionSummaryBankActualCash = 353,//US5373
        PlayCooldownTimerSound = 354, //US5249
        InactiveAutoLogout = 355, // US5294
        ColorCircleMinDisplayTime = 356, //US5289
        EnableRNGBallCalls = 357, //US5326
        WidescreenPOSHasTwoMenuPagesPerPage = 358,
        AutomaticallyApplyCouponsToSalesOnSimpleKiosks = 359,
        AllowBarcodedPaperToBeSoldAtSimpleKiosk = 360,
        IncludeTheCouponsButtonOnTheHybridKiosk = 361,
        AllowUseOfSimpleKioskWithoutPlayerCard = 362,        
        KioskGuardianAddress = 363, //when this setting is TEST, the simple kiosks will have $1, $5, and $10 buttons for testing.
        KioskPeripheralsTicketPrinterName = 364,
        ScannableBallQty = 365, //US5335
        BlowerAckTimeout = 366,
        BlowerDropBallDuration = 367,
        BlowerBallMixEnabled = 368,
        BlowerBallMixDuration = 369,
        BlowerBallMixStartVoltPerc = 370,
        BlowerBallMixEndVoltPerc = 371,       
        SessionSummaryViewMode = 372, // US5345        
        BankCloseReceiptSignatureLineCount = 373, //DE13632
        AllowWidescreenPOS = 374,
        KioskIdleTimeout = 376,
        KioskShortIdleTimeout = 377,
        KioskMessageTimeout = 378,
        KioskClosedText = 379,
        KioskAttractText = 380,
        UseSimplePaymentFormForAdvancedKiosk = 383,
        AllowUseLastPurchaseButtonOnKiosk = 384,
        ShowQuantitiesOnMenuButtons = 385,
        PrintPlayerIdentityAsAccountNumber = 386,
        ScannedReceiptsStartNewSale = 387,
        AllowCreditCardsOnKiosks = 388,
        PrintIncompleteTransactionReceipt = 389,
        IncompleteTransactionReceiptText1 = 390,
        IncompleteTransactionReceiptText2 = 391,
        MonetaryRaffleDisplayDuration = 392, //US5414
        AllowScanningProductsOnSimplePOSKiosk = 393, //US5438
        ForceDeviceSelectionWhenNoFees = 394,
        ForceAuthorizationOnVoidsAtPOS = 395,
        ShowFreeOnDeviceButtonsWithFeeOfZero = 396,
        VoidingWithClosedSessionMode = 397,
        AllowB3SalesOnAPOSKiosk = 398,
        KiosksCanOnlySellFromTheirButtons = 399,

        //US5340
        ProtocolAdapterEnabled = 400,
        ProtocolAdapterStreamIdx = 401,
        ProtocolAdapterSendFreq = 402,
        ProtocolAdapterCommPort = 403,

        LEDAdapterEnabled = 404,
        LEDAdapterStreamIdx = 405,
        LEDAdapterSendFreq = 406,

        LEDBingoFlashOff = 407,
        LEDBingoFlashOn = 408,
        LEDBingoNormal = 409,
        LEDCountSign = 410,
        LEDCallColorOff = 411,
        LEDCallColor = 412,
        LEDCallFlashOn = 413,
        LEDCallFlashOff = 414,
        LEDHotball = 415,
        LEDHotballCall = 416,
        LEDHotballFlashOn = 417,
        LEDHotballFlashOff = 418,
        LEDGameSign = 419,
        LEDLastCallSign = 420,
        LEDPatternOff = 421,
        LEDPatternOn = 422,
        //US5340 end
        GetPlayerWithVerify = 423, // US5426

        ThirdPartyVoidMode = 424,
        RemoteRNG = 428,

        KioskChangeDispensingMethod = 427,

        AllowKiosksToPrintCBBPlayItSheetsFromReceiptScan = 429,
        UseKeyClickSoundsOnKiosk = 430,
        KioskVideoVolume = 431,
        KioskTimeoutPulseDefaultButton = 432,
        KioskCrashRecoveryNeedAttendantAfterNMinutes = 433, 

        DeviceFeesQualifyForPoints = 434,
        AutoDiscountInfoGoesOnBottomOfScreenReceipt = 435,
        AllowVoidingOfKioskSales = 436,
        FlashboardTheme = 437,

        PrintPlayerID = 439,
        ConfirmDamagedPaperForPaperExchange = 440,
        StabilizeCabinet = 441,

        DisplayBonusBallImage = 442,

        AllowSalesToBannedPlayers = 443,

        HeadcountMethod = 444,
        ShowBonanzaButtonOnPlayerUnit = 445,
        AutomatedPlayerPointExpire = 446,

        RefundTicketExpiration = 447, //US5715

        PrintWinnerReceiptForVerifiedPaper = 448,

        DeferExportsOfPlayerPointsFromSales = 449,

        EnablePromoTextAfterReceiptAtPOS = 451,
        PromoTextAfterReceiptAtPOSRequiresPlayer = 452,
        ForceMenuButtonAuthorization = 453,

    }   // (note: C# doesn't complain about the extra comma on the last item, so you don't have to change that line when adding a new item )

    /// <summary>
    /// Represents a setting value in the system.
    /// </summary>
    public struct SettingValue
    {
        public int Id;
        public int Category;
        public string Value;
    }

    /// <summary>
    /// Represents a license setting in the system.
    /// </summary>
    public enum LicenseSetting
    {
        MinValueId = 1000,
        CBBEnabled = 1001,
        PermVersion = 1002,
        EnableAnonymousMachineAccounts = 1003,
        MainStageMode = 1004,
        CreditEnabled = 1005,
        UsePrePrintedPacks = 1006,
        PlayWithPaper = 1007,
        AllowMelangeSpecialGames = 1008,
        Enable90NumberGames = 1009,
        EnableBingoRNG = 1010,
        QuickPickEnabled = 1011,
        PayoutsEnabled = 1012, // Rally US1569
        ForceWholeProductPoints = 1013, // US3692 Adding support for Whole product points
        InventoryCenterEnabled = 1014,
        EnableRaffle = 1015, //RALLY TA 8297 Setting for enabling raffle
        AccrualEnabled = 1016,
        StaticDropInMode = 1017, //RALLY TA 8743
        EnableB3Management = 1019, //US4380
        EnableNonSessionAccruals = 1021, // US2243
        EnableTXPayouts = 1022, //Enable the TX payouts in the Money
        EnableBankModifications = 1023, //US2981
        NDSalesMode = 1027, //US4120 All electronic sales require a PIN
        Presales = 1028,
        //US4230: Hide Slingo from license file settings
        //SlingoEnabled = 1018, // TA8849 - Add Slingo Support to Program Manager
        //SlingoFreeSpace = 1020,//US2166
    }

    /// <summary>
    /// Represents a license setting value in the system.
    /// </summary>
    public struct LicenseSettingValue
    {
        public int Id;
        public string Value;
    }
    // END: US1274

    /// <summary>
    /// Represents a module in the system.
    /// </summary>
    public enum EliteModule
    {
        POS = 1,
        UnitManagement = 2,
        PlayerCenter = 3,
        SystemSettings = 4,
        SecurityCenter = 5,
        ProductCenter = 6,
        ProgramCenter = 7,
        ReceiptManagement = 8,
        ReportCenter = 9,
        RemoteDisplay = 10,
        Caller = 11,
        Kiosk = 12,
        Win32BowlingForCash = 13,
		Win32SunkenTreasure = 14,
        PlayerLoyality = 15,
        RoccoPoker = 16,
        CELinkedBonusBingo = 17,
	    TravelerModule = 18,
	    TrackerModule = 19,
	    CEBowlingForCash = 20,
	    CrateModule = 21,
	    ProgramCalendar = 22,
	    CrateViewer = 23,
	    DragonsGate = 24,
	    CascadePoker = 25,
	    EliteMCP = 26,
	    PatLib = 27,
	    CEDuecesWildPoker = 28,
	    CEJacksOrBetterPoker = 29,
	    CEClassicKeno = 30,
	    Win32LinkedBonusBingo = 31,
	    EliteCommonFiles = 32,
	    CESunkenTreasure = 33,
	    Win32CrawfishCookin = 34,
	    CECrawfishCookin = 35,
	    PlayerTerminal = 36,
	    GRS = 37,
	    Reservations = 38,
	    Dining = 39,
        SportsBook = 40,
        Win32GPLaunch = 41,
        CEGPLaunch = 42,
        MoneyCenter = 43,
        Win32DuecesWildPoker = 44,
        Win32JacksOrBetterPoker = 45,
        Win32ClassicKeno = 46,
        GameManagement = 47,
        LinkedServerCenter = 48,
        DisputeResolution = 60,
        MainStageCenter = 90,
        B3Center = 247
    }
	
	//US2001 
    public enum ModuleFeature
    {
        CouponManagement = 39,
        ManualPointsAwardtoPlayer = 48,
        EditDailyMenu = 49, // US1772
        PurgePlayerPoints = 66,
        CardPositionMapManagement = 67,
        RemovedCouponToPlayer = 69,
    }
    
    /// <summary>
    /// Represents a type of bingo game.
    /// </summary>
    public enum GameType
    {
        SeventyFiveNumberBingo = 1,
        NinetyNumberBingo = 2,
        DoubleAction = 3,
        CrystalBall = 4, // Rally US505
        TwoOn = 5,
        ThreeOn = 6,
        FourOn = 7,
        SixOn = 8,
        NineOn = 9,
        TwelveOn = 10,
        FifteenOn = 11,
        EighteenOn = 12,
        // PDTS 1098
        EightyNumberBingo = 13,
        EightyNumberCash = 14,
        // Rally TA5664
        AllStar = 15,
        PotOfGold = 16,
        B13 = 17,
        BingoStorm = 18,
        PickYurPlatter = 19,
        Slingo = 20 // Rally TA8845
        // END: TA5664
    }

    /// <summary>
    /// Represents a bingo card media.
    /// </summary>
    public enum CardMedia
    {
        Electronic = 1,
        Paper = 2
    }

    /// <summary>
    /// Represents a type of bingo card.
    /// </summary>
    public enum CardType
    {
        Standard = 1,
        BonusLine = 2,
        Star = 3,
        DoubleFree = 4,
        NoFree = 5,
        DoubleAction = 6,
        QuickDraw = 7,
    }

    /// <summary>
    /// Enumerates the different kinds of Super Picks.
    /// </summary>
    public enum SuperPickType
    {
        Single = 7,
        Double = 8
    }

    /// <summary>
    /// Represents a type of transaction.
    /// </summary>
    public enum TransactionType
    {
        Sale = 1,
        Void = 2,
        Return = 3,
        AccrualVoid = 4,
        AccrualIncrease = 5,
        AccrualDecrease = 6,
        AccrualWin = 7,
        AccrualPrizeAdjustment = 8,
        PlayerTrackingPoints = 9,
        CreditCashout = 10,
        // Rally TA7465 
        InitialBankIssue = 11,
        CreditSale = 12,
        CreditGameWager = 13,
        UnitTransfer = 14,
        CreditGameWin = 15,
        CashOnlyPayout = 16,
        BankIssue = 17,
        CancelCardBuy = 18,
        PurgeCredit = 19,        //ATD
        CloseBank = 20,          // FIX: DE1930 - Allow closing of a drawer.
        InventoryMove = 21,
        ManualInventoryAdjustment = 22,
        Skip = 23,
        BonanzaTrade = 24,
        Issue = 25,
        Playback = 26,
        Damaged = 27,
        InventoryReceiving = 28,
        BankDrop = 29,
        InventoryRetire = 30, 
        InventoryTransfer = 32, 
        AccrualReset = 33,
        AccrualActivation = 34,
        AccrualDeactivation = 35,
        Payout = 36,
        ManualAccrualIncrease = 37,
        ManualAccrualReseed = 38,
        CashVoid = 39,
        PayoutVoid = 40,
        Refund = 46 //US5711
        // END: TA7465
    }

    // Rally DE452 - Register receipt does not show a quantity for JackpotStamp items.
    /// <summary>
    /// Represents a type of product item.
    /// </summary>
    public enum ProductType
    {
        CrystalBallQuickPick = 1,
        CrystalBallScan = 2,
        CrystalBallHandPick = 3,
        CrystalBallPrompt = 4,
        Electronic = 5, // Rally TA7626
        Concessions = 6, // FIX: DE6348
        Merchandise = 7,
        SuperPick = 8,
        SuperDoublePick = 9,
        CreditRefundableFixed = 10,
        CreditRefundableOpen = 11,
        CreditNonRefundableFixed = 12,
        CreditNonRefundableOpen = 13,
        BingoOther = 14,
        PNPBingo = 15,               //ATD
        // Rally TA7626
        Paper = 16,
        PullTab = 17,
        // END: TA7626
        Validation = 18,
        BonusValidation = 19,
    }

    /// <summary>
    /// Represents a type of discount.
    /// </summary>
    public enum DiscountType
    {
        Fixed = 1,
        Open = 2,
        Percent = 3
    }

    // Rally US419 - Display BINGO or LOTTO on all applicable elements on
    // screen.
    /// <summary>
    /// Enumerates whether a game is Bingo or Lotto.
    /// </summary>
    public enum BingoPlayType
    {
        Bingo = 1,
        Lotto = 2
    }

    // Rally US505
    /// <summary>
    /// The type of Crystal Ball Play-It sheet report.
    /// </summary>
    public enum CBBPlayItSheetType
    {
        Line = 1,
        Card = 2,
        // Rally TA8688 - Add support for thermal CBB play-it sheet.
        LineThermal = 3,
        CardThermal = 4,
        VerticalLineThermal = 5 // US2150
    }

    /// <summary>
    /// The type of Crystal Ball Play-It sheet report.
    /// </summary>
    public enum CBBPlayItSheetPrintMode
    {
        Off = 0,
        ElectronicOnly = 1,
        PaperOnly = 2,
        All = 3
    }

    // Rally TA5749
    /// <summary>
    /// Enumerates the different ways of printing card numbers.
    /// </summary>
    public enum PrintCardNumberMode
    {
        PrintGameCounts = 0,
        PrintCardNumbers = 1,
        PrintStartNumbers = 2,
        DoNotPrintOnReceipt = 3,
        OnlyPrintCBBCardNumbers = 4
    }
    // END: TA5749

    // Rally TA7465
    /// <summary>
    /// Enumerates the different ways customers can pay.
    /// </summary>
    public enum TenderType
    {
        Undefined = -1,
        Cash = 1,
        Check = 2,
        MoneyOrder = 3,
        CreditCard = 4,
        DebitCard = 5,
        Coupon = 6,
        GiftCard = 7,
        Chip = 8
    }

    /// <summary>
    /// Enumerates the different ways cash drawers are managed in the system.
    /// </summary>
    public enum CashMethod
    {
        ByStaffPOS = 1,
        ByMachinePOS = 2,
        ByStaffMoneyCenter = 3,
    }

    // Rally US1854
    /// <summary>
    /// Enumerates the different rules for allowing sales totalling 0 or below.
    /// </summary>
    public enum MinimumSaleAllowed
    {
        All = 0,
        ZeroOrGreater = 1,
        GreaterThanZero = 2
    }

    /// Rally US4490
    /// <summary>
    /// The different RF transmitter types that can be used
    /// </summary>
    public enum RFTransmitterType
    {
        [Description("GTI")]    // used for "friendly names"
        GTI = 0,                // Note this must be zero-based for combobox (CallerSettings.cs)
        [Description("FNet")]
        FNET = 1,
    }

    /// DE13418
    /// <summary>
    /// The different types of ball cameras allowed
    /// 
    /// Note: if the value is greater than zero, then it's a video channel
    /// </summary>
    public enum BallCameraType
    {
        [Description("S-Video")]    // used for "friendly names"
        SVideo = -1,
        [Description("Composite")]
        Composite = -2,
        [Description("USB")]
        USB = -3,
        [Description("Disable")]
        Disabled = -4,
    }

    /// US4487
    /// <summary>
    /// The different display modes the flashboard has for the number display
    /// </summary>
    public enum FlashboardDisplayMode
    {
        [Description("Game Number")]    // used for "friendly names"
        GameNumber = 0,                // Note this must be zero-based for combobox (CallerSettings.cs)
        [Description("Ball Count")]
        BallCount = 1,
    }
    public struct TenderTypeValue
    {
        public int TenderTypeID;
        public string TenderName;
        public byte IsActive;
    }

    /// US5345
    /// <summary>
    /// The different view modes for Session Summary
    /// </summary>
    public enum SessionSummaryViewModes
    {
        [Description("Default")]    // used for "friendly names"
        Default = 0,
        [Description("Nevada")] 
        Nevada = 1,
    }
}
