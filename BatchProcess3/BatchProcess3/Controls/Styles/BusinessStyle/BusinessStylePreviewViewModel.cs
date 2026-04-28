using BatchProcess3.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.Controls.Styles;

public partial class BusinessStylePreviewViewModel : ViewModelBase
{
    [ObservableProperty] private string _greeting = "Welcome to Avalonia!";
}