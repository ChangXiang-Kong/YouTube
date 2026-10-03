using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using BatchProcess3.Data;
using BatchProcess3.EntityFramework;
using BatchProcess3.Tools.Services;
using BatchProcess3.ViewModels.Process;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BatchProcess3.ViewModels.MainMenus;

public partial class ProcessPageViewModel : PageViewModel
{
    public ProcessPageViewModel(
        MainViewModel mainViewModel, 
        DialogService dialogService, 
        DatabaseService databaseService) : base(ApplicationPageName.Process)
    {
        _mainViewModel = mainViewModel;
        _dialogService = dialogService;
        _databaseService = databaseService;
        
        FetchProcesses();
    }
    
    // Design time only
    public ProcessPageViewModel() : this(new MainViewModel(), 
        new DialogService(new Func<TopLevel?>(() => null)),
        new DatabaseService(new AppDbContext()))
    {
        if (!Avalonia.Controls.Design.IsDesignMode) 
            throw new InvalidOperationException("Parameterless constructor is only for design time use");
    }
    
    #region Members

    private readonly MainViewModel  _mainViewModel;
    private readonly DialogService  _dialogService;
    private readonly DatabaseService  _databaseService;

    public string? Test { get; set; } = "Test Process";

    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(ProcessListHasItems))]
    private ObservableCollection<ProcessViewModel> _processList = [];

    public bool ProcessListHasItems => ProcessList.Any();

    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(SelectedProcessListItem))]
    private string _selectedProcessListItemId = "";
    
    public ProcessViewModel? SelectedProcessListItem => ProcessList.FirstOrDefault(f => f.Id == SelectedProcessListItemId);
    
    #endregion Members

    private void FetchProcesses()
    {
        var processes = _databaseService.GetProcesses();
        
        ProcessList = new ObservableCollection<ProcessViewModel>(processes
            .OrderBy(x => x.JobName)
            .Select(x => x.ToViewModel()));
        
        // Update ProcessListHasItems when collection changes
        ProcessList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(ProcessListHasItems));

        if (ProcessList.Count <= 0)
            return;
        
        // Select first item
        SelectedProcessListItemId = ProcessList.First().Id;

        // Store last fetched database save states
        foreach (var item in ProcessList)
            item.SetSaveState();
    }

    [RelayCommand]
    private void AddNewProcessItem()
    {
        // Create a new item
        var newItem = new ProcessViewModel()
        {
            Id = Guid.CreateVersion7().ToString(),
            JobName = "New Process",
            Description = "New Process",
            IsNewItem = true,
        };
        
        // Add to the process list
        ProcessList.Add(newItem);
        
        // TODO: 未知 Bug，赋值后 SelectedProcessListItemId 仍然为 null
        // Select item
        SelectedProcessListItemId = newItem.Id;
    }

    [RelayCommand]
    private Task SaveProcessItemAsync()
    {
        // Ignore if no selection
        if (SelectedProcessListItem == null)
            return Task.CompletedTask;
        
        // If the selected item is new
        if (SelectedProcessListItem.IsNewItem)
            _databaseService.AddProcess(SelectedProcessListItem.ToEntity());
        else
            _databaseService.UpdateProcess(SelectedProcessListItem.ToEntity());

        // Flag new item as not new
        SelectedProcessListItem.IsNewItem = false;
        // 保存状态以隐藏 Save 按钮
        SelectedProcessListItem.SetSaveState();
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task CancelProcessItemAsync()
    {
        // Ignore if nothing is selected
        if (SelectedProcessListItem == null)
            return;

        // If the selected item is new, delete it
        // Otherwise, restore from save state
        if (SelectedProcessListItem.IsNewItem)
            await DeleteProcessItemFromUIAsync(SelectedProcessListItem.Id, false);
        else
            SelectedProcessListItem.RestoreState();
    }

    [RelayCommand]
    private async Task DeleteProcessItem(string id)
    {
        if (ProcessList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;

        // If user selected to remove from UI (via confirm dialog)
        if (await DeleteProcessItemFromUIAsync(id))
            // Delete from database
            _databaseService.DeleteProcess(id);
    }

    // ReSharper disable once InconsistentNaming
    private async Task<bool> DeleteProcessItemFromUIAsync(string id, bool popupDialog = true)
    {
        var index = ProcessList.IndexOf(ProcessList.First(x => x.Id == id));
        if (index == -1)
            return false;
        
        if (popupDialog)
        {
            var confirmDialogViewModel = new ConfirmDialogViewModel
            {
                // 图标方式一：
                GeometryIcon = GeometryIcon.Warning,
                // 图标方式二：
                // IconMessage = "Warning";
                // IconForeground = "#fc8800";
                // IconGeometry = StreamGeometry.Parse("M943.644188 827.215696l-351.176649-608.204749c-42.945473-74.36249-113.147387-74.36249-156.092861 0l-351.176649 608.204749c-42.946498 74.431167-7.811716 135.14955 78.012605 135.14955l702.420949 0C951.455904 962.36422 986.555836 901.645838 943.644188 827.215696zM466.187532 391.579035c12.621133-13.644108 28.66175-20.466675 48.233578-20.466675 19.580028 0 35.612444 6.75389 48.241778 20.194018 12.544256 13.473954 18.820484 30.325365 18.820484 50.587035 0 17.430551-26.19759 145.621205-34.929778 238.882082l-63.105666 0c-7.666162-93.259852-36.090106-221.450507-36.090106-238.882082C447.358847 421.938226 453.643275 405.155491 466.187532 391.579035zM561.76804 835.026386c-13.268949 12.928641-29.062535 19.375023-47.345906 19.375023-18.275171 0-34.076957-6.447407-47.346931-19.375023-13.235123-12.89379-19.818859-28.517221-19.818859-46.869269 0-18.249546 6.583736-34.043131 19.818859-47.278254 13.268949-13.235123 29.07176-19.852685 47.346931-19.852685 18.283371 0 34.076957 6.617562 47.345906 19.852685 13.235123 13.235123 19.827059 29.028709 19.827059 47.278254C581.595099 806.51019 575.003163 822.132597 561.76804 835.026386z");
                Title = $"Delete Process Item?",
                Message = $"Are you sure you want to delete {ProcessList[index].JobName}?",
                DialogWidth = 500,
                // OnConfirm = async (vm) =>
                // {
                //     await Task.Delay(2000);
                //     
                //     vm.ProgressText = "This is taking a while...";
                //     vm.ProgressValue = 50;
                //     
                //     await Task.Delay(1000);
                //
                //     vm.StatusText = "Oh no, something went wrong...";
                //     
                //     return true;
                // },
            };
            
            // Wait for click button
            await _dialogService.ShowDialogAsync(_mainViewModel, confirmDialogViewModel);
            
            // Ignore if we clicked cancel
            if (!confirmDialogViewModel.IsConfirmed)
                return false;
        }
        
        // Remove item
        ProcessList.RemoveAt(index);

        // Select the item before the deleted one
        if (index > 0)
            index--;
        if (ProcessList.Count > 0)
            SelectedProcessListItemId = ProcessList[index].Id;

        return true;
    }

    [RelayCommand]
    private void DeleteSelectedProcessActionItem(int sortOrder)
    {
        if (SelectedProcessListItem == null)
            // TODO: Throw/Warn?
            return;

        var action = SelectedProcessListItem.Actions.FirstOrDefault(x => x.SortOrder == sortOrder);
        if (action != null)
            SelectedProcessListItem.Actions.Remove(action);
    }
    
}
