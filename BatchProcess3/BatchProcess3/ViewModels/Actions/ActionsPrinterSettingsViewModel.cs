using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Actions;

public partial class ActionsPrinterSettingsViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _id = "-1";
    
    [ObservableProperty]
    private string _type = "A Size";
    
    [ObservableProperty]
    private string _printerName = "(Default)";

    [ObservableProperty] private ObservableCollection<string> _printerNameOptions =
    [
        "(Default)"
    ];
    
    [ObservableProperty]
    private string _paperSize = "(Default)";
    
    [ObservableProperty]
    private ObservableCollection<string> _paperSizeOptions =
    [
        "(Default)"
    ];
    
    [ObservableProperty]
    private double _width;
    
    [ObservableProperty]
    private double _height;
    
    [ObservableProperty]
    private string  _orientation = "(Default)";
    
    [ObservableProperty]
    private ObservableCollection<string> _orientationOptions = 
    [
        "(Default)",
        "Portrait",
        "Landscape"
    ];
    
    [ObservableProperty]
    private string _sourceTray = "(Default)";
    
    [ObservableProperty]
    private ObservableCollection<string> _sourceTrayOptions =
    [
        "(Default)"
    ];
    
    [ObservableProperty]
    private string _drawingColor = "(Default)";
    
    [ObservableProperty]
    private ObservableCollection<string> _drawingColorOptions = 
    [
        "(Default)",
        "Automatic",
        "Color / Greyscale",
        "Black & White",
    ];
    
    [ObservableProperty]
    private bool _scaleToFil;

}