using System.ComponentModel.DataAnnotations.Schema;
using BatchProcess3.Data;

namespace BatchProcess3.EntityFramework.Entities.Actions;

/// <summary>
/// 注意：<br/>
///     1、Entity 与 ViewModel 中的 string 属性 都要给 默认值 "" 或 string.Empty，<br/>
///         否则在执行 xxx.SaveChanges() 时报错，如：SQLite Error 19: 'NOT NULL constraint failed: ActionsTabCustomProperties.JobName'.<br/>
///     2、
/// </summary>
[Table("ActionsTabCustomProperties")]
public class ActionsTabCustomPropertiesEntity : BaseEntity
{
    public string JobName { get; set; } = "";
    
    public string Description { get; set; } = "";
    
    public CustomPropertyRuleType RuleType { get; set; }

    public string FilterLogic { get; set; } = "";

    public bool SetCustomProperty { get; set; }

    public bool SetConfigSpecificProperties { get; set; }

    public string SetConfigurationPropertiesFilter { get; set; } = "";

    public bool ExcludeParts { get; set; }

    public bool ExcludeAssemblies { get; set; }

    public bool ExcludeDrawings { get; set; }

    public CustomPropertyFieldType FieldType { get; set; }
    
    public string FieldName { get; set; } = "";
    
    public string ValueRule { get; set; } = "";
    
    public string ChangeNameTo { get; set; } = "";
    
    public string CopyFromConfiguration { get; set; } = "";
    
    public string CopyToField { get; set; } = "";

}