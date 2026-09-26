using BatchProcess3.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BatchProcess3.ViewModels;

public partial class ErrorViewModel : ViewModelBase
{
    public ErrorViewModel()
    {
        
    }
    
    [ObservableProperty] private string _message = "Unknown Error";
    [ObservableProperty] private string _stackTrace = "Unknown error description";

    
}