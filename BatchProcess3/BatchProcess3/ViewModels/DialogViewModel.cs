using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels;

public partial class DialogViewModel : ViewModelBase
{
    // 参考视频：https://www.youtube.com/watch?v=suipJSELnrk&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO&index=29
    
    [ObservableProperty]
    private bool _isDialogOpen;
    
    protected TaskCompletionSource closeTask = new TaskCompletionSource();

    public async Task WaitAsync()
    {
        await closeTask.Task;
    }

    public void Show()
    {
        if (closeTask.Task.IsCompleted)
            closeTask = new TaskCompletionSource();
        
        IsDialogOpen = true;
    }

    public void Close()
    {
        IsDialogOpen = false;
        
        closeTask.TrySetResult();
    }
    
    
}