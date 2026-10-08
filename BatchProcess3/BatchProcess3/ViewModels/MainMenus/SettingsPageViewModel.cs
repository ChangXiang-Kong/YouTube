using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BatchProcess3.Data;
using BatchProcess3.EntityFramework;
using BatchProcess3.EntityFramework.Entities;
using BatchProcess3.Tools.Dialog;
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
        [ObservableProperty] private ObservableCollection<string> _locationPathsList = [];
        [ObservableProperty] private ObservableCollection<string> _drawingTemplatePaths = [];
        [ObservableProperty] private string _solidWorksHost = "";
        // TODO: Fetch from network pings
        [ObservableProperty] private ObservableCollection<string> _solidWorksHostsList = [ "localhost", "127.0.0.1", "192.168.0.10" ];
        // TODO: Fetch from PDME
        [ObservableProperty] private ObservableCollection<string> _pdmeVaultNamesList = [ "vault 1", "vault 2", "vault 3" ];
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
            LocationPathsList.Remove(path);
            
            // Commit to database
            SaveSettings();
        }

        [RelayCommand]
        private async Task AddLocationPath()
        {
            var res = await _dialogService.ShowSelectFolderDialogAsync(
                title: "Select a Folder", 
                allowMultiple: true);
            
            // Dot not add if duplicate or cancelled
            var newPaths = res.Where(path => !LocationPathsList.Any(x => string.Equals(x, path, StringComparison.InvariantCultureIgnoreCase))).ToList();
    
            if (newPaths.Count == 0)
                return;

            // Add to locations
            foreach (var path in newPaths)
                LocationPathsList.Add(path);
            
            // Sort alphabetically
            LocationPathsList = new ObservableCollection<string>(LocationPathsList.Order());
            
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
            LocationPathsList = new ObservableCollection<string>(dbContext.GetSettings().LocationPathsList);
            AllowDuplicateEntries = settings.AllowDuplicateEntries;
            PdmePassword = settings.PdmePassword;
            PdmeVaultName = settings.PdmeVaultName;
            PdmeUserName = settings.PdmeUserName;
        }

        private bool SaveSettings()
        {
            using var dbContext = _databaseFactory.GetDatabaseService();
            return dbContext.AddSettingsItem(ToEntity());
        }

        private SettingsEntity ToEntity() => new()
        {
            SkipNoActionFiles = SkipNoActionFiles,
            AllowDuplicateEntries = AllowDuplicateEntries,
            LocationPathsList = LocationPathsList.ToList(),
            DrawingTemplatePaths = DrawingTemplatePaths.ToList(),
            SolidWorksHost = SolidWorksHost,
            PdmeVaultName = PdmeVaultName,
            PdmeUserName = PdmeUserName,
            PdmePassword = PdmePassword,
        };
    }
}