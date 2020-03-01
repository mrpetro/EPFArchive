using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EPFArchive.UI.Controls
{
    /// <summary>
    /// Interaction logic for EPFArchiveEntityListCtrl.xaml
    /// </summary>
    public partial class EPFArchiveEntityListCtrl : UserControl
    {
        public EPFArchiveEntityListCtrl()
        {
            InitializeComponent();
        }

        public void Initialize(object dataContext)
        {
            this.DataContext = dataContext;
        }
    }
}
