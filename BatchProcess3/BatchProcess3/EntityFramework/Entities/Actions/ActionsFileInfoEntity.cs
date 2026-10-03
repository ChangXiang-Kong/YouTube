using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BatchProcess3.Data;

namespace BatchProcess3.EntityFramework.Entities.Actions;

/// <summary>
/// 注意：<br/>
///     1、Entity 与 ViewModel 中的 string 属性 都要给 默认值 "" 或 string.Empty，<br/>
///         否则在执行 xxx.SaveChanges() 时报错，如：SQLite Error 19: 'NOT NULL constraint failed: ActionsFileInfoEntity.JobName'.<br/>
///     2、
/// </summary>
[Table("ActionsFileInfo")]
public class ActionsFileInfoEntity : ActionEntityBase
{
    [MaxLength(1000)]
    public string Title { get; set; } = "";
    
    [MaxLength(1000)]
    public string Subject { get; set; } = "";
    
    [MaxLength(1000)]
    public string Author { get; set; } = "";
    
    [MaxLength(1000)]
    public string Keywords { get; set; } = "";
    
    [MaxLength(5000)]
    public string Comments { get; set; } = "";

}