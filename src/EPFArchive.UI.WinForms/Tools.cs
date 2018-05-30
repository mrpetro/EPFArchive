using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EPF.UI.WinForms
{
    public static class Tools
    {
        /// <summary>
        /// This method checks control object if it implements ISynchronizeInvoke,
        /// If not then checks it's parent control. This is repeated until any
        /// ISynchronizeInvoke is found or none.
        /// </summary>
        /// <param name="control">Control to check</param>
        /// <returns>Closest available ISynchronizeInvoke inteface object</returns>
        public static ISynchronizeInvoke GetInvokable(Control control)
        {
            var checkedControl = control;

            while (checkedControl != null)
            {
                if (checkedControl is ISynchronizeInvoke)
                    return (ISynchronizeInvoke)checkedControl;

                checkedControl = checkedControl.Parent;
            }

            return null;
        }

        public static void ChangeEnabled(Control control, bool enabled)
        {
            foreach (Control c in control.Controls)
            {
                c.Enabled = enabled;
            }
        }

    }
}
