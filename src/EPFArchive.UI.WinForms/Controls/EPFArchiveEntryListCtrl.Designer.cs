namespace EPF.UI.Controls
{
    partial class EPFArchiveEntryListCtrl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.DGV = new System.Windows.Forms.DataGridView();
            this.DGVColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DGVColumnStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DGVColumnSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DGVColumnPackedSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DGVColumnRatio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DGVColumnIsCompressed = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).BeginInit();
            this.SuspendLayout();
            // 
            // DGV
            // 
            this.DGV.AllowUserToAddRows = false;
            this.DGV.AllowUserToDeleteRows = false;
            this.DGV.AllowUserToOrderColumns = true;
            this.DGV.AllowUserToResizeRows = false;
            this.DGV.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.DGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DGVColumnName,
            this.DGVColumnStatus,
            this.DGVColumnSize,
            this.DGVColumnPackedSize,
            this.DGVColumnRatio,
            this.DGVColumnIsCompressed});
            this.DGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DGV.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.DGV.Location = new System.Drawing.Point(0, 0);
            this.DGV.Name = "DGV";
            this.DGV.RowHeadersVisible = false;
            this.DGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DGV.Size = new System.Drawing.Size(547, 301);
            this.DGV.TabIndex = 1;
            // 
            // DGVColumnName
            // 
            this.DGVColumnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DGVColumnName.HeaderText = "Name";
            this.DGVColumnName.Name = "DGVColumnName";
            this.DGVColumnName.ReadOnly = true;
            // 
            // DGVColumnStatus
            // 
            this.DGVColumnStatus.FillWeight = 90F;
            this.DGVColumnStatus.HeaderText = "Status";
            this.DGVColumnStatus.Name = "DGVColumnStatus";
            this.DGVColumnStatus.ReadOnly = true;
            this.DGVColumnStatus.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.DGVColumnStatus.Width = 90;
            // 
            // DGVColumnSize
            // 
            this.DGVColumnSize.FillWeight = 90F;
            this.DGVColumnSize.HeaderText = "Size";
            this.DGVColumnSize.Name = "DGVColumnSize";
            this.DGVColumnSize.ReadOnly = true;
            this.DGVColumnSize.Width = 90;
            // 
            // DGVColumnPackedSize
            // 
            this.DGVColumnPackedSize.FillWeight = 90F;
            this.DGVColumnPackedSize.HeaderText = "Packed Size";
            this.DGVColumnPackedSize.Name = "DGVColumnPackedSize";
            this.DGVColumnPackedSize.ReadOnly = true;
            this.DGVColumnPackedSize.Width = 90;
            // 
            // DGVColumnRatio
            // 
            this.DGVColumnRatio.FillWeight = 50F;
            this.DGVColumnRatio.HeaderText = "Ratio";
            this.DGVColumnRatio.Name = "DGVColumnRatio";
            this.DGVColumnRatio.ReadOnly = true;
            this.DGVColumnRatio.Width = 50;
            // 
            // DGVColumnIsCompressed
            // 
            this.DGVColumnIsCompressed.FillWeight = 90F;
            this.DGVColumnIsCompressed.HeaderText = "Is Compressed";
            this.DGVColumnIsCompressed.Name = "DGVColumnIsCompressed";
            this.DGVColumnIsCompressed.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.DGVColumnIsCompressed.Width = 90;
            // 
            // EPFArchiveEntryListCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.DGV);
            this.Name = "EPFArchiveEntryListCtrl";
            this.Size = new System.Drawing.Size(547, 301);
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView DGV;
        private System.Windows.Forms.DataGridViewTextBoxColumn DGVColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DGVColumnStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn DGVColumnSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn DGVColumnPackedSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn DGVColumnRatio;
        private System.Windows.Forms.DataGridViewCheckBoxColumn DGVColumnIsCompressed;
    }
}
