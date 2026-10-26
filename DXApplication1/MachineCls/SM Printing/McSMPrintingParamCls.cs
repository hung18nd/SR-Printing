using DXApplication1.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXApplication1.MachineCls.SM_Printing
{
    public class McSMPrintingParamCls
    {
        public int PrintingTime { get; set; } = 0;
        public int StencilAngle { get; set; }
        public int SqueezeSpeed { get; set; }
        public int ReturnSpeed { get; set; }
        public int SqueezePressure { get; set; }
        public int SqueezePringtingDistanceStart { get; set; }
        public int SqueezePringtingDistanceFinish { get; set; }
        public int CoaterPressure { get; set; }
        public int Gap { get; set; }
        public bool AutoRecipe { get; set; }
        public int MachineAirPressure { get; set; }

    }
}
