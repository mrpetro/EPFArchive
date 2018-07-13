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
            this.MenuItemFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemFileOpenReadOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemFileOpen = new EPF.UI.WinForms.Controls.ToolStripMenuItemEx();
            this.MenuItemFileSave = new EPF.UI.WinForms.Controls.ToolStripMenuItemEx();
            this.MenuItemFileSaveAs = new EPF.UI.WinForms.Controls.ToolStripMenuItemEx();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
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
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuItemArchiveProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.StatusStripTotalItemsNo = new EPF.UI.WinForms.Controls.ToolStripStatusLabelEx();
            this.StatusStripSelectedItemsNo = new EPF.UI.WinForms.Controls.ToolStripStatusLabelEx();
            this.StatusStripMessage = new EPF.UI.WinForms.Controls.ToolStripStatusLabelEx();
            this.StatusStripProgressBar = new EPF.UI.WinForms.Controls.ToolStripProgressBarEx();
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.ToolStripAdd = new Controls.ToolStripButtonEx();
            this.ToolStripRemove = new Controls.ToolStripButtonEx();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripExtractSelection = new Controls.ToolStripButtonEx();
            this.ToolStripExtractAll = new Controls.ToolStripButtonEx();
            this.EntryList = new EPF.UI.WinForms.Controls.EPFArchiveEntryListCtrl();
            this.MenuMain.SuspendLayout();
            this.StatusStrip.SuspendLayout();
            this.ToolStrip.SuspendLayout();
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
            this.MenuItemFileNew,
            this.MenuItemFileOpenReadOnly,
            this.MenuItemFileOpen,
            this.MenuItemFileSave,
            this.MenuItemFileSaveAs,
            this.toolStripSeparator4,
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
            // MenuItemFileNew
            // 
            this.MenuItemFileNew.Name = "MenuItemFileNew";
            this.MenuItemFileNew.Size = new System.Drawing.Size(171, 22);
            this.MenuItemFileNew.Text = "New";
            this.MenuItemFileNew.Click += new System.EventHandler(this.MenuItemFileNew_Click);
            // 
            // MenuItemFileOpenReadOnly
            // 
            this.MenuItemFileOpenReadOnly.Name = "MenuItemFileOpenReadOnly";
            this.MenuItemFileOpenReadOnly.Size = new System.Drawing.Size(171, 22);
            this.MenuItemFileOpenReadOnly.Text = "Open Read-Only...";
            this.MenuItemFileOpenReadOnly.Click += new System.EventHandler(this.MenuItemArchiveOpenReadOnly_Click);
            // 
            // MenuItemFileOpen
            // 
            this.MenuItemFileOpen.Name = "MenuItemFileOpen";
            this.MenuItemFileOpen.Size = new System.Drawing.Size(171, 22);
            this.MenuItemFileOpen.Text = "Open...";
            this.MenuItemFileOpen.Click += new System.EventHandler(this.MenuItemArchiveOpen_Click);
            // 
            // MenuItemFileSave
            // 
            this.MenuItemFileSave.Name = "MenuItemFileSave";
            this.MenuItemFileSave.Size = new System.Drawing.Size(171, 22);
            this.MenuItemFileSave.Text = "Save";
            this.MenuItemFileSave.Click += new System.EventHandler(this.MenuItemArchiveSave_Click);
            // 
            // MenuItemFileSaveAs
            // 
            this.MenuItemFileSaveAs.Name = "MenuItemFileSaveAs";
            this.MenuItemFileSaveAs.Size = new System.Drawing.Size(171, 22);
            this.MenuItemFileSaveAs.Text = "Save As...";
            this.MenuItemFileSaveAs.Click += new System.EventHandler(this.MenuItemArchiveSaveAs_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(168, 6);
            // 
            // MenuItemFileClose
            // 
            this.MenuItemFileClose.Name = "MenuItemFileClose";
            this.MenuItemFileClose.Size = new System.Drawing.Size(171, 22);
            this.MenuItemFileClose.Text = "Close";
            this.MenuItemFileClose.Click += new System.EventHandler(this.MenuItemArchiveClose_Click);
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
            this.MenuItemInvertSelection,
            this.toolStripSeparator5,
            this.MenuItemArchiveProperties});
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
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(152, 6);
            // 
            // MenuItemArchiveProperties
            // 
            this.MenuItemArchiveProperties.Name = "MenuItemArchiveProperties";
            this.MenuItemArchiveProperties.Size = new System.Drawing.Size(155, 22);
            this.MenuItemArchiveProperties.Text = "Properties...";
            this.MenuItemArchiveProperties.Click += new System.EventHandler(this.MenuItemArchiveProperties_Click);
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
            this.StatusStripTotalItemsNo.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.StatusStripTotalItemsNo.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.StatusStripTotalItemsNo.Name = "StatusStripTotalItemsNo";
            this.StatusStripTotalItemsNo.Size = new System.Drawing.Size(80, 17);
            // 
            // StatusStripSelectedItemsNo
            // 
            this.StatusStripSelectedItemsNo.AutoSize = false;
            this.StatusStripSelectedItemsNo.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.StatusStripSelectedItemsNo.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.StatusStripSelectedItemsNo.Name = "StatusStripSelectedItemsNo";
            this.StatusStripSelectedItemsNo.Size = new System.Drawing.Size(140, 17);
            // 
            // StatusStripMessage
            // 
            this.StatusStripMessage.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.StatusStripMessage.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
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
            // ToolStrip
            // 
            this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripAdd,
            this.ToolStripRemove,
            this.toolStripSeparator3,
            this.ToolStripExtractSelection,
            this.ToolStripExtractAll});
            this.ToolStrip.Location = new System.Drawing.Point(0, 24);
            this.ToolStrip.Name = "ToolStrip";
            this.ToolStrip.Size = new System.Drawing.Size(624, 25);
            this.ToolStrip.TabIndex = 3;
            this.ToolStrip.Text = "toolStrip1";
            // 
            // ToolStripAdd
            // 
            this.ToolStripAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripAdd.Image = global::EPF.UI.WinForms.Properties.Resources.Add;
            this.ToolStripAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripAdd.Name = "ToolStripAdd";
            this.ToolStripAdd.Size = new System.Drawing.Size(23, 22);
            this.ToolStripAdd.Text = "Add...";
            this.ToolStripAdd.Click += new System.EventHandler(this.ToolStripAdd_Click);
            // 
            // ToolStripRemove
            // 
            this.ToolStripRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripRemove.Image = global::EPF.UI.WinForms.Properties.Resources.Remove;
            this.ToolStripRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripRemove.Name = "ToolStripRemove";
            this.ToolStripRemove.Size = new System.Drawing.Size(23, 22);
            this.ToolStripRemove.Text = "Remove";
            this.ToolStripRemove.Click += new System.EventHandler(this.ToolStripRemove_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // ToolStripExtractSelection
            // 
            this.ToolStripExtractSelection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripExtractSelection.Image = global::EPF.UI.WinForms.Properties.Resources.ExtractSelection;
            this.ToolStripExtractSelection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripExtractSelection.Name = "ToolStripExtractSelection";
            this.ToolStripExtractSelection.Size = new System.Drawing.Size(23, 22);
            this.ToolStripExtractSelection.Text = "Extract Selection...";
            this.ToolStripExtractSelection.Click += new System.EventHandler(this.ToolStripExtractSelection_Click);
            // 
            // ToolStripExtractAll
            // 
            this.ToolStripExtractAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripExtractAll.Image = global::EPF.UI.WinForms.Properties.Resources.ExtractAll;
            this.ToolStripExtractAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripExtractAll.Name = "ToolStripExtractAll";
            this.ToolStripExtractAll.Size = new System.Drawing.Size(23, 22);
            this.ToolStripExtractAll.Text = "Extract All...";
            this.ToolStripExtractAll.Click += new System.EventHandler(this.ToolStripExtractAll_Click);
            // 
            // EntryList
            // 
            this.EntryList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EntryList.Location = new System.Drawing.Point(0, 49);
            this.EntryList.Name = "EntryList";
            this.EntryList.Size = new System.Drawing.Size(624, 370);
            this.EntryList.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.EntryList);
            this.Controls.Add(this.ToolStrip);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.MenuMain);
            this.MainMenuStrip = this.MenuMain;
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "MainForm";
            this.Text = "EPF Archive";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.MenuMain.ResumeLayout(false);
            this.MenuMain.PerformLayout();
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.ToolStrip.ResumeLayout(false);
            this.ToolStrip.PerformLayout();
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
        private System.Windows.Forms.ToolStrip ToolStrip;
        private Controls.ToolStripButtonEx ToolStripAdd;
        private Controls.ToolStripButtonEx ToolStripRemove;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private Controls.ToolStripButtonEx ToolStripExtractSelection;
        private Controls.ToolStripButtonEx ToolStripExtractAll;
        private Controls.EPFArchiveEntryListCtrl EntryList;
        private System.Windows.Forms.ToolStripMenuItem MenuItemFileNew;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem MenuItemArchiveProperties;
    }
}

