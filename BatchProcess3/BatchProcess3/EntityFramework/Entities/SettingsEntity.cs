using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BatchProcess3.EntityFramework.Entities;

public class SettingsEntity : BaseEntity
{
    public bool SkipNoActionFiles { get; set; }
    
    public bool AllowDuplicateEntries { get; set; }
    
    public List<string> LocationPaths { get; set; } = [];

    [MaxLength(100)]
    public string SolidWorksHost { get; set; } = "";    // 全部都要有默认值

    [MaxLength(100)]
    public string PdmeVaultName { get; set; } = "";
    
    [MaxLength(100)]
    public string PdmeUserName { get; set; } = "";
    
    [MaxLength(100)]
    public string PdmePassword { get; set; } = "";
}
