using EPF.VM;
using System;
using System.Windows.Forms;
using System.Windows.Threading;

namespace EPF.UI.Forms
{
    public partial class MainForm : Form
    {
        #region Private Fields

        private bool locked;
        private EPFArchiveViewModel vm;

        #endregion Private Fields

        #region Public Constructors

        public MainForm()
        {
            InitializeComponent();

            Initialize(new EPFArchiveViewModel(new DialogProvider(), Dispatcher.CurrentDispatcher));
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
                this.InvokeIfRequired(() => { base.Enabled = value; });
            }
        }

        public new string Text
        {
            get
            {
                return base.Text;
            }

            set
            {
                this.InvokeIfRequired(() => { base.Text = value; });
            }
        }

        public bool Locked
        {
            get
            {
                return locked;
            }

            set
            {
                this.InvokeIfRequired(() =>
                {
                    if (locked == value)
                        return;

                    MainMenuStrip.Enabled = !value;
                    EntryList.Enabled = !value;
                    locked = value;
                });
            }
        }

        #endregion Public Properties

        #region Public Methods

        public void Initialize(EPFArchiveViewModel viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            vm = viewModel;

            EntryList.Initialize(vm);

            DataBindings.Add("Locked", vm, nameof(vm.Locked), false, DataSourceUpdateMode.OnPropertyChanged);
            DataBindings.Add("Text", vm, nameof(vm.AppLabel), false, DataSourceUpdateMode.OnPropertyChanged);

            StatusStripTotalItemsNo.DataBindings.Add("Text", vm.Status, nameof(vm.Status.TotalItems), true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, "0", "0 items");
            StatusStripSelectedItemsNo.DataBindings.Add("Text", vm.Status, nameof(vm.Status.ItemsSelected), true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, "0", "0 items selected");
            StatusStripMessage.DataBindings.Add("Text", vm.Status.Log, nameof(vm.Status.Log.Message), false, DataSourceUpdateMode.OnPropertyChanged);
            StatusStripMessage.DataBindings.Add("ForeColor", vm.Status.Log, nameof(vm.Status.Log.Color), false, DataSourceUpdateMode.OnPropertyChanged);
            StatusStripProgressBar.DataBindings.Add("Value", vm.Status.Progress, nameof(vm.Status.Progress.Value), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            StatusStripProgressBar.DataBindings.Add("Visible", vm.Status.Progress, nameof(vm.Status.Progress.Visible), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);

            MenuItemDeselectAll.DataBindings.Add("Enabled", vm, nameof(vm.IsArchiveOpened), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            MenuItemSelectAll.DataBindings.Add("Enabled", vm, nameof(vm.IsArchiveOpened), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            MenuItemInvertSelection.DataBindings.Add("Enabled", vm, nameof(vm.IsArchiveOpened), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            MenuItemFileClose.DataBindings.Add("Enabled", vm, nameof(vm.IsArchiveOpened), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            MenuItemExtractAll.DataBindings.Add("Enabled", vm, nameof(vm.IsArchiveOpened), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            MenuItemExtractSelection.DataBindings.Add("Enabled", vm, nameof(vm.IsArchiveOpened), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);

            MenuItemFileSave.DataBindings.Add("Enabled", vm, nameof(vm.IsArchiveModified), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            MenuItemFileSaveAs.DataBindings.Add("Enabled", vm, nameof(vm.IsArchiveSaveAllowed), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);

            ToolStripAdd.DataBindings.Add("Enabled", vm, nameof(vm.IsArchiveSaveAllowed), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            ToolStripRemove.DataBindings.Add("Enabled", vm, nameof(vm.IsArchiveSaveAllowed), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);

            ToolStripExtractAll.DataBindings.Add("Enabled", vm, nameof(vm.IsArchiveOpened), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            ToolStripExtractSelection.DataBindings.Add("Enabled", vm, nameof(vm.IsArchiveOpened), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);

            MenuItemHiddenData.DataBindings.Add("Enabled", vm, nameof(vm.IsArchiveOpened), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            MenuItemHiddenDataAdd.DataBindings.Add("Enabled", vm, nameof(vm.IsArchiveSaveAllowed), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            MenuItemHiddenDataExtract.DataBindings.Add("Enabled", vm, nameof(vm.HasHiddenData), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
            MenuItemHiddenDataRemove.DataBindings.Add("Enabled", vm, nameof(vm.HasHiddenData), false, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
        }

        #endregion Public Methods

        #region Private Methods

        private void DGV_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                vm.TryRemoveSelectedEntries();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            vm.TryCloseArchive();
        }

        private void MenuItemArchiveClose_Click(object sender, EventArgs e)
        {
            vm.TryCloseArchive();
        }

        private void MenuItemArchiveOpen_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(() => { vm.CommandTryOpenArchive.Execute(null); }));
        }

        private void MenuItemArchiveOpenReadOnly_Click(object sender, EventArgs e)
        {
            vm.TryOpenArchiveReadOnly();
        }

        private void MenuItemArchiveSave_Click(object sender, EventArgs e)
        {
            vm.TrySaveArchive();
        }

        private void MenuItemArchiveSaveAs_Click(object sender, EventArgs e)
        {
            vm.TrySaveArchiveAs();
        }

        private void MenuItemDeselectAll_Click(object sender, EventArgs e)
        {
            vm.DeselectAll();
        }

        private void MenuItemExit_Click(object sender, EventArgs e)
        {
            vm.TryCloseArchive();
        }

        private void MenuItemExtractAll_Click(object sender, EventArgs e)
        {
            vm.TryExtractAll();
        }

        private void MenuItemExtractSelection_Click(object sender, EventArgs e)
        {
            vm.TryExtractSelection();
        }

        private void MenuItemFileNew_Click(object sender, EventArgs e)
        {
            vm.TryCreateArchive();
        }

        private void MenuItemInvertSelection_Click(object sender, EventArgs e)
        {
            vm.InvertSelection();
        }

        private void MenuItemSelectAll_Click(object sender, EventArgs e)
        {
            vm.SelectAll();
        }

        private void ToolStripAdd_Click(object sender, EventArgs e)
        {
            vm.TryAddEntries();
        }

        private void ToolStripExtractAll_Click(object sender, EventArgs e)
        {
            vm.TryExtractAll();
        }

        private void ToolStripExtractSelection_Click(object sender, EventArgs e)
        {
            vm.TryExtractSelection();
        }

        private void ToolStripRemove_Click(object sender, EventArgs e)
        {
            vm.TryRemoveSelectedEntries();
        }

        private void MenuItemHiddenDataExtract_Click(object sender, EventArgs e)
        {
            vm.TryExtractHiddenData();
        }

        private void MenuItemHiddenDataAdd_Click(object sender, EventArgs e)
        {
            vm.TryUpdateHiddenData();
        }

        private void MenuItemHiddenDataRemove_Click(object sender, EventArgs e)
        {
            vm.TryRemoveHiddenData();
        }

        #endregion Private Methods
    }
}