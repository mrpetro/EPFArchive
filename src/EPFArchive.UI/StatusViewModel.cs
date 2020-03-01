namespace EPF.VM
{
    public class StatusViewModel : BaseViewModel
    {
        #region Private Fields

        private int _itemsSelected;
        private int _totalItems;

        #endregion Private Fields

        #region Public Constructors

        public StatusViewModel()
        {
            Progress = new ProgressViewModel();
            Log = new LogViewModel();

            Progress.Visible = false;
        }

        #endregion Public Constructors

        #region Public Properties

        public int ItemsSelected
        {
            get
            {
                return _itemsSelected;
            }

            set
            {
                if (_itemsSelected == value)
                    return;

                _itemsSelected = value;
                OnPropertyChanged(nameof(ItemsSelected));
            }
        }

        public LogViewModel Log { get; private set; }
        public ProgressViewModel Progress { get; private set; }

        public int TotalItems
        {
            get
            {
                return _totalItems;
            }

            internal set
            {
                if (_totalItems == value)
                    return;

                _totalItems = value;
                OnPropertyChanged(nameof(TotalItems));
            }
        }

        #endregion Public Properties
    }
}