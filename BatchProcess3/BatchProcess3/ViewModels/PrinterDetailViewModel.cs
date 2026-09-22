using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels;

public partial class PrinterDetailViewModel : ViewModelBase
{
    [ObservableProperty] private string _id = Guid.CreateVersion7().ToString("N");
    [ObservableProperty] private string _name = "";
    [ObservableProperty] private ObservableCollection<string> _paperSizes = [];
    [ObservableProperty] private ObservableCollection<string> _sourceTrays = [];
}