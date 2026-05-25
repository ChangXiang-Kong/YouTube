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
    ///         // 图标方式一：
    ///         GeometryIcon = GeometryIcon.Ask,
    ///         // 图标方式二：
    ///         // IconMessage = "Ask";
    ///         // IconForeground = "DodgerBlue";
    ///         // IconGeometry = StreamGeometry.Parse("M512 0 30.11843 240.941297l0 542.117406 481.88157 240.941297 481.88157-240.941297L993.88157 240.941297 512 0zM575.776472 768.799969 460.188012 768.799969 460.188012 656.222073l115.588459 0L575.776472 768.799969zM623.335603 509.329685c-52.375829 36.723353-59.600363 55.988096-59.600363 84.885211l0 19.866447L468.616977 614.081343l0-26.489278c0-45.754021 13.846342-80.67124 61.406497-116.791866 46.957428-36.723353 57.79423-62.0082 57.79423-84.282484 0-25.284848-21.67258-54.181962-55.386393-54.181962-42.743457 0-70.436142 26.489278-82.477374 85.486914l-105.956088-21.67258c24.683144-111.976192 82.477374-157.127486 205.289345-157.127486 98.12985 0 157.72919 63.212631 157.72919 131.842639C707.017407 423.240044 688.956071 461.76953 623.335603 509.329685z");
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