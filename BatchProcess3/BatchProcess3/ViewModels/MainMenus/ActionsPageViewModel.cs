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

    #region Print
    
    // 使用 [] 进行初始化以消除警告，当误写 PrintTabsList = null; 时会提示 Cannot convert null literal to non-nullable reference type
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(PrintTabsListHasItems))]
    private ObservableCollection<ActionsPrintViewModel> _printTabsList = [];
    
    // 因为 PrintTabsList 是 ObservableCollection 类型，
    // 需要添加 PrintTabsList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(PrintTabsListHasItems)); 才能生效
    public bool PrintTabsListHasItems => PrintTabsList.Any();
    
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(SelectedPrintTabItem))]
    private string _selectedPrintTabItemId = "";
    
    public ActionsPrintViewModel? SelectedPrintTabItem 
        => PrintTabsList.FirstOrDefault(x => x.Id == SelectedPrintTabItemId);
    
    [ObservableProperty] 
    private ObservableCollection<ActionsPrintSettingsViewModel> _printSettingsList = [];
    
    #endregion Print
    
    #region Custom Properties
    
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(CustomPropertiesListHasItems))]
    private ObservableCollection<ActionsCustomPropertyViewModel> _customPropertiesList = [];
    
    public bool CustomPropertiesListHasItems => CustomPropertiesList.Any();
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedCustomPropertyItem))]
    private string _selectedCustomPropertyItemId = "";
    
    public ActionsCustomPropertyViewModel? SelectedCustomPropertyItem
        => CustomPropertiesList.FirstOrDefault(x => x.Id == SelectedCustomPropertyItemId);

    public ObservableCollection<CustomPropertyRuleType> CustomPropertyRuleTypes 
        => new(Enum.GetValues<CustomPropertyRuleType>());
    
    public ObservableCollection<CustomPropertyFieldType> CustomPropertyFieldTypes 
        => new(Enum.GetValues<CustomPropertyFieldType>());
    
    #endregion Custom Properties
    
    #region File Info
    
    [ObservableProperty] 
    private SelectableItemsListViewModel<ActionsFileInfoViewModel> _fileInfoList = new SelectableItemsListViewModel<ActionsFileInfoViewModel>(
        mainViewModel: mainViewModel,
        dialogService: dialogService,
        title: "File Info",
        getList: () =>
        {
            var list = databaseService.GetFileInfo();
        
            return new ObservableCollection<ActionsFileInfoViewModel>(list
                .OrderBy(x => x.JobName)
                .Select(x => x.ToViewModel()));
        },
        createItem: () => new ActionsFileInfoViewModel()
        {
            Id = Guid.CreateVersion7().ToString(),
            JobName = "New File Info Job",
            Description = "New File Info Job",
            IsNewItem = true,
        },
        deleteItem: (id) => databaseService.DeleteFileInfo(id),
        addItem: item => databaseService.AddFileInfo(item.ToEntity()),
        updateItem: item => databaseService.UpdateFileInfo(item.ToEntity())
    );
    
    #endregion File Info
    
    #region Save Model
    
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(SaveModelListHasItems))]
    private ObservableCollection<ActionsSaveModelViewModel> _saveModelList = [];
    
    public bool SaveModelListHasItems => SaveModelList.Any();
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedSaveModelItem))]
    private string _selectedSaveModelItemId = "";
    
    public ActionsSaveModelViewModel? SelectedSaveModelItem
        => SaveModelList.FirstOrDefault(x => x.Id == SelectedSaveModelItemId);

    public ObservableCollection<string> SaveModelFormats =
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
    [NotifyPropertyChangedFor(nameof(SaveDrawingListHasItems))]
    private ObservableCollection<ActionsSaveDrawingViewModel> _saveDrawingList = [];
    
    public bool SaveDrawingListHasItems => SaveDrawingList.Any();
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedSaveDrawingItem))]
    private string _selectedSaveDrawingItemId = "";
    
    public ActionsSaveDrawingViewModel? SelectedSaveDrawingItem
        => SaveDrawingList.FirstOrDefault(x => x.Id == SelectedSaveDrawingItemId);
    
    public ObservableCollection<string> SaveDrawingFormats =>
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
    [NotifyPropertyChangedFor(nameof(ImportFileListHasItems))]
    private ObservableCollection<ActionsImportFileViewModel> _importFileList = [];
    
    public bool ImportFileListHasItems => ImportFileList.Any();
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedImportFileItem))]
    private string _selectedImportFileItemId = "";
    
    public ActionsImportFileViewModel? SelectedImportFileItem
        => ImportFileList.FirstOrDefault(x => x.Id == SelectedImportFileItemId);
    
    #endregion Import File
    
    #region Drawing Templates
    
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(DrawingTemplateListHasItems))]
    private ObservableCollection<ActionsDrawingTemplateViewModel> _drawingTemplateList = [];
    
    public bool DrawingTemplateListHasItems => DrawingTemplateList.Any();
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedDrawingTemplateItem))]
    private string _selectedDrawingTemplateItemId = "";
    
    public ActionsDrawingTemplateViewModel? SelectedDrawingTemplateItem
        => DrawingTemplateList.FirstOrDefault(x => x.Id == SelectedDrawingTemplateItemId);
    
    public ObservableCollection<DrawingTemplateOperation> DrawingTemplateOperations => new(Enum.GetValues<DrawingTemplateOperation>());

    [ObservableProperty]
    private ObservableCollection<string> _drawingTemplateSelectedPaths = [];

    public ObservableCollection<string> DrawingTemplatePaths => new(databaseService.GetSettings().DrawingTemplatePaths);
    #endregion Drawing Templates
    
    #region Macros
    
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(MacrosListHasItems))]
    private ObservableCollection<ActionsMacrosViewModel> _macrosList = [];
    
    public bool MacrosListHasItems => MacrosList.Any();
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedMacrosItem))]
    private string _selectedMacrosItemId = "";
    
    public ActionsMacrosViewModel? SelectedMacrosItem
        => MacrosList.FirstOrDefault(x => x.Id == SelectedMacrosItemId);
    
    #endregion Macros

    #endregion Members

    protected override void OnDesignTimeConstructor()
    {
        FetchPrintTabList();
        FetchCustomPropertiesList();
        FileInfoList.FetchList();
        FetchSaveModelList();
        FetchSaveDrawingList();
        FetchImportFileList();
        FetchDrawingTemplateList();
        FetchMacrosList();
    }

    #region Actions Page Methods
    
    [RelayCommand]
    public void RefreshActionsPage(ActionsPageName actionsPageName)
    {
        switch (actionsPageName)
        {
            case ActionsPageName.Print: FetchPrintTabList(); break;
            case ActionsPageName.CustomProperties: FetchCustomPropertiesList(); break;
            case ActionsPageName.FileInfo: FileInfoList.FetchList(); break;
            case ActionsPageName.SaveModelAs: FetchSaveModelList(); break;
            case ActionsPageName.SaveDrawingAs: FetchSaveDrawingList(); break;
            case ActionsPageName.ImportFile: FetchImportFileList(); break;
            case ActionsPageName.DrawingTemplates: FetchDrawingTemplateList(); break;
            case ActionsPageName.Macros: FetchMacrosList(); break;
        }
    }
    
    #endregion Actions Page Methods
    
    #region Print Methods
    
    [RelayCommand]
    private void FetchPrintTabList()
    {
        // 将 PrinterProfilesList = ... 放到 PrintTabsList = ... 之前，
        // 因为 PrinterProfilesList 会引用到 PrintTabsList 中的项
        FetchPrintSettings();

        var printTabs = databaseService.GetPrintTab();
        
        PrintTabsList = new ObservableCollection<ActionsPrintViewModel>(printTabs
            .OrderBy(x => x.JobName)
            .Select(x => x.ToViewModel()));
        
        // Update PrintTabsListHasItems when collection changes
        PrintTabsList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(PrintTabsListHasItems));

        if (PrintTabsList.Count <= 0)
            return;
        
        // Select first item
        SelectedPrintTabItemId = PrintTabsList.First().Id;

        // Store last fetched database save states
        foreach (var item in PrintTabsList)
            item.SetSaveState();
    }

    [RelayCommand]
    private void AddNewPrintTabItem()
    {
        // Fetch print Settings
        var printSettings = databaseService.GetPrintSettings();
        
        // Crate a new item
        var newItem = new ActionsPrintViewModel
        {
            Id = Guid.CreateVersion7().ToString(),
            JobName = "New Print Item",
            Description = "New Print Item",
            IsNewItem = true,
            PrintSettingsId = printSettings.FirstOrDefault()?.Id ?? "null",
        };

        // Add to the print list
        PrintTabsList.Add(newItem);

        // Select item
        SelectedPrintTabItemId = newItem.Id;
    }

    [RelayCommand]
    private Task SavePrintTabItemAsync()
    {
        // Ignore if no selection
        if (SelectedPrintTabItem == null)
            return Task.CompletedTask;
        
        // If the selected item is new
        if (SelectedPrintTabItem.IsNewItem)
            databaseService.AddPrintTab(SelectedPrintTabItem.ToEntity());
        else
            databaseService.UpdatePrintTab(SelectedPrintTabItem.ToEntity());

        // Flag new item as not new
        SelectedPrintTabItem.IsNewItem = false;
        // 保存状态以隐藏 Save 按钮
        SelectedPrintTabItem.SetSaveState();
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task CancelPrintTabItemAsync()
    {
        // Ignore if nothing is selected
        if (SelectedPrintTabItem == null)
            return;

        // If the selected item is new, delete it
        // Otherwise, restore from save state
        if (SelectedPrintTabItem.IsNewItem)
            await DeletePrintTabItemFromUIAsync(SelectedPrintTabItem.Id, false);
        else
            SelectedPrintTabItem.RestoreState();
    }

    [RelayCommand]
    private async Task DeletePrintTabItemAsync(string id)
    {
        if (PrintTabsList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;

        // If user selected to remove from UI (via confirm dialog)
        if (await DeletePrintTabItemFromUIAsync(id))
            // Delete from database
            databaseService.DeletePrintTab(id);
    }

    // ReSharper disable once InconsistentNaming
    private async Task<bool> DeletePrintTabItemFromUIAsync(string id, bool popupDialog = true)
    {
        var index = PrintTabsList.IndexOf(PrintTabsList.First(x => x.Id == id));
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
                Title = $"Delete Print Item?",
                Message = $"Are you sure you want to delete {PrintTabsList[index].JobName}?",
                DialogWidth = 500,
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
            
            // Wait for click button
            await dialogService.ShowDialogAsync(mainViewModel, confirmDialogViewModel);
            
            // Ignore if we clicked cancel
            if (!confirmDialogViewModel.IsConfirmed)
                return false;
        }
        
        // Remove item
        PrintTabsList.RemoveAt(index);

        // Select the item before the deleted one
        if (index > 0)
            index--;
        if (PrintTabsList.Count > 0)
            SelectedPrintTabItemId = PrintTabsList[index].Id;

        return true;
    }
    
    [RelayCommand]
    private void FetchPrintSettings()
    {
        var printSettings = databaseService.GetPrintSettings();

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
            PrintSettingsProfilesList = databaseService.GetPrintSettingsProfiles().ToViewModels(),

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
        databaseService.AddPrintSettings(confirmDialogViewModel.ToEntity());
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
        databaseService.UpdatePrintSettings(copiedProfileViewModel.ToEntity());
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
            databaseService.DeletePrintSettings(id);
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
        if (SelectedPrintTabItem != null && PrintSettingsList.Count > 0)
        {
            SelectedPrintTabItem.PrintSettingsId = PrintSettingsList[index].Id;
            await SavePrintTabItemAsync();
        }

        return true;
    }
    
    #endregion Print Methods
    
    #region Custom Properties Methods 
    
    [RelayCommand]
    private void FetchCustomPropertiesList()
    {
        var customProperties = databaseService.GetCustomProperties();
        
        // TODO: Move this logic to a service / provider
        CustomPropertiesList = new ObservableCollection<ActionsCustomPropertyViewModel>(customProperties
            .OrderBy(x => x.JobName)
            .Select(x => x.ToViewModel()));
        
        // Update CustomPropertiesListHasItems when collection changes
        CustomPropertiesList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(CustomPropertiesListHasItems));

        if (CustomPropertiesList.Count <= 0)
            return;
        
        // Select first item
        SelectedCustomPropertyItemId = CustomPropertiesList.First().Id;

        // Store last fetched database save states
        foreach (var item in CustomPropertiesList)
            item.SetSaveState();
    }

    [RelayCommand]
    private void AddNewCustomPropertyItem()
    {
        // Crate a new item
        var newItem = new ActionsCustomPropertyViewModel()
        {
            Id = Guid.CreateVersion7().ToString(),
            JobName = "New Custom Property Action",
            Description = "New Custom Property Action",
            IsNewItem = true,
        };

        // Add to the print list
        CustomPropertiesList.Add(newItem);

        // Select item
        SelectedCustomPropertyItemId = newItem.Id;
    }

    [RelayCommand]
    private Task SaveCustomPropertyItemAsync()
    {
        // Ignore if no selection
        if (SelectedCustomPropertyItem == null)
            return Task.CompletedTask;
        
        // If the selected item is new
        if (SelectedCustomPropertyItem.IsNewItem)
            databaseService.AddCustomProperty(SelectedCustomPropertyItem.ToEntity());
        else
            databaseService.UpdateCustomProperty(SelectedCustomPropertyItem.ToEntity());

        // Flag new item as not new
        SelectedCustomPropertyItem.IsNewItem = false;
        // 保存状态以隐藏 Save 按钮
        SelectedCustomPropertyItem.SetSaveState();
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task CancelCustomPropertyItemAsync()
    {
        // Ignore if nothing is selected
        if (SelectedCustomPropertyItem == null)
            return;

        // If the selected item is new, delete it
        // Otherwise, restore from save state
        if (SelectedCustomPropertyItem.IsNewItem)
            await DeleteCustomPropertyItemFromUIAsync(SelectedCustomPropertyItem.Id, false);
        else
            SelectedCustomPropertyItem.RestoreState();
    }

    [RelayCommand]
    private async Task DeleteCustomPropertyItemAsync(string id)
    {
        if (CustomPropertiesList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;

        // If user selected to remove from UI (via confirm dialog)
        if (await DeleteCustomPropertyItemFromUIAsync(id))
            // Delete from database
            databaseService.DeleteCustomProperty(id);
    }

    // ReSharper disable once InconsistentNaming
    private async Task<bool> DeleteCustomPropertyItemFromUIAsync(string id, bool popupDialog = true)
    {
        var index = CustomPropertiesList.IndexOf(CustomPropertiesList.First(x => x.Id == id));
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
                Title = $"Delete Custom Property Job?",
                Message = $"Are you sure you want to delete {CustomPropertiesList[index].JobName}?",
                DialogWidth = 500,
            };
            
            // Wait for click button
            await dialogService.ShowDialogAsync(mainViewModel, confirmDialogViewModel);
            
            // Ignore if we clicked cancel
            if (!confirmDialogViewModel.IsConfirmed)
                return false;
        }
        
        // Remove item
        CustomPropertiesList.RemoveAt(index);

        // Select the item before the deleted one
        if (index > 0)
            index--;
        if (CustomPropertiesList.Count > 0)
            SelectedCustomPropertyItemId = CustomPropertiesList[index].Id;

        return true;
    }
    
    #endregion Custom Properties Methods
    
    #region Save Model Methods 
    
    [RelayCommand]
    private void FetchSaveModelList()
    {
        var saveModels = databaseService.GetSaveModel();
        
        // TODO: Move this logic to a service / provider
        SaveModelList = new ObservableCollection<ActionsSaveModelViewModel>(saveModels
            .OrderBy(x => x.JobName)
            .Select(x => x.ToViewModel(SaveModelFormats)));
        
        // Update SaveModelListHasItems when collection changes
        SaveModelList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(SaveModelListHasItems));

        if (SaveModelList.Count <= 0)
            return;
        
        // Select first item
        SelectedSaveModelItemId = SaveModelList.First().Id;

        // Store last fetched database save states
        foreach (var item in SaveModelList)
            item.SetSaveState();
    }

    [RelayCommand]
    private void AddNewSaveModelItem()
    {
        // Crate a new item
        var newItem = new ActionsSaveModelViewModel()
        {
            Id = Guid.CreateVersion7().ToString(),
            JobName = "New Save Model Job",
            Description = "New Save Model Job",
            IsNewItem = true,
        };

        // Add to the print list
        SaveModelList.Add(newItem);

        // Select item
        SelectedSaveModelItemId = newItem.Id;
    }

    [RelayCommand]
    private Task SaveSaveModelItemAsync()
    {
        // Ignore if no selection
        if (SelectedSaveModelItem == null)
            return Task.CompletedTask;
        
        // If the selected item is new
        if (SelectedSaveModelItem.IsNewItem)
            databaseService.AddSaveModel(SelectedSaveModelItem.ToEntity());
        else
            databaseService.UpdateSaveModel(SelectedSaveModelItem.ToEntity());

        // Flag new item as not new
        SelectedSaveModelItem.IsNewItem = false;
        // 保存状态以隐藏 Save 按钮
        SelectedSaveModelItem.SetSaveState();
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task CancelSaveModelItemAsync()
    {
        // Ignore if nothing is selected
        if (SelectedSaveModelItem == null)
            return;

        // If the selected item is new, delete it
        // Otherwise, restore from save state
        if (SelectedSaveModelItem.IsNewItem)
            await DeleteSaveModelItemFromUIAsync(SelectedSaveModelItem.Id, false);
        else
            SelectedSaveModelItem.RestoreState();
    }

    [RelayCommand]
    private async Task DeleteSaveModelItemAsync(string id)
    {
        if (SaveModelList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;

        // If user selected to remove from UI (via confirm dialog)
        if (await DeleteSaveModelItemFromUIAsync(id))
            // Delete from database
            databaseService.DeleteSaveModel(id);
    }

    // ReSharper disable once InconsistentNaming
    private async Task<bool> DeleteSaveModelItemFromUIAsync(string id, bool popupDialog = true)
    {
        var index = SaveModelList.IndexOf(SaveModelList.First(x => x.Id == id));
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
                Title = $"Delete Save Model Item?",
                Message = $"Are you sure you want to delete {SaveModelList[index].JobName}?",
                DialogWidth = 500,
            };
            
            // Wait for click button
            await dialogService.ShowDialogAsync(mainViewModel, confirmDialogViewModel);
            
            // Ignore if we clicked cancel
            if (!confirmDialogViewModel.IsConfirmed)
                return false;
        }
        
        // Remove item
        SaveModelList.RemoveAt(index);

        // Select the item before the deleted one
        if (index > 0)
            index--;
        if (SaveModelList.Count > 0)
            SelectedSaveModelItemId = SaveModelList[index].Id;

        return true;
    }
    
    #endregion Save Model Methods
    
    #region Save Drawing Methods 
    
    [RelayCommand]
    private void FetchSaveDrawingList()
    {
        var saveDrawings = databaseService.GetSaveDrawing();
        
        // TODO: Move this logic to a service / provider
        SaveDrawingList = new ObservableCollection<ActionsSaveDrawingViewModel>(saveDrawings
            .OrderBy(x => x.JobName)
            .Select(x => x.ToViewModel(SaveDrawingFormats)));
        
        // Update SaveDrawingListHasItems when collection changes
        SaveDrawingList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(SaveDrawingListHasItems));

        if (SaveDrawingList.Count <= 0)
            return;
        
        // Select first item
        SelectedSaveDrawingItemId = SaveDrawingList.First().Id;

        // Store last fetched database save states
        foreach (var item in SaveDrawingList)
            item.SetSaveState();
    }

    [RelayCommand]
    private void AddNewSaveDrawingItem()
    {
        // Crate a new item
        var newItem = new ActionsSaveDrawingViewModel()
        {
            Id = Guid.CreateVersion7().ToString(),
            JobName = "New Save Drawing Job",
            Description = "New Save Drawing Job",
            ExportFormats = new ObservableCollection<ObservableKeyValuePair<string, bool>>(SaveDrawingFormats.Select(x => new ObservableKeyValuePair<string, bool>(x, false))),
            IsNewItem = true,
        };

        // Add to the print list
        SaveDrawingList.Add(newItem);

        // Select item
        SelectedSaveDrawingItemId = newItem.Id;
    }

    [RelayCommand]
    private Task SaveSaveDrawingItemAsync()
    {
        // Ignore if no selection
        if (SelectedSaveDrawingItem == null)
            return Task.CompletedTask;
        
        // If the selected item is new
        if (SelectedSaveDrawingItem.IsNewItem)
            databaseService.AddSaveDrawing(SelectedSaveDrawingItem.ToEntity());
        else
            databaseService.UpdateSaveDrawing(SelectedSaveDrawingItem.ToEntity());

        // Flag new item as not new
        SelectedSaveDrawingItem.IsNewItem = false;
        // 保存状态以隐藏 Save 按钮
        SelectedSaveDrawingItem.SetSaveState();
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task CancelSaveDrawingItemAsync()
    {
        // Ignore if nothing is selected
        if (SelectedSaveDrawingItem == null)
            return;

        // If the selected item is new, delete it
        // Otherwise, restore from save state
        if (SelectedSaveDrawingItem.IsNewItem)
            await DeleteSaveDrawingItemFromUIAsync(SelectedSaveDrawingItem.Id, false);
        else
            SelectedSaveDrawingItem.RestoreState();
    }

    [RelayCommand]
    private async Task DeleteSaveDrawingItemAsync(string id)
    {
        if (SaveDrawingList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;

        // If user selected to remove from UI (via confirm dialog)
        if (await DeleteSaveDrawingItemFromUIAsync(id))
            // Delete from database
            databaseService.DeleteSaveDrawing(id);
    }

    // ReSharper disable once InconsistentNaming
    private async Task<bool> DeleteSaveDrawingItemFromUIAsync(string id, bool popupDialog = true)
    {
        var index = SaveDrawingList.IndexOf(SaveDrawingList.First(x => x.Id == id));
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
                Title = $"Delete Save Drawing Item?",
                Message = $"Are you sure you want to delete {SaveDrawingList[index].JobName}?",
                DialogWidth = 500,
            };
            
            // Wait for click button
            await dialogService.ShowDialogAsync(mainViewModel, confirmDialogViewModel);
            
            // Ignore if we clicked cancel
            if (!confirmDialogViewModel.IsConfirmed)
                return false;
        }
        
        // Remove item
        SaveDrawingList.RemoveAt(index);

        // Select the item before the deleted one
        if (index > 0)
            index--;
        if (SaveDrawingList.Count > 0)
            SelectedSaveDrawingItemId = SaveDrawingList[index].Id;

        return true;
    }
    
    #endregion Save Drawing Methods
    
    #region Import File Methods 
    
    [RelayCommand]
    private void FetchImportFileList()
    {
        var importFiles = databaseService.GetImportFile();
        
        // TODO: Move this logic to a service / provider
        ImportFileList = new ObservableCollection<ActionsImportFileViewModel>(importFiles
            .OrderBy(x => x.JobName)
            .Select(x => x.ToViewModel()));
        
        // Update ImportFileListHasItems when collection changes
        ImportFileList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(ImportFileListHasItems));

        if (ImportFileList.Count <= 0)
            return;
        
        // Select first item
        SelectedImportFileItemId = ImportFileList.First().Id;

        // Store last fetched database save states
        foreach (var item in ImportFileList)
            item.SetSaveState();
    }

    [RelayCommand]
    private void AddNewImportFileItem()
    {
        // Crate a new item
        var newItem = new ActionsImportFileViewModel()
        {
            Id = Guid.CreateVersion7().ToString(),
            JobName = "New Import File Job",
            Description = "New Import File Job",
            IsNewItem = true,
        };

        // Add to the print list
        ImportFileList.Add(newItem);

        // Select item
        SelectedImportFileItemId = newItem.Id;
    }

    [RelayCommand]
    private Task SaveImportFileItemAsync()
    {
        // Ignore if no selection
        if (SelectedImportFileItem == null)
            return Task.CompletedTask;
        
        // If the selected item is new
        if (SelectedImportFileItem.IsNewItem)
            databaseService.AddImportFile(SelectedImportFileItem.ToEntity());
        else
            databaseService.UpdateImportFile(SelectedImportFileItem.ToEntity());

        // Flag new item as not new
        SelectedImportFileItem.IsNewItem = false;
        // 保存状态以隐藏 Save 按钮
        SelectedImportFileItem.SetSaveState();
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task CancelImportFileItemAsync()
    {
        // Ignore if nothing is selected
        if (SelectedImportFileItem == null)
            return;

        // If the selected item is new, delete it
        // Otherwise, restore from save state
        if (SelectedImportFileItem.IsNewItem)
            await DeleteImportFileItemFromUIAsync(SelectedImportFileItem.Id, false);
        else
            SelectedImportFileItem.RestoreState();
    }

    [RelayCommand]
    private async Task DeleteImportFileItemAsync(string id)
    {
        if (ImportFileList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;

        // If user selected to remove from UI (via confirm dialog)
        if (await DeleteImportFileItemFromUIAsync(id))
            // Delete from database
            databaseService.DeleteImportFile(id);
    }

    // ReSharper disable once InconsistentNaming
    private async Task<bool> DeleteImportFileItemFromUIAsync(string id, bool popupDialog = true)
    {
        var index = ImportFileList.IndexOf(ImportFileList.First(x => x.Id == id));
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
                Title = $"Delete Import File Item?",
                Message = $"Are you sure you want to delete {ImportFileList[index].JobName}?",
                DialogWidth = 500,
            };
            
            // Wait for click button
            await dialogService.ShowDialogAsync(mainViewModel, confirmDialogViewModel);
            
            // Ignore if we clicked cancel
            if (!confirmDialogViewModel.IsConfirmed)
                return false;
        }
        
        // Remove item
        ImportFileList.RemoveAt(index);

        // Select the item before the deleted one
        if (index > 0)
            index--;
        if (ImportFileList.Count > 0)
            SelectedImportFileItemId = ImportFileList[index].Id;

        return true;
    }
    
    #endregion Import File Methods
    
    #region Drawing Templates Methods 
    
    [RelayCommand]
    private void FetchDrawingTemplateList()
    {
        var drawingTemplate = databaseService.GetDrawingTemplate();
        
        // TODO: Move this logic to a service / provider
        DrawingTemplateList = new ObservableCollection<ActionsDrawingTemplateViewModel>(drawingTemplate
            .OrderBy(x => x.JobName)
            .Select(x => x.ToViewModel()));
        
        // Update DrawingTemplateListHasItems when collection changes
        DrawingTemplateList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(DrawingTemplateListHasItems));

        if (DrawingTemplateList.Count <= 0)
            return;
        
        // Select first item
        SelectedDrawingTemplateItemId = DrawingTemplateList.First().Id;

        // Store last fetched database save states
        foreach (var item in DrawingTemplateList)
            item.SetSaveState();
    }

    [RelayCommand]
    private void AddNewDrawingTemplateItem()
    {
        // Crate a new item
        var newItem = new ActionsDrawingTemplateViewModel()
        {
            Id = Guid.CreateVersion7().ToString(),
            JobName = "New Drawing Templates Job",
            Description = "New Drawing Templates Job",
            IsNewItem = true,
        };

        // Add to the print list
        DrawingTemplateList.Add(newItem);

        // Select item
        SelectedDrawingTemplateItemId = newItem.Id;
    }

    [RelayCommand]
    private Task SaveDrawingTemplateItemAsync()
    {
        // Ignore if no selection
        if (SelectedDrawingTemplateItem == null)
            return Task.CompletedTask;
        
        // If the selected item is new
        if (SelectedDrawingTemplateItem.IsNewItem)
            databaseService.AddDrawingTemplate(SelectedDrawingTemplateItem.ToEntity());
        else
            databaseService.UpdateDrawingTemplate(SelectedDrawingTemplateItem.ToEntity());

        // Flag new item as not new
        SelectedDrawingTemplateItem.IsNewItem = false;
        // 保存状态以隐藏 Save 按钮
        SelectedDrawingTemplateItem.SetSaveState();
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task CancelDrawingTemplateItemAsync()
    {
        // Ignore if nothing is selected
        if (SelectedDrawingTemplateItem == null)
            return;

        // If the selected item is new, delete it
        // Otherwise, restore from save state
        if (SelectedDrawingTemplateItem.IsNewItem)
            await DeleteDrawingTemplateItemFromUIAsync(SelectedDrawingTemplateItem.Id, false);
        else
            SelectedDrawingTemplateItem.RestoreState();
    }

    [RelayCommand]
    private async Task DeleteDrawingTemplateItemAsync(string id)
    {
        if (DrawingTemplateList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;

        // If user selected to remove from UI (via confirm dialog)
        if (await DeleteDrawingTemplateItemFromUIAsync(id))
            // Delete from database
            databaseService.DeleteDrawingTemplate(id);
    }

    // ReSharper disable once InconsistentNaming
    private async Task<bool> DeleteDrawingTemplateItemFromUIAsync(string id, bool popupDialog = true)
    {
        var index = DrawingTemplateList.IndexOf(DrawingTemplateList.First(x => x.Id == id));
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
                Title = $"Delete Drawing Templates Item?",
                Message = $"Are you sure you want to delete {DrawingTemplateList[index].JobName}?",
                DialogWidth = 500,
            };
            
            // Wait for click button
            await dialogService.ShowDialogAsync(mainViewModel, confirmDialogViewModel);
            
            // Ignore if we clicked cancel
            if (!confirmDialogViewModel.IsConfirmed)
                return false;
        }
        
        // Remove item
        DrawingTemplateList.RemoveAt(index);

        // Select the item before the deleted one
        if (index > 0)
            index--;
        if (DrawingTemplateList.Count > 0)
            SelectedDrawingTemplateItemId = DrawingTemplateList[index].Id;

        return true;
    }
    
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
        if (DrawingTemplateSelectedPaths.Count == 0)
            return;
        
        // Delete from database
        databaseService.DeleteDrawingTemplatePaths(DrawingTemplateSelectedPaths.ToArray());
        
        // Let the UI know the paths have changed
        OnPropertyChanged(nameof(DrawingTemplatePaths));
    }
    
    #endregion Drawing Templates Methods
    
    #region Macros Methods 
    
    [RelayCommand]
    private void FetchMacrosList()
    {
        var macros = databaseService.GetMacros();
        
        // TODO: Move this logic to a service / provider
        MacrosList = new ObservableCollection<ActionsMacrosViewModel>(macros
            .OrderBy(x => x.JobName)
            .Select(x => x.ToViewModel()));
        
        // Update MacrosListHasItems when collection changes
        MacrosList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(MacrosListHasItems));

        if (MacrosList.Count <= 0)
            return;
        
        // Select first item
        SelectedMacrosItemId = MacrosList.First().Id;

        // Store last fetched database save states
        foreach (var item in MacrosList)
            item.SetSaveState();
    }

    [RelayCommand]
    private void AddNewMacrosItem()
    {
        // Crate a new item
        var newItem = new ActionsMacrosViewModel()
        {
            Id = Guid.CreateVersion7().ToString(),
            JobName = "New Macros Job",
            Description = "New Macros Job",
            IsNewItem = true,
        };

        // Add to the print list
        MacrosList.Add(newItem);

        // Select item
        SelectedMacrosItemId = newItem.Id;
    }

    [RelayCommand]
    private Task SaveMacrosItemAsync()
    {
        // Ignore if no selection
        if (SelectedMacrosItem == null)
            return Task.CompletedTask;
        
        // If the selected item is new
        if (SelectedMacrosItem.IsNewItem)
            databaseService.AddMacros(SelectedMacrosItem.ToEntity());
        else
            databaseService.UpdateMacros(SelectedMacrosItem.ToEntity());

        // Flag new item as not new
        SelectedMacrosItem.IsNewItem = false;
        // 保存状态以隐藏 Save 按钮
        SelectedMacrosItem.SetSaveState();
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task CancelMacrosItemAsync()
    {
        // Ignore if nothing is selected
        if (SelectedMacrosItem == null)
            return;

        // If the selected item is new, delete it
        // Otherwise, restore from save state
        if (SelectedMacrosItem.IsNewItem)
            await DeleteMacrosItemFromUIAsync(SelectedMacrosItem.Id, false);
        else
            SelectedMacrosItem.RestoreState();
    }

    [RelayCommand]
    private async Task DeleteMacrosItemAsync(string id)
    {
        if (MacrosList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;

        // If user selected to remove from UI (via confirm dialog)
        if (await DeleteMacrosItemFromUIAsync(id))
            // Delete from database
            databaseService.DeleteMacros(id);
    }

    // ReSharper disable once InconsistentNaming
    private async Task<bool> DeleteMacrosItemFromUIAsync(string id, bool popupDialog = true)
    {
        var index = MacrosList.IndexOf(MacrosList.First(x => x.Id == id));
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
                Title = $"Delete Macros Item?",
                Message = $"Are you sure you want to delete {MacrosList[index].JobName}?",
                DialogWidth = 500,
            };
            
            // Wait for click button
            await dialogService.ShowDialogAsync(mainViewModel, confirmDialogViewModel);
            
            // Ignore if we clicked cancel
            if (!confirmDialogViewModel.IsConfirmed)
                return false;
        }
        
        // Remove item
        MacrosList.RemoveAt(index);

        // Select the item before the deleted one
        if (index > 0)
            index--;
        if (MacrosList.Count > 0)
            SelectedMacrosItemId = MacrosList[index].Id;

        return true;
    }
    
    #endregion Macros Methods
    
    
    
    
}