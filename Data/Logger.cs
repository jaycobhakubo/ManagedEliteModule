// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.Runtime.InteropServices;

namespace GTI.Modules.Shared
{
    public enum LoggerLevel
    {
        Severe          = 7,
        Warning         = 6, 
        Information     = 5,
        Configuration   = 4,
        Debug           = 3,
        Message         = 2,
        SQL             = 1,
        All             = 0
    }

    public class Logger
    {
        public const string StandardPrefix = "Elite";
        private const string sLoggerPathName = "Logger.dll"; 
        private Logger() {} // No instance allowed.

        // Rally US1596
        [DllImport(sLoggerPathName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void StartLogger(string sLoggerName);

        [DllImport(sLoggerPathName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void EnableWindowLog(int iLevel);

        [DllImport(sLoggerPathName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void EnableFileLog(int iLevel, long lRecycleDays);

        [DllImport(sLoggerPathName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void EnableConsoleLog(int iLevel);

        [DllImport(sLoggerPathName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void EnableDebugLog(int iLevel);

        [DllImport(sLoggerPathName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void EnableDatabaseLog(int iLevel, string sServerName, string sDatabaseName, string sUserName, string sPassword);

        [DllImport(sLoggerPathName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void EnableSocketLog(int iLevel, string sSocketIPAddress, int iSocketPort);

        [DllImport(sLoggerPathName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void EnableUdpLog(int iLevel, string sUdpIPAddress, int iUdpPort);

        [DllImport(sLoggerPathName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void EnableEventLog(int iLevel);

        [DllImport(sLoggerPathName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void LogSevere(string sMessage, string sFileName, int iLineNumber);

        [DllImport(sLoggerPathName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void LogWarning(string sMessage, string sFileName, int iLineNumber);

        [DllImport(sLoggerPathName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void LogInfo(string sMessage, string sFileName, int iLineNumber);

        [DllImport(sLoggerPathName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void LogConfig(string sMessage, string sFileName, int iLineNumber);

        [DllImport(sLoggerPathName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void LogDebug(string sMessage, string sFileName, int iLineNumber);

        [DllImport(sLoggerPathName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void LogMessage(string sMessage, string sFileName, int iLineNumber);

        [DllImport(sLoggerPathName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void LogSql(string sMessage, string sFileName, int iLineNumber);
        // END: US1596
    }
}
