using System;
using System.Linq;
using BatchProcess3.EntityFramework;
using BatchProcess3.EntityFramework.Entities;

namespace BatchProcess3.Tools.Services;

public class DatabaseService(AppDbContext dbContext) : IDisposable
{
    private readonly AppDbContext _dbContext = dbContext;

    public void ApplyMigrations()
    {
        // TODO: Change to migrations once we start persisting data
        _dbContext.Database.EnsureCreated();
    }

    public SettingsEntity GetSettings()
    {
        var settings = _dbContext.Settings.FirstOrDefault();
        
        if (settings != null)
            return settings;
        
        // If we have no settings, generate default
        settings = new SettingsEntity
        {
            SkipNoActionFiles =  true,
            LocationPaths = ["Initial Path 1", "Initial Path 2", "Initial Path 3",]
        };
        
        SaveSettings(settings);
        
        return settings;
    }

    public int SaveSettings(SettingsEntity entity)
    {
        // Remove all settings
        _dbContext.Settings.RemoveRange(_dbContext.Settings);
        
        _dbContext.Settings.Add(entity);
        
         return _dbContext.SaveChanges();
    }


    public void Dispose()
    {
        _dbContext.Dispose();
    }
}