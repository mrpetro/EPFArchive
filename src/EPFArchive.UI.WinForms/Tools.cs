using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EPF.UI
{
    public static class Tools
    {
        public static void ChangeEnabled(Control control, bool enabled)
        {
            foreach (Control c in control.Controls)
            {
                c.Enabled = enabled;
            }
        }

    }
}
