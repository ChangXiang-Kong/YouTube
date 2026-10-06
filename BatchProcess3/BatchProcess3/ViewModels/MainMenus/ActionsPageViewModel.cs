using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using BatchProcess3.Data;
using BatchProcess3.EntityFramework;
using BatchProcess3.Tools.Services;
using BatchProcess3.ViewModels.Actions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BatchProcess3.ViewModels.MainMenus;

public partial class ActionsPageViewModel(
    MainViewModel mainViewModel, 
    DialogService dialogService,
    PrinterService printerService,
    DatabaseService databaseService) : PageViewModel(ApplicationPageName.Actions)
{
    // 使用上面的方式替代以下方式构造函数
    // public ActionsPageViewModel() : base(ApplicationPageName.Actions)
    // {
    //     // Some logic
    // }
    
    // Design time only
    public ActionsPageViewModel() : this(new MainViewModel(), 
        new DialogService(new Func<TopLevel?>(() => null)),
        new PrinterService(), 
        new DatabaseService(new AppDbContext()))
    {
        if (!Avalonia.Controls.Design.IsDesignMode) 
            throw new InvalidOperationException("Parameterless constructor is only for design time use");
    }

    #region Members
    
    [ObservableProperty] private string _test = "Test Actions";

    #region Print List
    
    [ObservableProperty] 
    private SelectableItemsListViewModel<ActionsPrintViewModel> _printList = new SelectableItemsListViewModel<ActionsPrintViewModel>(
        mainViewModel: mainViewModel,
        dialogService: dialogService,
        title: "Print",
        getList: () =>
        {
            var list = databaseService.GetPrintList();
        
            return new ObservableCollection<ActionsPrintViewModel>(list
                .OrderBy(x => x.JobName)
                .Select(x => x.ToViewModel()));
        },
        createItem: () =>
        {
            // Fetch print settings
            var printSettings = databaseService.GetPrintSettingsList();
            
            return new ActionsPrintViewModel()
            {
                // Id = Guid.CreateVersion7().ToString(),
                JobName = "New File Info Job",
                Description = "New File Info Job",
                IsNewItem = true,
                PrintSettingsId = printSettings.FirstOrDefault()?.Id
            };
        },
        deleteItem: (id) => databaseService.DeletePrintListItem(id),
        addItem: item => databaseService.AddPrintListItem(item.ToEntity()),
        updateItem: item => databaseService.UpdatePrintListItem(item.ToEntity())
    );
    
    [ObservableProperty] 
    private ObservableCollection<ActionsPrintSettingsViewModel> _printSettingsList = [];
    
    #endregion Print List
    
    #region Custom Properties
    
    [ObservableProperty] 
    private SelectableItemsListViewModel<ActionsCustomPropertyViewModel> _customPropertiesList = new SelectableItemsListViewModel<ActionsCustomPropertyViewModel>(
        mainViewModel: mainViewModel,
        dialogService: dialogService,
        title: "Custom Properties",
        getList: () =>
        {
            var list = databaseService.GetCustomPropertiesList();
        
            return new ObservableCollection<ActionsCustomPropertyViewModel>(list
                .OrderBy(x => x.JobName)
                .Select(x => x.ToViewModel()));
        },
        createItem: () => new ActionsCustomPropertyViewModel()
        {
            // Id = Guid.CreateVersion7().ToString(),
            JobName = "New Custom Property Job",
            Description = "New Custom Property Job",
            IsNewItem = true,
        },
        deleteItem: (id) => databaseService.DeleteCustomPropertyItem(id),
        addItem: item => databaseService.AddCustomPropertyItem(item.ToEntity()),
        updateItem: item => databaseService.UpdateCustomPropertyItem(item.ToEntity())
    );

    public ObservableCollection<CustomPropertyRuleType> CustomPropertyRuleTypes => new(Enum.GetValues<CustomPropertyRuleType>());
    
    public ObservableCollection<CustomPropertyFieldType> CustomPropertyFieldTypes => new(Enum.GetValues<CustomPropertyFieldType>());
    
    #endregion Custom Properties
    
    #region File Info
    
    [ObservableProperty] 
    private SelectableItemsListViewModel<ActionsFileInfoViewModel> _fileInfoList = new SelectableItemsListViewModel<ActionsFileInfoViewModel>(
        mainViewModel: mainViewModel,
        dialogService: dialogService,
        title: "File Info",
        getList: () =>
        {
            var list = databaseService.GetFileInfoList();
        
            return new ObservableCollection<ActionsFileInfoViewModel>(list
                .OrderBy(x => x.JobName)
                .Select(x => x.ToViewModel()));
        },
        createItem: () => new ActionsFileInfoViewModel()
        {
            // Id = Guid.CreateVersion7().ToString(),
            JobName = "New File Info Job",
            Description = "New File Info Job",
            IsNewItem = true,
        },
        deleteItem: (id) => databaseService.DeleteFileInfoItem(id),
        addItem: item => databaseService.AddFileInfoItem(item.ToEntity()),
        updateItem: item => databaseService.UpdateFileInfoItem(item.ToEntity())
    );
    
    #endregion File Info
    
    #region Save Model
    
    [ObservableProperty] 
    private SelectableItemsListViewModel<ActionsSaveModelViewModel> _saveModelList = new SelectableItemsListViewModel<ActionsSaveModelViewModel>(
        mainViewModel: mainViewModel,
        dialogService: dialogService,
        title: "Save Model",
        getList: () =>
        {
            var list = databaseService.GetSaveModelList();
        
            return new ObservableCollection<ActionsSaveModelViewModel>(list
                .OrderBy(x => x.JobName)
                .Select(x => x.ToViewModel(SaveModelFormats)));
        },
        createItem: () => new ActionsSaveModelViewModel()
        {
            // Id = Guid.CreateVersion7().ToString(),
            JobName = "New Save Model Job",
            Description = "New Save Model Job",
            ExportFormats = new ObservableCollection<ObservableKeyValuePair<string, bool>>(SaveModelFormats.Select(x => new ObservableKeyValuePair<string, bool>(x, false))),
            IsNewItem = true,
            
        },
        deleteItem: (id) => databaseService.DeleteSaveModelItem(id),
        addItem: item => databaseService.AddSaveModelItem(item.ToEntity()),
        updateItem: item => databaseService.UpdateSaveModelItem(item.ToEntity())
    );

    public static ObservableCollection<string> SaveModelFormats =
    [
        "Lib Feat Part (*.sldfp)",
        "Assembly file to Part (*.sldprt)",
        "Part Templates (*.prtdot)",
        "Assembly Templates (*.asmdot)",
        "Form Tool (*.sldftp)",
        "Parasolid (*.x_t)",
        "Parasolid Binary (*.x_b)",
        "DXF (*.dxf)",
        "DWG (*.dwg)",
        "IGES (*.igs)",
        "STEP (*.step)",
        "ACIS (*.sat)",
        "VDAFS (*.vda)",
        "VRML (*.wrl)",
        "STL (*.stl)",
        "eDrawings Part (*.eprt)",
        "eDrawings Assembly (*.easm)",
        "Adobe PDF (*.pdf)",
        "Universal 3D (*.u3d)",
        "3D XML (*.3dxml)",
        "Adobe Photoshop (*.psd)",
        "Adobe Illustrator (*.ai)",
        "Microsoft XAML (*.xaml)",
        "Catia Graphics (*.cgr)",
        "ProE Part (*.prt)",
        "ProE Assembly (*.asm)",
        "JPEG (*.jpg)",
        "HCG (*.hcg)",
        "HOOPS HSF (*.hsf)",
        "Tif (*.tif)"
    ];
    
    #endregion Save Model
    
    #region Save Drawing
    
    [ObservableProperty] 
    private SelectableItemsListViewModel<ActionsSaveDrawingViewModel> _saveDrawingList = new SelectableItemsListViewModel<ActionsSaveDrawingViewModel>(
        mainViewModel: mainViewModel,
        dialogService: dialogService,
        title: "Save Drawing",
        getList: () =>
        {
            var list = databaseService.GetSaveDrawingList();
        
            return new ObservableCollection<ActionsSaveDrawingViewModel>(list
                .OrderBy(x => x.JobName)
                .Select(x => x.ToViewModel(SaveDrawingFormats)));
        },
        createItem: () => new ActionsSaveDrawingViewModel()
        {
            // Id = Guid.CreateVersion7().ToString(),
            JobName = "New Save Drawing Job",
            Description = "New Save Drawing Job",
            ExportFormats = new ObservableCollection<ObservableKeyValuePair<string, bool>>(SaveDrawingFormats.Select(x => new ObservableKeyValuePair<string, bool>(x, false))),
            IsNewItem = true,
        },
        deleteItem: (id) => databaseService.DeleteSaveDrawingItem(id),
        addItem: item => databaseService.AddSaveDrawingItem(item.ToEntity()),
        updateItem: item => databaseService.UpdateSaveDrawingItem(item.ToEntity())
    );
    public static ObservableCollection<string> SaveDrawingFormats =>
    [
        "Detached Drawing (*.slddrw)",
        "DXF (*.dxf)",
        "DWG (*.dwg)",
        "Photoshop File (*.psd)",
        "Illustrator File (*.ai)",
        "PDF (*.pdf)",
        "eDrawing (*.edrw)",
        "JPEG (*.jpg)",
        "Tif (*.tif)"
    ];
    
    #endregion Save Drawing
    
    #region Import File
    
    [ObservableProperty] 
    private SelectableItemsListViewModel<ActionsImportFileViewModel> _importFileList = new SelectableItemsListViewModel<ActionsImportFileViewModel>(
        mainViewModel: mainViewModel,
        dialogService: dialogService,
        title: "Import File",
        getList: () =>
        {
            var list = databaseService.GetImportFileList();
        
            return new ObservableCollection<ActionsImportFileViewModel>(list
                .OrderBy(x => x.JobName)
                .Select(x => x.ToViewModel()));
        },
        createItem: () => new ActionsImportFileViewModel()
        {
            // Id = Guid.CreateVersion7().ToString(),
            JobName = "New Import File Job",
            Description = "New Import File Job",
            IsNewItem = true,
        },
        deleteItem: (id) => databaseService.DeleteImportFileItem(id),
        addItem: item => databaseService.AddImportFileItem(item.ToEntity()),
        updateItem: item => databaseService.UpdateImportFileItem(item.ToEntity())
    );
    
    #endregion Import File
    
    #region Drawing Templates
    
    [ObservableProperty] 
    private SelectableItemsListViewModel<ActionsDrawingTemplateViewModel> _drawingTemplateList = new SelectableItemsListViewModel<ActionsDrawingTemplateViewModel>(
        mainViewModel: mainViewModel,
        dialogService: dialogService,
        title: "Drawing Template",
        getList: () =>
        {
            var list = databaseService.GetDrawingTemplateList();
        
            return new ObservableCollection<ActionsDrawingTemplateViewModel>(list
                .OrderBy(x => x.JobName)
                .Select(x => x.ToViewModel()));
        },
        createItem: () => new ActionsDrawingTemplateViewModel()
        {
            // Id = Guid.CreateVersion7().ToString(),
            JobName = "New Drawing Templates Job",
            Description = "New Drawing Templates Job",
            IsNewItem = true,
        },
        deleteItem: (id) => databaseService.DeleteDrawingTemplateItem(id),
        addItem: item => databaseService.AddDrawingTemplateItem(item.ToEntity()),
        updateItem: item => databaseService.UpdateDrawingTemplateItem(item.ToEntity())
    );
    
    public ObservableCollection<DrawingTemplateOperation> DrawingTemplateOperations => new(Enum.GetValues<DrawingTemplateOperation>());

    [ObservableProperty]
    private ObservableCollection<string> _selectedDrawingTemplatePaths = [];

    public ObservableCollection<string> DrawingTemplatePaths => new(databaseService.GetSettings().DrawingTemplatePaths);
    #endregion Drawing Templates
    
    #region Macros
    
    [ObservableProperty] 
    private SelectableItemsListViewModel<ActionsMacrosViewModel> _macrosList = new SelectableItemsListViewModel<ActionsMacrosViewModel>(
        mainViewModel: mainViewModel,
        dialogService: dialogService,
        title: "Macros",
        getList: () =>
        {
            var list = databaseService.GetMacrosList();
        
            return new ObservableCollection<ActionsMacrosViewModel>(list
                .OrderBy(x => x.JobName)
                .Select(x => x.ToViewModel()));
        },
        createItem: () => new ActionsMacrosViewModel()
        {
            // Id = Guid.CreateVersion7().ToString(),
            JobName = "New Macros Job",
            Description = "New Macros Job",
            IsNewItem = true,
        },
        deleteItem: (id) => databaseService.DeleteMacrosItem(id),
        addItem: item => databaseService.AddMacrosItem(item.ToEntity()),
        updateItem: item => databaseService.UpdateMacrosList(item.ToEntity())
    );
    
    #endregion Macros

    #endregion Members

    protected override void OnDesignTimeConstructor()
    {
        PrintList.FetchList();
        CustomPropertiesList.FetchList();
        FileInfoList.FetchList();
        SaveModelList.FetchList();
        SaveDrawingList.FetchList();
        ImportFileList.FetchList();
        DrawingTemplateList.FetchList();
        MacrosList.FetchList();
    }

    #region Actions Page Methods
    
    [RelayCommand]
    public void RefreshActionsPage(ActionsPageName actionsPageName)
    {
        switch (actionsPageName)
        {
            case ActionsPageName.Print: 
                PrintList.FetchList(); 
                // Fetch print settings
                FetchPrintSettings();
                break;
            case ActionsPageName.CustomProperties: CustomPropertiesList.FetchList(); break;
            case ActionsPageName.FileInfo: FileInfoList.FetchList(); break;
            case ActionsPageName.SaveModelAs: SaveModelList.FetchList(); break;
            case ActionsPageName.SaveDrawingAs: SaveDrawingList.FetchList(); break;
            case ActionsPageName.ImportFile: ImportFileList.FetchList(); break;
            case ActionsPageName.DrawingTemplates: DrawingTemplateList.FetchList(); break;
            case ActionsPageName.Macros: MacrosList.FetchList(); break;
        }
    }
    
    #endregion Actions Page Methods
    
    #region Print List Methods
    [RelayCommand]
    private void FetchPrintSettings()
    {
        var printSettings = databaseService.GetPrintSettingsList();

        PrintSettingsList = printSettings.ToViewModels();
    }

    [RelayCommand]
    private async Task AddNewPrintSettingsAsync()
    {
        var confirmDialogViewModel = new ActionsPrintSettingsViewModel()
        {
            Title = "New Printer Settings",
            // 图标方式一：
            // GeometryIcon = GeometryIcon.PrinterPosCog,                  // 不需要了，内部构造函数已有 IconGeometry 与 IconForeground 替代
            // // 图标方式二：
            // // IconMessage = "PrinterPosCog";
            // // IconForeground = "DodgerBlue";
            // // IconGeometry = StreamGeometry.Parse("M505.6512 39.0144c-261.2224 3.4816-470.1184 218.112-466.6368 479.4368 3.4816 261.12 218.112 470.1184 479.3344 466.6368 261.2224-3.4816 470.1184-218.112 466.7392-479.3344C981.504 244.4288 766.8736 35.5328 505.6512 39.0144zM558.08 196.608c48.128 0 62.2592 27.9552 62.2592 59.8016 0 39.8336-31.9488 76.6976-86.3232 76.6976-45.568 0-67.1744-22.9376-65.9456-60.8256C468.0704 240.4352 494.7968 196.608 558.08 196.608zM434.7904 807.6288c-32.8704 0-56.9344-19.968-33.8944-107.6224l37.6832-155.5456c6.5536-24.8832 7.68-34.9184 0-34.9184-9.8304 0-52.5312 17.2032-77.7216 34.2016l-16.384-26.9312c79.9744-66.7648 171.8272-105.8816 211.2512-105.8816 32.8704 0 38.2976 38.912 21.9136 98.6112l-43.2128 163.5328c-7.68 28.8768-4.4032 38.912 3.2768 38.912 9.9328 0 42.1888-11.9808 73.9328-36.9664l18.6368 24.8832C552.5504 777.728 467.6608 807.6288 434.7904 807.6288z");
            Name = "New Printer Settings",
            Description = "New Printer Settings",
            PrintSettingsProfilesList = databaseService.GetPrintSettingsProfilesList().ToViewModels(),

            // Title = $"Printer Settings",                                // 不需要了，内部构造函数已有
            // Message = "Are you sure you want to delete this print?",    // 不需要了，内部构造函数已有
            // DialogWidth = 1200,                                         // 不需要了，内部构造函数已有
            // OnConfirm = async (vm) =>
            // {
            //     await Task.Delay(2000);
            //     
            //     vm.ProgressText = "This is taking a while...";
            //     vm.ProgressValue = 50;
            //     
            //     await Task.Delay(1000);
            //
            //     vm.StatusText = "Oh no, something went wrong...";
            //     
            //     return true;
            // },
        };
        
        // TODO: Remove once we confirm view model dialog is pulled from database
        confirmDialogViewModel.RestoreState(confirmDialogViewModel.GetState());
        
        InjectPrintSettingsDetails(confirmDialogViewModel);
        
        // Wait for click button
        await dialogService.ShowDialogAsync(mainViewModel, confirmDialogViewModel);
            
        // Ignore if we clicked cancel
        if (!confirmDialogViewModel.IsConfirmed)
            return;
        
        PrintSettingsList.Add(confirmDialogViewModel);
        databaseService.AddPrintSettingsItem(confirmDialogViewModel.ToEntity());
    }

    [RelayCommand]
    private async Task EditPrintSettingsAsync(string id)
    {
        var profileViewModel = PrintSettingsList.FirstOrDefault(x => x.Id == id);
        if (profileViewModel == null)
            // TODO: Throw/Warn?
            return;
        
        // Copy view model
        var copiedProfileViewModel = new ActionsPrintSettingsViewModel
        {
            Title = "Edit Printer Settings",
            // IconForeground = "Yellow"
        };
        
        // ===== 用于测试 ===== 
        // JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        // {
        //     WriteIndented = true,
        //     PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        //     IgnoreReadOnlyFields = true,
        //     IgnoreReadOnlyProperties = true,
        //     NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,    // 处理 如 double.NaN 之类的无限数对象
        // };
        // var a = JsonSerializer.Serialize(this, GetType().DeclaringType ?? GetType(), _jsonSerializerOptions);
        // =====================
        var test = profileViewModel.GetState();
        copiedProfileViewModel.RestoreState(profileViewModel.GetState());

        InjectPrintSettingsDetails(copiedProfileViewModel);
        
        await dialogService.ShowDialogAsync(mainViewModel, copiedProfileViewModel);
        
        // Ignore if we clicked cancel
        if (!copiedProfileViewModel.IsConfirmed)
            return;
        
        // Commit copied view model back
        profileViewModel.RestoreState(copiedProfileViewModel.GetState());
        databaseService.UpdatePrintSettingsItem(copiedProfileViewModel.ToEntity());
    }

    private void InjectPrintSettingsDetails(ActionsPrintSettingsViewModel viewModel)
    {
        // Fetch live printers available on machine
        var availablePrinters = printerService.GetAvailablePrinters();
        var printerNameOptions = new ObservableCollection<string>(availablePrinters.Select(x => x.Name));

        foreach (var item in viewModel.PrintSettingsProfilesList)
        {
            item.PrinterNameOptions = printerNameOptions;
            
            item.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName != nameof(ActionsPrintSettingsProfileViewModel.PrinterName))
                    return;
                
                // Printer changed, update paper size and tray
                item.PaperSizeOptions = new ObservableCollection<string>(
                    availablePrinters.FirstOrDefault(x => x.Name == item.PrinterName)?.PaperSizesList ?? []
                );
                item.PaperSizeOptions.Insert(0, "(Default)");
                
                item.SourceTrayOptions = new ObservableCollection<string>(
                    availablePrinters.FirstOrDefault(x => x.Name == item.PrinterName)?.SourceTraysList ?? []
                );
                item.SourceTrayOptions.Insert(0, "(Default)");
                
                // Change paper size and source tray to first item
                if (!item.PaperSizeOptions.Any(x => x == item.PaperSize))
                    item.PaperSize = item.PaperSizeOptions.FirstOrDefault() ?? "-";
                
                if (item.SourceTrayOptions.All(x => x != item.SourceTray))
                    item.SourceTray = item.SourceTrayOptions.FirstOrDefault() ?? "-";
            };
            
            // Force a printer name change for initial list
            // Otherwise, the ComboBox will not select PaperSize, Orientation, Tray, DrawingColor that already saved in database when editing a print settings
            item.OnPropertyChanged(nameof(item.PrinterName));
        }
    }

    [RelayCommand]
    private async Task DeletePrintSettingsAsync(string id)
    {
        if (PrintSettingsList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;

        var item = PrintSettingsList.First(x => x.Id == id);
        if (!item.CanDelete)
            throw new InvalidOperationException($"The print setting {item.Name} cannot be deleted.");
        
        if (await DeletePrintSettingsFromUIAsync(id))
            databaseService.DeletePrintSettingsItem(id);
    }

    // ReSharper disable once InconsistentNaming
    private async Task<bool> DeletePrintSettingsFromUIAsync(string id, bool popupDialog = true)
    {
        var index = PrintSettingsList.IndexOf(PrintSettingsList.First(x => x.Id == id));
        if (index == -1)
            return false;
        
        if (popupDialog)
        {
            var confirmDialogViewModel = new ConfirmDialogViewModel
            {
                // 图标方式一：
                GeometryIcon = GeometryIcon.Warning,
                // 图标方式二：
                // IconMessage = "Warning";
                // IconForeground = "#fc8800";
                // IconGeometry = StreamGeometry.Parse("M943.644188 827.215696l-351.176649-608.204749c-42.945473-74.36249-113.147387-74.36249-156.092861 0l-351.176649 608.204749c-42.946498 74.431167-7.811716 135.14955 78.012605 135.14955l702.420949 0C951.455904 962.36422 986.555836 901.645838 943.644188 827.215696zM466.187532 391.579035c12.621133-13.644108 28.66175-20.466675 48.233578-20.466675 19.580028 0 35.612444 6.75389 48.241778 20.194018 12.544256 13.473954 18.820484 30.325365 18.820484 50.587035 0 17.430551-26.19759 145.621205-34.929778 238.882082l-63.105666 0c-7.666162-93.259852-36.090106-221.450507-36.090106-238.882082C447.358847 421.938226 453.643275 405.155491 466.187532 391.579035zM561.76804 835.026386c-13.268949 12.928641-29.062535 19.375023-47.345906 19.375023-18.275171 0-34.076957-6.447407-47.346931-19.375023-13.235123-12.89379-19.818859-28.517221-19.818859-46.869269 0-18.249546 6.583736-34.043131 19.818859-47.278254 13.268949-13.235123 29.07176-19.852685 47.346931-19.852685 18.283371 0 34.076957 6.617562 47.345906 19.852685 13.235123 13.235123 19.827059 29.028709 19.827059 47.278254C581.595099 806.51019 575.003163 822.132597 561.76804 835.026386z");
                Title = $"Delete Printer Profile?",
                Message = $"Are you sure you want to delete {PrintSettingsList[index].Name}?",
                DialogWidth = 500,
            };
            
            // Wait for click button
            await dialogService.ShowDialogAsync(mainViewModel, confirmDialogViewModel);
            
            // Ignore if we clicked cancel
            if (!confirmDialogViewModel.IsConfirmed)
                return false;
        }
        
        // Remove item
        PrintSettingsList.RemoveAt(index);

        // Select the item before the deleted one
        if (index > 0)
            index--;
        if (PrintList.SelectedItem != null && PrintSettingsList.Count > 0)
        {
            PrintList.SelectedItem.PrintSettingsId = PrintSettingsList[index].Id;
            await PrintList.SaveItemAsync();
        }

        return true;
    }
    
    #endregion Print List Methods
    
    #region Drawing Templates Methods 
    
    [RelayCommand]
    private async Task AddDrawingTemplatePaths()
    {
        var paths = await dialogService.ShowSelectFileDialogAsync(
            title: "Select a drawing template", 
            allowMultiple: true,
            fileTypes: [
                new FilePickerFileType("*.slddrt") { Patterns = ["*.slddrt"] }
            ]);
        
        // Add to database
        databaseService.AddDrawingTemplatePaths(paths);
        
        // Let the UI know the paths have changed
        OnPropertyChanged(nameof(DrawingTemplatePaths));
    }

    [RelayCommand]
    private void DeleteDrawingTemplatePaths()
    {
        // Ignore empty list
        if (SelectedDrawingTemplatePaths.Count == 0)
            return;
        
        // Delete from database
        databaseService.DeleteDrawingTemplatePaths(SelectedDrawingTemplatePaths.ToArray());
        
        // Let the UI know the paths have changed
        OnPropertyChanged(nameof(DrawingTemplatePaths));
    }
    
    #endregion Drawing Templates Methods
    
}