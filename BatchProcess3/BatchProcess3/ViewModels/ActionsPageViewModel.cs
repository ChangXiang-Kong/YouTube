using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatchProcess3.Data;
using BatchProcess3.ViewModels.Actions;
using CommunityToolkit.Mvvm.Input;

namespace BatchProcess3.ViewModels;

public partial class ActionsPageViewModel() : PageViewModel(ApplicationPageName.Actions)
{
    // 使用上面的方式替代以下方式构造函数
    // public ActionsPageViewModel() : base(ApplicationPageName.Actions)
    // {
    //     // Some logic
    // }

    [ObservableProperty] private string _test = "Test Actions";

    // TODO: Remove once we have database service
    private ActionsPrinterProfileViewModel _defaultPrinterProfile = new ActionsPrinterProfileViewModel
    {
        Id = "0",
        Name = "(Default)",
        Description = "Use all default settings",
        Copies = 1,
        // TODO: Populate PrinterSettings
    };

    // 使用 [] 进行初始化以消除警告，当误写 PrintList = null; 时会提示 Cannot convert null literal to non-nullable reference type
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(PrintListHasItems))]
    private ObservableCollection<ActionsPrintViewModel> _printList = [];

    // 因为 PrintList 是 ObservableCollection 类型，
    // 需要添加 PrintList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(PrintListHasItems)); 才能生效
    public bool PrintListHasItems => PrintList.Any();

    [ObservableProperty] private ActionsPrintViewModel? _selectedPrintListItem;

    [ObservableProperty] private ObservableCollection<ActionsPrinterProfileViewModel> _printerProfilesList = [];

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
                PrinterProfile = _defaultPrinterProfile,
            },
            new ActionsPrintViewModel
            {
                Id = "2",
                JobName = "Print All Drawings Scale To Fit",
                Description = "Prints drawing scaled to fit the paper",
                PrintModels = true,
                PrintDrawings = false,
                PrinterProfile = _defaultPrinterProfile,
            },
            new ActionsPrintViewModel
            {
                Id = "3",
                JobName = "Print 3D Models A3",
                Description = "Prints models as 3D visuals",
                PrintModels = false,
                PrintDrawings = true,
                PrinterProfile = _defaultPrinterProfile,
            },
        ];
        
        // Update PrintListHasItems when collection changes
        PrintList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(PrintListHasItems));
        
        if (PrintList.Count > 0)
        {
            // Select first item
            PrintList.First().IsSelected = true;
            
            // Store last fetched database save states
            foreach (var printItem in PrintList)
                printItem.SetSaveState();
        }

        PrinterProfilesList =
        [
            _defaultPrinterProfile,
            new ActionsPrinterProfileViewModel
            {
                Name = "Print Landscape",
                Description = "Print all files in landscape mode",
                Copies = 1,
                // TODO: Populate PrinterSettings
            },
            new ActionsPrinterProfileViewModel
            {
                Name = "Print Portrait",
                Description = "Print all files in portrait mode",
                Copies = 3,
                // TODO: Populate PrinterSettings
            },
            new ActionsPrinterProfileViewModel
            {
                Name = "A3 Black & White",
                Description = "Make all A3 prints black and white",
                Copies = 5,
                // TODO: Populate PrinterSettings
            },
        ];
    }

    protected override void OnDesignTimeConstructor() => FetchPrintActionsData();

    [RelayCommand]
    private void DeletePrintItem(string id)
    {
        // TODO: Pass this logic to a service that handles the database/storage/fetching
        //       For now just do it direct in here
        if (PrintList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;
        
        DeletePrintItemFromUI(id);
    }

    [RelayCommand]
    private void AddNewPrintItem()
    {
        // Crate a new item
        var newItem = new ActionsPrintViewModel
        {
            Id = Guid.NewGuid().ToString("N"),
            JobName = "New Print Item",
            IsSelected = true,
            IsNewItem = true,
            PrinterProfile = _defaultPrinterProfile,
        };

        // Add to the print list
        PrintList.Add(newItem);
    }

    [RelayCommand]
    private void CancelPrintItem()
    {
        // Ignore if nothing is selected
        if (SelectedPrintListItem == null)
            return;
            
        // If the selected item is new, delete it
        // Otherwise, restore from save state
        if (SelectedPrintListItem.IsNewItem)
            DeletePrintItemFromUI(SelectedPrintListItem.Id);
    }

    private void DeletePrintItemFromUI(string id)
    {
        // Remove item
        var index = PrintList.IndexOf((PrintList.First(x => x.Id == id)));
        PrintList.RemoveAt(index);

        // Select the item before the deleted one
        if (index > 0)
            index--;
        if (PrintList.Count > 0)
            PrintList[index].IsSelected = true;
    }
    
}