

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GTI.Modules.Shared
{



    public class GetPlayerLocationPer
    {

        public static List<LocationCity> CityName { get; set; }
        public static List<LocationState> StateName { get; set; }
        public static List<LocationZipCode> ZipCodeName { get; set; }
        public static List<LocationCountry> CountryName { get; set; }
    }

    public class LocationCity
    {
        public string City { get; set; }
        public override string ToString() { return City; }

    }


    public class LocationState
    {
        public string State { get; set; }
        public override string ToString()
        {
            return State;
        }
    }


    public class LocationZipCode
    {
        public string ZipCode { get; set; }
        public override string ToString()
        {
            return ZipCode;
        }
    }


    public class LocationCountry
    {
        public string Country { get; set; }
        public override string ToString()
        {
            return Country;
        }
    }

   


    
}

