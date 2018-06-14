using EPF.UI.ViewModel;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace EPF.UI.WinForms.Controls
{
    public partial class EPFArchiveEntryListCtrl : UserControl
    {

        private CheckBox headerCheckBox;

        #region Private Fields

        private EPFArchiveViewModel _viewModel;

        #endregion Private Fields

        #region Public Constructors

        public EPFArchiveEntryListCtrl()
        {
            InitializeComponent();

            headerCheckBox = new CheckBox();

            var rectangle = this.DGV.GetCellDisplayRectangle(DGVColumnIsCompressed.Index, -1, true);
            //Place the Header CheckBox in the Location of the Header Cell.
            headerCheckBox.Location = new System.Drawing.Point(rectangle.Right - headerCheckBox.Width, rectangle.Bottom );
            headerCheckBox.BackColor = System.Drawing.Color.White;
            headerCheckBox.Size = new System.Drawing.Size(13, 13);

            //Assign Click event to the Header CheckBox.
            headerCheckBox.Click += new EventHandler(HeaderCheckBox_Clicked);
            DGV.Controls.Add(headerCheckBox);
        }

        #endregion Public Constructors

        #region Public Methods

        private void HeaderCheckBox_Clicked(object sender, EventArgs e)
        {
            var cell = headerCheckBox.Tag as DataGridViewCheckBoxCell;

            if (cell == null)
                return;

            if ((bool)cell.Value == true)
                cell.Value = false;
            else
                cell.Value = true;

            if (DGV.SelectedCells.Contains(cell))
            {
                foreach (DataGridViewCell selectedCell in DGV.SelectedCells)
                {
                    if (selectedCell.ColumnIndex != cell.ColumnIndex)
                        continue;

                    if (selectedCell.RowIndex == cell.RowIndex)
                        continue;

                    selectedCell.Value = cell.Value;
                }
            }
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

        private void DGV_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (DGV.SelectedRows.Count > 1)
            {
            }
        }

        private void DGV_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var cell = DGV.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewCheckBoxCell;

            if (cell == null)
                return;

            if (_viewModel.IsReadOnly)
                return;

            var rectangle = this.DGV.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
            headerCheckBox.Location = new System.Drawing.Point(rectangle.Left + (rectangle.Width - headerCheckBox.Width) / 2, rectangle.Top + (rectangle.Height - headerCheckBox.Height) / 2);
            headerCheckBox.Checked = (bool)cell.Value;
            headerCheckBox.Tag = cell;
        }
    }
}