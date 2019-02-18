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
   public class SetOperatorDeviceFeesMessage : ServerMessage 
    {
       private Device[] mDevices;
       private int mOperatorID;
       public SetOperatorDeviceFeesMessage (int operatorID, Device [] devices)
       {
            m_id = 18086;
            mOperatorID = operatorID;
            mDevices = new Device[devices.Length];
            devices.CopyTo(mDevices, 0);
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

           // Player Id
           requestWriter.Write(mOperatorID);
           requestWriter.Write((ushort)mDevices.Length);
           for (int iDevice = 0; iDevice < mDevices.Length; iDevice++)
           { 
                requestWriter.Write (mDevices [iDevice].Id);
                requestWriter.Write((ushort)mDevices[iDevice].Fee.ToString().Length);
                requestWriter.Write(mDevices[iDevice].Fee.ToString().ToCharArray());
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
       }
       #endregion
    }
}
