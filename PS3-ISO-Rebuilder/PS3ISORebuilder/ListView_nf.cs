using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PS3ISORebuilder
{
    public class ListView_nf : ListView
    {
        public ListView_nf()
        {
            DoubleBuffered = true;
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            ResumeLayout(performLayout: false);
        }
    }
}
