namespace EPF.UI.WinForms.Forms
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MenuMain = new System.Windows.Forms.MenuStrip();
            this.MenuItemFile = new EPF.UI.WinForms.Controls.ToolStripMenuItemEx();
            this.MenuItemFileOpenReadOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemFileOpen = new EPF.UI.WinForms.Controls.ToolStripMenuItemEx();
            this.MenuItemFileSave = new EPF.UI.WinForms.Controls.ToolStripMenuItemEx();
            this.MenuItemFileSaveAs = new EPF.UI.WinForms.Controls.ToolStripMenuItemEx();
            this.MenuItemFileClose = new EPF.UI.WinForms.Controls.ToolStripMenuItemEx();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuItemExtractAll = new EPF.UI.WinForms.Controls.ToolStripMenuItemEx();
            this.MenuItemExtractSelection = new EPF.UI.WinForms.Controls.ToolStripMenuItemEx();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuItemExit = new EPF.UI.WinForms.Controls.ToolStripMenuItemEx();
            this.MenuItemEdit = new EPF.UI.WinForms.Controls.ToolStripMenuItemEx();
            this.MenuItemSelectAll = new EPF.UI.WinForms.Controls.ToolStripMenuItemEx();
            this.MenuItemDeselectAll = new EPF.UI.WinForms.Controls.ToolStripMenuItemEx();
            this.MenuItemInvertSelection = new EPF.UI.WinForms.Controls.ToolStripMenuItemEx();
            this.DGV = new System.Windows.Forms.DataGridView();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.StatusStripTotalItemsNo = new EPF.UI.WinForms.Controls.ToolStripStatusLabelEx();
            this.StatusStripSelectedItemsNo = new EPF.UI.WinForms.Controls.ToolStripStatusLabelEx();
            this.StatusStripMessage = new EPF.UI.WinForms.Controls.ToolStripStatusLabelEx();
            this.StatusStripProgressBar = new EPF.UI.WinForms.Controls.ToolStripProgressBarEx();
            this.DGVColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DGVColumnStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DGVColumnSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DGVColumnPackedSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DGVColumnIsCompressed = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.MenuMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).BeginInit();
            this.StatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuMain
            // 
            this.MenuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemFile,
            this.MenuItemEdit});
            this.MenuMain.Location = new System.Drawing.Point(0, 0);
            this.MenuMain.Name = "MenuMain";
            this.MenuMain.Size = new System.Drawing.Size(624, 24);
            this.MenuMain.TabIndex = 0;
            this.MenuMain.Text = "menuStrip1";
            // 
            // MenuItemFile
            // 
            this.MenuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemFileOpenReadOnly,
            this.MenuItemFileOpen,
            this.MenuItemFileSave,
            this.MenuItemFileSaveAs,
            this.MenuItemFileClose,
            this.toolStripSeparator1,
            this.MenuItemExtractAll,
            this.MenuItemExtractSelection,
            this.toolStripSeparator2,
            this.MenuItemExit});
            this.MenuItemFile.Name = "MenuItemFile";
            this.MenuItemFile.Size = new System.Drawing.Size(37, 20);
            this.MenuItemFile.Text = "File";
            // 
            // MenuItemFileOpenReadOnly
            // 
            this.MenuItemFileOpenReadOnly.Name = "MenuItemFileOpenReadOnly";
            this.MenuItemFileOpenReadOnly.Size = new System.Drawing.Size(171, 22);
            this.MenuItemFileOpenReadOnly.Text = "Open Read-Only...";
            this.MenuItemFileOpenReadOnly.Click += new System.EventHandler(this.MenuItemFileOpenReadOnly_Click);
            // 
            // MenuItemFileOpen
            // 
            this.MenuItemFileOpen.Name = "MenuItemFileOpen";
            this.MenuItemFileOpen.Size = new System.Drawing.Size(171, 22);
            this.MenuItemFileOpen.Text = "Open...";
            this.MenuItemFileOpen.Click += new System.EventHandler(this.MenuItemFileOpen_Click);
            // 
            // MenuItemFileSave
            // 
            this.MenuItemFileSave.Name = "MenuItemFileSave";
            this.MenuItemFileSave.Size = new System.Drawing.Size(171, 22);
            this.MenuItemFileSave.Text = "Save";
            // 
            // MenuItemFileSaveAs
            // 
            this.MenuItemFileSaveAs.Name = "MenuItemFileSaveAs";
            this.MenuItemFileSaveAs.Size = new System.Drawing.Size(171, 22);
            this.MenuItemFileSaveAs.Text = "Save As...";
            // 
            // MenuItemFileClose
            // 
            this.MenuItemFileClose.Name = "MenuItemFileClose";
            this.MenuItemFileClose.Size = new System.Drawing.Size(171, 22);
            this.MenuItemFileClose.Text = "Close";
            this.MenuItemFileClose.Click += new System.EventHandler(this.MenuItemFileClose_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(168, 6);
            // 
            // MenuItemExtractAll
            // 
            this.MenuItemExtractAll.Name = "MenuItemExtractAll";
            this.MenuItemExtractAll.Size = new System.Drawing.Size(171, 22);
            this.MenuItemExtractAll.Text = "Extract All...";
            this.MenuItemExtractAll.Click += new System.EventHandler(this.MenuItemExtractAll_Click);
            // 
            // MenuItemExtractSelection
            // 
            this.MenuItemExtractSelection.Name = "MenuItemExtractSelection";
            this.MenuItemExtractSelection.Size = new System.Drawing.Size(171, 22);
            this.MenuItemExtractSelection.Text = "Extract Selected...";
            this.MenuItemExtractSelection.Click += new System.EventHandler(this.MenuItemExtractSelection_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(168, 6);
            // 
            // MenuItemExit
            // 
            this.MenuItemExit.Name = "MenuItemExit";
            this.MenuItemExit.Size = new System.Drawing.Size(171, 22);
            this.MenuItemExit.Text = "Exit";
            this.MenuItemExit.Click += new System.EventHandler(this.MenuItemExit_Click);
            // 
            // MenuItemEdit
            // 
            this.MenuItemEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemSelectAll,
            this.MenuItemDeselectAll,
            this.MenuItemInvertSelection});
            this.MenuItemEdit.Name = "MenuItemEdit";
            this.MenuItemEdit.Size = new System.Drawing.Size(39, 20);
            this.MenuItemEdit.Text = "Edit";
            // 
            // MenuItemSelectAll
            // 
            this.MenuItemSelectAll.Name = "MenuItemSelectAll";
            this.MenuItemSelectAll.Size = new System.Drawing.Size(155, 22);
            this.MenuItemSelectAll.Text = "Select All";
            this.MenuItemSelectAll.Click += new System.EventHandler(this.MenuItemSelectAll_Click);
            // 
            // MenuItemDeselectAll
            // 
            this.MenuItemDeselectAll.Name = "MenuItemDeselectAll";
            this.MenuItemDeselectAll.Size = new System.Drawing.Size(155, 22);
            this.MenuItemDeselectAll.Text = "Deselect All";
            this.MenuItemDeselectAll.Click += new System.EventHandler(this.MenuItemDeselectAll_Click);
            // 
            // MenuItemInvertSelection
            // 
            this.MenuItemInvertSelection.Name = "MenuItemInvertSelection";
            this.MenuItemInvertSelection.Size = new System.Drawing.Size(155, 22);
            this.MenuItemInvertSelection.Text = "Invert Selection";
            this.MenuItemInvertSelection.Click += new System.EventHandler(this.MenuItemInvertSelection_Click);
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
            this.DGVColumnIsCompressed});
            this.DGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DGV.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.DGV.Location = new System.Drawing.Point(0, 24);
            this.DGV.Name = "DGV";
            this.DGV.RowHeadersVisible = false;
            this.DGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DGV.Size = new System.Drawing.Size(624, 417);
            this.DGV.TabIndex = 1;
            this.DGV.SelectionChanged += new System.EventHandler(this.DGV_SelectionChanged);
            // 
            // StatusStrip
            // 
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusStripTotalItemsNo,
            this.StatusStripSelectedItemsNo,
            this.StatusStripMessage,
            this.StatusStripProgressBar});
            this.StatusStrip.Location = new System.Drawing.Point(0, 419);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(624, 22);
            this.StatusStrip.TabIndex = 2;
            this.StatusStrip.Text = "statusStrip1";
            // 
            // StatusStripTotalItemsNo
            // 
            this.StatusStripTotalItemsNo.AutoSize = false;
            this.StatusStripTotalItemsNo.Name = "StatusStripTotalItemsNo";
            this.StatusStripTotalItemsNo.Size = new System.Drawing.Size(80, 17);
            // 
            // StatusStripSelectedItemsNo
            // 
            this.StatusStripSelectedItemsNo.AutoSize = false;
            this.StatusStripSelectedItemsNo.Name = "StatusStripSelectedItemsNo";
            this.StatusStripSelectedItemsNo.Size = new System.Drawing.Size(140, 17);
            // 
            // StatusStripMessage
            // 
            this.StatusStripMessage.Name = "StatusStripMessage";
            this.StatusStripMessage.Size = new System.Drawing.Size(287, 17);
            this.StatusStripMessage.Spring = true;
            this.StatusStripMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // StatusStripProgressBar
            // 
            this.StatusStripProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.StatusStripProgressBar.AutoSize = false;
            this.StatusStripProgressBar.Name = "StatusStripProgressBar";
            this.StatusStripProgressBar.Size = new System.Drawing.Size(100, 16);
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
            this.DGVColumnStatus.HeaderText = "Status";
            this.DGVColumnStatus.Name = "DGVColumnStatus";
            this.DGVColumnStatus.ReadOnly = true;
            this.DGVColumnStatus.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // DGVColumnSize
            // 
            this.DGVColumnSize.HeaderText = "Size";
            this.DGVColumnSize.Name = "DGVColumnSize";
            this.DGVColumnSize.ReadOnly = true;
            // 
            // DGVColumnPackedSize
            // 
            this.DGVColumnPackedSize.HeaderText = "Packed Size";
            this.DGVColumnPackedSize.Name = "DGVColumnPackedSize";
            this.DGVColumnPackedSize.ReadOnly = true;
            // 
            // DGVColumnIsCompressed
            // 
            this.DGVColumnIsCompressed.HeaderText = "Is Compressed";
            this.DGVColumnIsCompressed.Name = "DGVColumnIsCompressed";
            this.DGVColumnIsCompressed.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.DGV);
            this.Controls.Add(this.MenuMain);
            this.MainMenuStrip = this.MenuMain;
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "MainForm";
            this.Text = "EPF Archive";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.MenuMain.ResumeLayout(false);
            this.MenuMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).EndInit();
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MenuMain;
        private Controls.ToolStripMenuItemEx MenuItemFile;
        private Controls.ToolStripMenuItemEx MenuItemFileOpen;
        private Controls.ToolStripMenuItemEx MenuItemFileSave;
        private Controls.ToolStripMenuItemEx MenuItemFileSaveAs;
        private Controls.ToolStripMenuItemEx MenuItemExit;
        private Controls.ToolStripMenuItemEx MenuItemEdit;
        private Controls.ToolStripMenuItemEx MenuItemSelectAll;
        private Controls.ToolStripMenuItemEx MenuItemDeselectAll;
        private Controls.ToolStripMenuItemEx MenuItemInvertSelection;
        private System.Windows.Forms.DataGridView DGV;
        private Controls.ToolStripMenuItemEx MenuItemFileClose;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private Controls.ToolStripMenuItemEx MenuItemExtractAll;
        private Controls.ToolStripMenuItemEx MenuItemExtractSelection;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.StatusStrip StatusStrip;
        private Controls.ToolStripStatusLabelEx StatusStripTotalItemsNo;
        private Controls.ToolStripStatusLabelEx StatusStripSelectedItemsNo;
        private Controls.ToolStripProgressBarEx StatusStripProgressBar;
        private Controls.ToolStripStatusLabelEx StatusStripMessage;
        private System.Windows.Forms.ToolStripMenuItem MenuItemFileOpenReadOnly;
        private System.Windows.Forms.DataGridViewTextBoxColumn DGVColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DGVColumnStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn DGVColumnSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn DGVColumnPackedSize;
        private System.Windows.Forms.DataGridViewCheckBoxColumn DGVColumnIsCompressed;
    }
}

