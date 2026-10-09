using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BatchProcess3.EntityFramework;
using BatchProcess3.EntityFramework.Entities.Actions;
using BatchProcess3.ViewModels.Process;

namespace BatchProcess3.Tools.Actions;

public class ActionService(DatabaseService databaseService)
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

        var prints = ToAvailableActionItemViewModelList("Print", databaseService.GetPrintList());
        var customProperties =
            ToAvailableActionItemViewModelList("Custom Properties", databaseService.GetCustomPropertiesList());
        var fileInfos = ToAvailableActionItemViewModelList("File Info", databaseService.GetFileInfoList());
        var saveModels = ToAvailableActionItemViewModelList("Save Model", databaseService.GetSaveModelList());
        var saveDrawings = ToAvailableActionItemViewModelList("Save Drawing", databaseService.GetSaveDrawingList());
        var importFiles = ToAvailableActionItemViewModelList("Import File", databaseService.GetImportFileList());
        var drawingTemplates =
            ToAvailableActionItemViewModelList("Drawing Template", databaseService.GetDrawingTemplateList());
        var macros = ToAvailableActionItemViewModelList("Macros", databaseService.GetMacrosList());

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