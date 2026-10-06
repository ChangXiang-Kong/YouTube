using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BatchProcess3.Data;
using BatchProcess3.Tools.Services;
using BatchProcess3.ViewModels.Actions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BatchProcess3.ViewModels;

public interface ISelectableItemsListViewModel
{
    string Id { get; set; }
    
    string JobName { get; set; }
    
    bool IsNewItem { get; set; }
    
    void SetSaveState();
    
    void RestoreState(string? stateToRestore = null);
}

public partial class SelectableItemsListViewModel<TViewModel>(
    MainViewModel mainViewModel,
    DialogService dialogService,
    string title,
    Func<ObservableCollection<TViewModel>> getList,
    Func<TViewModel> createItem,
    Action<string> deleteItem,
    Action<TViewModel> addItem,
    Action<TViewModel> updateItem) : ViewModelBase
    where TViewModel : class, ISelectableItemsListViewModel
{
    // 使用 [] 进行初始化以消除警告，当误写 PrintList = null; 时会提示 Cannot convert null literal to non-nullable reference type
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ListHasItems))]
    private ObservableCollection<TViewModel> _itemsList = [];
    
    // 因为 PrintList 是 ObservableCollection 类型，
    // 需要添加 PrintList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(PrintListHasItems)); 才能生效
    public bool ListHasItems => ItemsList.Any();
    
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(SelectedItem))]
    private string _selectedItemId = "";
    
    public TViewModel? SelectedItem => ItemsList.FirstOrDefault(x => x.Id == SelectedItemId);
    
    
    
    [RelayCommand]
    public void FetchList()
    {
        ItemsList = getList();
        
        // Update FileInfoListHasItems when collection changes
        ItemsList.CollectionChanged += (_, _) => OnPropertyChanged(nameof(ListHasItems));

        if (ItemsList.Count <= 0)
            return;
        
        // Select first item
        SelectedItemId = ItemsList.First().Id;

        // Store last fetched database save states
        foreach (var item in ItemsList)
            item.SetSaveState();
    }

    [RelayCommand]
    private void AddNewItem()
    {
        // Crate a new item
        var newItem = createItem();

        // Add to the print list
        ItemsList.Add(newItem);

        // Select item
        SelectedItemId = newItem.Id;
    }

    [RelayCommand]
    private async Task DeleteItemAsync(string id)
    {
        if (ItemsList.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;

        // If user selected to remove from UI (via confirm dialog)
        if (await DeleteItemFromUIAsync(id))
            deleteItem(id);
    }

    // ReSharper disable once InconsistentNaming
    private async Task<bool> DeleteItemFromUIAsync(string id, bool popupDialog = true)
    {
        var index = ItemsList.IndexOf(ItemsList.First(x => x.Id == id));
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
                Title = $"Delete {title} Item?",
                Message = $"Are you sure you want to delete {ItemsList[index].JobName}?",
                DialogWidth = 500,
            };
            
            // Wait for click button
            await dialogService.ShowDialogAsync(mainViewModel, confirmDialogViewModel);
            
            // Ignore if we clicked cancel
            if (!confirmDialogViewModel.IsConfirmed)
                return false;
        }
        
        // Remove item
        ItemsList.RemoveAt(index);

        // Select the item before the deleted one
        if (index > 0)
            index--;
        if (ItemsList.Count > 0)
            SelectedItemId = ItemsList[index].Id;

        return true;
    }

    [RelayCommand]
    public Task SaveItemAsync()
    {
        // Ignore if no selection
        if (SelectedItem == null)
            return Task.CompletedTask;
        
        // If the selected item is new
        if (SelectedItem.IsNewItem)
            addItem(SelectedItem);
        else
            updateItem(SelectedItem);

        // Flag new item as not new
        SelectedItem.IsNewItem = false;
        // 保存状态以隐藏 Save 按钮
        SelectedItem.SetSaveState();
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task CancelItemAsync()
    {
        // Ignore if nothing is selected
        if (SelectedItem == null)
            return;

        // If the selected item is new, delete it
        // Otherwise, restore from save state
        if (SelectedItem.IsNewItem)
            await DeleteItemFromUIAsync(SelectedItem.Id, false);
        else
            SelectedItem.RestoreState();
    }
    
}