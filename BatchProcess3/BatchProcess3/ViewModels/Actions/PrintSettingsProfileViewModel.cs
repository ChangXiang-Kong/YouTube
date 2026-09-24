using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Actions;

public partial class PrintSettingsProfileViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _type = "A Size";
    
    [ObservableProperty]
    private string _printerName = "(Default)";

    [ObservableProperty] 
    private ObservableCollection<string> _printerNameOptions =
    [
        "(Default)"
    ];

    #region KeyValuePair
    /*
        用于绑定单个 CheckBox 的 KeyValuePair，用于获取该 CheckBox 的 Content 与 实际的选中结果
          <CheckBox Content="{Binding CheckBoxItem1.Key}" IsChecked="{Binding CheckBoxItem1.Value}" />
     */
    [ObservableProperty]
    private KeyValuePair<string, bool> _checkBoxItem1 = new("Save before close", true);
    /*
        用于绑定多个 CheckBox 的 KeyValuePair 集合，用于获取多个 CheckBox 的 Content 与 实际的选中结果
          <ItemsControl ItemsSource="{Binding CheckBoxItems}">
            <ItemsControl.ItemsPanel>
              <ItemsPanelTemplate>
                <StackPanel />
              </ItemsPanelTemplate>
            </ItemsControl.ItemsPanel>
            <ItemsControl.DataTemplates>
              <DataTemplate>
                <CheckBox Content="{Binding Key}" IsChecked="{Binding Value}"/>
              </DataTemplate>
            </ItemsControl.DataTemplates>
          </ItemsControl>
     */
    [ObservableProperty] 
    private ObservableCollection<KeyValuePair<string, bool>> _checkBoxItems = 
    [
        new("Save before close", true),
        new("Auto load existing profiles", true),
    ];
    #endregion KeyValuePair
    
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
    private string _orientation = "(Default)";
    
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