using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BatchProcess3.ViewModels;

public partial class ConfirmDialogViewModel : DialogViewModel
{
    // 参考视频：https://www.youtube.com/watch?v=suipJSELnrk&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO&index=29
    // 图标来源：https://phosphoricons.com/

    [ObservableProperty] private string _iconText = "\xe3e8";
    // [ObservableProperty] private string _iconText = "\xe4e0";
    [ObservableProperty] private string _iconForeground = "DodgerBlue";
    [ObservableProperty] private string _title = "Confirm";
    [ObservableProperty] private string _message = "Are you sure?";
    [ObservableProperty] private string _confirmText = "Yes";
    [ObservableProperty] private string _cancelText = "No";
    [ObservableProperty] private string _applyText = "Apply";
    
    [ObservableProperty]
    private bool _isConfirmed;

    [RelayCommand]
    private void Confirm()
    {
        IsConfirmed = true;
        Close();
    }

    [RelayCommand]
    private void Cancel()
    {
        IsConfirmed = false;
        Close();
    }

    [RelayCommand]
    private void Apply()
    {
        IsConfirmed = true;
    }
}