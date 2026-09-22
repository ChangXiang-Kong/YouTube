using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatchProcess3.Data;
using BatchProcess3.Tools.Factories;

namespace BatchProcess3.ViewModels
{
    public partial class SettingsPageViewModel : PageViewModel
    {
        public SettingsPageViewModel(DatabaseFactory databaseFactory) : base(ApplicationPageName.Settings)
        {
            _databaseFactory = databaseFactory;
            LoadSettings();
        }
        // 使用上面的方式替代以下方式构造函数
        // public SettingsPageViewModel(DatabaseFactory databaseFactory)
        // {
        //     _databaseFactory = databaseFactory;
        //     PageName = ApplicationPageName.Settings;
        //     LoadSettings();
        // }
        
        private readonly DatabaseFactory _databaseFactory;

        [ObservableProperty]
        private string _test = "Test Settings";

        [ObservableProperty]
        private ObservableCollection<string> _locationPaths = [];

        private void LoadSettings()
        {
            using var dbContext = _databaseFactory.GetDatabaseService();
            // LocationPaths = dbContext.GetSettings()?.LocationPaths ?? [];
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