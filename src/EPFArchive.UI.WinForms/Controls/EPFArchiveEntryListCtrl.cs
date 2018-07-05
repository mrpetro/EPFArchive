using EPF.UI.ViewModel;
using System;
using System.ComponentModel;
using System.Globalization;
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
            _viewModel.PropertyChanged += _viewModel_PropertyChanged;

            DGV.AutoGenerateColumns = false;
            DGV.DataSource = _viewModel.Entries;
            DGVColumnName.DataPropertyName = "Name";
            DGVColumnStatus.DataPropertyName = "Status";
            DGVColumnSize.DataPropertyName = "Length";
            DGVColumnPackedSize.DataPropertyName = "CompressedLength";
            DGVColumnRatio.DataPropertyName = "CompressionRatio";
            DGVColumnIsCompressed.DataPropertyName = "IsCompressed";
            DGV.SelectionChanged += DGV_SelectionChanged;
            DGV.PreviewKeyDown += DGV_PreviewKeyDown;
            DGV.CellFormatting += DGV_CellFormatting;
        }

        private void _viewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_viewModel.IsReadOnly):
                    DGVColumnIsCompressed.ReadOnly = _viewModel.IsReadOnly;
                    break;
                default:
                    break;
            }
        }

        private void DGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (e.ColumnIndex == DGVColumnRatio.Index)
            {
                var value = 100.0f * (float)e.Value;

                if (value > 100.0f)
                    e.CellStyle.BackColor = System.Drawing.Color.Orange;

                e.Value = string.Format(CultureInfo.InvariantCulture, "{0:F1}%", value);

            }
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