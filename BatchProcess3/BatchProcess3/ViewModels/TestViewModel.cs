using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BatchProcess3.ViewModels;

public partial class TestViewModel : ObservableObject
{
    public TestViewModel()
    {
        
    }

    [ObservableProperty]
    private ObservableCollection<TestModel>  _testItemsList = [];

    [RelayCommand]
    void Test()
    {
        for (int i = 0; i < 10; i++)
        {
            TestItemsList.Add(new TestModel() { Name = "Test" + i , Copies = i, Auto = true });
        }
    }
}

public partial class TestModel : ObservableObject
{
    [ObservableProperty] private string _Id  = Guid.CreateVersion7().ToString("N");

    [ObservableProperty] private string _Name;

    [ObservableProperty] private int _Copies;

    [ObservableProperty] private bool _Auto;
}