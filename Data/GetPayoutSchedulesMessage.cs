using System;
using System.IO;
using System.Text;
using GTI.Modules.Shared;
using System.Collections.Generic;


namespace GTI.Modules.Shared
{
    public class GetPayoutSchedulesMessage : ServerMessage
    {
        private const int MinResponseMessageLength = 6;

        // Properties
        public List<PayoutSchedule> PayoutScheduleList{get;set;}
        public int ScheduleId { get; set; }

        /// <summary>
        /// Constructor for get payout schedules
        /// </summary>
        /// <param name="scheduleId">the specific schedule id or 0 for all</param>
        public GetPayoutSchedulesMessage(int scheduleId)
		{
            PayoutScheduleList = new List<PayoutSchedule>();
            ScheduleId = scheduleId;

		    m_id = 18043; // Message ID
			m_strMessageName = "Get Payout Schedules";
		}

		protected override void PackRequest()
        {
            // Create the streams we will be writing to
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Request Params get the specific schedule it (0 for all)
            requestWriter.Write(ScheduleId);

            // Set the bytes to be sent.
            m_requestPayload = requestStream.ToArray();

            // Close the streams.
            requestWriter.Close();
        }

        protected override void UnpackResponse()
        {
            base.UnpackResponse();

            // Create the streams we will be reading from.
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Check the response length.
            if (responseStream.Length < MinResponseMessageLength)
                throw new MessageWrongSizeException(m_strMessageName);

            // Try to unpack the data.
            try
            {
                // Seek past return code
                responseReader.BaseStream.Seek( sizeof(Int32), SeekOrigin.Begin);

				// Get the schedule count
				Int16 wCount = responseReader.ReadInt16();

                PayoutScheduleList  = new List<PayoutSchedule>();

				// Read the schedules
                for (int i = 0; i < wCount; i++)
				{
                    PayoutSchedule payoutSchedule = new PayoutSchedule();

                    //ID
                    payoutSchedule.Id = responseReader.ReadInt32();
                    
                    //Name
                    ushort wStringLen = responseReader.ReadUInt16();
					payoutSchedule.Name = new string(responseReader.ReadChars(wStringLen));
                    
                    //Is Active
                    payoutSchedule.IsActive = responseReader.ReadBoolean();

                    PayoutScheduleList.Add(payoutSchedule);
				}
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException(m_strMessageName, e);
            }
            catch (Exception e)
            {
                throw new ServerException(m_strMessageName, e);
            }

            // Close the streams.
            responseReader.Close();
        }

    }
}
