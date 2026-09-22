using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Avalonia.Media;
using BatchProcess3.Data;
using BatchProcess3.Tools.Services;
using BatchProcess3.ViewModels.Actions;
using CommunityToolkit.Mvvm.Input;

namespace BatchProcess3.ViewModels;

public partial class ActionsPageViewModel(
    MainViewModel mainViewModel, 
    DialogService dialogService,
    PrinterService printerService) : PageViewModel(ApplicationPageName.Actions)
{
    // 使用上面的方式替代以下方式构造函数
    // public ActionsPageViewModel() : base(ApplicationPageName.Actions)
    // {
    //     // Some logic
    // }
    
    // Design time only
    public ActionsPageViewModel() : this(new MainViewModel(), new DialogService(), new PrinterService()) { }

    [ObservableProperty] private string _test = "Test Actions";

    // TODO: Remove once we have database service
    private PrintProfileViewModel _defaultPrinterProfile = new PrintProfileViewModel
    {
        Id = "0",
        Name = "(Default)",
        Description = "Use all default settings",
        Copies = 1,
        // TODO: Populate PrintSettings
    };

    // 使用 [] 进行初始化以消除警告，当误写 PrintList = null; 时会提示 Cannot convert null literal to non-nullable reference type
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(PrintListHasItems))]
    private ObservableCollection<ActionsPrintViewModel> _printList = [];

    // 因为 PrintList 是 ObservableCollection 类型，
    // 需要添加 PrintList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(PrintListHasItems)); 才能生效
    public bool PrintListHasItems => PrintList.Any();

    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(SelectedPrintListItem))]
    private string _selectedPrintListItemId = "";

    public ActionsPrintViewModel? SelectedPrintListItem =>
        PrintList.FirstOrDefault(x => x.Id == SelectedPrintListItemId);

    [ObservableProperty] private ObservableCollection<PrintProfileViewModel> _printerProfilesList = [];

    [RelayCommand]
    public void RefreshActionsPage(ActionsPageName actionsPageName)
    {
        switch (actionsPageName)
        {
            case ActionsPageName.Print: FetchPrintActionsData(); break;
        }
    }

    [RelayCommand]
    private void FetchPrintActionsData()
    {
        // 将 PrinterProfilesList = ... 放到 PrintList = ... 之前，
        // 因为 PrinterProfilesList 会引用到 PrintList 中的项
        FetchPrinterProfiles();
        
        // TODO: Fetch from a database/service provider
        PrintList =
        [
            new ActionsPrintViewModel
            {
                Id = "1",
                JobName = "Print Only Orawings",
                Description = "Prints only drawing files",
                PrintDrawingRange = "0, 5, 7-8",
                DrawingExclusionIsWhiteList = true,
                PrintModels = true,
                PrintDrawings = true,
                DrawingExclusionList =
                    $"Some item 1{Environment.NewLine}Some item 2{Environment.NewLine}Some item 3",
                PrinterProfileId = "1",
            },
            new ActionsPrintViewModel
            {
                Id = "2",
                JobName = "Print All Drawings Scale To Fit",
                Description = "Prints drawing scaled to fit the paper",
                PrintModels = true,
                PrintDrawings = false,
                PrinterProfileId = "2",
            },
            new ActionsPrintViewModel
            {
                Id = "3",
                JobName = "Print 3D Models A3",
                Description = "Prints models as 3D visuals",
                PrintModels = false,
                PrintDrawings = true,
                PrinterProfileId = "3",
            },
        ];

        // Update PrintListHasItems when collection changes
        PrintList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(PrintListHasItems));

        if (PrintList.Count > 0)
        {
            // Select first item
            SelectedPrintListItemId = PrintList.First().Id;

            // Store last fetched database save states
            foreach (var printItem in PrintList)
                printItem.SetSaveState();
        }

    }

    [RelayCommand]
    private void FetchPrinterProfiles()
    {
        // TODO: Pull from database
        var printerSettingsItem = new ActionsPrinterSettingsViewModel()
        {
            Id = "2",
            Height = 200,
            Width = 140,
            ScaleToFil = true,
        };

        var printerSettings = new ObservableCollection<ActionsPrinterSettingsViewModel>
        {
            printerSettingsItem, printerSettingsItem, printerSettingsItem, printerSettingsItem, printerSettingsItem,
            printerSettingsItem, printerSettingsItem, printerSettingsItem, printerSettingsItem, printerSettingsItem,
            printerSettingsItem, printerSettingsItem, printerSettingsItem, printerSettingsItem, printerSettingsItem,
            printerSettingsItem, printerSettingsItem, printerSettingsItem, printerSettingsItem, printerSettingsItem,
        };

        _defaultPrinterProfile.PrinterSettings = printerSettings;
        
        PrinterProfilesList =
        [
            _defaultPrinterProfile,
            new PrintProfileViewModel
            {
                Id = "1",
                Name = "Print Landscape",
                Description = "Print all files in landscape mode",
                Copies = 1,
                PrinterSettings = printerSettings
            },
            new PrintProfileViewModel
            {
                Id = "2",
                Name = "Print Portrait",
                Description = "Print all files in portrait mode",
                Copies = 3,
                PrinterSettings = printerSettings
            },
            new PrintProfileViewModel
            {
                Id = "3",
                Name = "A3 Black & White",
                Description = "Make all A3 prints black and white",
                Copies = 5,
                PrinterSettings = printerSettings
            },
        ];

    }

    protected override void OnDesignTimeConstructor() => FetchPrintActionsData();

    [RelayCommand]
    private async Task DeletePrintItemAsync(string id)
    {
        // TODO: Pass this logic to a service that handles the database/storage/fetching
        //       For now just do it direct in here
        if (PrintList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;

        await DeletePrintItemFromUIAsync(id);
    }

    [RelayCommand]
    private async Task DeletePrintSettingsAsync(string id)
    {
        // TODO: Pass this logic to a service that handles the database/storage/fetching
        //       For now just do it direct in here
        if (PrinterProfilesList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;
        
        // TODO: Delete from database, then re-fetch to update UI
        //       1. Delete from database
        //       2. FetchPrintProfiles();

        await DeletePrinterProfileFromUIAsync(id);
    }

    [RelayCommand]
    private async Task EditPrintSettingsAsync(string id)
    {
        // TODO: Pass this logic to a service that handles database etc...

        var profileViewModel = PrinterProfilesList.FirstOrDefault(x => x.Id == id);

        if (profileViewModel == null)
            // TODO: Throw/Warn?
            return;

        // Copy view model
        var copiedProfileViewModel = new PrintProfileViewModel
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

        InjectPrinterDetails(copiedProfileViewModel);
        
        await dialogService.ShowDialogAsync(mainViewModel, copiedProfileViewModel);
        
        // Ignore if we clicked cancel
        if (!copiedProfileViewModel.IsConfirmed)
            return;
        
        // TODO: Database stuff
        
        // Commit copied view model back
        profileViewModel.RestoreState(copiedProfileViewModel.GetState());
    }

    private void InjectPrinterDetails(PrintProfileViewModel viewModel)
    {
        // Fetch live printers available on machine
        var availablePrinters = printerService.GetAvailablePrinters();
        var printerNameOptions = new ObservableCollection<string>(availablePrinters.Select(x => x.Name));

        foreach (var printerSettingsItem in viewModel.PrinterSettings)
        {
            printerSettingsItem.PrinterNameOptions = printerNameOptions;
            
            printerSettingsItem.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName != nameof(ActionsPrinterSettingsViewModel.PrinterName))
                    return;
                
                // Printer changed, update paper size and tray
                printerSettingsItem.PaperSizeOptions = new ObservableCollection<string>(
                    availablePrinters.FirstOrDefault(x => x.Name == printerSettingsItem.PrinterName)?.PaperSizes ?? []
                );
                
                printerSettingsItem.SourceTrayOptions = new ObservableCollection<string>(
                    availablePrinters.FirstOrDefault(x => x.Name == printerSettingsItem.PrinterName)?.SourceTrays ?? []
                );
                
                // Change paper size and source tray to first item
                printerSettingsItem.PaperSize = printerSettingsItem.PaperSizeOptions.FirstOrDefault() ?? "-";
                printerSettingsItem.SourceTray = printerSettingsItem.SourceTrayOptions.FirstOrDefault() ?? "-";
            };
        }
    }

    [RelayCommand]
    private void AddNewPrintItem()
    {
        // Crate a new item
        var newItem = new ActionsPrintViewModel
        {
            Id = Guid.NewGuid().ToString("N"),
            JobName = "New Print Item",
            IsNewItem = true,
            PrinterProfileId = "0",
        };

        // Add to the print list
        PrintList.Add(newItem);

        // Select item
        SelectedPrintListItemId = newItem.Id;
    }

    [RelayCommand]
    private async Task AddNewPrintSettingsAsync()
    {
        var confirmDialogViewModel = new PrintProfileViewModel()
        {
            Title = "New Printer Settings",
            // 图标方式一：
            // GeometryIcon = GeometryIcon.PrinterPosCog,                  // 不需要了，内部构造函数已有 IconGeometry 与 IconForeground 替代
            // // 图标方式二：
            // // IconMessage = "PrinterPosCog";
            // // IconForeground = "DodgerBlue";
            // // IconGeometry = StreamGeometry.Parse("M505.6512 39.0144c-261.2224 3.4816-470.1184 218.112-466.6368 479.4368 3.4816 261.12 218.112 470.1184 479.3344 466.6368 261.2224-3.4816 470.1184-218.112 466.7392-479.3344C981.504 244.4288 766.8736 35.5328 505.6512 39.0144zM558.08 196.608c48.128 0 62.2592 27.9552 62.2592 59.8016 0 39.8336-31.9488 76.6976-86.3232 76.6976-45.568 0-67.1744-22.9376-65.9456-60.8256C468.0704 240.4352 494.7968 196.608 558.08 196.608zM434.7904 807.6288c-32.8704 0-56.9344-19.968-33.8944-107.6224l37.6832-155.5456c6.5536-24.8832 7.68-34.9184 0-34.9184-9.8304 0-52.5312 17.2032-77.7216 34.2016l-16.384-26.9312c79.9744-66.7648 171.8272-105.8816 211.2512-105.8816 32.8704 0 38.2976 38.912 21.9136 98.6112l-43.2128 163.5328c-7.68 28.8768-4.4032 38.912 3.2768 38.912 9.9328 0 42.1888-11.9808 73.9328-36.9664l18.6368 24.8832C552.5504 777.728 467.6608 807.6288 434.7904 807.6288z");

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
        
        InjectPrinterDetails(confirmDialogViewModel);
        
        // Wait for click button
        await dialogService.ShowDialogAsync(mainViewModel, confirmDialogViewModel);
            
        // Ignore if we clicked cancel
        if (!confirmDialogViewModel.IsConfirmed)
            return;
        
        PrinterProfilesList.Add(confirmDialogViewModel);
    }

    [RelayCommand]
    private async Task CancelPrintItemAsync()
    {
        // Ignore if nothing is selected
        if (SelectedPrintListItem == null)
            return;

        // If the selected item is new, delete it
        // Otherwise, restore from save state
        if (SelectedPrintListItem.IsNewItem)
            await DeletePrintItemFromUIAsync(SelectedPrintListItem.Id, false);
        else
            SelectedPrintListItem.RestoreState();
    }

    // ReSharper disable once InconsistentNaming
    private async Task DeletePrintItemFromUIAsync(string id, bool warn = true)
    {
        var index = PrintList.IndexOf(PrintList.First(x => x.Id == id));
        if (index == -1)
            return;
        
        if (warn)
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
                Message = $"Are you sure you want to delete {PrintList[index].JobName}?",
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
                return;
        }
        
        // Remove item
        PrintList.RemoveAt(index);

        // Select the item before the deleted one
        if (index > 0)
            index--;
        if (PrintList.Count > 0)
            SelectedPrintListItemId = PrintList[index].Id;
    }

    // ReSharper disable once InconsistentNaming
    private async Task DeletePrinterProfileFromUIAsync(string id, bool warn = true)
    {
        var index = PrinterProfilesList.IndexOf(PrinterProfilesList.First(x => x.Id == id));
        if (index == -1)
            return;
        
        if (warn)
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
                Message = $"Are you sure you want to delete {PrinterProfilesList[index].Name}?",
                DialogWidth = 500,
            };
            
            // Wait for click button
            await dialogService.ShowDialogAsync(mainViewModel, confirmDialogViewModel);
            
            // Ignore if we clicked cancel
            if (!confirmDialogViewModel.IsConfirmed)
                return;
        }
        
        // Remove item
        PrinterProfilesList.RemoveAt(index);

        // Select the item before the deleted one
        if (index > 0)
            index--;
        if (PrinterProfilesList.Count > 0)
            SelectedPrintListItem!.PrinterProfileId = PrinterProfilesList[index].Id;
    }
}