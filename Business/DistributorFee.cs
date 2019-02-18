using System;
using System.Collections.Generic;
using System.Text;

namespace GTI.Modules.Shared.Business
{
    public class DistributorFeeDataItem
    {
        private int m_distributorFeeID = 0;
        private decimal m_distributorFee = 0M;
        private int m_minRange = 0;
        private int m_maxRange = 0;
        private int m_feeTypeId = 0; // US3339

        public int DistributorFeeId
        {
            get { return m_distributorFeeID; }
            set { m_distributorFeeID = value; }
        }

        public decimal DistributorFee
        {
            get { return m_distributorFee; }
            set { m_distributorFee = value; }
        }

        public int MinRange
        {
            get { return m_minRange; }
            set { m_minRange = value; }
        }

        public int MaxRange
        {
            get { return m_maxRange; }
            set { m_maxRange = value; }
        }

        // US3339
        // fees that are not tied to a device
        //  need to be tracked at the detail 
        //  level
        public int FeeType
        {
            get { return m_feeTypeId; }
            set { m_feeTypeId = value; }
        }
             
    }

    public class DistributorFee
    {
        
        private int m_deviceId = 0;
        private int m_operatorId = 0;
        private int m_deviceFeeTypeId = 0;
        private List<DistributorFeeDataItem> m_distributorFeeData;

        public DistributorFee()
        {
            m_distributorFeeData = new List<DistributorFeeDataItem>();
        }
     
        public int DeviceId
        {
            get { return m_deviceId; }
            set { m_deviceId = value; }
        }

        public int OperatorId
        {
            get { return m_operatorId; }
            set { m_operatorId = value; }
        }

        public int DeviceFeeTypeId
        {
            get { return m_deviceFeeTypeId; }
            set { m_deviceFeeTypeId = value; }
        }

        public List<DistributorFeeDataItem> DistributorFeeData
        {
            get { return m_distributorFeeData; }
            set { m_distributorFeeData = value; }
        }
    }
}
