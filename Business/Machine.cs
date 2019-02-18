using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using GTI.Modules.Shared.Properties;

namespace GTI.Modules.Shared.Business
{
    public class Machine
    {
        #region Member Variables
        protected int m_id;
        protected Device m_deviceType;
        protected int m_locationId;
        protected string m_clientId;
        protected string m_description;
        protected string m_serialNum;
        protected short m_unitNum;
        protected Player m_assignedPlayer;
        protected List<int> m_packNums = new List<int>(); // Rally DE2245 - Need to display the pack # of a logged in machine.
        protected bool m_isEnabled = true;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Machine class.
        /// </summary>
        public Machine()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Machine class.
        /// </summary>
        /// <param name="machineData">The SMachineData to use to create the 
        /// machine.</param>
        public Machine(Machine machineData)
        {
            m_id = machineData.Id;
            m_deviceType = machineData.DeviceType;
            m_locationId = machineData.LocationId;
            m_clientId = machineData.ClientIdentifier;
            m_description = machineData.Description;
            m_isEnabled = machineData.IsEnabled;
            m_unitNum = machineData.UnitNumber;
            // Rally US247
            m_assignedPlayer = machineData.AssignedPlayer;
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Returns a string that represents the current Machine.
        /// </summary>
        /// <returns>A string that represents the current Machine.</returns>
        public override string ToString()
        {
            string name = null;

            if (m_unitNum != 0)
                name = m_unitNum.ToString(CultureInfo.CurrentCulture);
            else if (!string.IsNullOrEmpty(m_description))
                name = m_description;
            else if (!string.IsNullOrEmpty(m_clientId))
                name = m_clientId;
            else
                name = string.Format(CultureInfo.CurrentCulture, Resources.MachineId, m_id);

            return string.Format(CultureInfo.CurrentCulture, Resources.Machine, name, (!m_isEnabled) ? Resources.DisabledText : string.Empty).Trim();
        }

        /// <summary>
        /// Performs a shallow copy based on the instance passed in.
        /// </summary>
        /// <param name="unit">The instance to copy.</param>
        public void Copy(Machine machine)
        {
            m_id = machine.m_id;
            m_deviceType = machine.m_deviceType;
            m_locationId = machine.m_locationId;
            m_clientId = machine.m_clientId;
            m_description = machine.m_description;
            m_serialNum = machine.m_serialNum;
            m_unitNum = machine.m_unitNum;
            m_assignedPlayer = machine.m_assignedPlayer; // Rally US247
            m_isEnabled = machine.m_isEnabled;

            // Rally DE2245
            // Copy all the pack numbers.
            m_packNums.Clear();

            foreach (int packNum in machine.m_packNums)
            {
                m_packNums.Add(packNum);
            }
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// The id of the machine.
        /// </summary>
        public int Id
        {
            get
            {
                return m_id;
            }
            set
            {
                m_id = value;
            }
        }

        /// <summary>
        /// The device type of the machine.
        /// </summary>
        public Device DeviceType
        {
            get
            {
                return m_deviceType;
            }
            set
            {
                m_deviceType = value;
            }
        }

        /// <summary>
        /// The id of the location of the machine.
        /// </summary>
        public int LocationId
        {
            get
            {
                return m_locationId;
            }
            set
            {
                m_locationId = value;
            }
        }

        /// <summary>
        /// The client identifier (usually MAC address) of the machine.
        /// </summary>
        public string ClientIdentifier
        {
            get
            {
                return m_clientId;
            }
            set
            {
                m_clientId = value;
            }
        }

        /// <summary>
        /// The text description of the machine.
        /// </summary>
        public string Description
        {
            get
            {
                return m_description;
            }
            set
            {
                m_description = value;
            }
        }

        /// <summary>
        /// The serial number of the machine (if applicable).
        /// </summary>
        public string SerialNumber
        {
            get
            {
                return m_serialNum;
            }
            set
            {
                m_serialNum = value;
            }
        }

        /// <summary>
        /// Gets or sets the machine's unit number.
        /// </summary>
        public short UnitNumber
        {
            get
            {
                return m_unitNum;
            }
            set
            {
                m_unitNum = value;
            }
        }

        // Rally US247
        /// <summary>
        /// Gets or sets the player currently assigned to this machine or null
        /// if no player is assigned.
        /// </summary>
        public Player AssignedPlayer
        {
            get
            {
                return m_assignedPlayer;
            }
            set
            {
                m_assignedPlayer = value;
            }
        }

        // Rally DE2245
        /// <summary>
        /// Gets a collection of pack numbers currently associated with the
        /// machine.
        /// </summary>
        public List<int> PackNumbers
        {
            get
            {
                return m_packNums;
            }
            set
            {
                m_packNums = value;
            }
        }

        /// <summary>
        /// Whether the machine is active.
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return m_isEnabled;
            }
            set
            {
                m_isEnabled = value;
            }
        }
        #endregion
    }
}
