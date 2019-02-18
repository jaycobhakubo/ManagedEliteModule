// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using GTI.Modules.Shared.Properties;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// An enumeration that represents the different login methods for devices.
    /// </summary>
    public enum DeviceLoginConnectionType
    {
        NotApplicable = 0,
        AutoLogin = 1,
        Staff = 2,
        Player = 3
    }

    /// <summary>
    /// A bit packed field to represent which devices are compatible with
    /// a product package in the GameTech Elite system.
    /// </summary>
    [Flags]
    public enum CompatibleDevices
    {
        Traveler = 0x01,
        Tracker = 0x02,
        Fixed = 0x04,
        Explorer = 0x08, // Rally TA7729 - Change Mini to Explorer.
        Traveler2 = 0x10, // PDTS 964, Rally US765 - WiFi now called II
        Tablet = 0x20 //fixed value
    }

    /// <summary>
    /// Represents a hardware device in the GameTech Elite system.
    /// </summary>
    public struct Device
    {
        #region Member Variables
        private int m_id;
        private string m_name;
        private DeviceLoginConnectionType m_loginConnectionType;
        private decimal m_fee;
        private CompatibleDevices m_compatibleDevicesValue;
        private bool m_isActive;
        #endregion

        #region Static Methods
        /// <summary>
        /// Returns the Device that matches the specified id.  If the 
        /// id is invalid, then a device with no name and a 0 id is returned.
        /// </summary>
        /// <param name="id">The id of the desired device.</param>
        /// <returns>A Device object.</returns>
        public static Device FromId(int id)
        {
            Device dev;

            switch(id)
            {
                case 1:
                    dev = Device.Traveler;
                    break;

                case 2:
                    dev = Device.Tracker;
                    break;

                case 3:
                    dev = Device.Fixed;
                    break;

                case 4:
                    dev = Device.Explorer; // Rally TA7729 
                    break;

                case 5:
                    dev = Device.POS;
                    break;

                case 6:
                    dev = Device.Caller;
                    break;

                case 7:
                    dev = Device.POSPortable;
                    break;

                case 8:
                    dev = Device.Kiosk;
                    break;

                case 9:
                    dev = Device.RemoteDisplay;
                    break;

                case 10:
                    dev = Device.UserDefined;
                    break;

                case 11:
                    dev = Device.Management;
                    break;

                case 12:
                    dev = Device.CrateServer;
                    break;

                case 13:
                    dev = Device.POSManagement;
                    break;

                // PDTS 964
                // Rally US765
                case 14:
                    dev = Device.Traveler2;
                    break;

                case 15:
                    dev = Device.UKSocketServer;
                    break;

               //US2908
                case 17:
                    dev = Device.Tablet;
                    break;

                case 19:
                    dev = Device.VLTBingoKiosk;
                    break;

                case 20:
                    dev = Device.AdvancedPOSKiosk;
                    break;
                
                case 21:
                    dev = Device.BuyAgainKiosk;
                    break;

                case 22:
                    dev = Device.SimplePOSKiosk;
                    break;

                case 23:
                    dev = Device.HybridKiosk;
                    break;

                case 24:
                    dev = Device.B3Kiosk;
                    break;

                default:
                    dev = new Device();
                    break;
            }

            return dev;
        }

        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the device's id.
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
        /// Gets or sets the device's name.
        /// </summary>
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        /// <summary>
        /// Gets or sets the device's login connection type.
        /// </summary>
        public DeviceLoginConnectionType LoginConnectionType
        {
            get
            {
                return m_loginConnectionType;
            }
            set
            {
                m_loginConnectionType = value;
            }
        }

        /// <summary>
        /// Gets or sets the device's fee (if applicable).
        /// </summary>
        public decimal Fee
        {
            get
            {
                return m_fee;
            }
            set
            {
                m_fee = value;
            }
        }

        /// <summary>
        /// Gets or sets the device's CompatibleDevices flag.
        /// If a device doesn't have a CompatibleDevices flag then 0
        /// is returned.
        /// </summary>
        public CompatibleDevices CompatibleDevicesValue
        {
            get
            {
                return m_compatibleDevicesValue;
            }
            set
            {
                m_compatibleDevicesValue = value;
            }
        }
        
        /// <summary>
        /// Gets or sets whether the device is active.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return m_isActive;
            }
            set
            {
                m_isActive = value;
            }
        }

        /// <summary>
        /// Returns the string representation of this object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Returns whether or not the sent in object equals this object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Device)
            {
                Device other = (Device)obj;
                return this.Id == other.Id;
            }
            return false;
        }

        /// <summary>
        /// returns the uniquely identifying hash code for this object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Id;
        }
        #endregion

        #region Static Declarations

        /// <summary>
        /// A known device.
        /// </summary>
        public static Device Traveler
        {
            get
            {
                Device dev = new Device();
                dev.Id = 1;
                dev.Name = Resources.DeviceTraveler;
                dev.LoginConnectionType = DeviceLoginConnectionType.Player;
                dev.CompatibleDevicesValue = CompatibleDevices.Traveler;
                dev.IsActive = true;

                return dev;
            }
        }

        /// <summary>
        /// A known device.
        /// </summary>
        public static Device Tracker
        {
            get
            {
                Device dev = new Device();
                dev.Id = 2;
                dev.Name = Resources.DeviceTracker;
                dev.LoginConnectionType = DeviceLoginConnectionType.Player;               
                dev.CompatibleDevicesValue = CompatibleDevices.Tracker;
                dev.IsActive = true;

                return dev;
            }
        }

        /// <summary>
        /// A known device.
        /// </summary>
        public static Device Fixed
        {
            get
            {
                Device dev = new Device();
                dev.Id = 3;
                dev.Name = Resources.DeviceFixedUnit;
                dev.LoginConnectionType = DeviceLoginConnectionType.Player;
                dev.CompatibleDevicesValue = CompatibleDevices.Fixed;
                dev.IsActive = true;

                return dev;
            }
        }

        // Rally TA7729
        /// <summary>
        /// A known device.
        /// </summary>
        public static Device Explorer
        {
            get
            {
                Device dev = new Device();
                dev.Id = 4;
                dev.Name = Resources.DeviceExplorer;
                dev.LoginConnectionType = DeviceLoginConnectionType.Player;
                dev.CompatibleDevicesValue = CompatibleDevices.Explorer;
                dev.IsActive = true;

                return dev;
            }
        }

        /// <summary>
        /// A known device.
        /// </summary>
        public static Device POS
        {
            get
            {
                Device dev = new Device();
                dev.Id = 5;
                dev.Name = Resources.DevicePOS;
                dev.LoginConnectionType = DeviceLoginConnectionType.Staff;
                dev.IsActive = true;

                return dev;
            }
        }

        /// <summary>
        /// A known device.
        /// </summary>
        public static Device Caller
        {
            get
            {
                Device dev = new Device();
                dev.Id = 6;
                dev.Name = Resources.DeviceCaller;
                dev.LoginConnectionType = DeviceLoginConnectionType.Staff;
                dev.IsActive = true;

                return dev;
            }
        }

        /// <summary>
        /// A known device.
        /// </summary>
        public static Device POSPortable
        {
            get
            {
                Device dev = new Device();
                dev.Id = 7;
                dev.Name = Resources.DevicePOSPortable;
                dev.LoginConnectionType = DeviceLoginConnectionType.Staff;
                dev.IsActive = true;

                return dev;
            }
        }

        /// <summary>
        /// A known device.
        /// </summary>
        public static Device Kiosk
        {
            get
            {
                Device dev = new Device();
                dev.Id = 8;
                dev.Name = Resources.DeviceKiosk;
                dev.LoginConnectionType = DeviceLoginConnectionType.AutoLogin;
                dev.IsActive = true;

                return dev;
            }
        }

        /// <summary>
        /// A known device.
        /// </summary>
        public static Device RemoteDisplay
        {
            get
            {
                Device dev = new Device();
                dev.Id = 9;
                dev.Name = Resources.DeviceRemoteDisplay;
                dev.LoginConnectionType = DeviceLoginConnectionType.AutoLogin;
                dev.IsActive = true;

                return dev;
            }
        }

        /// <summary>
        /// A known device.
        /// </summary>
        public static Device UserDefined
        {
            get
            {
                Device dev = new Device();
                dev.Id = 10;
                dev.Name = Resources.DeviceUserDefined;
                dev.LoginConnectionType = DeviceLoginConnectionType.NotApplicable;
                dev.IsActive = true;

                return dev;
            }
        }

        /// <summary>
        /// A known device.
        /// </summary>
        public static Device Management
        {
            get
            {
                Device dev = new Device();
                dev.Id = 11;
                dev.Name = Resources.DeviceManagement;
                dev.LoginConnectionType = DeviceLoginConnectionType.Staff;
                dev.IsActive = true;

                return dev;
            }
        }

        // TTP 50339
        /// <summary>
        /// A known device.
        /// </summary>
        public static Device CrateServer
        {
            get
            {
                Device dev = new Device();
                dev.Id = 12;
                dev.Name = Resources.DeviceCrateServer;
                dev.LoginConnectionType = DeviceLoginConnectionType.Staff;
                dev.IsActive = true;

                return dev;
            }
        }

        /// <summary>
        /// A known device.
        /// </summary>
        public static Device POSManagement
        {
            get
            {
                Device dev = new Device();
                dev.Id = 13;
                dev.Name = Resources.DevicePOSManagement;
                dev.LoginConnectionType = DeviceLoginConnectionType.Staff;
                dev.IsActive = true;

                return dev;
            }
        }

        // PDTS 964
        // Rally US765
        /// <summary>
        /// A known device.
        /// </summary>
        public static Device Traveler2
        {
            get
            {
                Device dev = new Device();
                dev.Id = 14;
                dev.Name = Resources.DeviceTraveler2;
                dev.LoginConnectionType = DeviceLoginConnectionType.Player;
                dev.CompatibleDevicesValue = CompatibleDevices.Traveler2;
                dev.IsActive = true;

                return dev;
            }
        }

        // PDTS 964
        /// <summary>
        /// A known device.
        /// </summary>
        public static Device UKSocketServer
        {
            get
            {
                Device dev = new Device();
                dev.Id = 15;
                dev.Name = Resources.DeviceUKSocketServer;
                dev.LoginConnectionType = DeviceLoginConnectionType.AutoLogin;
                dev.IsActive = true;

                return dev;
            }
        }
        //US2908
        public static Device Tablet
        {
            get
            {
                Device dev = new Device();
                dev.Id = 17;
                dev.Name = Resources.DeviceTablet;
                dev.LoginConnectionType = DeviceLoginConnectionType.Player;
                dev.CompatibleDevicesValue = CompatibleDevices.Tablet;
                dev.IsActive = true;
                return dev;
 
            }
        }

        /// <summary>
        /// A known device.
        /// </summary>
        public static Device VLTBingoKiosk
        {
            get
            {
                Device dev = new Device();
                dev.Id = 19;
                dev.Name = Resources.DeviceVLTBingoKiosk;
                dev.LoginConnectionType = DeviceLoginConnectionType.AutoLogin;
                dev.IsActive = true;

                return dev;
            }
        }

        /// <summary>
        /// Advanced POS Kiosk.
        /// </summary>
        public static Device AdvancedPOSKiosk
        {
            get
            {
                Device dev = new Device();
                dev.Id = 20;
                dev.Name = Resources.DeviceAdvancedPOSKiosk;
                dev.LoginConnectionType = DeviceLoginConnectionType.AutoLogin;
                dev.IsActive = true;

                return dev;
            }
        }

        /// <summary>
        /// Buy Again Kiosk.
        /// </summary>
        public static Device BuyAgainKiosk
        {
            get
            {
                Device dev = new Device();
                dev.Id = 21;
                dev.Name = Resources.DeviceBuyAgainKiosk;
                dev.LoginConnectionType = DeviceLoginConnectionType.AutoLogin;
                dev.IsActive = true;

                return dev;
            }
        }

        /// <summary>
        /// Simple POS Kiosk.
        /// </summary>
        public static Device SimplePOSKiosk
        {
            get
            {
                Device dev = new Device();
                dev.Id = 22;
                dev.Name = Resources.DeviceSimplePOSKiosk;
                dev.LoginConnectionType = DeviceLoginConnectionType.AutoLogin;
                dev.IsActive = true;

                return dev;
            }
        }

        /// <summary>
        /// Hybrid Kiosk - Combination of Buy Again and Simple POS Kiosks.
        /// </summary>
        public static Device HybridKiosk
        {
            get
            {
                Device dev = new Device();
                dev.Id = 23;
                dev.Name = Resources.DeviceHybridKiosk;
                dev.LoginConnectionType = DeviceLoginConnectionType.AutoLogin;
                dev.IsActive = true;

                return dev;
            }
        }
        
        /// <summary>
        /// B3 Kiosk - Only allows B3 credit sales.
        /// </summary>
        public static Device B3Kiosk
        {
            get
            {
                Device dev = new Device();
                dev.Id = 24;
                dev.Name = Resources.DeviceB3Kiosk;
                dev.LoginConnectionType = DeviceLoginConnectionType.AutoLogin;
                dev.IsActive = true;

                return dev;
            }
        }

        #endregion
    }
}
