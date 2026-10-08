using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BatchProcess3.EntityFramework.Entities.Actions;

namespace BatchProcess3.EntityFramework.Entities.Process;

/// <summary>
/// 注意：<br/>
///     1、Entity 与 ViewModel 中的 string 属性 都要给 默认值 "" 或 string.Empty，<br/>
///         否则在执行 xxx.SaveChanges() 时报错，如：SQLite Error 19: 'NOT NULL constraint failed: ProcessActionEntity.JobName'.<br/>
///     2、
/// </summary>
[Table("ProcessAction")]
public class ProcessActionEntity : ActionEntity
{
    /// <summary>
    /// The underlying action Id
    /// </summary>
    [MaxLength(100)] 
    public string ActionId { get; init; } = "";
    
    [MaxLength(100)]
    // public Guid PrintSettingsId { get; set; }
    public string ProcessId { get; set; } = "";   // （外键属性）

    // 不能 new()，初次启动时，会添加两条数据
    // public PrintSettingsEntity PrintSettings { get; set; } = new();  
    public ProcessEntity? Process { get; set; }  // 引用导航属性，指向主表，C# 对象引用，数据库没有这一列，仅 EF 用来做 Join、Include 查询
}