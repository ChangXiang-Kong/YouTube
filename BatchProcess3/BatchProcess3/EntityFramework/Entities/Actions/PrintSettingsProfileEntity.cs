using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatchProcess3.EntityFramework.Entities.Actions;

[Table("ActionsPrintSettingsProfile")]
public class PrintSettingsProfileEntity : BaseEntity
{
    public string Type { get; set; } = "";
    
    public string PrinterName { get; set; } = "(Default)";  // 给默认值，否则编辑 Edit Printer Settings 页面时， Printer Name 不会自动选中 "(Default)"
    
    public string PaperSize { get; set; } = "(Default)";

    public double Width { get; set; } = -1;

    public double Height { get; set; } = -1;
    
    public string Orientation { get; set; } = "(Default)";
    
    public string SourceTray { get; set; } = "(Default)";
    
    public string DrawingColor { get; set; } = "(Default)";
    
    public bool ScaleToFil { get; set; }
    
    // public Guid PrintSettingsId { get; set; }
    public string PrintSettingsId { get; set; } = "";   // （外键属性）

    // 不能 new()，初次启动时，会添加两条数据
    // https://www.youtube.com/watch?v=ZV4-4PgnGKY&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO    37:00
    // public PrintSettingsEntity PrintSettings { get; set; } = new();  
    public PrintSettingsEntity PrintSettings { get; set; }  // 引用导航属性，指向主表，C# 对象引用，数据库没有这一列，仅 EF 用来做 Join、Include 查询
    // 导航属性 `PrintSettings`：
    //     - 它是对象引用，如果在这里赋值，需要传入完整的`PrintSettingsEntity`对象，容易造成 循环引用、跟踪异常、不必要的实体加载 等问题
    //     - EF Core 最佳实践：优先操作外键（PrintSettingsId），不要手动给导航属性赋值。
    //     导航属性一般是查询时 `.Include(x=>x.PrintSettings)` 由 EF 自动填充，新增 / 修改时直接操作外键 ID 即可。
    
    /*
        3、PrintSettingsProfileEntity 为什么要包含 PrintSettingsId 与 PrintSettings 属性？
            和第 1 点原理完全一致，它也是**多方**。
                - `PrintSettingsId`：数据库真实字段，存储所属 PrintSettings 的 Id 字符串；用来建立外键约束。
                - `PrintSettings`：引用导航，C# 中访问父级 PrintSettings 实体，需要 Include 加载。

            关系：**1 个 PrintSettings 可以有多个 Profile 档案，每个 Profile 归属唯一的 PrintSettings**。
            
            > 简单记忆：
            > ✅ 一方（父实体）：`集合导航（多个子对象），不需要额外主键字段`
            > ✅ 多方（子实体）：`外键字段 + 引用导航（单个父对象）`
     */
}