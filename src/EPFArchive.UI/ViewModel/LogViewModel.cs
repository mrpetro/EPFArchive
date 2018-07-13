using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPF.UI.ViewModel
{
    public class LogViewModel : BaseViewModel
    {
        private string _message;
        private Color _color;

        public Color Color
        {
            get { return _color; }
            private set{ SetProperty(ref _color, value); }
        }

        public string Message
        {
            get { return _message; }
            private set { SetProperty(ref _message, value); }
        }

        public void Error(string message)
        {
            Color = Color.Red;
            Message = message;
        }

        public void Warning(string message)
        {
            Color = Color.DarkOrange;
            Message = message;
        }

        public void Success(string message)
        {
            Color = Color.Green;
            Message = message;
        }

        public void Info(string message)
        {
            Color = Color.Black;
            Message = message;
        }
    }
}
