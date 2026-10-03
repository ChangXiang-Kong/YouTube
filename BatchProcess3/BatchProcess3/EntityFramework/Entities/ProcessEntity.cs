using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BatchProcess3.EntityFramework.Entities.Actions;

namespace BatchProcess3.EntityFramework.Entities;

/// <summary>
/// 注意：<br/>
///     1、Entity 与 ViewModel 中的 string 属性 都要给 默认值 "" 或 string.Empty，<br/>
///         否则在执行 xxx.SaveChanges() 时报错，如：SQLite Error 19: 'NOT NULL constraint failed: Process.Description'.<br/>
///     2、
/// </summary>
[Table("Process")]
public class ProcessEntity : EntityBase
{
    [MaxLength(200)]
    public string JobName { get; set; } = "";
    
    [MaxLength(5000)]
    public string Description { get; set; } = "";

    public List<ActionEntityBase> Actions { get; set; } = [];
}