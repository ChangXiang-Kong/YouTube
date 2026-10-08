using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Process;

public partial class ProcessAvailableActionItemViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSelectable))]
    [NotifyPropertyChangedFor(nameof(Padding))]
    private ProcessActionViewModel? _processActionViewModel;
    
    [ObservableProperty]
    private string? _category;
    
    [ObservableProperty]
    private string? _iconPath;
    
    public bool IsSelectable => ProcessActionViewModel != null;
    
    public Thickness Padding => IsSelectable ? new Thickness(5) : new Thickness(5, 5, 5, 2);
}