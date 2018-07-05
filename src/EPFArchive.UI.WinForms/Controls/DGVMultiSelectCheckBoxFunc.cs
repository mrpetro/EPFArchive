using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EPF.UI.WinForms.Controls
{
    /// <summary>
    /// This class adds multiselection of checkboxes (in DataGridViewCheckBox columns) function to DataGridView.
    /// </summary>
    public class DGVMultiSelectCheckBoxFunc
    {
        private DataGridView _dgv;
        private CheckBox _checkBox;

        public DGVMultiSelectCheckBoxFunc(DataGridView dgv)
        {
            if (dgv == null)
                throw new ArgumentNullException(nameof(dgv));

            _dgv = dgv;

            _checkBox = new CheckBox();
            _checkBox.Visible = false;
            _checkBox.Size = new System.Drawing.Size(13, 13);
            _checkBox.BackColor = System.Drawing.Color.White;
            _checkBox.Click += _checkBox_Clicked;


            _dgv.Controls.Add(_checkBox);
            _dgv.CellMouseEnter += _dgv_CellMouseEnter;
        }

        private void _checkBox_Clicked(object sender, EventArgs e)
        {
            var cell = _checkBox.Tag as DataGridViewCheckBoxCell;

            if (cell == null)
                return;

            if (cell.ReadOnly)
            {
                _checkBox.Checked = !_checkBox.Checked;
                return;
            }

            if ((bool)cell.Value == true)
                cell.Value = false;
            else
                cell.Value = true;

            if (_dgv.SelectedCells.Contains(cell))
            {
                foreach (DataGridViewCell selectedCell in _dgv.SelectedCells)
                {
                    if (selectedCell.ColumnIndex != cell.ColumnIndex)
                        continue;

                    if (selectedCell.RowIndex == cell.RowIndex)
                        continue;

                    selectedCell.Value = cell.Value;
                }
            }
        }

        private void _dgv_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            _checkBox.Visible = false;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var cell = _dgv.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewCheckBoxCell;

            if (cell == null)
                return;

            _checkBox.Visible = true;

            var rectangle = _dgv.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
            _checkBox.Location = new System.Drawing.Point(rectangle.Left + (rectangle.Width - _checkBox.Width) / 2, rectangle.Top + (rectangle.Height - _checkBox.Height) / 2);
            _checkBox.Checked = (bool)cell.Value;
            _checkBox.Tag = cell;
        }

    }
}
