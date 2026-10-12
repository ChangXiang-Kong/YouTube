using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using BatchProcess3.Core.SolidWorksHost;
using BatchProcess3.Data;
using BatchProcess3.EntityFramework;
using BatchProcess3.Tools.Actions;
using BatchProcess3.Tools.Dialog;
using BatchProcess3.Tools.Extensions;
using BatchProcess3.Tools.SolidWorks;
using BatchProcess3.ViewModels.Process;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.MainMenus;

public partial class JobsPageViewModel() : PageViewModel(ApplicationPageName.Jobs)
{
    #region Constructor
    
    
    
    #endregion Constructor
    

    #region Members

    [ObservableProperty] private ObservableCollection<ProcessActionViewModel> _processActionsList = [];
    
    [ObservableProperty] private ObservableCollection<SolidWorksFileDetails> _solidWorksFileList = [];
    
    [ObservableProperty] private bool _quickView = true;
    
    [ObservableProperty] private bool _saveBeforeClose = true;

    #endregion Members
    
    
}