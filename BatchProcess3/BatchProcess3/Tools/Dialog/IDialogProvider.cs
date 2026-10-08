using BatchProcess3.ViewModels;

namespace BatchProcess3.Tools.Dialog;

public interface IDialogProvider
{
    DialogViewModel? Dialog { get; set; }
}