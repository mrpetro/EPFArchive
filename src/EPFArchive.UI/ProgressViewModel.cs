using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPF.VM
{
    public class ProgressViewModel : BaseViewModel
    {
        private int _value;
        private bool _visible;

        public int Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (_value == value)
                    return;

                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public bool Visible
        {
            get
            {
                return _visible;
            }

            set
            {
                if (_visible == value)
                    return;

                _visible = value;
                OnPropertyChanged(nameof(Visible));
            }
        }

    }
}
