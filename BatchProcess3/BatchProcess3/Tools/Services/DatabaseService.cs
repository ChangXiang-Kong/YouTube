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
    public List<ActionsPrintEntity> GetPrintTab()
    {
        var res = _dbContext.ActionsPrint.ToList();

        if (!res.Any())
        {
            // Ensure we have at least one print settings
            GetPrintSettings();
            
            // Create a default item
            // res.Add(new PrintTabEntity()    // 报错：Sequence contains no elements
            _dbContext.ActionsPrint.Add(new ActionsPrintEntity()
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
            res = _dbContext.ActionsPrint.ToList();
        }
        
        return res;
    }

    public bool AddPrintTab(ActionsPrintEntity printEntity)
    {
        _dbContext.ActionsPrint.Add(printEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeletePrintTab(string id)
    {
        // if (!Guid.TryParse(id, out Guid guid))
        //     throw  new ArgumentException("Invalid print tab id");

        // Remove existing
        var existingEntity = _dbContext.ActionsPrint.FirstOrDefault(x => x.Id == id);
        if (existingEntity == null)
            return false;
        
        _dbContext.ActionsPrint.Remove(existingEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool UpdatePrintTab(ActionsPrintEntity printEntity)
    {
        // Remove existing
        if (!DeletePrintTab(printEntity.Id))
            return false;
        
        // Add new
        return AddPrintTab(printEntity);
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

    public bool AddPrintSettings(ActionsPrintSettingsEntity printSettingsEntity)
    {
        _dbContext.ActionsPrintSettings.Add(printSettingsEntity);
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

    public bool UpdatePrintSettings(ActionsPrintSettingsEntity printSettingsEntity)
    {
        // If it is not editable
        if (!printSettingsEntity.CanEdit)
            throw new InvalidOperationException($"The print setting {printSettingsEntity.JobName} cannot be edited.");
        
        // Remove existing
        if (!DeletePrintSettings(printSettingsEntity.Id, bypass: true, saveChanges: false))
            return false;
        
        // Add new
        return AddPrintSettings(printSettingsEntity);
    }
    #endregion PrintSettings

    #region Custom Properties
    public List<ActionsCustomPropertiesEntity> GetCustomProperties()
    {
        return _dbContext.ActionsCustomProperties.ToList();
    }

    public bool AddCustomProperty(ActionsCustomPropertiesEntity customPropertiesEntity)
    {
        _dbContext.ActionsCustomProperties.Add(customPropertiesEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteCustomProperty(string id)
    {
        // if (!Guid.TryParse(id, out Guid guid))
        //     throw  new ArgumentException("Invalid print tab id");

        // Remove existing
        var existingEntity = _dbContext.ActionsCustomProperties.FirstOrDefault(x => x.Id == id);
        if (existingEntity == null)
            return false;
        
        _dbContext.ActionsCustomProperties.Remove(existingEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool UpdateCustomProperty(ActionsCustomPropertiesEntity customPropertiesEntity)
    {
        // Remove existing
        if (!DeleteCustomProperty(customPropertiesEntity.Id))
            return false;
        
        // Add new
        return AddCustomProperty(customPropertiesEntity);
    }
    #endregion Custom Properties

    #region File Info
    public List<ActionsFileInfoEntity> GetFileInfo()
    {
        return _dbContext.ActionsFileInfo.ToList();
    }

    public bool AddFileInfo(ActionsFileInfoEntity fileInfoEntity)
    {
        _dbContext.ActionsFileInfo.Add(fileInfoEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteFileInfo(string id)
    {
        // if (!Guid.TryParse(id, out Guid guid))
        //     throw  new ArgumentException("Invalid print tab id");

        // Remove existing
        var existingEntity = _dbContext.ActionsFileInfo.FirstOrDefault(x => x.Id == id);
        if (existingEntity == null)
            return false;
        
        _dbContext.ActionsFileInfo.Remove(existingEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool UpdateFileInfo(ActionsFileInfoEntity fileInfoEntity)
    {
        // Remove existing
        if (!DeleteFileInfo(fileInfoEntity.Id))
            return false;
        
        // Add new
        return AddFileInfo(fileInfoEntity);
    }
    #endregion File Info

    #region Save Model
    public List<ActionsSaveModelEntity> GetSaveModel()
    {
        return _dbContext.ActionsSaveModel.ToList();
    }

    public bool AddSaveModel(ActionsSaveModelEntity saveModelEntity)
    {
        _dbContext.ActionsSaveModel.Add(saveModelEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteSaveModel(string id)
    {
        // if (!Guid.TryParse(id, out Guid guid))
        //     throw  new ArgumentException("Invalid print tab id");

        // Remove existing
        var existingEntity = _dbContext.ActionsSaveModel.FirstOrDefault(x => x.Id == id);
        if (existingEntity == null)
            return false;
        
        _dbContext.ActionsSaveModel.Remove(existingEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool UpdateSaveModel(ActionsSaveModelEntity saveModelEntity)
    {
        // Remove existing
        if (!DeleteSaveModel(saveModelEntity.Id))
            return false;
        
        // Add new
        return AddSaveModel(saveModelEntity);
    }
    #endregion Save Model

    #region Save Drawing
    public List<ActionsSaveDrawingEntity> GetSaveDrawing()
    {
        return _dbContext.ActionsSaveDrawing.ToList();
    }

    public bool AddSaveDrawing(ActionsSaveDrawingEntity saveDrawingEntity)
    {
        _dbContext.ActionsSaveDrawing.Add(saveDrawingEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteSaveDrawing(string id)
    {
        // if (!Guid.TryParse(id, out Guid guid))
        //     throw  new ArgumentException("Invalid print tab id");

        // Remove existing
        var existingEntity = _dbContext.ActionsSaveDrawing.FirstOrDefault(x => x.Id == id);
        if (existingEntity == null)
            return false;
        
        _dbContext.ActionsSaveDrawing.Remove(existingEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool UpdateSaveDrawing(ActionsSaveDrawingEntity saveDrawingEntity)
    {
        // Remove existing
        if (!DeleteSaveDrawing(saveDrawingEntity.Id))
            return false;
        
        // Add new
        return AddSaveDrawing(saveDrawingEntity);
    }
    #endregion Save Drawing

    #region Import File
    public List<ActionsImportFileEntity> GetImportFile()
    {
        return _dbContext.ActionsImportFile.ToList();
    }

    public bool AddImportFile(ActionsImportFileEntity importFileEntity)
    {
        _dbContext.ActionsImportFile.Add(importFileEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteImportFile(string id)
    {
        // if (!Guid.TryParse(id, out Guid guid))
        //     throw  new ArgumentException("Invalid print tab id");

        // Remove existing
        var existingEntity = _dbContext.ActionsImportFile.FirstOrDefault(x => x.Id == id);
        if (existingEntity == null)
            return false;
        
        _dbContext.ActionsImportFile.Remove(existingEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool UpdateImportFile(ActionsImportFileEntity importFileEntity)
    {
        // Remove existing
        if (!DeleteImportFile(importFileEntity.Id))
            return false;
        
        // Add new
        return AddImportFile(importFileEntity);
    }
    #endregion Import File

    #region Drawing Templates
    public List<ActionsDrawingTemplateEntity> GetDrawingTemplate()
    {
        return _dbContext.ActionsDrawingTemplate.ToList();
    }

    public bool AddDrawingTemplate(ActionsDrawingTemplateEntity drawingTemplateEntity)
    {
        _dbContext.ActionsDrawingTemplate.Add(drawingTemplateEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteDrawingTemplate(string id)
    {
        // if (!Guid.TryParse(id, out Guid guid))
        //     throw  new ArgumentException("Invalid print tab id");

        // Remove existing
        var existingEntity = _dbContext.ActionsDrawingTemplate.FirstOrDefault(x => x.Id == id);
        if (existingEntity == null)
            return false;
        
        _dbContext.ActionsDrawingTemplate.Remove(existingEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool UpdateDrawingTemplate(ActionsDrawingTemplateEntity drawingTemplateEntity)
    {
        // Remove existing
        if (!DeleteDrawingTemplate(drawingTemplateEntity.Id))
            return false;
        
        // Add new
        return AddDrawingTemplate(drawingTemplateEntity);
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
    public List<ActionsMacrosEntity> GetMacros()
    {
        return _dbContext.ActionsMacros.ToList();
    }

    public bool AddMacros(ActionsMacrosEntity macrosEntity)
    {
        _dbContext.ActionsMacros.Add(macrosEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteMacros(string id)
    {
        // if (!Guid.TryParse(id, out Guid guid))
        //     throw  new ArgumentException("Invalid print tab id");

        // Remove existing
        var existingEntity = _dbContext.ActionsMacros.FirstOrDefault(x => x.Id == id);
        if (existingEntity == null)
            return false;
        
        _dbContext.ActionsMacros.Remove(existingEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool UpdateMacros(ActionsMacrosEntity macrosEntity)
    {
        // Remove existing
        if (!DeleteMacros(macrosEntity.Id))
            return false;
        
        // Add new
        return AddMacros(macrosEntity);
    }
    #endregion Macros

    #endregion Actions
    
    #region Process
    
    public List<ProcessEntity> GetProcesses()
    {
        return _dbContext.Process.ToList();
    }

    public bool AddProcess(ProcessEntity entity)
    {
        _dbContext.Process.Add(entity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteProcess(string id)
    {
        // if (!Guid.TryParse(id, out Guid guid))
        //     throw  new ArgumentException("Invalid print tab id");

        // Remove existing
        var existingEntity = _dbContext.Process.FirstOrDefault(x => x.Id == id);
        if (existingEntity == null)
            return false;
        
        _dbContext.Process.Remove(existingEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool UpdateProcess(ProcessEntity entity)
    {
        // Remove existing
        if (!DeleteProcess(entity.Id))
            return false;
        
        // Add new
        return AddProcess(entity);
    }
    
    #endregion Process
    

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}