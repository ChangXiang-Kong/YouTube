using System;

namespace BatchProcess3.EntityFramework.Entities;

public class PrintSettingsProfileEntity : BaseEntity
{
    public Guid PrintSettingsId { get; set; }

    public PrintSettingsEntity PrintSettings { get; set; } = new();
    
    public string Type { get; set; } = "";
    
    public string PrinterName { get; set; } = "";
    
    public string PaperSize { get; set; } = "";
    
    public double Width { get; set; }
    
    public double Height { get; set; }
    
    public string Orientation { get; set; } = "";
    
    public string SourceTray { get; set; } = "";
    
    public string DrawingColor { get; set; } = "";
    
    public bool ScaleToFil;
}