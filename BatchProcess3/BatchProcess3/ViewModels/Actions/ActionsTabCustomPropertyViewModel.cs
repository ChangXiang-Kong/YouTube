using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using BatchProcess3.Data;
using BatchProcess3.EntityFramework.Entities.Actions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Actions;

public partial class ActionsTabCustomPropertyViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _id = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _jobName = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _description = "";

    [ObservableProperty]
    private bool _isNewItem;

    [ObservableProperty]
    private CustomPropertyRuleType _ruleType;

    [ObservableProperty]
    private string _filterLogic;

    [ObservableProperty]
    private bool _setCustomProperty;

    [ObservableProperty]
    private bool _setAllConfigSpecificProperties;

    [ObservableProperty]
    private bool _setNamedConfigurationProperties;

    [ObservableProperty]
    private bool _excludeParts;

    [ObservableProperty]
    private bool _excludeAssemblies;

    [ObservableProperty]
    private bool _excludeDrawings;

    [ObservableProperty]
    private string _fieldType;

    [ObservableProperty]
    private ObservableCollection<string> _fieldTypeOptions = [];
    
    [ObservableProperty]
    private string _fieldName;
    
    [ObservableProperty]
    private string _valueRule;
    
    [ObservableProperty]
    private string _changeNameTo;
    
    [ObservableProperty]
    private string _copyFromConfiguration;
    
    [ObservableProperty]
    private string _copyToField;

    [JsonIgnore]
    public new bool HasChanged => IsNewItem || (SavedState != "" && SavedState != JsonSerializer.Serialize(this, JsonSerializerOptions));

}

public static class ActionsTabCustomPropertiesViewModelExtensions
{
    public static ActionsTabCustomPropertiesEntity ToEntity(this ActionsTabCustomPropertyViewModel viewModel) => new()
    {
        Id = viewModel.Id,
        JobName = viewModel.JobName,
        Description = viewModel.Description,
        RuleType = viewModel.RuleType,
        FilterLogic = viewModel.FilterLogic,
        SetCustomProperty = viewModel.SetCustomProperty,
        SetAllConfigSpecificProperties = viewModel.SetAllConfigSpecificProperties,
        SetNamedConfigurationProperties = viewModel.SetNamedConfigurationProperties,
        ExcludeParts = viewModel.ExcludeParts,
        ExcludeAssemblies = viewModel.ExcludeAssemblies,
        ExcludeDrawings = viewModel.ExcludeDrawings,
        FieldType = viewModel.FieldType,
        FieldName = viewModel.FieldName,
        ValueRule = viewModel.ValueRule,
        ChangeNameTo = viewModel.ChangeNameTo,
        CopyFromConfiguration = viewModel.CopyFromConfiguration,
        CopyToField = viewModel.CopyToField,
    };

    public static ActionsTabCustomPropertyViewModel ToViewModel(this ActionsTabCustomPropertiesEntity entity) => new()
    {
        Id = entity.Id,
        JobName = entity.JobName,
        Description = entity.Description,
        RuleType = entity.RuleType,
        FilterLogic = entity.FilterLogic,
        SetCustomProperty = entity.SetCustomProperty,
        SetAllConfigSpecificProperties = entity.SetAllConfigSpecificProperties,
        SetNamedConfigurationProperties = entity.SetNamedConfigurationProperties,
        ExcludeParts = entity.ExcludeParts,
        ExcludeAssemblies = entity.ExcludeAssemblies,
        ExcludeDrawings = entity.ExcludeDrawings,
        FieldType = entity.FieldType,
        FieldName = entity.FieldName,
        ValueRule = entity.ValueRule,
        ChangeNameTo = entity.ChangeNameTo,
        CopyFromConfiguration = entity.CopyFromConfiguration,
        CopyToField = entity.CopyToField,
    };
}