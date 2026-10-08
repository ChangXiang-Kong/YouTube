using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BatchProcess3.Data;

namespace BatchProcess3.EntityFramework.Entities.Actions;

/// <summary>
/// 注意：<br/>
///     1、Entity 与 ViewModel 中的 string 属性 都要给 默认值 "" 或 string.Empty，<br/>
///         否则在执行 xxx.SaveChanges() 时报错，如：SQLite Error 19: 'NOT NULL constraint failed: ActionsImportFileEntity.JobName'.<br/>
///     2、
/// </summary>
[Table("ActionsImportFile")]
public class ActionsImportFileEntity : ActionEntity
{
    [MaxLength(1000)]
    public string FileName { get; set; } = "";

    [MaxLength(1000)]
    public string SaveLocation { get; set; } = "";

    [MaxLength(1000)]
    public string ImportArguments { get; set; } = "";

    public bool AddToProject { get; set; }

}