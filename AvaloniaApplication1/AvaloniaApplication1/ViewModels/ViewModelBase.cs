using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    public ViewModelBase()
    {
        

        // Detect design time 
        if (Avalonia.Controls.Design.IsDesignMode)
            OnDesignTimeConstructor();
    }

    protected virtual void OnDesignTimeConstructor()
    {
    }
}