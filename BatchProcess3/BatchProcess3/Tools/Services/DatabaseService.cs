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
            LocationPathsList = ["Initial Path 1", "Initial Path 2", "Initial Path 3",],
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

    #region Actions
    
    #region PrintTab
    public List<ActionsTabPrintEntity> GetPrintTab()
    {
        var res = _dbContext.ActionsTabPrint.ToList();

        if (!res.Any())
        {
            // Ensure we have at least one print settings
            GetPrintSettings();
            
            // Create a default item
            // res.Add(new PrintTabEntity()    // 报错：Sequence contains no elements
            _dbContext.ActionsTabPrint.Add(new ActionsTabPrintEntity()
            {
                JobName = "Print Only Drawings",
                Description = "Prints only drawing files",
                PrintDrawingRange = "0, 5, 7-8",
                DrawingExclusionIsWhiteList = true,
                PrintModels = false,
                PrintDrawings = true,
                DrawingExclusionList = $"Some item 1{Environment.NewLine}Some item 2{Environment.NewLine}Some item 3",
                PrintSettingsId = _dbContext.ActionsPrintSettings.First().Id,
            });
            
            // Save changes to database
            _dbContext.SaveChanges();
            
            // Refresh from DB to include ID
            res = _dbContext.ActionsTabPrint.ToList();
        }
        
        return res;
    }

    public bool AddPrintTab(ActionsTabPrintEntity entity)
    {
        _dbContext.ActionsTabPrint.Add(entity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeletePrintTab(string id)
    {
        // if (!Guid.TryParse(id, out Guid guid))
        //     throw  new ArgumentException("Invalid print tab id");

        // Remove existing
        var existingEntity = _dbContext.ActionsTabPrint.FirstOrDefault(x => x.Id == id);
        if (existingEntity == null)
            return false;
        
        _dbContext.ActionsTabPrint.Remove(existingEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool UpdatePrintTab(ActionsTabPrintEntity entity)
    {
        // Remove existing
        if (!DeletePrintTab(entity.Id))
            return false;
        
        // Add new
        return AddPrintTab(entity);
    }
    #endregion PrintTab
    
    #region PrintSettings
    public List<ActionsPrintSettingsProfileEntity> GetPrintSettingsProfiles()
    {
        return
        [
            new ActionsPrintSettingsProfileEntity(){ Type = "A0Size" },
            new ActionsPrintSettingsProfileEntity(){ Type = "A1Size" },
            new ActionsPrintSettingsProfileEntity(){ Type = "A2Size" },
            new ActionsPrintSettingsProfileEntity(){ Type = "A3Size" },
            new ActionsPrintSettingsProfileEntity(){ Type = "A4Size" },
            new ActionsPrintSettingsProfileEntity(){ Type = "A4VerticalSize" },
            new ActionsPrintSettingsProfileEntity(){ Type = "ASize" },
            new ActionsPrintSettingsProfileEntity(){ Type = "AVerticalSize" },
            new ActionsPrintSettingsProfileEntity(){ Type = "BSize" },
            new ActionsPrintSettingsProfileEntity(){ Type = "CSize" },
            new ActionsPrintSettingsProfileEntity(){ Type = "DSize" },
            new ActionsPrintSettingsProfileEntity(){ Type = "ESize" },
            new ActionsPrintSettingsProfileEntity(){ Type = "UserSize1" },
            new ActionsPrintSettingsProfileEntity(){ Type = "UserSize2" },
            new ActionsPrintSettingsProfileEntity(){ Type = "UserSize3" },
            new ActionsPrintSettingsProfileEntity(){ Type = "UserSize4" },
            new ActionsPrintSettingsProfileEntity(){ Type = "UserSize5" },
            new ActionsPrintSettingsProfileEntity(){ Type = "UserSize6" },
            new ActionsPrintSettingsProfileEntity(){ Type = "UserSize7" },
            new ActionsPrintSettingsProfileEntity(){ Type = "UserSize8" },
            new ActionsPrintSettingsProfileEntity(){ Type = "UserSize9" },
            new ActionsPrintSettingsProfileEntity(){ Type = "UserSize10" },
            new ActionsPrintSettingsProfileEntity(){ Type = "UserSize11" },
            new ActionsPrintSettingsProfileEntity(){ Type = "UserSize12" },
        ];
    }
    
    public List<ActionsPrintSettingsEntity> GetPrintSettings()
    {
        var res = _dbContext.ActionsPrintSettings
            // 注意：需要调用 Include() 才能自动获取 PrintSettingsProfilesList
            .Include(x => x.PrintSettingsProfilesList)
            .ToList();

        if (!res.Any())
        {
            // Add default settings
            // res.Add(new PrintSettingsEntity()   // 报错：Sequence contains no elements
            _dbContext.ActionsPrintSettings.Add(new ActionsPrintSettingsEntity()
            {
                JobName = "(Default)",
                Description = "Use all default settings",
                Copies = 1,
                PrintSettingsProfilesList = GetPrintSettingsProfiles()
            });

            // Save changes to database
            _dbContext.SaveChanges();
            
            res =  _dbContext.ActionsPrintSettings.ToList();
        }
        
        return res;
    }

    public bool AddPrintSettings(ActionsPrintSettingsEntity entity)
    {
        _dbContext.ActionsPrintSettings.Add(entity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeletePrintSettings(string id, bool bypass = false, bool saveChanges = true)
    {
        // if (!Guid.TryParse(id, out Guid guid))
        //     throw  new ArgumentException("Invalid print tab id");

        // Remove existing
        var existingEntity = _dbContext.ActionsPrintSettings.FirstOrDefault(x => x.Id == id);
        if (existingEntity == null)
            return false;
        
        // If this item is not deletable
        if (!bypass && !existingEntity.CanDelete)
            throw new InvalidOperationException($"The print setting {existingEntity.JobName} cannot be deleted.");
        
        _dbContext.ActionsPrintSettings.Remove(existingEntity);
        if (saveChanges)
            return _dbContext.SaveChanges() > 0;
        return true;
    }

    public bool UpdatePrintSettings(ActionsPrintSettingsEntity entity)
    {
        // If it is not editable
        if (!entity.CanEdit)
            throw new InvalidOperationException($"The print setting {entity.JobName} cannot be edited.");
        
        // Remove existing
        if (!DeletePrintSettings(entity.Id, bypass: true, saveChanges: false))
            return false;
        
        // Add new
        return AddPrintSettings(entity);
    }
    #endregion PrintSettings

    #region Custom Properties
    public List<ActionsTabCustomPropertiesEntity> GetCustomProperties()
    {
        return _dbContext.ActionsTabCustomProperties.ToList();
    }

    public bool AddCustomProperty(ActionsTabCustomPropertiesEntity entity)
    {
        _dbContext.ActionsTabCustomProperties.Add(entity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteCustomProperty(string id)
    {
        // if (!Guid.TryParse(id, out Guid guid))
        //     throw  new ArgumentException("Invalid print tab id");

        // Remove existing
        var existingEntity = _dbContext.ActionsTabCustomProperties.FirstOrDefault(x => x.Id == id);
        if (existingEntity == null)
            return false;
        
        _dbContext.ActionsTabCustomProperties.Remove(existingEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool UpdateCustomProperty(ActionsTabCustomPropertiesEntity entity)
    {
        // Remove existing
        if (!DeleteCustomProperty(entity.Id))
            return false;
        
        // Add new
        return AddCustomProperty(entity);
    }
    #endregion Custom Properties

    #region File Info
    public List<ActionsTabFileInfoEntity> GetFileInfo()
    {
        return _dbContext.ActionsTabFileInfo.ToList();
    }

    public bool AddFileInfo(ActionsTabFileInfoEntity entity)
    {
        _dbContext.ActionsTabFileInfo.Add(entity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteFileInfo(string id)
    {
        // if (!Guid.TryParse(id, out Guid guid))
        //     throw  new ArgumentException("Invalid print tab id");

        // Remove existing
        var existingEntity = _dbContext.ActionsTabFileInfo.FirstOrDefault(x => x.Id == id);
        if (existingEntity == null)
            return false;
        
        _dbContext.ActionsTabFileInfo.Remove(existingEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool UpdateFileInfo(ActionsTabFileInfoEntity entity)
    {
        // Remove existing
        if (!DeleteFileInfo(entity.Id))
            return false;
        
        // Add new
        return AddFileInfo(entity);
    }
    #endregion File Info

    #region Save Model
    public List<ActionsTabSaveModelEntity> GetSaveModel()
    {
        return _dbContext.ActionsTabSaveModel.ToList();
    }

    public bool AddSaveModel(ActionsTabSaveModelEntity entity)
    {
        _dbContext.ActionsTabSaveModel.Add(entity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteSaveModel(string id)
    {
        // if (!Guid.TryParse(id, out Guid guid))
        //     throw  new ArgumentException("Invalid print tab id");

        // Remove existing
        var existingEntity = _dbContext.ActionsTabSaveModel.FirstOrDefault(x => x.Id == id);
        if (existingEntity == null)
            return false;
        
        _dbContext.ActionsTabSaveModel.Remove(existingEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool UpdateSaveModel(ActionsTabSaveModelEntity entity)
    {
        // Remove existing
        if (!DeleteSaveModel(entity.Id))
            return false;
        
        // Add new
        return AddSaveModel(entity);
    }
    #endregion Save Model

    #region Save Drawing
    public List<ActionsTabSaveDrawingEntity> GetSaveDrawing()
    {
        return _dbContext.ActionsTabSaveDrawing.ToList();
    }

    public bool AddSaveDrawing(ActionsTabSaveDrawingEntity entity)
    {
        _dbContext.ActionsTabSaveDrawing.Add(entity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteSaveDrawing(string id)
    {
        // if (!Guid.TryParse(id, out Guid guid))
        //     throw  new ArgumentException("Invalid print tab id");

        // Remove existing
        var existingEntity = _dbContext.ActionsTabSaveDrawing.FirstOrDefault(x => x.Id == id);
        if (existingEntity == null)
            return false;
        
        _dbContext.ActionsTabSaveDrawing.Remove(existingEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool UpdateSaveDrawing(ActionsTabSaveDrawingEntity entity)
    {
        // Remove existing
        if (!DeleteSaveDrawing(entity.Id))
            return false;
        
        // Add new
        return AddSaveDrawing(entity);
    }
    #endregion Save Drawing

    #region Import File
    public List<ActionsTabImportFileEntity> GetImportFile()
    {
        return _dbContext.ActionsTabImportFile.ToList();
    }

    public bool AddImportFile(ActionsTabImportFileEntity entity)
    {
        _dbContext.ActionsTabImportFile.Add(entity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteImportFile(string id)
    {
        // if (!Guid.TryParse(id, out Guid guid))
        //     throw  new ArgumentException("Invalid print tab id");

        // Remove existing
        var existingEntity = _dbContext.ActionsTabImportFile.FirstOrDefault(x => x.Id == id);
        if (existingEntity == null)
            return false;
        
        _dbContext.ActionsTabImportFile.Remove(existingEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool UpdateImportFile(ActionsTabImportFileEntity entity)
    {
        // Remove existing
        if (!DeleteImportFile(entity.Id))
            return false;
        
        // Add new
        return AddImportFile(entity);
    }
    #endregion Import File

    #region Drawing Templates
    public List<ActionsTabDrawingTemplateEntity> GetDrawingTemplate()
    {
        return _dbContext.ActionsTabDrawingTemplate.ToList();
    }

    public bool AddDrawingTemplate(ActionsTabDrawingTemplateEntity entity)
    {
        _dbContext.ActionsTabDrawingTemplate.Add(entity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteDrawingTemplate(string id)
    {
        // if (!Guid.TryParse(id, out Guid guid))
        //     throw  new ArgumentException("Invalid print tab id");

        // Remove existing
        var existingEntity = _dbContext.ActionsTabDrawingTemplate.FirstOrDefault(x => x.Id == id);
        if (existingEntity == null)
            return false;
        
        _dbContext.ActionsTabDrawingTemplate.Remove(existingEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool UpdateDrawingTemplate(ActionsTabDrawingTemplateEntity entity)
    {
        // Remove existing
        if (!DeleteDrawingTemplate(entity.Id))
            return false;
        
        // Add new
        return AddDrawingTemplate(entity);
    }

    public void AddDrawingTemplatePaths(string[] paths)
    {
        // Ignore empty
        if (paths.Length == 0)
            return;
        
        var settings = GetSettings();
        // Get existing paths
        var existingPaths = settings.DrawingTemplatePaths;
        
        // Add if not already in the list
        foreach (var path in paths)
        {
            if (!existingPaths.Any(x => string.Equals(x, path, StringComparison.OrdinalIgnoreCase)))
                existingPaths.Add(path);
        }
        
        // Sort alphabetically
        settings.DrawingTemplatePaths = existingPaths.Order().ToList();
        
        // Update and Save settings
        UpdateSettings(settings);
    }

    public void DeleteDrawingTemplatePaths(string[] paths)
    {
        // Get settings
        var settings = GetSettings();
        
        // Get paths to keep
        var filteredPathsToKeep = settings.DrawingTemplatePaths.Where(x => paths.All(f => !string.Equals(x, f, StringComparison.InvariantCultureIgnoreCase)));
        
        // Update paths
        settings.DrawingTemplatePaths = filteredPathsToKeep.Order().ToList();
        
        // Save
        UpdateSettings(settings);
    }
    #endregion Drawing Templates

    #region Macros
    public List<ActionsTabMacrosEntity> GetMacros()
    {
        return _dbContext.ActionsTabMacros.ToList();
    }

    public bool AddMacros(ActionsTabMacrosEntity entity)
    {
        _dbContext.ActionsTabMacros.Add(entity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteMacros(string id)
    {
        // if (!Guid.TryParse(id, out Guid guid))
        //     throw  new ArgumentException("Invalid print tab id");

        // Remove existing
        var existingEntity = _dbContext.ActionsTabMacros.FirstOrDefault(x => x.Id == id);
        if (existingEntity == null)
            return false;
        
        _dbContext.ActionsTabMacros.Remove(existingEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool UpdateMacros(ActionsTabMacrosEntity entity)
    {
        // Remove existing
        if (!DeleteMacros(entity.Id))
            return false;
        
        // Add new
        return AddMacros(entity);
    }
    #endregion Macros

    #endregion Actions
    

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}