using System;
using System.ComponentModel.DataAnnotations;

namespace BatchProcess3.EntityFramework.Entities;

public class BaseEntity
{
    [Key]       // 标记为主键
    //public long Id { get; set; }
    // public Guid Id { get; set; } = Guid.CreateVersion7();  // 使用 Guid.CreateVersion7() 生成有序的 GUID，避免随机 GUID 导致索引性能下降
    // [MaxLength(100)]
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    /// <summary>
    /// 用于需要拼接 Id 时的可选项字段，如（父Id|子Id）<br/>
    /// 当使用 Guid 类型的 Id 时，无法拼接字符串，这时就可以使用该字段了
    /// </summary>
    public string? ConcatenatedId { get; set; }
    
    /// <summary> 删除标记，主要用于伪删除 </summary>
    public bool IsDeleted { get; set; }
    
    public DateTimeOffset? UpdateTime { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset CreateTime { get; set; } = DateTimeOffset.Now;

}