using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BatchProcess3.ViewModels.Actions;

namespace BatchProcess3.Views.Actions;

public partial class ActionsPrintView : UserControl
{
    public ActionsPrintView()
    {
        InitializeComponent();
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
        if (e.AddedItems?[0] is ActionsPrintViewModel { IsNewItem: true })  // 等价于 if (e.AddedItems?[0] is ActionsPrintViewModel viewModel && viewModel.IsNewItem)
        {
            TextBox_JobName.SelectAll();
            TextBox_JobName.Focus();
        }
    }
}