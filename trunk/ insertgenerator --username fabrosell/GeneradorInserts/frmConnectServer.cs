using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GeneradorInserts
{
    public partial class frmConnectServer : Form
    {
        public String FoobarMessage = "[I won't show it!]";

        public frmConnectServer()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}