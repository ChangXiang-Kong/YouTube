using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using BatchProcess3.Data;
using BatchProcess3.EntityFramework;
using BatchProcess3.EntityFramework.Entities.Actions;
using BatchProcess3.Tools.Actions;
using BatchProcess3.Tools.Dialog;
using BatchProcess3.ViewModels.Actions;
using BatchProcess3.ViewModels.Process;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace BatchProcess3.ViewModels.MainMenus;

public partial class ProcessPageViewModel(
    MainViewModel mainViewModel,
    DialogService dialogService,
    DatabaseService databaseService,
    ActionService actionService) : PageViewModel(ApplicationPageName.Process)
{
    // Design time only
    public ProcessPageViewModel() : this(new MainViewModel(),
        new DialogService(new Func<TopLevel?>(() => null)),
        new DatabaseService(new AppDbContext()),
        new ActionService(new DatabaseService(new  AppDbContext())))
    {
        if (!Avalonia.Controls.Design.IsDesignMode)
            throw new InvalidOperationException("Parameterless constructor is only for design time use");
    }

    #region Members

    public string? Test { get; set; } = "Test Process";

    [ObservableProperty] private SelectableItemsListViewModel<ProcessViewModel>? _processList;

    [ObservableProperty] private ObservableCollection<ProcessAvailableActionItemViewModel>? _availableActionsList;

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
            updateItem: item =>
            {
                UpdateActionSortOrder();
                databaseService.UpdateProcessItem(item.ToEntity());
            });

        AvailableActionsList = actionService.GetAvailableActionsList();

        ProcessList.FetchList();
    }

    [RelayCommand]
    private void DeleteActionFromProcess(ProcessActionViewModel item)
    {
        ProcessList?.SelectedItem?.ProcessActions.Remove(item);
    }

    [RelayCommand]
    private void AddActionToProcess(ProcessAvailableActionItemViewModel item)
    {
        InsertActionToProcess(item, -1);
        
        /*
         说明：该方式只是提供一个额外的思路
             使用如下方式时，隐藏 Flyout 的方法
                 <Interaction.Behaviors>
                   <TappedEventTrigger>
                     <InvokeCommandAction Command="{Binding $parent[ListBox].((vmMainMenus:ProcessPageViewModel)DataContext).AddActionToProcessCommand}" CommandParameter="{Binding}" />
                   </TappedEventTrigger>
                 </Interaction.Behaviors>
         */
        // 发送消息，通知View关闭Flyout
        // WeakReferenceMessenger.Default.Send(new Dictionary<string, ProcessAvailableActionItemViewModel>() {{"HideListBox_ActionsListContextMenu", item}});
    }
    
    public void InsertActionToProcess(ProcessAvailableActionItemViewModel item, int index)
    {
        if (ProcessList?.SelectedItem == null)
            return;
        if (item.ProcessActionViewModel == null)
            return;

        // 视频链接：https://www.youtube.com/watch?v=QxZ7v6OwMrE&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO&index=54  18:20
        var copy = new ProcessAvailableActionItemViewModel();
        copy.RestoreState(item.GetState());
        
        // Give the copy a new unique ID
        // 不使用 ViewModelBase 原来的 Id 值，而是获取一个新的 Id，
        // 用于解决在 Process 页面时，添加两个相同的 Available Actions 项 到 Actions List 后，点击 Save 按钮后报错的问题
        // 报错内容：System.InvalidOperationException: The instance of entity type 'ProcessActionEntity' cannot be tracked because another instance with the key value '{Id: 01a11b09-dfa1-7d87-85e1-55419f63eb31}' is already being tracked. When attaching existing entities, ensure that only one entity instance with a given key value is attached.
        copy.ProcessActionViewModel!.Id = Guid.CreateVersion7().ToString();
        
        if (index <= -1  || ProcessList.SelectedItem.ProcessActions.Count == 0 || index > ProcessList.SelectedItem.ProcessActions.Count)
            ProcessList.SelectedItem.ProcessActions.Add(copy.ProcessActionViewModel!);
        else
            ProcessList.SelectedItem.ProcessActions.Insert(index, copy.ProcessActionViewModel!);
        
        // Update sort order
        UpdateActionSortOrder();
    }

    // 视频链接：https://www.youtube.com/watch?v=zmsrQumi_Zo&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO&index=56    20:00
    [RelayCommand]
    private void UpdateActionSortOrder()
    {
        if (ProcessList?.SelectedItem == null)
            return;

        foreach (var (action, index) in ProcessList.SelectedItem.ProcessActions.Select((x, idx) => (x, idx)))
        {
            // Sort order should match position in list
            action.SortOrder = index;
            
            // Update the Id（多余的，不需要，这里的 Id 不重要，因为有 SortOrder）
            // action.Id = $"{ProcessList.SelectedItemId}:{action.SortOrder}:{action.ActionId}";
        }
    }
    
}
