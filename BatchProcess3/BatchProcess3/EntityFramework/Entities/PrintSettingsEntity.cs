using System.Collections.Generic;

namespace BatchProcess3.EntityFramework.Entities;

public class PrintSettingsEntity : BaseEntity
{
    public string Name { get; set; } = "";

    public string Description { get; set; } = "";
    
    public int Copies = 1;

    public List<PrintSettingsProfileEntity> PrintSettingsProfiles = [];
}