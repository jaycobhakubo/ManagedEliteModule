// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2009 GameTech
// International, Inc.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using GTI.Modules.Shared.Business;

namespace GTI.Modules.Shared.Data
{
    public class GetDistributorFeesMessage : ServerMessage
    {
        private List<DistributorFee> m_distributorFeeList;
        private int m_operatorID;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operatorId">a value of 0 returns all the operator device fees</param>
        public GetDistributorFeesMessage(int operatorId)
        {
            m_operatorID = operatorId;
            m_id = 25017;
            DistributorFeeList = new List<DistributorFee>();
        }

        public List<DistributorFee> DistributorFeeList
        {
            get { return m_distributorFeeList; }
            set { m_distributorFeeList = value; }
        }

        /// <summary>
        /// Prepares the request to be sent to the server.
        /// </summary>
        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            //write the operator ID
            requestWriter.Write(m_operatorID);

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

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                //Get the operatorID
                int operatorID = responseReader.ReadInt32();

                //Get the number of distributor Fees
                short feeCount = responseReader.ReadInt16();

                for(int i=0;i<feeCount;i++)
                {
                    DistributorFee fee = new DistributorFee();
                    
                    //OperatorID
                    fee.OperatorId = operatorID;
                    
                    //Device Fee Type ID
                    fee.DeviceFeeTypeId = responseReader.ReadInt32();

                    //DeviceID
                    fee.DeviceId = responseReader.ReadInt32();

                    short feeDataCount = responseReader.ReadInt16();

                    for(int j=0; j<feeDataCount; j++)
                    {
                        DistributorFeeDataItem item = new DistributorFeeDataItem();

                        //DistributorFeeID
                        item.DistributorFeeId = responseReader.ReadInt32();
                      
                        //Fee
                        short stringLen = responseReader.ReadInt16();

                        string tempDec = new string(responseReader.ReadChars(stringLen));
                        
                        if (tempDec != string.Empty)
                            item.DistributorFee = decimal.Parse(tempDec, CultureInfo.InvariantCulture);

                        //Minimum Range
                        item.MinRange = responseReader.ReadInt32();

                        //Maximum Range
                        item.MaxRange = responseReader.ReadInt32();

                        // Type of fee
                        item.FeeType = responseReader.ReadInt32();

                        fee.DistributorFeeData.Add(item);
                    }
                     m_distributorFeeList.Add(fee);
                }
               
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Distributor Fees", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Get Distributor Fees", e);
            }

            // Close the streams.
            responseReader.Close();
        }
    }
}
