// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2009 GameTech
// International, Inc.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;

namespace GTI.Modules.Shared
{
    public class GetOperatorCompleteMessage : ServerMessage 
    {
        #region Member Variables

        private List<Operator> m_operatorList;
        private int m_OperatorParameters;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetOperatorDataMessage class.
        /// </summary>
        public GetOperatorCompleteMessage()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetOperatorDataMessage class
        /// with the specified operator id.
        /// </summary>
        /// <param name="operatorId">The id of the operator to get 
        /// data for.</param>
        public GetOperatorCompleteMessage(int operatorId)
        {
            m_id = 18053; // Get Operator Data
            m_OperatorParameters = operatorId;
            OperatorList = new List<Operator>();
        }

        public List<Operator> OperatorList
        {
            get { return m_operatorList; }
            set { m_operatorList = value; }
        }

        #endregion

        #region Member Methods
        /// <summary>
        /// Prepares the request to be sent to the server.
        /// </summary>
        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Operator Id
            requestWriter.Write(m_OperatorParameters);

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
                // Seek past return code .
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                Int16 operatorCount = responseReader.ReadInt16();

                for (int i = 0; i < operatorCount; i++)
                {
                    Operator newOperator = new Operator();
                    newOperator.Id = responseReader.ReadInt32();
                    newOperator.CashMethodID = responseReader.ReadInt32();
                    newOperator.CompanyID = responseReader.ReadInt32();
                    newOperator.IsActive = responseReader.ReadBoolean();
                    // Operator Name
                    ushort stringLen = responseReader.ReadUInt16();
                    newOperator.Name = new string(responseReader.ReadChars(stringLen));
                    // Phone
                    stringLen = responseReader.ReadUInt16();
                    newOperator.Phone = new string(responseReader.ReadChars(stringLen));

                    // Modem
                    stringLen = responseReader.ReadUInt16();
                    newOperator.Modem = new string(responseReader.ReadChars(stringLen));

                    // License 
                    stringLen = responseReader.ReadUInt16();
                    newOperator.Licence = new string(responseReader.ReadChars(stringLen));
                    
                    // Code
                    stringLen = responseReader.ReadUInt16();
                    newOperator.Code = new string(responseReader.ReadChars(stringLen));

                    // Contact Name
                    stringLen = responseReader.ReadUInt16();
                    newOperator.ContactName = new string(responseReader.ReadChars(stringLen));
                    
                    // Max Points per session
                    stringLen = responseReader.ReadUInt16();
                    string tempDec = new string(responseReader.ReadChars(stringLen));
                    if (tempDec != string.Empty)
                        newOperator.MaxPtsPerSession = decimal.Parse(tempDec, CultureInfo.InvariantCulture);

                    // Max Points per day
                    stringLen = responseReader.ReadUInt16();
                    tempDec = new string(responseReader.ReadChars(stringLen));
                    if (tempDec != string.Empty)
                        newOperator.MaxPointsPerDay = decimal.Parse(tempDec, CultureInfo.InvariantCulture);

                    newOperator.AddressID = responseReader.ReadInt32();

                    stringLen = responseReader.ReadUInt16();
                    newOperator.Address1 = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    newOperator.Address2 = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    newOperator.City = new string(responseReader.ReadChars(stringLen));
                    
                    stringLen = responseReader.ReadUInt16();
                    newOperator.State = new string(responseReader.ReadChars(stringLen));
                    
                    stringLen = responseReader.ReadUInt16();
                    newOperator.Zip = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    newOperator.Country = new string(responseReader.ReadChars(stringLen));

                    newOperator.BillingAddressId = responseReader.ReadInt32();

                    stringLen = responseReader.ReadUInt16();
                    newOperator.BillingAddress1 = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    newOperator.BillingAddress2 = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    newOperator.BillingCity = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    newOperator.BillingState = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    newOperator.BillingZip = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    newOperator.BillingCountry = new string(responseReader.ReadChars(stringLen));

                    //Hall Rent Amount
                    stringLen = responseReader.ReadUInt16();
                    tempDec = new string(responseReader.ReadChars(stringLen));
                    if (tempDec != string.Empty)
                        newOperator.HallRent = decimal.Parse(tempDec, CultureInfo.InvariantCulture);

                    //TaxPayerID
                    stringLen = responseReader.ReadUInt16();
                    newOperator.TaxPayerId = new string(responseReader.ReadChars(stringLen));

                    //StatePercent
                    stringLen = responseReader.ReadUInt16();
                    tempDec = new string(responseReader.ReadChars(stringLen));
                    if (tempDec != string.Empty)
                        newOperator.PercentPrizesToState = decimal.Parse(tempDec, CultureInfo.InvariantCulture);

                    //Profit Percent
                    stringLen = responseReader.ReadUInt16();
                    tempDec = new string(responseReader.ReadChars(stringLen));
                    if (tempDec != string.Empty)
                        newOperator.PercentOfProfitsToCharity = decimal.Parse(tempDec, CultureInfo.InvariantCulture);

                    newOperator.PlayerTierCalcId = responseReader.ReadInt32();
                    Int16 operatorFeeCount = responseReader.ReadInt16();
                    
                    for (int j = 0; j < operatorFeeCount;j++ )
                    {
                        OperatorFee operatorFee = new OperatorFee();
                        operatorFee.DeviceId = responseReader.ReadInt32();
                        stringLen = responseReader.ReadUInt16();
                        operatorFee.Fee = new string(responseReader.ReadChars(stringLen));
                        newOperator.OperatorFeeList.Add(operatorFee);

                        // US2018 //US2908-DE11626 
                        if (operatorFee.DeviceId == Device.Fixed.Id)
                            newOperator.FixedDeviceFee = decimal.Parse(operatorFee.Fee, CultureInfo.InvariantCulture);
                        else if (operatorFee.DeviceId == Device.Tracker.Id)
                            newOperator.TrackerDeviceFee = decimal.Parse(operatorFee.Fee, CultureInfo.InvariantCulture);
                        else if (operatorFee.DeviceId == Device.Traveler.Id)
                            newOperator.TravelerDeviceFee = decimal.Parse(operatorFee.Fee, CultureInfo.InvariantCulture);
                        else if (operatorFee.DeviceId == Device.Traveler2.Id)
                            newOperator.Traveler2DeviceFee = decimal.Parse(operatorFee.Fee, CultureInfo.InvariantCulture);// you re trying to parse ""                       
                        else if (operatorFee.DeviceId == Device.Explorer.Id)
                            newOperator.ExplorerDeviceFee = decimal.Parse(operatorFee.Fee, CultureInfo.InvariantCulture);
                        //TA12156
                        else if (operatorFee.DeviceId == Device.Tablet.Id)
                            newOperator.TabletDeviceFee = decimal.Parse(operatorFee.Fee, CultureInfo.InvariantCulture);

                       
                        
                    }
                    OperatorList.Add(newOperator);
                }
            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Operator Data", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Get Operator Data", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

    }
}
