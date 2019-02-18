#region Copyright
// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2016 FortuNet, Inc.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTI.Modules.Shared.Data
{
    public enum ClientDataStoreTypes
    {
        Shift4AccessToken = 1,
        Shift4TransInvoiceNumber = 2
    }

    /// <summary>
    /// Basic testing functionality provider for the ClientDataStoreAccessor.
    /// </summary>
    public interface IClientDataStoreAccessor
    {
        void SetValue(ClientDataStoreTypes clientDatastoreType
            , int? operatorId
            , int? machineId
            , int? staffId
            , string value);

        string GetValue(ClientDataStoreTypes clientDatastoreType
            , int? operatorId
            , int? machineId
            , int? staffId);
    }

    /// <summary>
    /// Accessor used to get/set values from the client datastore.
    /// </summary>
    public class ClientDataStoreAccessor : IClientDataStoreAccessor
    {
        /// <summary>
        /// Sets a value into the client datastore
        /// </summary>
        /// <param name="clientDatastoreType"></param>
        /// <param name="operatorId"></param>
        /// <param name="machineId"></param>
        /// <param name="staffId"></param>
        /// <param name="value"></param>
        public void SetValue(ClientDataStoreTypes clientDatastoreType
            , int? operatorId
            , int? machineId
            , int? staffId
            , string value)
        {
            var sendMsg = new SetClientDataStoreMessage(clientDatastoreType
                , operatorId
                , machineId
                , staffId
                , value);

            sendMsg.Send();
        }

        /// <summary>
        /// Retrieves a value back from the client datastore
        /// </summary>
        /// <param name="clientDatastoreType"></param>
        /// <param name="operatorId"></param>
        /// <param name="machineId"></param>
        /// <param name="staffId"></param>
        /// <returns></returns>
        public string GetValue(ClientDataStoreTypes clientDatastoreType
            , int? operatorId
            , int? machineId
            , int? staffId)
        {
            var getMsg = new GetClientDataStoreMessage(clientDatastoreType
                , operatorId
                , machineId
                , staffId);

            getMsg.Send();

            return getMsg.ResponseData;
        }
    }
}
