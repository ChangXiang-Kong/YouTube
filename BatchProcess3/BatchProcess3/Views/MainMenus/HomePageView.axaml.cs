using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using BatchProcess3.ViewModels.MainMenus;
using BatchProcess3.ViewModels.Process;

namespace BatchProcess3.Views.MainMenus;

public partial class HomePageView : UserControl
{
    public HomePageView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }
    
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        ((HomePageViewModel)DataContext).InitializeCommand.Execute(null);
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        
    }

    private void ListBox_ActionsList_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is Control control && e.InitialPressMouseButton == MouseButton.Right)
        {
            FlyoutBase.ShowAttachedFlyout(control);
        }
    }

    private void Border_ActionContextMenu_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton == MouseButton.Left
            && DataContext is HomePageViewModel viewModel
            && sender is Control control
            && control.DataContext is ProcessAvailableActionItemViewModel itemViewModel)
        {
            viewModel.InsertAction(itemViewModel, ++ListBox_ActionsListContextMenu.SelectedIndex);
            FlyoutBase.GetAttachedFlyout(ListBox_ActionsListContextMenu)?.Hide();
        }
    }
}