using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DXApplication1
{
    public partial class FrmAlert : DevExpress.XtraEditors.XtraForm
    {
        public FrmAlert( string message)
        {
            InitializeComponent();
            txtLog.Text = message;
        }

        public void SetMessage(string message)
        {
            txtLog.Text = message;
        }
    }
}