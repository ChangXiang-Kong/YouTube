using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvaloniaUIRealWorld.Data.EnumValues;

namespace AvaloniaUIRealWorld.ViewModels
{
    public partial class SettingsPageViewModel : PageViewModel
    {
        public SettingsPageViewModel()
        {
            PageName = ApplicationPageNames.Settings;
            LoadSettings();
        }

        [ObservableProperty] private string _Test = "Test Settings";

        [ObservableProperty] private ObservableCollection<string> _locationPaths = [];


        private void LoadSettings()
        {
            LocationPaths.Add(@"c:\\Dir\Test1.txt");
            LocationPaths.Add(@"c:\\Dir\Test2.txt");
            LocationPaths.Add(@"c:\\Dir\Test3.txt");
        }
        // private void LoadSettings()
        // {
        //     // Get settings from database
        //     using var dbContext = _factory.GetDatabaseService();
        //     var settings = dbContext.GetSettings();
        //
        //     // Update view model
        //     LocationPaths = new ObservableCollection<string>(dbContext.GetSettings().LocationPaths ?? []);
        //     SolidWorksHost = settings.SolidWorksHost;
        //     SkipNoActionFiles = settings.SkipNoActionFiles;
        //     AllowDuplicateEntries = settings.AllowDuplicateEntries;
        //     PdmePassword = settings.PdmePassword;
        //     PdmeVaultName = settings.PdmeVaultName;
        //     PdmeUsername = settings.PdmeUsername;
        // }
    }
}