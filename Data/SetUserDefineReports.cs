using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTI.Modules.Shared.Business;
using System.IO;

namespace GTI.Modules.Shared.Data
{
    public class SetUserDefineReports : ServerMessage
    {
        private List<ReportData> mListOfListReportsEnableDisable;

        public SetUserDefineReports(List<ReportData> ListOfListReportsEnableDisable)
        {
            m_id = 18260;
            mListOfListReportsEnableDisable = ListOfListReportsEnableDisable;
        }

     
        protected override void PackRequest()
        {
            MemoryStream requestStream = new MemoryStream();            // Create the streams we will be writing to.
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            requestWriter.Write((ushort)mListOfListReportsEnableDisable.Count());

            foreach (ReportData rptInfo in mListOfListReportsEnableDisable)  //settings count
            {
                requestWriter.Write((int)rptInfo.ReportId);

                byte tempIsEnable;
                if (rptInfo.IsActive == true)
                {
                    tempIsEnable = (byte)1;
                }
                else
                {
                    tempIsEnable = (byte)0;
                }

                requestWriter.Write(tempIsEnable);
                requestWriter.Write((ushort)rptInfo.ReportDisplayName.Length);
                requestWriter.Write(rptInfo.ReportDisplayName.ToCharArray());
                //requestWriter.Write((ushort)rptInfo.ReportFileName.Length);
                //requestWriter.Write(rptInfo.ReportFileName.ToCharArray());

            }

            m_requestPayload = requestStream.ToArray();
            requestWriter.Close();            // Close the streams.bn
        }
    }
}
