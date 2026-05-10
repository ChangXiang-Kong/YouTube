using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AvaloniaApplication1.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.ViewModels;

public partial class StylePreviewPageViewModel() : PageViewModel(ApplicationPageName.StylePreview)
{
    [ObservableProperty] 
    private string _greeting = "Welcome to Avalonia!";
    
    [ObservableProperty]
    private MenuItem? _selectedMenuItem;
    
    public ObservableCollection<MenuItem> MenuItems { get; set; } = new ObservableCollection<MenuItem>
    {
        new MenuItem { Header = "Introduction" , Children =
        {
            new MenuItem() { Header = "Getting Started", Children =
            {
                new MenuItem() { Header = "Code of Conduct" },
                new MenuItem() { Header = "How to Contribute" },
                new MenuItem() { Header = "Development Workflow" },
            }},
            new MenuItem() { Header = "Design Principles"},
            new MenuItem() { Header = "Contributing", Children =
            {
                new MenuItem() { Header = "Code of Conduct" },
                new MenuItem() { Header = "How to Contribute" },
                new MenuItem() { Header = "Development Workflow" },
            }},
        }},
        new MenuItem { Header = "Controls", IsSeparator = true},
        new MenuItem { Header = "Badge" },
        new MenuItem { Header = "Banner" },
        new MenuItem { Header = "ButtonGroup" },
        new MenuItem { Header = "Class Input" },
        new MenuItem { Header = "Dialog" },
        new MenuItem { Header = "Divider" },
        new MenuItem { Header = "Drawer" },
        new MenuItem { Header = "DualBadge" },
        new MenuItem { Header = "EnumSelector" },
        new MenuItem { Header = "ImageViewer" },
        new MenuItem { Header = "IPv4Box" },
        new MenuItem { Header = "IconButton" },
        new MenuItem { Header = "KeyGestureInput" },
        new MenuItem { Header = "Loading" },
        new MenuItem { Header = "MessageBox" },
        new MenuItem { Header = "Navigation" },
        new MenuItem { Header = "NavMenu" },
        new MenuItem { Header = "NumericUpDown" },
        new MenuItem { Header = "Pagination" },
        new MenuItem { Header = "RangeSlider" },
        new MenuItem { Header = "SelectionList" },
        new MenuItem { Header = "TagInput" },
        new MenuItem { Header = "Timeline" },
        new MenuItem { Header = "TwoTonePathIcon" },
        new MenuItem { Header = "ThemeToggler" }
    };

    [RelayCommand]
    private void RandomSelectMenuItem()
    {
        var items = GetLeaves();
        var index = new Random().Next(items.Count);
        SelectedMenuItem = items[index];
    }
    
    private List<MenuItem> GetLeaves()
    {
        List<MenuItem> items = new();
        foreach (var item in MenuItems)
        {
            items.AddRange(item.GetLeaves());
        }

        return items;
    }
    
    
    
}