using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTI.Modules.Shared.Business;
using System.IO;

namespace GTI.Modules.Shared.Data
{
   public class GetAllReports : ServerMessage
    {

public  List<ReportData> mListRptData {get; set;}


        public GetAllReports()
        {
            m_id = 18253;
            mListRptData = new List<ReportData>();
        }

        public GetAllReports(int ReportTypeId):this()
        {
            //m_id = 18253;
            //mListRptData = new List<ReportData>();
        }


        protected override void PackRequest()
        {
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);
            requestWriter.Write((int)0);
            m_requestPayload = requestStream.ToArray();
            requestWriter.Close();
        }


        protected override void UnpackResponse()
        {
            base.UnpackResponse();
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);
            responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);        
            ushort Count = responseReader.ReadUInt16();
            string tempName = string.Empty;
            ushort tempLength = 0;

            for (int iType = 0; iType < Count; iType++)
            {
                var tempValue = new ReportData();
                tempValue.ReportId = responseReader.ReadInt32();//1
                tempValue.ReportTypeId = responseReader.ReadInt32();//5
                byte tIsActive = 0;
                tIsActive = responseReader.ReadByte();//2
                tempValue.IsActive = tIsActive == 0 ? false : true;
                tempLength = responseReader.ReadUInt16();//3
                tempValue.ReportDisplayName = new string(responseReader.ReadChars(tempLength));
                tempLength = responseReader.ReadUInt16();//4
                tempValue.ReportFileName = new string(responseReader.ReadChars(tempLength));
                tempValue.ReportType = Enum.GetName(typeof(ReportTypes), tempValue.ReportTypeId);

                mListRptData.Add(tempValue);    
            }

            // Close the streams.
            responseReader.Close();
        }


    }
}
