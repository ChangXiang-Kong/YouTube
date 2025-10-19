using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaUIRealWorld.ViewModels.Actions;

public partial class ActionsPrintViewModel : ViewModelBase
{
    [ObservableProperty] private string _id;
    [ObservableProperty] private string _jobName;
    [ObservableProperty] private bool _isSelected;




}