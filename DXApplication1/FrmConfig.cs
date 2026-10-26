using DevExpress.Utils.Filtering;
using DevExpress.XtraEditors;
using DXApplication1.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
namespace DXApplication1
{
    public partial class FrmConfig : DevExpress.XtraEditors.XtraForm
    {
        public FrmConfig()
        {
            InitializeComponent();
        }

        List<MachineCfg> LstFixMachine = new List<MachineCfg>()
        {
            //new MachineCfg(){ MachineType = ClsMachineName.McUvCleaningNo1},
            
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo12},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo14},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo15},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo16},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo17},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo18},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo19},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo20},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo21},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo22},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo23},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo24},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo25},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo26},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo27},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo28},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo29},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo30},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo31},
            
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo33},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo34},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo35},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo36},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo37},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo38},
            
            

            //new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo1L},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo1R},
            
            //new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo11L},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo11R},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo12L},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo12R},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo13L},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo13R},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo18L},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo18R},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo19L},
            //new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo19R},
            //new MachineCfg(){ MachineType = ClsMachineName.McRTSCuttingNo1},
            //new MachineCfg(){ MachineType = ClsMachineName.McRTSCuttingNo2},
            //new MachineCfg(){ MachineType = ClsMachineName.McRTSCuttingNo3},
            //new MachineCfg(){ MachineType = ClsMachineName.McRTSCuttingNo4},
            //new MachineCfg(){ MachineType = ClsMachineName.McRTSCuttingNo5},
            //new MachineCfg(){ MachineType = ClsMachineName.McRTSCuttingNo6},
            //new MachineCfg(){ MachineType = ClsMachineName.McRTSCuttingNo7},

           

          

            //new MachineCfg(){ MachineType = ClsMachineName.McRTREtchingNo1},
            //new MachineCfg(){ MachineType = ClsMachineName.McRTREtchingNo3},
            //new MachineCfg(){ MachineType = ClsMachineName.McRTREtchingNo4},
            //new MachineCfg(){ MachineType = ClsMachineName.McRTREtchingNo5},

            //new MachineCfg(){ MachineType = ClsMachineName.McRTRDfrNo1},
            //new MachineCfg(){ MachineType = ClsMachineName.McRTRDfrNo2},
            //new MachineCfg(){ MachineType = ClsMachineName.McRTRDfrNo3},
            //new MachineCfg(){ MachineType = ClsMachineName.McRTRDfrNo4},
            //new MachineCfg(){ MachineType = ClsMachineName.McRTRDfrNo5},
            //new MachineCfg(){ MachineType = ClsMachineName.McRTRDfrNo6},

            
            //new MachineCfg(){ MachineType = ClsMachineName.McShadowNo4},

            
            //new MachineCfg(){ MachineType = ClsMachineName.McSTRVacuumLamiNo1},
            //new MachineCfg(){ MachineType = ClsMachineName.McSTRVacuumLamiNo2},
            //new MachineCfg(){ MachineType = ClsMachineName.McSTRVacuumLamiNo4},
            //new MachineCfg(){ MachineType = ClsMachineName.McSTRVacuumLamiNo5},

            //new MachineCfg(){ MachineType = ClsMachineName.McCopperBrightNo6},
            //new MachineCfg(){ MachineType = ClsMachineName.McRTSDevelopNo2},

            //new MachineCfg(){ MachineType = ClsMachineName.McBakingEtchingNo1L},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingEtchingNo1R},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingEtchingNo2L},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingEtchingNo2R},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingEtchingNo3L},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingEtchingNo3R},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingEtchingNo4L},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingEtchingNo4R},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingEtchingNo5L},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingEtchingNo5R},

            //new MachineCfg(){ MachineType = ClsMachineName.McBakingPrintingNo1L},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingPrintingNo1R},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingPrintingNo5L},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingPrintingNo5R},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingPrintingNo6L},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingPrintingNo6R},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingPrintingNo7L},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingPrintingNo7R},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingPrintingNo8L},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingPrintingNo8R},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingPrintingNo9L},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingPrintingNo9R},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingPrintingNo10L},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingPrintingNo10R},

            //new MachineCfg(){ MachineType = ClsMachineName.McBakingCLNo2L},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingCLNo2R},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingCLNo3L},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingCLNo3R},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingCLNo4L},
            //new MachineCfg(){ MachineType = ClsMachineName.McBakingCLNo4R},
            //new MachineCfg(){ MachineType = ClsMachineName.McCopperSTSNo6X},

            //new MachineCfg(){ MachineType = ClsMachineName.McCopperSTSNo5X},
            //new MachineCfg(){ MachineType = ClsMachineName.McCopperSTSNo8X},
            //new MachineCfg(){ MachineType = ClsMachineName.McCopperSTSNo9X},
            //new MachineCfg(){ MachineType = ClsMachineName.McCopperSTSNo10X},

            new MachineCfg(){ MachineType = ClsMachineName.McShadowRTRNo1},
            new MachineCfg(){ MachineType = ClsMachineName.McShadowRTRNo2},
            new MachineCfg(){ MachineType = ClsMachineName.McShadowSTSNo4},
            new MachineCfg(){ MachineType = ClsMachineName.McRTSCuttingNo2},
            new MachineCfg(){ MachineType = ClsMachineName.McRTSCuttingNo3},
            new MachineCfg(){ MachineType = ClsMachineName.McRTSCuttingNo4},
            new MachineCfg(){ MachineType = ClsMachineName.McRTSCuttingNo5},

            new MachineCfg(){ MachineType = ClsMachineName.McCLAutoPastingNo1},
            new MachineCfg(){ MachineType = ClsMachineName.McCLAutoPastingNo2},
            new MachineCfg(){ MachineType = ClsMachineName.McCLAutoPastingNo3},
            new MachineCfg(){ MachineType = ClsMachineName.McCLAutoPastingNo4},
            new MachineCfg(){ MachineType = ClsMachineName.McCLAutoPastingNo5},
            new MachineCfg(){ MachineType = ClsMachineName.McCLAutoPastingNo6},
            new MachineCfg(){ MachineType = ClsMachineName.McCLAutoPastingNo7},
            new MachineCfg(){ MachineType = ClsMachineName.McCLAutoPastingNo8},
            new MachineCfg(){ MachineType = ClsMachineName.McCLAutoPastingNo9},
            new MachineCfg(){ MachineType = ClsMachineName.McCLAutoPastingNo10},

            new MachineCfg(){ MachineType = ClsMachineName.McRTSDevelopNo1},
            new MachineCfg(){ MachineType = ClsMachineName.McRTSDevelopNo2},

            new MachineCfg(){ MachineType = ClsMachineName.McCopperSTSNo8},
            new MachineCfg(){ MachineType = ClsMachineName.McCopperSTSNo9},
            new MachineCfg(){ MachineType = ClsMachineName.McCopperSTSNo5},
            new MachineCfg(){ MachineType = ClsMachineName.McCopperSTSNo6},
            new MachineCfg(){ MachineType = ClsMachineName.McCopperSTSNo10},

            new MachineCfg(){ MachineType = ClsMachineName.McLayupNo1},
            new MachineCfg(){ MachineType = ClsMachineName.McLayupNo2},
            new MachineCfg(){ MachineType = ClsMachineName.McLayupNo3},

            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo32},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo15},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo14},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo34},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo18},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo21},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo22},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo23},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo25},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo26},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo27},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo16},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo33},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo29},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo31},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo19},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo20},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo24},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo35},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo12},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo36},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo17},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo28},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo37},
            new MachineCfg(){ MachineType = ClsMachineName.McSRPrintingNo38},

            new MachineCfg(){ MachineType = ClsMachineName.McRTRStrippingNo1},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRStrippingNo2},

            new MachineCfg(){ MachineType = ClsMachineName.McCopperRTRNo9E},
            new MachineCfg(){ MachineType = ClsMachineName.McCopperRTRNo9F},
            new MachineCfg(){ MachineType = ClsMachineName.McCopperRTRNo10G},
            new MachineCfg(){ MachineType = ClsMachineName.McCopperRTRNo10H},
            new MachineCfg(){ MachineType = ClsMachineName.McCopperRTRNo12K},
            new MachineCfg(){ MachineType = ClsMachineName.McCopperRTRNo12L},
            new MachineCfg(){ MachineType = ClsMachineName.McCopperRTRNo14M},
            new MachineCfg(){ MachineType = ClsMachineName.McCopperRTRNo14Q},

            new MachineCfg(){ MachineType = ClsMachineName.McSRLaminateNo1},
            new MachineCfg(){ MachineType = ClsMachineName.McSRLaminateNo2},
            new MachineCfg(){ MachineType = ClsMachineName.McSRLaminateNo3},

            new MachineCfg(){ MachineType = ClsMachineName.McSTSStrippingNo1},
            new MachineCfg(){ MachineType = ClsMachineName.McSTSStrippingNo2},

            new MachineCfg(){ MachineType = ClsMachineName.McUvCleaningNo2},
            new MachineCfg(){ MachineType = ClsMachineName.McUvCleaningNo3},
            new MachineCfg(){ MachineType = ClsMachineName.McUvCleaningNo4},
            new MachineCfg(){ MachineType = ClsMachineName.McUvCleaningNo5},

            new MachineCfg(){ MachineType = ClsMachineName.McUvCureNo1},
            new MachineCfg(){ MachineType = ClsMachineName.McUvCureNo2},
            new MachineCfg(){ MachineType = ClsMachineName.McUvCureNo3},

            new MachineCfg(){ MachineType = ClsMachineName.McSRDevelopNo3},
            new MachineCfg(){ MachineType = ClsMachineName.McSRDevelopNo4},
            new MachineCfg(){ MachineType = ClsMachineName.McSRDevelopNo5},

            new MachineCfg(){ MachineType = ClsMachineName.McSTSPlasmaNo2},
            new MachineCfg(){ MachineType = ClsMachineName.McSTSPlasmaNo3},
            new MachineCfg(){ MachineType = ClsMachineName.McDryDesmearNo4},

            new MachineCfg(){ MachineType = ClsMachineName.McElecGoldNo3},
            new MachineCfg(){ MachineType = ClsMachineName.McElecGoldNo4},
            new MachineCfg(){ MachineType = ClsMachineName.McElecGoldNo5},

            new MachineCfg(){ MachineType = ClsMachineName.McElecCopperNo1},
            new MachineCfg(){ MachineType = ClsMachineName.McElecCopperNo2},
            new MachineCfg(){ MachineType = ClsMachineName.McElecCopperNo3},

            new MachineCfg(){ MachineType = ClsMachineName.McRTRSurfaceNo6},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRSurfaceNo8},
            new MachineCfg(){ MachineType = ClsMachineName.McSurfaceNo4},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRSurfaceNo5},

            new MachineCfg(){ MachineType = ClsMachineName.McCopperBrightNo1},
            new MachineCfg(){ MachineType = ClsMachineName.McCopperBrightNo2},
            new MachineCfg(){ MachineType = ClsMachineName.McCopperBrightNo3},
            new MachineCfg(){ MachineType = ClsMachineName.McCopperBrightNo4},
            new MachineCfg(){ MachineType = ClsMachineName.McCopperBrightNo5},
            new MachineCfg(){ MachineType = ClsMachineName.McCopperBrightNo6},
            new MachineCfg(){ MachineType = ClsMachineName.McCopperBrightNo8},

            new MachineCfg(){ MachineType = ClsMachineName.McRTRDevelopNo1},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDevelopNo2},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDevelopNo3},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDevelopNo4},

            new MachineCfg(){ MachineType = ClsMachineName.McMicroEtchingNo2},
            new MachineCfg(){ MachineType = ClsMachineName.McMicroEtchingNo3},
            new MachineCfg(){ MachineType = ClsMachineName.McMicroEtchingNo4},
            new MachineCfg(){ MachineType = ClsMachineName.McMicroEtchingNo6},

            new MachineCfg(){ MachineType = ClsMachineName.McDFBackslit500No6},
            new MachineCfg(){ MachineType = ClsMachineName.McDFBackslit500No1},
            new MachineCfg(){ MachineType = ClsMachineName.McDFBackslit500No2},
            new MachineCfg(){ MachineType = ClsMachineName.McDFBackslit500No3},
            new MachineCfg(){ MachineType = ClsMachineName.McDFBackslit500No4},
            new MachineCfg(){ MachineType = ClsMachineName.McDFBackslit500No5},

            new MachineCfg(){ MachineType = ClsMachineName.McEtchingNo4},
            new MachineCfg(){ MachineType = ClsMachineName.McEtchingNo5},
            new MachineCfg(){ MachineType = ClsMachineName.McRTREtchingNo1},
            new MachineCfg(){ MachineType = ClsMachineName.McRTREtchingNo2},
            new MachineCfg(){ MachineType = ClsMachineName.McRTREtchingNo3},

            new MachineCfg(){ MachineType = ClsMachineName.McDFBackslit250No1},
            new MachineCfg(){ MachineType = ClsMachineName.McDFBackslit250No2},

            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo2L},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo2R},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo3L},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo3R},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo4L},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo4R},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo5L},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo5R},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo6L},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo6R},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo7L},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo7R},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo8L},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo8R},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo9L},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo9R},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo10L},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo10R},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo11L},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo11R},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo12L},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo12R},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo13L},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo13R},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo18L},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo18R},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo19L},
            new MachineCfg(){ MachineType = ClsMachineName.McSRExposingNo19R},

            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo1L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo1R},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo2L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo2R},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo3L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo3R},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo4L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo4R},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo5L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo5R},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo6L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo6R},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo7L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo7R},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo8L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo8R},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo12L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo12R},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo9L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo9R},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo10L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo10R},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo11L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo11R},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo14L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo14R},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo15L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo15R},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo16L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo16R},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo17L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo17R},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo18L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo18R},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo19L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo19R},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo20L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo20R},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo21L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo21R},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo22L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo22R},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo23L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDoubleSideNo23R},

            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo47},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo45},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo46},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo71},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo40},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo101},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo10},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo51},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo14},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo31},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo38},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo50},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo69},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo57},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo36},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo58},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo8},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo56},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo52},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo70},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo53},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo48},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo9},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo55},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo11},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo12},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo15},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo16},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo19},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo20},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo21},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo22},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo23},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo24},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo25},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo26},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo27},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo28},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo29},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo32},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo33},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo34},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo35},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo37},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo39},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo41},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo42},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo43},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo44},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo49},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo59},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo62},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo63},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo67},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo68},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo102},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo103},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo104},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo105},
            new MachineCfg(){ MachineType = ClsMachineName.McUvRRNo106},

            new MachineCfg(){ MachineType = ClsMachineName.McRTRDINo1L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDINo1R},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDINo2L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDINo2R},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDINo3L},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRDINo3R},

            new MachineCfg(){ MachineType = ClsMachineName.McRTRPlasmaNo1},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRPlasmaNo2},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRPlasmaNo4},
            new MachineCfg(){ MachineType = ClsMachineName.McRTRPlasmaNo6},
        };



        private void FrmConfig_Shown(object sender, EventArgs e)
        {
            Global.DoubleBuffered(dgrConn, true);
            LoadMachineCfg();
        }

        private void LoadMachineCfg()
        {
            bool ret = false;
            List<MachineCfg> lstMachineCfgTemp = new List<MachineCfg>();

            string pathCfg = Global.ApplicationPath() + @"\AppConfig";
            if (!Directory.Exists(pathCfg)) Directory.CreateDirectory(pathCfg);
            string pathCfgFile = pathCfg + @"\Machines.config";
            if (File.Exists(pathCfgFile))
            {
                try
                {
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(List<MachineCfg>));
                    StreamReader reader = File.OpenText(pathCfgFile);
                    var Configs = (List<MachineCfg>)xs.Deserialize(reader);

                    reader.Close();

                    if(Configs != null)
                    {
                        lstMachineCfgTemp = Configs;
                        
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Load config failed\r\n{ex.Message}!", "MESSAGE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

            List<MachineCfg> result = LstFixMachine.Where(p => lstMachineCfgTemp.All(p2 => p2.MachineType != p.MachineType)).ToList();
            if (result.Count>0)
            {
                for(int i = 0; i< result.Count; i++)
                {
                    lstMachineCfgTemp.Add(new MachineCfg() { MachineType = result[i].MachineType, MachineID = result[i].MachineID, MachineName = result[i].MachineName, PlcIp = result[i].PlcIp, PlcPort = result[i].PlcPort, Factory = result[i].Factory });
                }
            }

            lstMachineCfgTemp = lstMachineCfgTemp.OrderBy(x => x.MachineName).ToList();

            dgrConn.SuspendLayout();
            dgrConn.Rows.Clear();
            for (int i = 0; i < lstMachineCfgTemp.Count; i++)
            {
                MachineCfg Configs = lstMachineCfgTemp[i];
                dgrConn.Rows.Add(dgrConn.Rows.Count + 1, Configs.Enable, Configs.MachineType, Configs.MachineID, Configs.MachineName, Configs.PlcIp, Configs.PlcPort, Configs.Factory, Configs.Note);
                dgrConn.Rows[dgrConn.Rows.Count - 1].Tag = Configs;
            }
            dgrConn.ResumeLayout();
            dgrConn.ReadOnly = true;
            dgrConn.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            btnEdit.Enabled = !btnPassword.Enabled;
            btnSave.Enabled = false;
        }
        private void SaveMachineCfg()
        {
            dgrConn.EndEdit();

            List<MachineCfg> LstMachineCfg = new List<MachineCfg>();

            for(int i = 0; i< dgrConn.Rows.Count; i++)
            {
                object obj = dgrConn.Rows[i].Tag;
                if(obj.GetType() == typeof(MachineCfg))
                {
                    MachineCfg machineCfg = (MachineCfg)obj;
                    machineCfg.Enable = Conv.atob(dgrConn.Rows[i].Cells["Enable"].Value);
                    machineCfg.MachineID = Conv.atos(dgrConn.Rows[i].Cells["MachineID"].Value);
                    machineCfg.MachineName = Conv.atos(dgrConn.Rows[i].Cells["MachineName"].Value);
                    machineCfg.PlcIp = Conv.atos(dgrConn.Rows[i].Cells["MachineIP"].Value);
                    machineCfg.PlcPort = Conv.atoi32(dgrConn.Rows[i].Cells["MachinePort"].Value);
                    machineCfg.Factory = Conv.atos(dgrConn.Rows[i].Cells["Factory"].Value);
                    machineCfg.Note = Conv.atos(dgrConn.Rows[i].Cells["Note"].Value);

                    LstMachineCfg.Add(machineCfg);
                }
                
            }

            string PathSave = Global.ApplicationPath() + @"\AppConfig" + @"\Machines.config";
            try
            {
                XmlSerializer xsSubmit = new XmlSerializer(typeof(List<MachineCfg>));

                var subReq = LstMachineCfg;
                using (StringWriter sww = new StringWriter())
                {
                    XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
                    {
                        Indent = true,
                        OmitXmlDeclaration = false,
                        Encoding = Encoding.Unicode
                    };

                    using (XmlWriter writer = XmlWriter.Create(sww, xmlWriterSettings))
                    {
                        xsSubmit.Serialize(writer, subReq);
                        var xml = sww.ToString();
                        if (File.Exists(PathSave))
                        {
                            File.Delete(PathSave);
                            File.WriteAllText(PathSave, xml);
                            Console.WriteLine(xml);
                        }
                        else
                        {
                            File.WriteAllText(PathSave, xml);
                            Console.WriteLine(xml);
                        }

                        MessageBox.Show("Lưu thành công!", "MESSAGE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lưu lỗi!\r\n" + ex.Message, "MESSAGE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            dgrConn.ReadOnly = false;
            dgrConn.SelectionMode = DataGridViewSelectionMode.CellSelect;

            btnSave.Enabled = !btnPassword.Enabled;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveMachineCfg();
            LoadMachineCfg();
        }

        private void btnPassword_Click(object sender, EventArgs e)
        {
            string password = Global.PasswordInput();
            if (password == "1")
            {
                btnPassword.Enabled = false;
                btnEdit.Enabled = !btnPassword.Enabled;
            }
            else
            {
                MessageBox.Show("Sai mật khẩu!", "LỖI THAO TÁC", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadMachineCfg();
        }
    }


    public class MachineCfg
    {
        public string MachineType { get; set; } = "MCTP";
        public string MachineID { get; set; } = "MCID";
        public string MachineName { get; set; } = "";
        public string PlcIp { get; set; } = "127.0.0.0";
        public int PlcPort { get; set; } = 5000;
        public bool Enable { get; set; } = false;
        public string Factory { get; set; } = "";
        public string Note { get; set; } = "";
    }

    public class MachineCfgRunning
    {
        public string MachineType { get; set; } = "MCTP";
        public string MachineID { get; set; } = "MCID";
        public string MachineName { get; set; } = "";
        public string PlcIp { get; set; } = "127.0.0.0";
        public int PlcPort { get; set; } = 5000;
        public bool Enable { get; set; } = false;
        public string Factory { get; set; } = "";
        public string Note { get; set; } = "";
        public string PlcIpKV { get; set; } = "127.0.0.0";
       
        public bool IsConnected { get; set; } = false;
        public bool IsOnline { get; set; } = false;
        public string Status { get; set; } = "";
        public string ErrorCode { get; set; } = "";
        public string Message { get; set; } = "";

        public bool IsConnectedPlcKV { get; set; } = false;
        
        public int Cyctime { get; set; } = 0;
    }
    public class ConnCfg
    {
        public string MachineType { get; set; } = "MCTP";
        public string MachineID { get; set; } = "MCID";
        public string MachineName { get; set; } = "";
        public string PlcIp { get; set; } = "127.0.0.0";
        public int PlcPort { get; set; } = 5000;
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

    public class BlockID2KV
    {
        public string BlockID { get; set; } = "";
        public bool IsSync { get; set; } = false;
    }
    public class MachineInfo
    {
        [Category("1.Info")]
        public string MachineType { get; set; } = "MCTP";
        [Category("1.Info")]
        public string MachineID { get; set; } = "MCID";
        [Category("1.Info")]
        public string MachineName { get; set; } = "";
        [Category("1.Info")]
        public string Factory { get; set; } = "";
        [Category("2.Setting")]
        public string PlcIp { get; set; } = "127.0.0.0";
        [Category("2.Setting")]
        public int PlcPort { get; set; } = 5000;
        [Category("2.Setting")]
        public string PlcIpKV { get; set; } = "127.0.0.0";
        [Category("2.Setting")]
        public int PlcPortKV { get; set; } = 5000;
        [Category("2.Setting")]
        [DisplayName("Enable PLC KV")]
        [Description("Bật/Tắt kết nối và gửi dữ liệu tới PLC Keyence KV")]
        public bool EnablePlcKV { get; set; } = true;
        [Category("2.Setting")]
        public bool Enable { get; set; } = false;
        [Category("2.Setting")]
        public bool Connection { get; set; } = true;
        [Category("2.Setting")]
        public string Note { get; set; } = "";
        [Category("2.Setting")]
        public int ReinputDelayTime { get; set; } = 5;
        [Category("2.Setting")]
        public bool EnableMixing { get; set; } = false;
        [Category("2.Setting")]
        public bool EnableReinput { get; set; } = false;


        [Category("2.Setting")]
        public string BcrSrx300Ip { get; set; } = "127.0.0.0";
        [Category("2.Setting")]
        public int BcrSrx300Port { get; set; } = 5000;
        [Category("2.Setting")]
        public string BcrCognexIp { get; set; } = "127.0.0.0";
        [Category("2.Setting")]
        public int BcrCognexPort { get; set; } = 5000;

        [Category("2.Setting")]
        public int BlockNumPerPrints { get; set; } = 2;
        [Category("2.Setting")]
        public int BcrMode { get; set; } = 1;
        [Category("2.Setting")]
        public int BcrSelect { get; set; } = 0;


        [Category("2.Setting")]
        public string RegPrintingTime { get; set; } 
        [Category("2.Setting")]
        public string RegStencilAngle { get; set; }
        [Category("2.Setting")]
        public string RegSqueezeSpeed { get; set; }
        [Category("2.Setting")]
        public string RegSqueezePressure { get; set; }
        [Category("2.Setting")]
        public string RegCoaterPressure { get; set; }
        [Category("2.Setting")]
        public string RegSqueezePringtingDistanceStart { get; set; }
        [Category("2.Setting")]
        public string RegSqueezePringtingDistanceFinish { get; set; }
        [Category("2.Setting")]
        public string RegGap { get; set; }
        [Category("2.Setting")]
        public string RegMachineAirPressure { get; set; }
        [Category("2.Setting")]
        public string RegReturnSpeed { get; set; }


        [Category("2.Setting")]
        public string RegBlockID { get; set; }



        [Category("3.Status")]
        public bool IsConnectedPlcKV { get; set; } = false;
        [Category("3.Status")]
        public bool IsConnected { get; set; } = false;
        [Category("3.Status")]
        public bool IsOnline { get; set; } = false;
        [Category("3.Status")]
        public string Status { get; set; } = "";
        [Category("3.Status")]
        public string StatusBlock { get; set; } = "";
        [Category("3.Status")]
        public string ErrorCode { get; set; } = "";
        [Category("3.Status")]
        public string Message { get; set; } = "";
        [Category("3.Status")]
        public string MessageKV { get; set; } = "";

        [Category("3.Status")]
        public bool IsConnectedSRX { get; set; } = false;
        [Category("3.Status")]
        public bool IsConnectedCognex { get; set; } = false;


        [Category("4.Datas")]
        public int Cyctime { get; set; } = 0;
        [Category("4.Datas")]
        public int CyctimeKV { get; set; } = 0;
        [Category("3.Status")]
        public bool IsAlarmMixing { get; set; } = false;
        [Category("3.Status")]
        public bool IsAlarmReInput { get; set; } = false;
        [Category("5.Command")]
        public bool ReqAlarmMixing { get; set; } = false;
        [Category("5.Command")]
        public bool ReqAlarmReInput { get; set; } = false;


        [Category("4.Datas")]
        public List<BlockID2KV> BlockID2KVList { get; set; } = new List<BlockID2KV>();



    }
}