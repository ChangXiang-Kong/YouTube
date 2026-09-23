using System;

namespace BatchProcess3.EntityFramework.Entities.Actions;

public class PrintSettingsProfileEntity : BaseEntity
{
    public string Type { get; set; } = "";
    
    public string PrinterName { get; set; } = "";
    
    public string PaperSize { get; set; } = "(Default)";

    public double Width { get; set; } = -1;

    public double Height { get; set; } = -1;
    
    public string Orientation { get; set; } = "(Default)";
    
    public string SourceTray { get; set; } = "(Default)";
    
    public string DrawingColor { get; set; } = "(Default)";
    
    public bool ScaleToFil;
    
    public Guid PrintSettingsId { get; set; }

    // 不能 new()，初次启动时，会添加两条数据
    // https://www.youtube.com/watch?v=ZV4-4PgnGKY&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO    37:00
    // public PrintSettingsEntity PrintSettings { get; set; } = new();  
    public PrintSettingsEntity PrintSettings { get; set; }
}