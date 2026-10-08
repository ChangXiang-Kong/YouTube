using System;
using System.Collections.Generic;
using System.Linq;
using BatchProcess3.EntityFramework.Entities;
using BatchProcess3.EntityFramework.Entities.Actions;
using BatchProcess3.EntityFramework.Entities.Process;
using Microsoft.EntityFrameworkCore;

namespace BatchProcess3.EntityFramework;

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
        
        AddSettingsItem(entity);
        
        return entity;
    }

    public bool AddSettingsItem(SettingsEntity entity)
    {
        // Remove all settings
        _dbContext.Settings.RemoveRange(_dbContext.Settings);
        
        _dbContext.Settings.Add(entity);
        
         return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteSettingsItem(string id)
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

    public bool UpdateSettingsItem(SettingsEntity entity)
    {
        // Remove existing
        if (!DeleteSettingsItem(entity.Id))
            return false;
        
        // Add new
        return AddSettingsItem(entity);
    }
    
    #endregion Settings

    #region Actions
    
    #region Print List
    public List<ActionsPrintEntity> GetPrintList()
    {
        var res = _dbContext.ActionsPrint.ToList();

        if (!res.Any())
        {
            // Ensure we have at least one print settings
            GetPrintSettingsList();
            
            // Create a default item
            // res.Add(new PrintEntity()    // 报错：Sequence contains no elements
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

    public bool AddPrintListItem(ActionsPrintEntity entity)
    {
        _dbContext.ActionsPrint.Add(entity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeletePrintListItem(string id)
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

    public bool UpdatePrintListItem(ActionsPrintEntity printEntity)
    {
        // Remove existing
        if (!DeletePrintListItem(printEntity.Id))
            return false;
        
        // Add new
        return AddPrintListItem(printEntity);
    }
    #endregion Print List
    
    #region PrintSettings
    public List<ActionsPrintSettingsProfileEntity> GetPrintSettingsProfilesList()
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
    
    public List<ActionsPrintSettingsEntity> GetPrintSettingsList()
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
                PrintSettingsProfilesList = GetPrintSettingsProfilesList()
            });

            // Save changes to database
            _dbContext.SaveChanges();
            
            res =  _dbContext.ActionsPrintSettings.ToList();
        }
        
        return res;
    }

    public bool AddPrintSettingsItem(ActionsPrintSettingsEntity printSettingsEntity)
    {
        _dbContext.ActionsPrintSettings.Add(printSettingsEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeletePrintSettingsItem(string id, bool bypass = false, bool saveChanges = true)
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

    public bool UpdatePrintSettingsItem(ActionsPrintSettingsEntity printSettingsEntity)
    {
        // If it is not editable
        if (!printSettingsEntity.CanEdit)
            throw new InvalidOperationException($"The print setting {printSettingsEntity.JobName} cannot be edited.");
        
        // Remove existing
        if (!DeletePrintSettingsItem(printSettingsEntity.Id, bypass: true, saveChanges: false))
            return false;
        
        // Add new
        return AddPrintSettingsItem(printSettingsEntity);
    }
    #endregion PrintSettings

    #region Custom Properties
    public List<ActionsCustomPropertiesEntity> GetCustomPropertiesList()
    {
        return _dbContext.ActionsCustomProperties.ToList();
    }

    public bool AddCustomPropertyItem(ActionsCustomPropertiesEntity customPropertiesEntity)
    {
        _dbContext.ActionsCustomProperties.Add(customPropertiesEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteCustomPropertyItem(string id)
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

    public bool UpdateCustomPropertyItem(ActionsCustomPropertiesEntity customPropertiesEntity)
    {
        // Remove existing
        if (!DeleteCustomPropertyItem(customPropertiesEntity.Id))
            return false;
        
        // Add new
        return AddCustomPropertyItem(customPropertiesEntity);
    }
    #endregion Custom Properties

    #region File Info
    public List<ActionsFileInfoEntity> GetFileInfoList()
    {
        return _dbContext.ActionsFileInfo.ToList();
    }

    public bool AddFileInfoItem(ActionsFileInfoEntity fileInfoEntity)
    {
        _dbContext.ActionsFileInfo.Add(fileInfoEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteFileInfoItem(string id)
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

    public bool UpdateFileInfoItem(ActionsFileInfoEntity fileInfoEntity)
    {
        // Remove existing
        if (!DeleteFileInfoItem(fileInfoEntity.Id))
            return false;
        
        // Add new
        return AddFileInfoItem(fileInfoEntity);
    }
    #endregion File Info

    #region Save Model
    public List<ActionsSaveModelEntity> GetSaveModelList()
    {
        return _dbContext.ActionsSaveModel.ToList();
    }

    public bool AddSaveModelItem(ActionsSaveModelEntity saveModelEntity)
    {
        _dbContext.ActionsSaveModel.Add(saveModelEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteSaveModelItem(string id)
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

    public bool UpdateSaveModelItem(ActionsSaveModelEntity saveModelEntity)
    {
        // Remove existing
        if (!DeleteSaveModelItem(saveModelEntity.Id))
            return false;
        
        // Add new
        return AddSaveModelItem(saveModelEntity);
    }
    #endregion Save Model

    #region Save Drawing
    public List<ActionsSaveDrawingEntity> GetSaveDrawingList()
    {
        return _dbContext.ActionsSaveDrawing.ToList();
    }

    public bool AddSaveDrawingItem(ActionsSaveDrawingEntity saveDrawingEntity)
    {
        _dbContext.ActionsSaveDrawing.Add(saveDrawingEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteSaveDrawingItem(string id)
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

    public bool UpdateSaveDrawingItem(ActionsSaveDrawingEntity saveDrawingEntity)
    {
        // Remove existing
        if (!DeleteSaveDrawingItem(saveDrawingEntity.Id))
            return false;
        
        // Add new
        return AddSaveDrawingItem(saveDrawingEntity);
    }
    #endregion Save Drawing

    #region Import File
    public List<ActionsImportFileEntity> GetImportFileList()
    {
        return _dbContext.ActionsImportFile.ToList();
    }

    public bool AddImportFileItem(ActionsImportFileEntity importFileEntity)
    {
        _dbContext.ActionsImportFile.Add(importFileEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteImportFileItem(string id)
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

    public bool UpdateImportFileItem(ActionsImportFileEntity importFileEntity)
    {
        // Remove existing
        if (!DeleteImportFileItem(importFileEntity.Id))
            return false;
        
        // Add new
        return AddImportFileItem(importFileEntity);
    }
    #endregion Import File

    #region Drawing Templates
    public List<ActionsDrawingTemplateEntity> GetDrawingTemplateList()
    {
        return _dbContext.ActionsDrawingTemplate.ToList();
    }

    public bool AddDrawingTemplateItem(ActionsDrawingTemplateEntity drawingTemplateEntity)
    {
        _dbContext.ActionsDrawingTemplate.Add(drawingTemplateEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteDrawingTemplateItem(string id)
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

    public bool UpdateDrawingTemplateItem(ActionsDrawingTemplateEntity drawingTemplateEntity)
    {
        // Remove existing
        if (!DeleteDrawingTemplateItem(drawingTemplateEntity.Id))
            return false;
        
        // Add new
        return AddDrawingTemplateItem(drawingTemplateEntity);
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
        UpdateSettingsItem(settings);
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
        UpdateSettingsItem(settings);
    }
    #endregion Drawing Templates

    #region Macros
    public List<ActionsMacrosEntity> GetMacrosList()
    {
        return _dbContext.ActionsMacros.ToList();
    }

    public bool AddMacrosItem(ActionsMacrosEntity macrosEntity)
    {
        _dbContext.ActionsMacros.Add(macrosEntity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteMacrosItem(string id)
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

    public bool UpdateMacrosList(ActionsMacrosEntity macrosEntity)
    {
        // Remove existing
        if (!DeleteMacrosItem(macrosEntity.Id))
            return false;
        
        // Add new
        return AddMacrosItem(macrosEntity);
    }
    #endregion Macros

    #endregion Actions
    
    #region Process
    
    public List<ProcessEntity> GetProcessesList()
    {
        return _dbContext.Process
            .Include(x => x.ProcessActions)
            .ToList();
    }

    public bool AddProcessItem(ProcessEntity entity)
    {
        _dbContext.Process.Add(entity);
        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteProcessItem(string id)
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

    public bool UpdateProcessItem(ProcessEntity entity)
    {
        // Remove existing
        if (!DeleteProcessItem(entity.Id))
            return false;
        
        // Add new
        return AddProcessItem(entity);
    }
    
    #endregion Process
    

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}