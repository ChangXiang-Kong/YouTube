using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using BatchProcess3.Data;
using BatchProcess3.EntityFramework.Entities.Actions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Actions;

public partial class ActionsCustomPropertyViewModel : ActionViewModel
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    [NotifyPropertyChangedFor(nameof(FieldNameIsVisible))]
    [NotifyPropertyChangedFor(nameof(FieldTypeIsVisible))]
    [NotifyPropertyChangedFor(nameof(ChangeNameToIsVisible))]
    [NotifyPropertyChangedFor(nameof(ValueRuleIsVisible))]
    [NotifyPropertyChangedFor(nameof(CopyFromConfigurationIsVisible))]
    [NotifyPropertyChangedFor(nameof(CopyToFieldIsVisible))]
    private CustomPropertyRuleType _ruleType;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _filterLogic = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _setCustomProperty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _setConfigSpecificProperties;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _setConfigurationPropertiesFilter = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _excludeParts;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _excludeAssemblies;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _excludeDrawings;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private CustomPropertyFieldType _fieldType;

    [JsonIgnore]
    public bool FieldTypeIsVisible => RuleType is CustomPropertyRuleType.Add or CustomPropertyRuleType.Update;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _fieldName = "";

    [JsonIgnore]
    public bool FieldNameIsVisible => RuleType is not CustomPropertyRuleType.Clear;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _valueRule = "";
    
    [JsonIgnore]
    public bool ValueRuleIsVisible => RuleType is CustomPropertyRuleType.Add or CustomPropertyRuleType.Update;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _changeNameTo = "";
    
    [JsonIgnore]
    public bool ChangeNameToIsVisible => RuleType is CustomPropertyRuleType.Update;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _copyFromConfiguration = "";
    
    [JsonIgnore]
    public bool CopyFromConfigurationIsVisible => RuleType is CustomPropertyRuleType.Copy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _copyToField = "";

    [JsonIgnore]
    public bool CopyToFieldIsVisible => RuleType is CustomPropertyRuleType.Copy;

}

public static class ActionsCustomPropertiesViewModelExtensions
{
    public static ActionsCustomPropertiesEntity ToEntity(this ActionsCustomPropertyViewModel viewModel) => new()
    {
        Id = viewModel.Id,
        JobName = viewModel.JobName,
        Description = viewModel.Description,
        RuleType = viewModel.RuleType,
        FilterLogic = viewModel.FilterLogic,
        SetCustomProperty = viewModel.SetCustomProperty,
        SetConfigSpecificProperties = viewModel.SetConfigSpecificProperties,
        SetConfigurationPropertiesFilter = viewModel.SetConfigurationPropertiesFilter,
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

    public static ActionsCustomPropertyViewModel ToViewModel(this ActionsCustomPropertiesEntity customPropertiesEntity) => new()
    {
        Id = customPropertiesEntity.Id,
        JobName = customPropertiesEntity.JobName,
        Description = customPropertiesEntity.Description,
        RuleType = customPropertiesEntity.RuleType,
        FilterLogic = customPropertiesEntity.FilterLogic,
        SetCustomProperty = customPropertiesEntity.SetCustomProperty,
        SetConfigSpecificProperties = customPropertiesEntity.SetConfigSpecificProperties,
        SetConfigurationPropertiesFilter = customPropertiesEntity.SetConfigurationPropertiesFilter,
        ExcludeParts = customPropertiesEntity.ExcludeParts,
        ExcludeAssemblies = customPropertiesEntity.ExcludeAssemblies,
        ExcludeDrawings = customPropertiesEntity.ExcludeDrawings,
        FieldType = customPropertiesEntity.FieldType,
        FieldName = customPropertiesEntity.FieldName,
        ValueRule = customPropertiesEntity.ValueRule,
        ChangeNameTo = customPropertiesEntity.ChangeNameTo,
        CopyFromConfiguration = customPropertiesEntity.CopyFromConfiguration,
        CopyToField = customPropertiesEntity.CopyToField,
    };
}