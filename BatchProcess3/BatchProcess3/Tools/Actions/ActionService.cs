using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BatchProcess3.EntityFramework;
using BatchProcess3.EntityFramework.Entities.Actions;
using BatchProcess3.ViewModels.Process;

namespace BatchProcess3.Tools.Actions;

public class ActionService(DatabaseFactory databaseFactory)
{
    public ObservableCollection<ProcessAvailableActionItemViewModel> GetAvailableActionsList()
    {
        List<ProcessAvailableActionItemViewModel> ToAvailableActionItemViewModelList<T>(string category, List<T> list)
            where T : ActionEntity
        {
            var ret = new List<ProcessAvailableActionItemViewModel>
            {
                // Add header
                new ProcessAvailableActionItemViewModel() { Category = category }
            };

            // Add items
            ret.AddRange(list.Select(x => new ProcessAvailableActionItemViewModel()
            {
                ProcessActionViewModel = x.ToProcessActionViewModel(),
                Category = category,
            }));

            return ret;
        }

        using var dbContext = databaseFactory.GetDatabaseService();
        var prints = ToAvailableActionItemViewModelList("Print", dbContext.GetPrintList());
        var customProperties = ToAvailableActionItemViewModelList("Custom Properties", dbContext.GetCustomPropertiesList());
        var fileInfos = ToAvailableActionItemViewModelList("File Info", dbContext.GetFileInfoList());
        var saveModels = ToAvailableActionItemViewModelList("Save Model", dbContext.GetSaveModelList());
        var saveDrawings = ToAvailableActionItemViewModelList("Save Drawing", dbContext.GetSaveDrawingList());
        var importFiles = ToAvailableActionItemViewModelList("Import File", dbContext.GetImportFileList());
        var drawingTemplates =
            ToAvailableActionItemViewModelList("Drawing Template", dbContext.GetDrawingTemplateList());
        var macros = ToAvailableActionItemViewModelList("Macros", dbContext.GetMacrosList());

        return new ObservableCollection<ProcessAvailableActionItemViewModel>(
            prints
                .Concat(customProperties)
                .Concat(fileInfos)
                .Concat(saveModels)
                .Concat(saveDrawings)
                .Concat(importFiles)
                .Concat(drawingTemplates)
                .Concat(macros)
        );
        
    }
}