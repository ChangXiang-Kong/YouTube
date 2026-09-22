using System;
using System.ComponentModel.DataAnnotations;

namespace BatchProcess3.EntityFramework.Entities;

public class BaseEntity
{
    [Key]       // 标记为主键
    //public int Id { get; set; }
    public Guid Id { get; set; } = Guid.CreateVersion7();  // 使用 Guid.CreateVersion7() 生成有序的 GUID，避免随机 GUID 导致索引性能下降

    public DateTimeOffset? UpdateTime { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset CreateTime { get; set; } = DateTimeOffset.Now;

}