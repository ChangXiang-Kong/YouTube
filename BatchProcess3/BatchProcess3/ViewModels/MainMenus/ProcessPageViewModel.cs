using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using BatchProcess3.Data;
using BatchProcess3.EntityFramework;
using BatchProcess3.EntityFramework.Entities.Actions;
using BatchProcess3.Tools.Dialog;
using BatchProcess3.ViewModels.Actions;
using BatchProcess3.ViewModels.Process;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BatchProcess3.ViewModels.MainMenus;

public partial class ProcessPageViewModel(
    MainViewModel mainViewModel, 
    DialogService dialogService, 
    DatabaseService databaseService) : PageViewModel(ApplicationPageName.Process)
{
    // Design time only
    public ProcessPageViewModel() : this(new MainViewModel(), 
        new DialogService(new Func<TopLevel?>(() => null)),
        new DatabaseService(new AppDbContext()))
    {
        if (!Avalonia.Controls.Design.IsDesignMode) 
            throw new InvalidOperationException("Parameterless constructor is only for design time use");
    }
    
    #region Members

    public string? Test { get; set; } = "Test Process";

    [ObservableProperty] 
    private SelectableItemsListViewModel<ProcessViewModel>? _processList;

    [ObservableProperty] 
    private ObservableCollection<ProcessAvailableActionItemViewModel>? _availableActionsList;
    
    #endregion Members

    // 也可以使用 OnViewLoaded() 代替
    [RelayCommand]
    private void Initialize()
    {
        ProcessList = new SelectableItemsListViewModel<ProcessViewModel>(
            mainViewModel: mainViewModel,
            dialogService: dialogService,
            title: "Process",
            getList: () =>
            {
                var list = databaseService.GetProcessesList();
        
                return new ObservableCollection<ProcessViewModel>(list
                    .OrderBy(x => x.JobName)
                    .Select(x => x.ToViewModel()));
            },
            createItem: () => new ProcessViewModel()
            {
                // Id = Guid.CreateVersion7().ToString(),
                JobName = "New Process",
                Description = "New Process",
                IsNewItem = true,
            },
            deleteItem: (id) => databaseService.DeleteProcessItem(id),
            addItem: item => databaseService.AddProcessItem(item.ToEntity()),
            updateItem: item => databaseService.UpdateProcessItem(item.ToEntity())
        );
        
        ProcessList.FetchList();

        List<ProcessAvailableActionItemViewModel> ToAvailableActionItemViewModelList<T>(string category, List<T> list)
            where T : ActionEntity
        {
            var ret = new List<ProcessAvailableActionItemViewModel> {
                // Add header
                new ProcessAvailableActionItemViewModel() { Category = category }
            };

            // Add items
            ret.AddRange(list.Select(x => new ProcessAvailableActionItemViewModel()
            {
                ProcessActionViewModel = x.ToProcessActionViewModel(),
                Category = category,
            }));
            
            // Edit all the Id's  视频链接：https://www.youtube.com/watch?v=QxZ7v6OwMrE&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO&index=54  18:00
            ret.ForEach(x =>
            {
                if (x.ProcessActionViewModel != null) 
                    x.ProcessActionViewModel.Id = $"{x.ProcessActionViewModel.SortOrder}:{x.ProcessActionViewModel.Id}";
            });
            
            return ret;
        }

        var prints = ToAvailableActionItemViewModelList("Print", databaseService.GetPrintList());
        var customProperties = ToAvailableActionItemViewModelList("Custom Properties", databaseService.GetCustomPropertiesList());
        var fileInfos = ToAvailableActionItemViewModelList("File Info", databaseService.GetFileInfoList());
        var saveModels = ToAvailableActionItemViewModelList("Save Model", databaseService.GetSaveModelList());
        var saveDrawings = ToAvailableActionItemViewModelList("Save Drawing", databaseService.GetSaveDrawingList());
        var importFiles = ToAvailableActionItemViewModelList("Import File", databaseService.GetImportFileList());
        var drawingTemplates = ToAvailableActionItemViewModelList("Drawing Template", databaseService.GetDrawingTemplateList());
        var macros = ToAvailableActionItemViewModelList("Macros", databaseService.GetMacrosList());

        AvailableActionsList = new ObservableCollection<ProcessAvailableActionItemViewModel>(
            prints
                .Concat(customProperties)
                .Concat(fileInfos)
                .Concat(saveModels)
                .Concat(saveDrawings)
                .Concat(importFiles)
                .Concat(drawingTemplates)
                .Concat(macros)
        );
    }

    [RelayCommand]
    private void DeleteActionFromProcess(ProcessActionViewModel item)
    {
        ProcessList?.SelectedItem?.ProcessActions.Remove(item);
    }

    [RelayCommand]
    private void AddActionToProcess(ProcessAvailableActionItemViewModel item)
    {
        if (ProcessList?.SelectedItem == null)
            return;
        if (item.ProcessActionViewModel == null)
            return;

        // 视频链接：https://www.youtube.com/watch?v=QxZ7v6OwMrE&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO&index=54  18:20
        var copy = new ProcessAvailableActionItemViewModel();
        copy.RestoreState(item.GetState());
        
        // Make the Id start with the process Id
        if (copy.ProcessActionViewModel != null)
            copy.ProcessActionViewModel.Id = $"{ProcessList.SelectedItemId}:{item.ProcessActionViewModel.Id}";
        
        ProcessList.SelectedItem.ProcessActions.Add(copy.ProcessActionViewModel!);
    }
}
