using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.Host.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] public partial string Greeting { get; set; } = "Welcome to Avalonia!";
}