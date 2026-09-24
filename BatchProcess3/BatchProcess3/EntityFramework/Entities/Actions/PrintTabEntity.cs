using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatchProcess3.EntityFramework.Entities.Actions;

[Table("ActionsPrintTab")]
public class PrintTabEntity : BaseEntity
{
    public string JobName { get; set; } = "";

    public string Description { get; set; } = "";

    public bool PrintModels { get; set; }

    public bool PrintDrawings { get; set; }

    public string PrintDrawingRange { get; set; } = "";

    public bool DrawingExclusionIsWhiteList { get; set; }

    public string DrawingExclusionList { get; set; } = "";

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
        1、PrintTabEntity 为什么要包含 PrintSettingsId 与 PrintSettings 属性？
            一对多里，多方（PrintTab）同时包含「外键字段」+「引用导航」，这叫 显式外键，是 EF Core 推荐写法。
            
            1. **`PrintSettingsId`（外键）**
               - 真实存在数据库表 `ActionsPrintTab` 的一列，保存主表 `PrintSettingsEntity.Id` 的字符串值。
               - 作用：数据库层面建立关联。可以**不加载主实体**，只通过 ID 就能关联、查询、修改归属关系。
               - 比如：新增 PrintTab 时，直接给`PrintSettingsId`赋值父 Id，不需要把完整 PrintSettings 实体查出来。
            2. **`PrintSettings`（引用导航属性）**
               - **数据库不会生成这一列**，只是 C# 对象引用。
               - 作用：在 C# 代码里，通过`printTab.PrintSettings.Name`直接访问所属打印设置的主实体数据；需要配合 `.Include(x=>x.PrintSettings)` 预加载。

            > 一句话总结：
            > `PrintSettingsId` 是**数据库关联纽带**；`PrintSettings` 是**C# 代码方便读取父对象的导航入口**。
            
            > 简单记忆：
            > ✅ 一方（父实体）：`集合导航（多个子对象），不需要额外主键字段`
            > ✅ 多方（子实体）：`外键字段 + 引用导航（单个父对象）`
     */
}