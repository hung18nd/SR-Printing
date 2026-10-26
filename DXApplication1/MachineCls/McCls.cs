using DXApplication1.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXApplication1.MachineCls
{
    public class McCls
    {
        public string MachineID { get; set; } = "MCID";
        public string MachineName { get; set; } = "";
        public bool Enable { get; set; } = false;
        public string Factory { get; set; } = "";
        public string Note { get; set; } = "";

        public bool IsConnected { get; set; } = false;
        public bool IsOnline { get; set; } = false;
        public string Status { get; set; } = "";
        public string ErrorCode { get; set; } = "";
        public string Message { get; set; } = "";
        public int Cyctime { get; set; } = 0;
    }


    public class TblBoardregistration
    {
        public string BlockID { get; set; }
        public string IndicationNumber { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string LotNumber { get; set; }
        public string CreatedByOperatorId { get; set; }
        public DateTime APIDateTime { get; set; }
        public string APIRequester { get; set; }
        public string APIVersion { get; set; }
        public string APIVersionDate { get; set; }
        public string Message { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BlockIDDatasSave
    {
        public string BlockID { get; set; } = "";
        public string LastBlockID { get; set; } = "";

        public DateTime LastBlockIDTime { get; set; } = Global.Y2K_VALUE;
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BlockIDDatas
    {
        public string BlockID { get; set; } = "";
        public string LastBlockID { get; set; } = "";

        private DateTime lastBlockIDTime = Global.Y2K_VALUE;
        public DateTime LastBlockIDTime
        {
            get { return lastBlockIDTime; }
            set
            {
                if(LastBlockID!= BlockID)
                {
                    lastBlockIDTime = value;
                }
            }
        }
        public int BlockIDIndex { get; set; } = -1;
        public int LastBlockIDIndex { get; set; } = -1;

        public bool CheckIndexChg()
        {
            return ((BlockIDIndex != LastBlockIDIndex) && BlockIDIndex != 0);
        }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ItemFromPlus
    {
        public string ItemName { get; set; } = "";
        public string ItemCode { get; set; } = "";
        public string ItemLot { get; set; } = "";
        public string ItemSeq { get; set; } = "";

    }
}
