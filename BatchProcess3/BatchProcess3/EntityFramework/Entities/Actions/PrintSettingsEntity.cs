using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatchProcess3.EntityFramework.Entities.Actions;

[Table("ActionsPrintSettings")]
public class PrintSettingsEntity : BaseEntity
{
    public string Name { get; set; } = "";

    public string Description { get; set; } = "";
    
    public int Copies { get; set; } = 1;

    // 一对多：一个 PrintSettingsEntity 对应 多个 PrintSettingsProfileEntity
    // 不能 new()
    // https://www.youtube.com/watch?v=ZV4-4PgnGKY&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO    37:00
    // public List<PrintSettingsProfileEntity> PrintSettingsProfilesList = []; // 不要 new()
    public List<PrintSettingsProfileEntity> PrintSettingsProfilesList { get; set; } // 不要 new()
    
    // 一对多：一个 PrintSettingsEntity 对应 多个 PrintTabEntity
    // 不能 new()
    // https://www.youtube.com/watch?v=ZV4-4PgnGKY&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO    37:00
    // public List<PrintTabEntity> PrintTabsList = []; // 不要 new()
    public List<PrintTabEntity> PrintTabsList { get; set; } // 不要 new()

    public bool CanEdit { get; set; }
    
    public bool CanDelete { get; set; }
    
    /*
        2、PrintSettingsEntity 为什么要包含 PrintSettingsProfilesList 与 PrintTabsList 属性？
            这两个是**集合导航属性（Collection Navigation）**，位于**一的那一方（主实体 PrintSettings）**。

            - 数据库**不会在 ActionsPrintSettings 表里生成这两列**。
            - 含义：**一个打印设置，拥有多个打印标签、多个打印配置模板**。
            - 代码作用：
              1. 查询：`_dbContext.PrintSettings.Include(x=>x.PrintTabsList)` 一次性把该设置下所有标签查出来；
              2. 新增：直接 `printSettings.PrintTabsList.Add(new PrintTabEntity())`，EF 自动填充子实体的`PrintSettingsId`外键；
              3. 业务语义：直观表达 “这个打印设置包含哪些标签、哪些配置档案”。
              
            > 简单记忆：
            > ✅ 一方（父实体）：`集合导航（多个子对象），不需要额外主键字段`
            > ✅ 多方（子实体）：`外键字段 + 引用导航（单个父对象）`
     */
}