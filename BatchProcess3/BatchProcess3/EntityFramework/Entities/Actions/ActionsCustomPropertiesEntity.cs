using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BatchProcess3.Data;

namespace BatchProcess3.EntityFramework.Entities.Actions;

/// <summary>
/// 注意：<br/>
///     1、Entity 与 ViewModel 中的 string 属性 都要给 默认值 "" 或 string.Empty，<br/>
///         否则在执行 xxx.SaveChanges() 时报错，如：SQLite Error 19: 'NOT NULL constraint failed: ActionsCustomProperties.JobName'.<br/>
///     2、
/// </summary>
[Table("ActionsCustomProperties")]
public class ActionsCustomPropertiesEntity : ActionEntity
{
    public CustomPropertyRuleType RuleType { get; set; }

    [MaxLength(5000)]
    public string FilterLogic { get; set; } = "";

    public bool SetCustomProperty { get; set; }

    public bool SetConfigSpecificProperties { get; set; }

    [MaxLength(1000)]
    public string SetConfigurationPropertiesFilter { get; set; } = "";

    public bool ExcludeParts { get; set; }

    public bool ExcludeAssemblies { get; set; }

    public bool ExcludeDrawings { get; set; }

    public CustomPropertyFieldType FieldType { get; set; }
    
    [MaxLength(500)]
    public string FieldName { get; set; } = "";
    
    [MaxLength(5000)]
    public string ValueRule { get; set; } = "";
    
    [MaxLength(500)]
    public string ChangeNameTo { get; set; } = "";
    
    [MaxLength(100)]
    public string CopyFromConfiguration { get; set; } = "";
    
    [MaxLength(500)]
    public string CopyToField { get; set; } = "";

}