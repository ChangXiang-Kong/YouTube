using System;
using System.Collections.Generic;
using System.Linq;
using BatchProcess3.EntityFramework;
using BatchProcess3.EntityFramework.Entities;
using BatchProcess3.EntityFramework.Entities.Actions;
using Microsoft.EntityFrameworkCore;

namespace BatchProcess3.Tools.Services;

public class DatabaseService(AppDbContext dbContext) : IDisposable
{
    private readonly AppDbContext _dbContext = dbContext;

    public void ApplyMigrations()
    {
        // TODO: Change to migrations once we start persisting data
        _dbContext.Database.EnsureCreated();
    }
    
    #region Settings
    public SettingsEntity GetSettings()
    {
        var entity = _dbContext.Settings.FirstOrDefault();
        
        if (entity != null)
            return entity;
        
        // If we have no settings, generate default
        entity = new SettingsEntity
        {
            SkipNoActionFiles =  true,
            LocationPathsList = ["Initial Path 1", "Initial Path 2", "Initial Path 3",]
        };
        
        AddSettings(entity);
        
        return entity;
    }

    public bool AddSettings(SettingsEntity entity)
    {
        // Remove all settings
        _dbContext.Settings.RemoveRange(_dbContext.Settings);
        
        _dbContext.Settings.Add(entity);
        
         return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteSettings(string id)
    {
        // if (!Guid.TryParse(id, out Guid guid))
        //     throw  new ArgumentException("Invalid print tab id");

        // Remove existing
        var existingEntity = _dbContext.Settings.FirstOrDefault(x => x.Id == id);
        if (existingEntity == null)
            return false;
        
        _dbContext.Settings.Remove(existingEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool UpdateSettings(SettingsEntity entity)
    {
        // Remove existing
        if (!DeleteSettings(entity.Id))
            return false;
        
        // Add new
        return AddSettings(entity);
    }
    #endregion Settings

    #region PrintTab
    public List<PrintTabEntity> GetPrintTab()
    {
        var res = _dbContext.PrintTab.ToList();

        if (!res.Any())
        {
            // Ensure we have at least one print settings
            GetPrintSettings();
            
            // Create a default item
            // res.Add(new PrintTabEntity()    // 报错：Sequence contains no elements
            _dbContext.PrintTab.Add(new PrintTabEntity()
            {
                JobName = "Print Only Drawings",
                Description = "Prints only drawing files",
                PrintDrawingRange = "0, 5, 7-8",
                DrawingExclusionIsWhiteList = true,
                PrintModels = false,
                PrintDrawings = true,
                DrawingExclusionList = $"Some item 1{Environment.NewLine}Some item 2{Environment.NewLine}Some item 3",
                PrintSettingsId = _dbContext.PrintSettings.First().Id,
            });
            
            // Save changes to database
            _dbContext.SaveChanges();
        }
        
        return res;
    }

    public bool AddPrintTab(PrintTabEntity entity)
    {
        _dbContext.PrintTab.Add(entity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeletePrintTab(string id)
    {
        // if (!Guid.TryParse(id, out Guid guid))
        //     throw  new ArgumentException("Invalid print tab id");

        // Remove existing
        var existingEntity = _dbContext.PrintTab.FirstOrDefault(x => x.Id == id);
        if (existingEntity == null)
            return false;
        
        _dbContext.PrintTab.Remove(existingEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool UpdatePrintTab(PrintTabEntity entity)
    {
        // Remove existing
        if (!DeletePrintTab(entity.Id))
            return false;
        
        // Add new
        return AddPrintTab(entity);
    }
    #endregion PrintTab
    
    #region PrintSettings
    public List<PrintSettingsProfileEntity> GetPrintSettingsProfiles()
    {
        return
        [
            new PrintSettingsProfileEntity(){ Type = "A0Size" },
            new PrintSettingsProfileEntity(){ Type = "A1Size" },
            new PrintSettingsProfileEntity(){ Type = "A2Size" },
            new PrintSettingsProfileEntity(){ Type = "A3Size" },
            new PrintSettingsProfileEntity(){ Type = "A4Size" },
            new PrintSettingsProfileEntity(){ Type = "A4VerticalSize" },
            new PrintSettingsProfileEntity(){ Type = "ASize" },
            new PrintSettingsProfileEntity(){ Type = "AVerticalSize" },
            new PrintSettingsProfileEntity(){ Type = "BSize" },
            new PrintSettingsProfileEntity(){ Type = "CSize" },
            new PrintSettingsProfileEntity(){ Type = "DSize" },
            new PrintSettingsProfileEntity(){ Type = "ESize" },
            new PrintSettingsProfileEntity(){ Type = "UserSize1" },
            new PrintSettingsProfileEntity(){ Type = "UserSize2" },
            new PrintSettingsProfileEntity(){ Type = "UserSize3" },
            new PrintSettingsProfileEntity(){ Type = "UserSize4" },
            new PrintSettingsProfileEntity(){ Type = "UserSize5" },
            new PrintSettingsProfileEntity(){ Type = "UserSize6" },
            new PrintSettingsProfileEntity(){ Type = "UserSize7" },
            new PrintSettingsProfileEntity(){ Type = "UserSize8" },
            new PrintSettingsProfileEntity(){ Type = "UserSize9" },
            new PrintSettingsProfileEntity(){ Type = "UserSize10" },
            new PrintSettingsProfileEntity(){ Type = "UserSize11" },
            new PrintSettingsProfileEntity(){ Type = "UserSize12" },
        ];
    }
    
    public List<PrintSettingsEntity> GetPrintSettings()
    {
        var res = _dbContext.PrintSettings
            // 注意：需要调用 Include() 才能自动获取 PrintSettingsProfilesList
            .Include(x => x.PrintSettingsProfilesList)
            .ToList();

        if (!res.Any())
        {
            // Add default settings
            // res.Add(new PrintSettingsEntity()   // 报错：Sequence contains no elements
            _dbContext.PrintSettings.Add(new PrintSettingsEntity()
            {
                Name = "(Default)",
                Description = "Use all default settings",
                Copies = 1,
                PrintSettingsProfilesList = GetPrintSettingsProfiles()
            });

            // Save changes to database
            _dbContext.SaveChanges();
        }
        
        return res;
    }

    public bool AddPrintSettings(PrintSettingsEntity entity)
    {
        _dbContext.PrintSettings.Add(entity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeletePrintSettings(string id, bool bypass = false, bool saveChanges = true)
    {
        // if (!Guid.TryParse(id, out Guid guid))
        //     throw  new ArgumentException("Invalid print tab id");

        // Remove existing
        var existingEntity = _dbContext.PrintSettings.FirstOrDefault(x => x.Id == id);
        if (existingEntity == null)
            return false;
        
        // If this item is not deletable
        if (!bypass && !existingEntity.CanDelete)
            throw new InvalidOperationException($"This print setting cannot be deleted. {existingEntity.Name}");
        
        _dbContext.PrintSettings.Remove(existingEntity);
        if (saveChanges)
            return _dbContext.SaveChanges() > 0;
        return true;
    }

    public bool UpdatePrintSettings(PrintSettingsEntity entity)
    {
        // If it is not editable
        if (!entity.CanEdit)
            throw new InvalidOperationException($"This print setting cannot be edited. {entity.Name}");
        
        // Remove existing
        if (!DeletePrintSettings(entity.Id, bypass: true, saveChanges: false))
            return false;
        
        // Add new
        return AddPrintSettings(entity);
    }
    #endregion PrintSettings

    

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}