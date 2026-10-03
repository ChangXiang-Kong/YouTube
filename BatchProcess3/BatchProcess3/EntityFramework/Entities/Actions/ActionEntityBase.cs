using System.ComponentModel.DataAnnotations;

namespace BatchProcess3.EntityFramework.Entities.Actions;

public class ActionEntityBase : EntityBase
{
    [MaxLength(200)]
    public string JobName { get; set; } = "";
    
    [MaxLength(5000)]
    public string Description { get; set; } = "";

    public int SortOrder { get; set; }
    
}