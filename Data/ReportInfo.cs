using System.Collections.Generic;

namespace GTI.Modules.Shared
{
    public class ReportInfo
    {
        public int ID {get; set;}
        public int TypeID {get; set;}
        public string DisplayName { get; set; }
        public string FileName {get; set;}
        public byte[] Hash {get; set;}
        public byte RemoveType {get; set;} //1 remove, 0 not
        public Dictionary<int, string> Parameters {get; set;}
        public bool IsEnable { get; set; }


        public ReportInfo() { }

        public ReportInfo(int id, int typeID, string name) 
            : this(id, typeID, name, 0) { }

        public ReportInfo(int id, int typeID, string name, byte remove) 
            : this(id, typeID, name, remove, string.Empty, null) { }

        public ReportInfo(int id, int typeID, string name, byte remove, string fileName, byte[] hash)
        {
            ID = id;
            TypeID = typeID;
            DisplayName = name;
            FileName = fileName;
            Hash = hash;
            RemoveType = remove;
            Parameters = new Dictionary<int, string>();
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}