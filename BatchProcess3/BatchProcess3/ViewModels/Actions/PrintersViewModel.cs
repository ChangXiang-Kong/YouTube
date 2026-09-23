using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Actions;

public partial class PrintersViewModel : ViewModelBase
{
    [ObservableProperty] private string _id = Guid.CreateVersion7().ToString("N");
    [ObservableProperty] private string _name = "";
    [ObservableProperty] private ObservableCollection<string> _paperSizesList = [];
    [ObservableProperty] private ObservableCollection<string> _sourceTraysList = [];
}