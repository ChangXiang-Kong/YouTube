using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using BatchProcess3.Tools.Extensions;
using BatchProcess3.ViewModels.MainMenus;
using BatchProcess3.ViewModels.Process;
using CommunityToolkit.Mvvm.Messaging;

namespace BatchProcess3.Views.MainMenus;

public partial class ProcessPageView : UserControl
{
    public ProcessPageView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        /*
         说明：该方式只是提供一个额外的思路
             使用如下方式时，隐藏 Flyout 的方法
                 <Interaction.Behaviors>
                   <TappedEventTrigger>
                     <InvokeCommandAction Command="{Binding $parent[ListBox].((vmMainMenus:ProcessPageViewModel)DataContext).AddActionToProcessCommand}" CommandParameter="{Binding}" />
                   </TappedEventTrigger>
                 </Interaction.Behaviors>
         */
        WeakReferenceMessenger.Default.Register<Dictionary<string, ProcessAvailableActionItemViewModel>>(this, (recipient, msg) =>
        {
            if (msg.TryGetValue(nameof(HideListBox_ActionsListContextMenu), out var itemViewModel))
                HideListBox_ActionsListContextMenu(itemViewModel);
        });
        
        ((ProcessPageViewModel)DataContext).InitializeCommand.Execute(null);
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // if (e.AddedItems == null) return;
        // var item = e.AddedItems[0];
        // if (item is ActionsPrintViewModel)
        // {
        //     var viewModell = (ActionsPrintViewModel)item;
        //     // some logic
        // }
        // 下 等价于 上
        /* 空条件运算符
            这里的 ? 叫 空条件运算符（Null-Conditional Operator）
            若前面的对象是 null，就不执行后面的操作，直接返回 null，不会抛出空引用异常（NullReferenceException），避免程序崩溃。
            等价逻辑（手动写出来就是）：
                if (e.AddedItems != null)
                    return e.AddedItems[0];
                else
                    return null;
            写法	        作用
            对象?.成员	    对象不为 null 才访问成员
            对象?[索引]	    对象不为 null 才访问索引器
         */
        // if (e.AddedItems?.Count > 0 && e.AddedItems?[0] is ActionsPrintViewModel { IsNewItem: true } viewModel)
        if (e.AddedItems?.Count > 0 && e.AddedItems?[0] is ProcessViewModel viewModel)
        {
            // When it is newly crated item
            if (viewModel.IsNewItem)
            {
                TextBox_JobName.SelectAll();
                TextBox_JobName.Focus();
            }
        }
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
            && DataContext is ProcessPageViewModel viewModel
            && sender is Control control
            && control.DataContext is ProcessAvailableActionItemViewModel itemViewModel)
        {
            viewModel.InsertActionToProcess(itemViewModel, ++ListBox_ActionsListContextMenu.SelectedIndex);
            FlyoutBase.GetAttachedFlyout(ListBox_ActionsListContextMenu)?.Hide();
        }
    }

    /*
     说明：该方式只是提供一个额外的思路
         使用如下方式时，隐藏 Flyout 的方法
             <Interaction.Behaviors>
               <TappedEventTrigger>
                 <InvokeCommandAction Command="{Binding $parent[ListBox].((vmMainMenus:ProcessPageViewModel)DataContext).AddActionToProcessCommand}" CommandParameter="{Binding}" />
               </TappedEventTrigger>
             </Interaction.Behaviors>
     */
    public void HideListBox_ActionsListContextMenu(ProcessAvailableActionItemViewModel itemViewModel)
    {
        ((ProcessPageViewModel)DataContext).InsertActionToProcess(itemViewModel, ++ListBox_ActionsListContextMenu.SelectedIndex);
        
        // 这是一个妥协的方法，因为 InvokeCommandAction 调用 AddActionToProcessCommand 后，
        // 会先在需末尾添加一个元素，之后再调用 WeakReferenceMessenger.Default.Send() 方法，
        // 从而调用本方法的 InsertActionToProcess() 方法，将元素插入到指定位置
        ((ProcessPageViewModel)DataContext).ProcessList.SelectedItem.ProcessActionsList.RemoveAtRelative(-1);
        
        FlyoutBase.GetAttachedFlyout(ListBox_ActionsListContextMenu)?.Hide();
    }
}