using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTI.Modules.Shared.Business
{
    public class CardPositionMapHandle
    {
        public int Id
        {
            get;
            set;
        }
        public string PositionMapName
        {
            get;
            set;
        }
        public bool IsActive
        {
            get;
            set;
        }
        public byte PositionsCovered
        {
            get;
            set;
        }
        public byte SequenceLength
        {
            get;
            set;
        }
        public int NumSequences
        {
            get;
            set;
        }
        public string PositionMapGUID
        {
            get;
            set;
        }
        public string PositionMapPath
        {
            get;
            set;
        }

        public void LoadFrom(CardPositionMapHandle other)
        {
            this.Id = other.Id;
            this.PositionMapName = other.PositionMapName;
            this.IsActive = other.IsActive;
            this.PositionsCovered = other.PositionsCovered;
            this.SequenceLength = other.SequenceLength;
            this.NumSequences = other.NumSequences;
            this.PositionMapGUID = other.PositionMapGUID;
            this.PositionMapPath = other.PositionMapPath;
        }
    }
}
