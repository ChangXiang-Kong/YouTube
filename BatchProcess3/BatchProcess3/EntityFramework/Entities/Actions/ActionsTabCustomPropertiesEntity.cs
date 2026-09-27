using System.ComponentModel.DataAnnotations.Schema;
using BatchProcess3.Data;

namespace BatchProcess3.EntityFramework.Entities.Actions;

[Table("ActionsTabCustomProperties")]
public class ActionsTabCustomPropertiesEntity : BaseEntity
{
    public string JobName { get; set; } = "";
    
    public string Description { get; set; } = "";
    
    public CustomPropertyRuleType RuleType { get; set; }

    public string FilterLogic { get; set; } = "";

    public bool SetCustomProperty { get; set; }

    public bool SetAllConfigSpecificProperties { get; set; }

    public bool SetNamedConfigurationProperties { get; set; }

    public bool ExcludeParts { get; set; }

    public bool ExcludeAssemblies { get; set; }

    public bool ExcludeDrawings { get; set; }

    public string FieldType { get; set; } = "";
    
    public string FieldName { get; set; } = "";
    
    public string ValueRule { get; set; } = "";
    
    public string ChangeNameTo { get; set; } = "";
    
    public string CopyFromConfiguration { get; set; } = "";
    
    public string CopyToField { get; set; } = "";

}