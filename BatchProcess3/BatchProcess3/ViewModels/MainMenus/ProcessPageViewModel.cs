using System.Collections.ObjectModel;
using System.Linq;
using BatchProcess3.Data;
using BatchProcess3.Tools.Services;
using BatchProcess3.ViewModels.Process;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.MainMenus
{
    public partial class ProcessPageViewModel : PageViewModel
    {
        public ProcessPageViewModel(DatabaseService databaseService) : base(ApplicationPageName.Process)
        {
            _databaseService = databaseService;
            
            FetchProcesses();
        }
        
        #region Members

        private readonly DatabaseService  _databaseService;

        public string? Test { get; set; } = "Test Process";

        [ObservableProperty] 
        private ObservableCollection<ProcessViewModel> _processList = [];

        public bool ProcessListHasItems => ProcessList.Any();

        [ObservableProperty] [NotifyPropertyChangedFor(nameof(SelectedProcessListItem))]
        private string _selectedProcessListItemId = "";

        public ProcessViewModel? SelectedProcessListItem => ProcessList.FirstOrDefault(f => f.Id == SelectedProcessListItemId);
        
        #endregion Members


        private void FetchProcesses()
        {
            
        }


    }
}
