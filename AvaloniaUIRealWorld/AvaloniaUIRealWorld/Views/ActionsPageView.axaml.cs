using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaUIRealWorld.Data.EnumValues;
using AvaloniaUIRealWorld.ViewModels;
using AvaloniaUIRealWorld.Views.Actions;

namespace AvaloniaUIRealWorld.Views;

public partial class ActionsPageView : UserControl
{
    public ActionsPageView()
    {
        InitializeComponent();
    }

    protected override void OnInitialized()
    {
        // Fire off initial refresh
        OnTabChanged();
        
        base.OnInitialized();
    }

    private void ActionsTab_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) => OnTabChanged();

    private void OnTabChanged()
    {
        // Get active tab control (Pages inside of each tab)
        var selectedPage = (TabControl_Actions?.SelectedItem as TabItem)?.Content as Control;
        if (selectedPage == null)
            return;
        
        // Convert to ActionsPageName
        var actionsPage = selectedPage switch
        {
            ActionsPrintView => ActionsPageName.Print,
            _ => ActionsPageName.Unknown
        };
        
        // Get view model
        var viewModel = selectedPage.DataContext as ActionsPageViewModel;
        
        // Type check
        viewModel?.RefreshActionsPage(actionsPage);
    }
    
}