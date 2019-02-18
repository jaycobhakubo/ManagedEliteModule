// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.IO;
using System.Text;
using System.Globalization;

namespace GTI.Modules.Shared
{
  public  class SetOperatorCompleteMessage : ServerMessage
    {
      private Operator mOperator;
      public int OperatorId;
      public SetOperatorCompleteMessage(Operator op)
      {
          m_id = 18054;
          mOperator = op;      
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

          // Operator Id
          
          requestWriter.Write(mOperator.Id);
          requestWriter.Write(mOperator.CashMethodID);
          requestWriter.Write(mOperator.AddressID);
          requestWriter.Write(mOperator.CompanyID);
          requestWriter.Write(mOperator.PlayerTierCalcId);

          
          requestWriter.Write((ushort)mOperator.Name.Length);
          if (mOperator.Name.Length > 0)
          {
              requestWriter.Write(mOperator.Name.ToCharArray());
          }
          requestWriter.Write((ushort)mOperator.Phone.Length);
          if (mOperator.Phone.Length > 0)
          {
              requestWriter.Write(mOperator.Phone.ToCharArray());
          }
          requestWriter.Write((ushort)mOperator.Modem.Length);
          if (mOperator.Modem.Length > 0)
          {
              requestWriter.Write(mOperator.Modem.ToCharArray());
          }
          requestWriter.Write((byte)(mOperator.IsActive ? 1 : 0));

          requestWriter.Write((ushort)mOperator.Licence.Length);
          if (mOperator.Licence.Length > 0)
          {
              requestWriter.Write(mOperator.Licence.ToCharArray());
          }
          requestWriter.Write((ushort)mOperator.Code.Length);
          if (mOperator.Code.Length > 0)
          {
              requestWriter.Write(mOperator.Code.ToCharArray());
          }
          requestWriter.Write((ushort)mOperator.ContactName.Length);
          if (mOperator.ContactName.Length > 0)
          {
              requestWriter.Write(mOperator.ContactName.ToCharArray());
          }
          requestWriter.Write((ushort)mOperator.MaxPtsPerSession.ToString().Length);
          if (mOperator.MaxPtsPerSession.ToString().Length > 0)
          {
              requestWriter.Write(mOperator.MaxPtsPerSession.ToString().ToCharArray());
          }
          requestWriter.Write((ushort)mOperator.MaxPointsPerDay.ToString().Length);
          if (mOperator.MaxPointsPerDay.ToString().Length > 0)
          {
              requestWriter.Write(mOperator.MaxPointsPerDay.ToString().ToCharArray());
          }
          
          //TaxPayerID
          requestWriter.Write((ushort)mOperator.TaxPayerId.ToString().Length);
          if(mOperator.TaxPayerId.ToString().Length > 0)
          {
              requestWriter.Write(mOperator.TaxPayerId.ToString().ToCharArray());
          }

          //BillingAddressID
          requestWriter.Write(mOperator.BillingAddressId);

          //Hall Rent 
          requestWriter.Write((ushort)mOperator.HallRent.ToString().Length);
          if (mOperator.MaxPointsPerDay.ToString().Length > 0)
          {
              requestWriter.Write(mOperator.HallRent.ToString().ToCharArray());
          }

          //Percent Profit to Charity
          requestWriter.Write((ushort)mOperator.PercentOfProfitsToCharity.ToString().Length);
          if (mOperator.MaxPointsPerDay.ToString().Length > 0)
          {
              requestWriter.Write(mOperator.PercentOfProfitsToCharity.ToString().ToCharArray());
          }

          //Percent Profit to State
          requestWriter.Write((ushort)mOperator.PercentPrizesToState.ToString().Length);
          if (mOperator.MaxPointsPerDay.ToString().Length > 0)
          {
              requestWriter.Write(mOperator.PercentPrizesToState.ToString().ToCharArray());
          }


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
          
          // Seek past return code .
          responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);
          int returnOperatorID = responseReader.ReadInt32();

          if ((mOperator.Id == 0 && returnOperatorID < 1) || (mOperator.Id > 0 && returnOperatorID != mOperator.Id))
          {
              throw new ServerException("Returned wrong operator id");
          }

          else
          {
              mOperator.Id = returnOperatorID;
              OperatorId = mOperator.Id;
          }
      
          // Close the streams.
          responseReader.Close();
      }
      #endregion

    }
}
