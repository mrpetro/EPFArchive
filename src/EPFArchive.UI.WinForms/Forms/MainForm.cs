using EPF.UI.ViewModel;
using System;
using System.Windows.Forms;

namespace EPF.UI.WinForms.Forms
{
    public partial class MainForm : Form
    {
        #region Private Fields

        private bool _locked;
        private EPFArchiveViewModel _viewModel;

        #endregion Private Fields

        #region Public Constructors

        public MainForm(EPFArchiveViewModel viewModel)
        {
            InitializeComponent();

            Initialize(viewModel);
        }

        #endregion Public Constructors

        #region Public Properties

        public new bool Enabled
        {
            get
            {
                return base.Enabled;
            }

            set
            {
                var invokeParent = Tools.GetInvokable(this);
                invokeParent.InvokeIfRequired(() => { base.Enabled = value; });
            }
        }

        #endregion Public Properties

        public bool Locked
        {
            get
            {
                return Locked;
            }

            set
            {
                this.InvokeIfRequired(() =>
                {
                    if (_locked == value)
                        return;

                    foreach (Control control in Controls)
                        control.Enabled = value;

                    _locked = value;
                });
            }
        }


        #region Public Methods

        public void Initialize(EPFArchiveViewModel viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            _viewModel = viewModel;

            DGV.AutoGenerateColumns = false;
            DGV.DataSource = _viewModel.Entries;
            DGVColumnName.DataPropertyName = "Name";
            DGVColumnStatus.DataPropertyName = "Status";
            DGVColumnSize.DataPropertyName = "Length";
            DGVColumnPackedSize.DataPropertyName = "CompressedLength";
            DGVColumnIsCompressed.DataPropertyName = "IsCompressed";

            DataBindings.Add("Locked", _viewModel, nameof(_viewModel.Locked), false, DataSourceUpdateMode.OnPropertyChanged);
            DataBindings.Add("Text", _viewModel, nameof(_viewModel.AppLabel), false, DataSourceUpdateMode.OnPropertyChanged);

            StatusStripTotalItemsNo.DataBindings.Add("Text", _viewModel.Status, nameof(_viewModel.Status.TotalItems), true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, "0", "0 items");
            StatusStripSelectedItemsNo.DataBindings.Add("Text", _viewModel.Status, nameof(_viewModel.Status.ItemsSelected), true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, "0", "0 items selected");
            StatusStripMessage.DataBindings.Add("Text", _viewModel.Status.Log, nameof(_viewModel.Status.Log.Message), false, DataSourceUpdateMode.OnPropertyChanged);
            StatusStripMessage.DataBindings.Add("ForeColor", _viewModel.Status.Log, nameof(_viewModel.Status.Log.Color), false, DataSourceUpdateMode.OnPropertyChanged);
            StatusStripProgressBar.DataBindings.Add("Value", _viewModel.Status.Progress, nameof(_viewModel.Status.Progress.Value), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            StatusStripProgressBar.DataBindings.Add("Visible", _viewModel.Status.Progress, nameof(_viewModel.Status.Progress.Visible), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);

            MenuItemDeselectAll.DataBindings.Add("Enabled", _viewModel, nameof(_viewModel.IsArchiveOpened), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            MenuItemSelectAll.DataBindings.Add("Enabled", _viewModel, nameof(_viewModel.IsArchiveOpened), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            MenuItemInvertSelection.DataBindings.Add("Enabled", _viewModel, nameof(_viewModel.IsArchiveOpened), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);

            MenuItemFileClose.DataBindings.Add("Enabled", _viewModel, nameof(_viewModel.IsArchiveOpened), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            MenuItemExtractAll.DataBindings.Add("Enabled", _viewModel, nameof(_viewModel.IsArchiveOpened), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            MenuItemExtractSelection.DataBindings.Add("Enabled", _viewModel, nameof(_viewModel.IsArchiveOpened), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);

            MenuItemFileSave.DataBindings.Add("Enabled", _viewModel, nameof(_viewModel.IsArchiveSaveAllowed), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            MenuItemFileSaveAs.DataBindings.Add("Enabled", _viewModel, nameof(_viewModel.IsArchiveSaveAllowed), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
        }

        #endregion Public Methods

        #region Private Methods

        private void DGV_SelectionChanged(object sender, EventArgs e)
        {
            int itemsSelected = 0;

            foreach (DataGridViewRow row in DGV.Rows)
            {
                ((EPFArchiveItemViewModel)row.DataBoundItem).IsSelected = row.Selected;

                if (row.Selected)
                    itemsSelected++;
            }

            _viewModel.Status.ItemsSelected = itemsSelected;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _viewModel.TryClose();
        }

        private void MenuItemDeselectAll_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in DGV.Rows)
                ((EPFArchiveItemViewModel)row.DataBoundItem).IsSelected = row.Selected = false;
        }

        private void MenuItemExit_Click(object sender, EventArgs e)
        {
            _viewModel.TryClose();
        }

        private void MenuItemExtractAll_Click(object sender, EventArgs e)
        {
            using (var folderBrowser = new FolderBrowserDialog())
            {
                var result = folderBrowser.ShowDialog();

                if (result == DialogResult.OK)
                {
                    try
                    {
                        Tools.ChangeEnabled(this, false);
                        _viewModel.ExtractAll(folderBrowser.SelectedPath);
                    }
                    finally
                    {
                        Tools.ChangeEnabled(this, true);
                    }
                }
            }
        }

        private void MenuItemExtractSelection_Click(object sender, EventArgs e)
        {
            using (var folderBrowser = new FolderBrowserDialog())
            {
                var result = folderBrowser.ShowDialog();

                if (result == DialogResult.OK)
                {
                    try
                    {
                        Tools.ChangeEnabled(this, false);
                        _viewModel.ExtractSelected(folderBrowser.SelectedPath);
                    }
                    finally
                    {
                        Tools.ChangeEnabled(this, true);
                    }
                }
            }
        }

        private void MenuItemFileClose_Click(object sender, EventArgs e)
        {
            _viewModel.TryClose();
        }

        private void MenuItemInvertSelection_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in DGV.Rows)
            {
                row.Selected = !row.Selected;
                ((EPFArchiveItemViewModel)row.DataBoundItem).IsSelected = row.Selected;
            }
        }

        private void MenuItemSelectAll_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in DGV.Rows)
                ((EPFArchiveItemViewModel)row.DataBoundItem).IsSelected = row.Selected = true;
        }

        #endregion Private Methods

        private void MenuItemFileOpen_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Open EPF archives in Read/Write mode is not implemented yet.");
            _viewModel.TryOpenArchive();
        }



        private void MenuItemFileOpenReadOnly_Click(object sender, EventArgs e)
        {
            _viewModel.TryOpenArchiveReadOnly();
        }
    }
}