using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GTI.Modules.Shared
{
    public class GetSessionCharityDataMessage : ServerMessage
    {
        #region Member Variables
//        protected Dictionary<int, Charity> charities = new Dictionary<int, Charity>();
        private SessionCharity[] charities = new SessionCharity[0];
        protected int receiptId;
        #endregion

        #region Constructors
        public GetSessionCharityDataMessage()
            : this(0)
        {
        }

        public GetSessionCharityDataMessage(int receiptId)
        {
            m_id = 18200;
            this.receiptId = receiptId;
        }
        #endregion

        #region Member Methods
        public static List<SessionCharity> GetList(int receipt)
        {
            GetSessionCharityDataMessage msg = new GetSessionCharityDataMessage(receipt);
            try
            {
                msg.Send();
            }
            catch (Exception)
            {
                return null;
            }

            List<SessionCharity> charities = new List<SessionCharity>();
            int count = msg.charities.Length;
            for (int n = 0; n < count; ++n)
            {
                charities.Add(msg.charities[n]);
            }

            return (charities);
        }

        //public static Dictionary<int, Charity> GetCharities()
        //{
        //    GetSessionCharityDataMessage msg = new GetSessionCharityDataMessage();
        //    try
        //    {
        //        msg.Send();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageForm.Show(ex.Message);
        //        return null;
        //    }

        //    return msg.Charities;
        //}

        protected override void PackRequest()
        {
            // Create the streams that will be written to
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Receipt Id
            requestWriter.Write((int)receiptId);

            // Set the bytes to be sent
            m_requestPayload = requestStream.ToArray();

            // close the streams
            requestWriter.Close();
        }

        protected override void UnpackResponse()
        {
//            base.UnpackResponse();

            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                ushort count = responseReader.ReadUInt16();
                charities = new SessionCharity[count];
                for (int n = 0; n < count; ++n)
                {
                    SessionCharity tmp = new SessionCharity();

                    tmp.Session = responseReader.ReadInt32();

                    ushort stringLen = responseReader.ReadUInt16();
                    if (stringLen > 0)
                        tmp.Name = new String(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    if (stringLen > 0)
                        tmp.LicenseNumber = new String(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    if (stringLen > 0)
                        tmp.TaxPayerId = new String(responseReader.ReadChars(stringLen));

                    charities.SetValue(tmp, n);
                }
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Session Charity Data", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Get Session Charity Data", e);
            }

            // Close the streams.
            responseReader.Close();
        }

        //protected override void UnpackResponse()
        //{
        //    base.UnpackResponse();

        //    MemoryStream responseStream = new MemoryStream(m_responsePayload);
        //    BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

        //    // Try to unpack the data.
        //    try
        //    {
        //        // Seek past return code.
        //        responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

        //        ushort count = responseReader.ReadUInt16();
        //        for (int n = 0; n < count; ++n)
        //        {
        //            int session = 0;
        //            Charity tmp = new Charity();

        //            session = responseReader.ReadInt32();

        //            ushort stringLen = responseReader.ReadUInt16();
        //            if (stringLen > 0)
        //                tmp.Name = new String(responseReader.ReadChars(stringLen));

        //            stringLen = responseReader.ReadUInt16();
        //            if (stringLen > 0)
        //                tmp.License = new String(responseReader.ReadChars(stringLen));

        //            stringLen = responseReader.ReadUInt16();
        //            if (stringLen > 0)
        //                tmp.TaxId = new String(responseReader.ReadChars(stringLen));

        //            if (!charities.ContainsKey(session))
        //            {
        //                charities.Add(session, tmp);
        //            }
        //        }
        //    }
        //    catch (EndOfStreamException e)
        //    {
        //        throw new MessageWrongSizeException("Get Session Charity Data", e);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new ServerException("Get Session Charity Data", e);
        //    }

        //    // Close the streams.
        //    responseReader.Close();

        //}
        #endregion

        #region Member Properties
        //public Dictionary<int, Charity> Charities
        //{
        //    get { return charities; }
        //}

        public SessionCharity[] Charities
        {
            get { return charities; }
        }
        #endregion
    }
}
