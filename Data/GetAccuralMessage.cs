#region copyright
// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2011 GameTech
// International, Inc.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GTI.Modules.Shared.Business;

namespace GTI.Modules.Shared
{
    
    /// <summary>
    /// Class to implement the get accrual message
    /// </summary>
    public class GetAccuralMessage : ServerMessage
    {
        public List<Accrual> AccuralList { get; set; }
        private int m_accuralID;
        private bool m_includeLocked;
        private bool m_includeInactive;
        protected const int MinResponseMessageLength = 6;

        /// <summary>
        /// Constructor for the accrual message class
        /// </summary>
        /// <param name="accuralID">the accrual id to get</param>
        /// <param name="includeLocked">includes locked accruals</param>
        /// <param name="includeInactive">includes inactive accruals</param>
        public GetAccuralMessage(int accuralID, bool includeLocked, bool includeInactive)
        {
            m_id = 21001;
            m_accuralID = accuralID;
            m_includeLocked = includeLocked;
            m_includeInactive = includeInactive;
            AccuralList = new List<Accrual>();
        }

        /// <summary>
        /// Packs the request
        /// </summary>
        protected override void PackRequest()
        {
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            requestWriter.Write(m_accuralID);
            requestWriter.Write(m_includeLocked);
            requestWriter.Write(m_includeInactive);

            m_requestPayload = requestStream.ToArray();
            requestWriter.Close();
        }

        /// <summary>
        /// Gets the server response
        /// </summary>
        protected override void UnpackResponse()
        {
            base.UnpackResponse();
            ushort stringLen = 0;
          

            // Create the streams we will be reading from.
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Check the response length.
            if (responseStream.Length < MinResponseMessageLength)
                throw new MessageWrongSizeException("Get Progressive Message");

            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                ushort accuralCount = responseReader.ReadUInt16();
                
                AccuralList = new List<Accrual>();

                for (ushort i = 0; i < accuralCount; i++)
                {
                    Accrual accrual = new Accrual();
                    accrual.IsChecked = false;
                    accrual.Id = responseReader.ReadInt32();

                    stringLen = responseReader.ReadUInt16();
                    accrual.Name = new string(responseReader.ReadChars(stringLen));

                    //Accrual type id
                    accrual.Type = responseReader.ReadInt32();

                    //is locked
                    responseReader.ReadBoolean();

                    //is active
                    responseReader.ReadBoolean();

                    //increase type
                    accrual.IncreaseType = responseReader.ReadByte();

                    //increase amount
                    stringLen = responseReader.ReadUInt16();
                    accrual.IncreaseAmount = new string( responseReader.ReadChars(stringLen));

                    //increase withholding
                    stringLen = responseReader.ReadUInt16();
                    accrual.WitholdingAmount = new string( responseReader.ReadChars(stringLen));

                    //Is rounding up
                    responseReader.ReadByte();

                    //rounding denomination id
                    responseReader.ReadInt32();               

                    //is auto reseed
                    responseReader.ReadByte();

                    //auto reseed on transfer
                    responseReader.ReadByte();

                    //reseed type id
                    responseReader.ReadInt32();

                    //reseed amount percent
                    stringLen = responseReader.ReadUInt16();
                    responseReader.ReadChars(stringLen);

                    //reseed source id
                    responseReader.ReadInt32();

                    //reseed required
                    responseReader.ReadByte();

                    //All Programs
                    accrual.AllPrograms = responseReader.ReadBoolean();

                    //Applies to Sessionless Sales
                    accrual.AppliesToSessionlessSales = responseReader.ReadBoolean();

                    accrual.AccrualAccounts = new List<AccrualAccount>();
                    //accounts count
                    ushort accountsCount = responseReader.ReadUInt16();
                    //accounts list
                    for (ushort j = 0; j < accountsCount; j++)
                    {
                        AccrualAccount accrualAccount = new AccrualAccount();

                        //sequence
                        responseReader.ReadInt32();

                        //account id
                        accrualAccount.Id = responseReader.ReadInt32();
                        
                        //account name
                        stringLen = responseReader.ReadUInt16();
                        accrualAccount.Name = new string(responseReader.ReadChars(stringLen));
                        
                        //percent
                        stringLen = responseReader.ReadUInt16();
                        accrualAccount.Percentage= new string(responseReader.ReadChars(stringLen));

                        //balance
                        stringLen = responseReader.ReadUInt16();
                        responseReader.ReadChars(stringLen);
                        
                        //maxbalance
                        stringLen = responseReader.ReadUInt16();
                        responseReader.ReadChars(stringLen);
                        
                        //minbalance
                        stringLen = responseReader.ReadUInt16();
                        responseReader.ReadChars(stringLen);

                        accrual.AccrualAccounts.Add(accrualAccount);
                    }
                    
                    // Associated Programs Count
                    ushort progCount = responseReader.ReadUInt16();

                    // Associated Programs
                    for(int y = 0; y < progCount; y++)
                    {
                        var p = new SchedProgram();
                        p.Id = responseReader.ReadInt32();
                        stringLen = responseReader.ReadUInt16();
                        p.Name = new string(responseReader.ReadChars(stringLen));
                        p.Type = (ProgramType)Enum.Parse(typeof(ProgramType), responseReader.ReadInt32().ToString());
                        p.IsActive = responseReader.ReadBoolean();
                        accrual.Programs.Add(p);
                    }

                    AccuralList.Add(accrual);
                }

            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Progressive List", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Get Progressive List", e);
            }

            // Close the streams.
            responseReader.Close();
        }

        /// <summary>
        /// Method to get the accrual list with default parameters
        /// </summary>
        /// <returns>a list of accruals</returns>
        public static List<Accrual> GetAccuralList(bool AllowAccruals)
        {
            if (AllowAccruals)
            {
                GetAccuralMessage msg = new GetAccuralMessage(0, false, false);
                try
                {
                    msg.Send();
                }
                catch (ServerCommException ex)
                {
                    throw new Exception("Get Progressive Message: " + ex.Message);
                }
                return msg.AccuralList;
            }
            else
            {
                return new List<Accrual>();
            }
        }
    
    }
}
