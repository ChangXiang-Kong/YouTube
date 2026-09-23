using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BatchProcess3.Data;
using BatchProcess3.EntityFramework;
using BatchProcess3.EntityFramework.Entities;
using BatchProcess3.Tools.Factories;
using BatchProcess3.Tools.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BatchProcess3.ViewModels.MainMenus
{
    public partial class SettingsPageViewModel : PageViewModel
    {
        // Design-time constructor
        public SettingsPageViewModel() : this(new DialogService(() => null), new DatabaseFactory(() => new DatabaseService(new AppDbContext())))
        {
        }
        
        public SettingsPageViewModel(DialogService dialogService, DatabaseFactory databaseFactory) : base(ApplicationPageName.Settings)
        {
            _dialogService = dialogService;
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
        
        private readonly DialogService _dialogService;
        private readonly DatabaseFactory _databaseFactory;

        [ObservableProperty] private string _test = "Test Settings";
        
        [ObservableProperty] private bool _skipNoActionFiles;
        [ObservableProperty] private bool _allowDuplicateEntries;
        [ObservableProperty] private ObservableCollection<string> _locationPaths = [];
        [ObservableProperty] private string _solidWorksHost = "";
        // TODO: Fetch from network pings
        [ObservableProperty] private ObservableCollection<string> _solidWorksHosts = [ "localhost", "127.0.0.1", "192.168.0.10" ];
        // TODO: Fetch from PDME
        [ObservableProperty] private ObservableCollection<string> _pdmeVaultNames = [ "vault 1", "vault 2", "vault 3" ];
        [ObservableProperty] private string _pdmeVaultName = "";    // 若无默认值，保存到数据库时会报错：SQLite Error 19: 'NOT NULL constraint failed: Settings.PdmeVaultName'.
        [ObservableProperty] private string _pdmeUserName = "";
        [ObservableProperty] private string _pdmePassword = "";

        public override void OnViewLoaded()
        {
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName is nameof(SkipNoActionFiles) or nameof(AllowDuplicateEntries) or nameof(SolidWorksHost))
                    SaveSettings();
            };
        }

        [RelayCommand]
        private void DeleteLocationPath(string path)
        {
            LocationPaths.Remove(path);
            
            // Commit to database
            SaveSettings();
        }

        [RelayCommand]
        private async Task AddLocationPath()
        {
            var res = await _dialogService.ShowSelectFolderDialogAsync();
            
            // Dot not add if duplicate or cancelled
            if (res == null || LocationPaths.Any(x => string.Equals(x, res, StringComparison.InvariantCultureIgnoreCase)))
                return;
            
            // Add to locations
            LocationPaths.Add(res);
            
            // Sort alphabetically
            LocationPaths = new ObservableCollection<string>(LocationPaths.Order());
            
            // Save to database
            SaveSettings();
        }

        [RelayCommand]
        private void PdmeLogin()
        {
            // TODO: Login to PDME
            
            // Save to database
            SaveSettings();
        }

        /// <summary>
        /// Update view model from settings stored in database
        /// </summary>
        private void LoadSettings()
        {
            // Get settings from database
            using var dbContext = _databaseFactory.GetDatabaseService();
            var settings = dbContext.GetSettings();
        
            // Update view model
            SolidWorksHost = settings.SolidWorksHost;
            SkipNoActionFiles = settings.SkipNoActionFiles;
            LocationPaths = new ObservableCollection<string>(dbContext.GetSettings().LocationPaths);
            AllowDuplicateEntries = settings.AllowDuplicateEntries;
            PdmePassword = settings.PdmePassword;
            PdmeVaultName = settings.PdmeVaultName;
            PdmeUserName = settings.PdmeUserName;
        }

        private int SaveSettings()
        {
            using var dbContext = _databaseFactory.GetDatabaseService();
            return dbContext.SaveSettings(ToEntity());
        }

        private SettingsEntity ToEntity() => new()
        {
            SkipNoActionFiles = SkipNoActionFiles,
            AllowDuplicateEntries = AllowDuplicateEntries,
            LocationPaths = LocationPaths.ToList(),
            SolidWorksHost = SolidWorksHost,
            PdmeVaultName = PdmeVaultName,
            PdmeUserName = PdmeUserName,
            PdmePassword = PdmePassword,
        };
    }
}