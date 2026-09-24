using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatchProcess3.EntityFramework.Entities;

[Table("Settings")]
public class SettingsEntity : BaseEntity
{
    public bool SkipNoActionFiles { get; set; }
    
    public bool AllowDuplicateEntries { get; set; }
    
    public List<string> LocationPathsList { get; set; } = [];

    [MaxLength(100)]
    public string SolidWorksHost { get; set; } = "";    // 全部都要有默认值

    [MaxLength(100)]
    public string PdmeVaultName { get; set; } = "";
    
    [MaxLength(100)]
    public string PdmeUserName { get; set; } = "";
    
    [MaxLength(100)]
    public string PdmePassword { get; set; } = "";
}
