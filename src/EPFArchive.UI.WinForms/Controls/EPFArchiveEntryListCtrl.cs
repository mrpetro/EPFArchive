using EPF.UI.ViewModel;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace EPF.UI.WinForms.Controls
{
    public partial class EPFArchiveEntryListCtrl : UserControl
    {
        #region Private Fields

        private DGVMultiSelectCheckBoxFunc _multiSelectCheckBoxFunc;

        private EPFArchiveViewModel _viewModel;

        #endregion Private Fields

        #region Public Constructors

        public EPFArchiveEntryListCtrl()
        {
            InitializeComponent();

            _multiSelectCheckBoxFunc = new DGVMultiSelectCheckBoxFunc(DGV);
        }

        #endregion Public Constructors

        #region Public Methods

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

            DGV.SelectionChanged += DGV_SelectionChanged;
            DGV.PreviewKeyDown += DGV_PreviewKeyDown;
        }

        #endregion Public Methods

        #region Private Methods

        private void DGV_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                _viewModel.TryRemoveSelectedEntries();
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

        #endregion Private Methods
    }
}