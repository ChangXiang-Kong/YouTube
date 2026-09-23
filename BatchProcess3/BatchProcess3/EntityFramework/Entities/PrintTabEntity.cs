using System;

namespace BatchProcess3.EntityFramework.Entities;

public class PrintTabEntity : BaseEntity
{
    public Guid PrintSettingsId { get; set; }

    public PrintSettingsEntity PrintSettings { get; set; } = new();
    
    public string JobName { get; set; } = "";

    public string Description { get; set; } = "";

    public bool PrintModels { get; set; }

    public bool PrintDrawings { get; set; }

    public string PrintDrawingRange { get; set; } = "";

    public bool DrawingExclusionIsWhiteList { get; set; }

    public string DrawingExclusionList { get; set; } = "";
}