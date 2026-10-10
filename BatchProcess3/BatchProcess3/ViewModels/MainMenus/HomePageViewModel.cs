using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using BatchProcess3.Data;
using BatchProcess3.EntityFramework;
using BatchProcess3.EntityFramework.Entities.Actions;
using BatchProcess3.Tools.Actions;
using BatchProcess3.Tools.Dialog;
using BatchProcess3.Tools.Extensions;
using BatchProcess3.ViewModels.Process;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace BatchProcess3.ViewModels.MainMenus;

public partial class HomePageViewModel : PageViewModel
{
    // #region Constructor (视频中的方式)
    //
    // public HomePageViewModel(
    //     MainViewModel mainViewModel,
    //     DialogService dialogService, 
    //     DatabaseService databaseService,
    //     ActionService actionService) : base(ApplicationPageName.Home)
    // {
    //     Initialize(mainViewModel, dialogService, databaseService, actionService);
    // }
    //
    // private void Initialize(
    //     MainViewModel mainViewModel, 
    //     DialogService dialogService, 
    //     DatabaseService databaseService, 
    //     ActionService actionService)
    // {
    //     _mainViewModel = mainViewModel;
    //     _dialogService = dialogService;
    //     _databaseService = databaseService;
    //     _actionService = actionService;
    //
    //     AvailableActionsList = _actionService.GetAvailableActionsList();
    // }
    //
    // // Design-time only
    // // ========== 仅Avalonia设计器使用，禁止DI调用 ==========
    // [Obsolete("Design time only, do NOT use in runtime DI", true)]
    // public HomePageViewModel() : this(new MainViewModel(), new DialogService(() => null), new DatabaseService(new AppDbContext()), new ActionService(new DatabaseService(new AppDbContext())))
    // {
    //     if (!Avalonia.Controls.Design.IsDesignMode) 
    //         throw new InvalidOperationException("Parameterless constructor is only for design time use");
    // }
    //
    // protected override void OnDesignTimeConstructor() => Initialize(new  MainViewModel(), new DialogService(() => null), new DatabaseService(new AppDbContext()), new ActionService(new DatabaseService(new AppDbContext())));
    //
    // #endregion
    
    #region Constructor (自己中的方式)
    
    // ========== DI 运行时使用，标记为DI首选构造 ==========
    [ActivatorUtilitiesConstructor]
    public HomePageViewModel(
        MainViewModel mainViewModel,
        DialogService dialogService,
        DatabaseService databaseService,
        ActionService actionService) : base(ApplicationPageName.Home)
    {
        _mainViewModel = mainViewModel;
        _dialogService = dialogService;
        _databaseService = databaseService;
        _actionService = actionService;
    
        ProcessActionsList = [];    // 2、可以在构造函数中设置一个值，这样就会调用 SetAndObserveEverything() 方法，
    }
    
    // Design time only
    // ========== 仅Avalonia设计器使用，禁止DI调用 ==========
    [Obsolete("Design time only, do NOT use in runtime DI", true)]
    public HomePageViewModel() : this(new MainViewModel(),
        new DialogService(new Func<TopLevel?>(() => null)),
        new DatabaseService(new AppDbContext()),
        new ActionService(new DatabaseService(new AppDbContext())))
    {
        if (!Avalonia.Controls.Design.IsDesignMode)
            throw new InvalidOperationException("Parameterless constructor is only for design time use");
        
        Initialize();
    }
    
    #endregion Constructor
    
    #region Members
    
    private readonly MainViewModel _mainViewModel;
    private readonly DialogService _dialogService;
    private readonly DatabaseService _databaseService;
    private readonly ActionService _actionService;
    
    public string? Test { get; set; } = "Test Home";

    public ObservableCollection<ProcessActionViewModel> ProcessActionsList
    {
        get => field;
        set => this.SetAndObserveEverything(ref field, value, [nameof(HasChanged)]);
    // } = [];      // 1、不能使用默认值，因为设置默认值不会触发 SetAndObserveEverything() 方法，导致没有调用内部事件
    }

    [ObservableProperty] private ObservableCollection<ProcessAvailableActionItemViewModel>? _availableActionsList;
    
    
    #endregion Members

    // 也可以使用 OnViewLoaded() 代替
    [RelayCommand]
    private void Initialize()
    {
        AvailableActionsList = _actionService.GetAvailableActionsList();
    }

    [RelayCommand]
    private void DeleteAction(ProcessActionViewModel item)
    {
        ProcessActionsList.Remove(item);
    }

    public void InsertAction(ProcessAvailableActionItemViewModel item, int index)
    {
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
        
        if (index <= -1  || ProcessActionsList.Count == 0 || index > ProcessActionsList.Count)
            ProcessActionsList.Add(copy.ProcessActionViewModel!);
        else
            ProcessActionsList.Insert(index, copy.ProcessActionViewModel!);
        
        // Update sort order
        UpdateActionSortOrder();
    }

    // 视频链接：https://www.youtube.com/watch?v=zmsrQumi_Zo&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO&index=56    20:00
    [RelayCommand]
    private void UpdateActionSortOrder()
    {
        foreach (var (action, index) in ProcessActionsList.Select((x, idx) => (x, idx)))
        {
            // Sort order should match position in list
            action.SortOrder = index;
            
            // Update the Id（多余的，不需要，这里的 Id 不重要，因为有 SortOrder）
            // action.Id = $"{ProcessList.SelectedItemId}:{action.SortOrder}:{action.ActionId}";
        }
    }
    




}
