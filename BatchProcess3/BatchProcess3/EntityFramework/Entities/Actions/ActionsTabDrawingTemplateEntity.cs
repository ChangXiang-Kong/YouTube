using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BatchProcess3.Data;

namespace BatchProcess3.EntityFramework.Entities.Actions;

/// <summary>
/// 注意：<br/>
///     1、Entity 与 ViewModel 中的 string 属性 都要给 默认值 "" 或 string.Empty，<br/>
///         否则在执行 xxx.SaveChanges() 时报错，如：SQLite Error 19: 'NOT NULL constraint failed: ActionsTabDrawingTemplateEntity.JobName'.<br/>
///     2、
/// </summary>
[Table("ActionsTabDrawingTemplate")]
public class ActionsTabDrawingTemplateEntity : BaseEntity
{
    [MaxLength(200)]
    public string JobName { get; set; } = "";
    
    [MaxLength(5000)]
    public string Description { get; set; } = "";
    
    public DrawingTemplateOperation Operation { get; set; }
    
    [MaxLength(1000)]
    public string? CurrentTemplatePath { get; set; }
    
    [MaxLength(1000)]
    public string? NewTemplatePath { get; set; }

}