using System.Threading.Tasks;
using BatchProcess3.Tools.Interfaces;
using BatchProcess3.ViewModels;

namespace BatchProcess3.Tools.Services;

public class DialogService
{
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