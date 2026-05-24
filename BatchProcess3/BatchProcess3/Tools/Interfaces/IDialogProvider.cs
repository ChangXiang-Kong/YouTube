using BatchProcess3.ViewModels;

namespace BatchProcess3.Tools.Interfaces;

public interface IDialogProvider
{
    DialogViewModel Dialog { get; set; }
}