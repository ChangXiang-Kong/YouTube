using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BatchProcess3.EntityFramework.Entities.Actions;
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
public static class PrintSettingsProfileViewModelExtensions
{
    public static PrintSettingsProfileEntity ToEntity(this PrintSettingsProfileViewModel viewModel)
    {
        return new PrintSettingsProfileEntity()
        {
            Id =  viewModel.Id,
            Type =   viewModel.Type,
            PrinterName = viewModel.PrinterName,
            PaperSize = viewModel.PaperSize,
            Width =  viewModel.Width,
            Height = viewModel.Height,
            Orientation = viewModel.Orientation,
            SourceTray = viewModel.SourceTray,
            DrawingColor =  viewModel.DrawingColor,
            ScaleToFil =  viewModel.ScaleToFil,
            // PrintSettingsId =    // 不需要手动赋值
            // PrintSettings =      // 不需要手动赋值
            // 为什么 ToEntity() 里不用赋值这两个？
            //     `ToEntity` 的职责：ViewModel → 实体数据映射，只映射【属于 Profile 本身业务字段】。
            //     `PrintSettingsId` 代表：这个 Profile 归属哪一条 PrintSettings 记录，这是 关联关系信息，不属于 Profile 自己的业务属性。
        };
    }

    public static List<PrintSettingsProfileEntity> ToEntities(this ObservableCollection<PrintSettingsProfileViewModel> viewModels)
    {
        return viewModels.Select(ToEntity).ToList();
        // 等于
        // return viewModels.Select(x => x.ToEntity()).ToList();
    }

    public static PrintSettingsProfileViewModel ToViewModel(this PrintSettingsProfileEntity entity)
    {
        return new PrintSettingsProfileViewModel()
        {
            Id =  entity.Id,
            Type =  entity.Type,
            PrinterName = entity.PrinterName,
            PaperSize = entity.PaperSize,
            Width = entity.Width,
            Height = entity.Height,
            Orientation = entity.Orientation,
            SourceTray = entity.SourceTray,
            DrawingColor = entity.DrawingColor,
            ScaleToFil = entity.ScaleToFil,
        };
    }

    public static ObservableCollection<PrintSettingsProfileViewModel> ToViewModels(this List<PrintSettingsProfileEntity> entities)
    {
        return new ObservableCollection<PrintSettingsProfileViewModel>(entities.Select(ToViewModel));
        // 等于
        // return new ObservableCollection<PrintSettingsProfileViewModel>(entities.Select(x => x.ToViewModel()));
    }
}