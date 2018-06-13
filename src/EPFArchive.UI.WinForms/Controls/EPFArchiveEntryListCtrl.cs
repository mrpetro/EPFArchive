using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EPF.UI.ViewModel;

namespace EPF.UI.WinForms.Controls
{
    public partial class EPFArchiveEntryListCtrl : UserControl
    {
        private EPFArchiveViewModel _viewModel;

        public EPFArchiveEntryListCtrl()
        {
            InitializeComponent();
        }

        public void Initialize(EPFArchiveViewModel viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            _viewModel = viewModel;

            _viewModel.SelectedEntries.ListChanged += SelectedEntries_ListChanged;

            DGV.AutoGenerateColumns = false;
            DGV.DataSource = _viewModel.Entries;
            DGVColumnName.DataPropertyName = "Name";
            DGVColumnStatus.DataPropertyName = "Status";
            DGVColumnSize.DataPropertyName = "Length";
            DGVColumnPackedSize.DataPropertyName = "CompressedLength";
            DGVColumnIsCompressed.DataPropertyName = "IsCompressed";
        }

        private void SelectedEntries_ListChanged(object sender, ListChangedEventArgs e)
        {
            try
            {
                DGV.SelectionChanged -= DGV_SelectionChanged;

                foreach (DataGridViewRow row in DGV.Rows)
                {
                    var entry = ((EPFArchiveItemViewModel)row.DataBoundItem);
                    row.Selected = _viewModel.SelectedEntries.Contains(entry);
                }
            }
            finally
            {
                DGV.SelectionChanged += DGV_SelectionChanged;
            }
        }

        private void DGV_SelectionChanged(object sender, EventArgs e)
        {
            _viewModel.SelectedEntries.RaiseListChangedEvents = false;

            _viewModel.SelectedEntries.Clear();

            foreach (DataGridViewRow row in DGV.SelectedRows)
                _viewModel.SelectedEntries.Add((EPFArchiveItemViewModel)row.DataBoundItem);

            _viewModel.SelectedEntries.RaiseListChangedEvents = true;
            _viewModel.SelectedEntries.ResetBindings();
        }

        private void DGV_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                _viewModel.TryRemoveSelectedEntries();
        }
    }
}
