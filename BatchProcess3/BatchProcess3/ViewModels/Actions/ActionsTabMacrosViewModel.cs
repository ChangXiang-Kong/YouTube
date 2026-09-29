using BatchProcess3.EntityFramework.Entities.Actions;

namespace BatchProcess3.ViewModels.Actions;

public partial class ActionsTabMacrosViewModel : ViewModelBase
{
    
}

public static class ActionsTabMacrosViewModelExtensions
{
    public static ActionsTabMacrosEntity ToEntity(this ActionsTabMacrosViewModel viewModel) => new()
    {
        Id = viewModel.Id,
        // JobName = viewModel.JobName,
        // Description = viewModel.Description,
        // RuleType = viewModel.RuleType,
        // FilterLogic = viewModel.FilterLogic,
        // SetCustomProperty = viewModel.SetCustomProperty,
        // SetConfigSpecificProperties = viewModel.SetConfigSpecificProperties,
        // SetConfigurationPropertiesFilter = viewModel.SetConfigurationPropertiesFilter,
        // ExcludeParts = viewModel.ExcludeParts,
        // ExcludeAssemblies = viewModel.ExcludeAssemblies,
        // ExcludeDrawings = viewModel.ExcludeDrawings,
        // FieldType = viewModel.FieldType,
        // FieldName = viewModel.FieldName,
        // ValueRule = viewModel.ValueRule,
        // ChangeNameTo = viewModel.ChangeNameTo,
        // CopyFromConfiguration = viewModel.CopyFromConfiguration,
        // CopyToField = viewModel.CopyToField,
    };

    public static ActionsTabMacrosViewModel ToViewModel(this ActionsTabMacrosEntity entity) => new()
    {
        Id = entity.Id,
        // JobName = entity.JobName,
        // Description = entity.Description,
        // RuleType = entity.RuleType,
        // FilterLogic = entity.FilterLogic,
        // SetCustomProperty = entity.SetCustomProperty,
        // SetConfigSpecificProperties = entity.SetConfigSpecificProperties,
        // SetConfigurationPropertiesFilter = entity.SetConfigurationPropertiesFilter,
        // ExcludeParts = entity.ExcludeParts,
        // ExcludeAssemblies = entity.ExcludeAssemblies,
        // ExcludeDrawings = entity.ExcludeDrawings,
        // FieldType = entity.FieldType,
        // FieldName = entity.FieldName,
        // ValueRule = entity.ValueRule,
        // ChangeNameTo = entity.ChangeNameTo,
        // CopyFromConfiguration = entity.CopyFromConfiguration,
        // CopyToField = entity.CopyToField,
    };
}