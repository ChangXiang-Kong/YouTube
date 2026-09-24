using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatchProcess3.EntityFramework.Entities.Actions;

[Table("ActionsPrintTab")]
public class PrintTabEntity : BaseEntity
{
    public string JobName { get; set; } = "";

    public string Description { get; set; } = "";

    public bool PrintModels { get; set; }

    public bool PrintDrawings { get; set; }

    public string PrintDrawingRange { get; set; } = "";

    public bool DrawingExclusionIsWhiteList { get; set; }

    public string DrawingExclusionList { get; set; } = "";

    // public Guid PrintSettingsId { get; set; }
    public string PrintSettingsId { get; set; } = "";

    // 不能 new()，初次启动时，会添加两条数据
    // https://www.youtube.com/watch?v=ZV4-4PgnGKY&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO    37:00
    // public PrintSettingsEntity PrintSettings { get; set; } = new();  
    public PrintSettingsEntity PrintSettings { get; set; }
}