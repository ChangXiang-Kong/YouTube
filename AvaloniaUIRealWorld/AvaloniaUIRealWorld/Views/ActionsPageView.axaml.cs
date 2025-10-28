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

    private void ActionsTab_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // 因为选择 ListBox 中的项时也会触发该事件，且 e.Source 为 ListBox，
        // 这里当只有 e.Source 为 TabControl_Actions 才触发
        if (Equals(e.Source, TabControl_Actions))
            OnTabChanged();
    }

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