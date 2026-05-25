using System.Threading.Tasks;
using BatchProcess3.Tools.Interfaces;
using BatchProcess3.ViewModels;

namespace BatchProcess3.Tools.Services;

public class DialogService
{
    /// <summary>
    ///
    /// <code>
    /// 使用示例：
    /// if (warn)
    /// {
    ///     var confirmDialogViewModel = new ConfirmDialogViewModel
    ///     {
    ///         InfoType = InfoType.Warning,
    ///         Title = $"Delete {PrintList[index].JobName}?",
    ///         Message = "Are you sure you want to delete this print?",
    ///         DialogWidth = 500,
    ///         OnConfirm = async (vm) =>
    ///         {
    ///             await Task.Delay(2000);
    ///                 
    ///             vm.ProgressText = "This is taking a while...";
    ///             vm.ProgressValue = 50;
    ///                 
    ///             await Task.Delay(1000);
    ///             
    ///             vm.StatusText = "Oh no, something went wrong...";
    ///                 
    ///             return false;
    ///         },
    ///     };
    ///         
    ///     // Wait for click button
    ///     await dialogService.ShowDialogAsync(mainViewModel, confirmDialogViewModel);
    ///         
    ///     // Ignore if we clicked cancel
    ///     if (!confirmDialogViewModel.IsConfirmed)
    ///         return;
    /// }
    /// </code>
    /// </summary>
    /// <param name="host"></param>
    /// <param name="dialogViewModel"></param>
    /// <typeparam name="THost"></typeparam>
    /// <typeparam name="TDialogViewModel"></typeparam>
    public async Task ShowDialogAsync<THost, TDialogViewModel>(THost host, TDialogViewModel dialogViewModel)
        where  THost : IDialogProvider
        where TDialogViewModel : DialogViewModel
    {
        // Set host dialog to provided one
        host.Dialog = dialogViewModel;
        dialogViewModel.Show();
        
        // Wait for dialog to close
        await dialogViewModel.WaitAsync();
    }
}