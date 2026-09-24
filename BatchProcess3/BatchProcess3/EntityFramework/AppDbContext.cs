using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BatchProcess3.Data;
using BatchProcess3.EntityFramework.Entities;
using BatchProcess3.EntityFramework.Entities.Actions;
using Microsoft.EntityFrameworkCore;

namespace BatchProcess3.EntityFramework;

public class AppDbContext : DbContext
{
    // 每个 DbSet 会映射一张表到数据库中，属性名为表名
    public DbSet<SettingsEntity> Settings { get; set; }
    
    // Actions
    public DbSet<PrintTabEntity> PrintTab { get; set; }
    public DbSet<PrintSettingsEntity> PrintSettings { get; set; }
    public DbSet<PrintSettingsProfileEntity> PrintSettingsProfile { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);     // 调用与否没有影响
        
        // 若在外部已经配置了数据库连接，则不再执行此处的配置
        if (!optionsBuilder.IsConfigured)
        {
            string tableName = $"{ResourceToken.AppName}.db";
            //string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tableName);  // 旧（.NET Framework）
            string dbPath = Path.Combine(AppContext.BaseDirectory, tableName);  // 新（.NET 6+）
            // 以上两种方式的结果都是：dbPath = D:\Desktop\YouTube\BatchProcess3\BatchProcess3.Desktop\bin\Debug\net10.0\
            
            /* 注意：
                在使用了
                     $ dotnet ef migrations add InitialCreate -v
                     $ dotnet ef database Update
                 命令后，
                 会生成数据库文件 D:\Desktop\YouTube\BatchProcess3\BatchProcess3\bin\Debug\net10.0\BatchProcess3.db，
                 其中有 __EFMigrationsHistory、__EFMigrationsLock、Settings 表，
                 但数据不会保存在该数据库文件
                 
                 而在执行数据库保存操作时，则会将数据保存到 D:\Desktop\YouTube\BatchProcess3\BatchProcess3.Desktop\bin\Debug\net10.0\BatchProcess3.db 中，
                 其中只有 Settings 表
             */
            
            // SQLite 支持数据库级联，但**必须开启外键**，连接字符串需添加："Foreign Keys=True"，否则数据库级联不生效。
            optionsBuilder.UseSqlite($"Data Source={dbPath};Foreign Keys=True")
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
            /* 常见数据库提供程序的示例 https://learn.microsoft.com/zh-cn/ef/core/dbcontext-configuration/
                数据库系统	                 配置示例	NuGet 程序包
                SQL Server 或 Azure SQL      .UseSqlServer(connectionString)             	Microsoft.EntityFrameworkCore.SqlServer
                Azure Cosmos DB	             .UseCosmos(connectionString, databaseName)     Microsoft.EntityFrameworkCore.Cosmos
                SQLite	                     .UseSqlite(connectionString)	                Microsoft.EntityFrameworkCore.Sqlite
                EF Core 内存数据库	         .UseInMemoryDatabase(databaseName)	            Microsoft.EntityFrameworkCore.InMemory
                PostgreSQL*	                 .UseNpgsql(connectionString)	                Npgsql.EntityFrameworkCore.PostgreSQL
                MySQL/MariaDB*	             .UseMySql(connectionString)	                Pomelo.EntityFrameworkCore.MySql
                Oracle*	                     .UseOracle(connectionString)	                Oracle.EntityFrameworkCore
             */
        }

        // 完成后点击【Tools】=>【NuGet Package Manager】=>【Package Manager Console】并输入如下命令
        //      PM> Add-Migration InitialCreate -v  // 初始化表结构
        //      PM> Update-Database                 // 更新数据库，创建表
        // 若新添加了一个属性，可使用如下命令更新表
        //      PM> Add-Migration AddEmail          // 表示表新添加了Email属性，更新表结构
        //      PM> Update-Database

        // 若使用 VS Code，可以先进入到工作目录并输入如下命令
        //      $ dotnet tool install -g dotnet-ef
        //      $ dotnet ef migrations add InitialCreate -v
        //      $ dotnet ef database Update

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);     // 调用与否没有影响

        // SettingsEntity
        // modelBuilder.Entity<SettingsEntity>().HasKey(x => x.Id);    // 显式标记 主键 为 Id。若 xxxEntity 已添加 名为 Id 的 属性，EFCore会自动将其识别为 主键，这里就不需要了
        
        // PrintTabEntity
        // modelBuilder.Entity<PrintTabEntity>().HasKey(x => x.Id);    // 显式标记 主键 为 Id。若 xxxEntity 已添加 名为 Id 的 属性，EFCore会自动将其识别为 主键，这里就不需要了
        
        // TODO: 以下代码需要查资料，搞明白 HasMany、WithOne、HasOne、WithMany 等如何用，Cascade 与 ClientCascade 的区别
        // PrintSettingsEntity
        // modelBuilder.Entity<PrintSettingsEntity>().HasKey(x => x.Id);    // 显式标记 主键 为 Id。若 xxxEntity 已添加 名为 Id 的 属性，EFCore会自动将其识别为 主键，这里就不需要了
        modelBuilder.Entity<PrintSettingsEntity>()
            .HasMany(x => x.PrintTabsList)
            .WithOne(x => x.PrintSettings)
            .HasForeignKey(x => x.PrintSettingsId)
            .OnDelete(DeleteBehavior.ClientCascade);
        
        /*
            ClientCascade 与 Cascade 的区别是什么？
                两个都是级联删除策略，核心差异：**删除逻辑是在数据库执行，还是在 EF 客户端（你的 C# 程序内存）执行**
                
                选项
                    DeleteBehavior.Cascade                          DeleteBehavior.ClientCascade
                适用场景
                    简单级联，数据库支持且无需额外业务逻辑              数据库不支持级联、需要软删除、避免多级联路径冲突、需要业务逻辑介入
                    SQLite/SQL Server 支持外键级联，数据量大          不想数据库承担级联逻辑、或者数规避循环外键报错
                执行位置
                    ✅ **数据库端**                                 ✅ **EF Core 客户端（内存）**
                性能
                    高，一次数据库操作完成                            较低，需要将子实体加载到内存并逐个删除
                数据库外键
                    数据库添加外键约束 `ON DELETE CASCADE`。          **数据库不会添加 ON DELETE CASCADE 约束**，通常为 Restrict 或 No Action。
                删除主实体时
                    删除父记录时，数据库自动删除所有关联子记录。         EF Core 在 C# 代码里，把已经被 DbContext 追踪加载的子实体，标记为 Remove，在执行 SaveChanges 时生成 DELETE 语句进行批量删除。
                未跟踪的子实体
                    数据库直接删除，不受影响                          如果子实体未被加载/跟踪，EF Core 不会自动删除，数据库外键会阻止删除并抛出异常（除非数据库外键允许）
                业务逻辑
                    绕过 EF Core 的 SaveChanges 拦截器、软删除等逻辑   会经过 EF Core 的变更跟踪，可以触发拦截器、软删除等
                注意事项
                    1. 不需要提前加载子实体到 DbContext               ⚠️ **必须提前 Include 加载所有子实体到 DbContext，否则没加载到内存的子实体不会被删除！**
                    2. SQLite 默认需要开启外键支持；
                    3. 部分场景会有循环级联报错
                    
                补充说明
                    - DeleteBehavior.Cascade 在 EF Core 中也会在客户端对已跟踪实体执行级联删除，但它的核心是数据库外键也设置为级联。
                    - DeleteBehavior.ClientCascade 只做客户端级联，数据库外键不级联。
                    - 还有 ClientSetNull（将外键设为 null）、Restrict（阻止删除）、NoAction 等。
                
                根据业务举例
                    Cascade（数据库级联）：.OnDelete(DeleteBehavior.Cascade);
                        - 数据库建表时，外键约束带上 `ON DELETE CASCADE`
                        - 代码：`_dbContext.Remove(parentPrintSettings); _dbContext.SaveChanges();`
                        - 哪怕**不 Include 加载 PrintTabsList**，数据库收到删除父记录 SQL，自动把子表对应的 PrintTab 全部删掉。
                        
                    ClientCascade（客户端级联）：.OnDelete(DeleteBehavior.ClientCascade);
                        - 数据库外键**没有 ON DELETE CASCADE**
                        - 如果只删除父实体，**没有 Include 加载子实体**：子实体保留在数据库，不会删除！
                        - 必须写：
                            var parent = _dbContext.PrintSettings
                                .Include(x => x.PrintTabsList)
                                .Include(x => x.PrintSettingsProfilesList)
                                .First(...);
                            _dbContext.Remove(parent);
                            _dbContext.SaveChanges();
                        EF 看到内存里加载出来的子集合，自动把子项标记删除，SaveChanges 后发送多条 DELETE 语句。
                
                选型建议（当前 SQLite 项目）
                    SQLite 支持数据库级联，但**必须开启外键**，连接字符串添加：Data Source=BatchProcess3.db;Foreign Keys=True
                    
                    - 如果你能保证每次删除父实体都 Include 加载全部子实体：可以用`ClientCascade`，数据库不处理级联；
                    - 如果希望数据库自动清理子记录，不需要每次都 Include：推荐`Cascade`，记得打开 SQLite 外键开关。
                    > 坑提醒：`ClientCascade`最容易踩坑：忘记 Include，删了父，子数据残留，数据脏了。
        */
        
        // PrintSettingsProfileEntity
        // modelBuilder.Entity<PrintSettingsProfileEntity>().HasKey(x => x.Id);    // 显式标记 主键 为 Id。若 xxxEntity 已添加 名为 Id 的 属性，EFCore会自动将其识别为 主键，这里就不需要了
        modelBuilder.Entity<PrintSettingsProfileEntity>()
            .HasOne(x => x.PrintSettings)
            .WithMany(x => x.PrintSettingsProfilesList)
            .HasForeignKey(x => x.PrintSettingsId)
            .OnDelete(DeleteBehavior.ClientCascade);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);    // 调用与否没有影响
        
        
    }

    // 同步保存时的额外逻辑（同步保存时自动调用SaveChanges()）
    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries)
        {
            var now = DateTimeOffset.Now;
            // now = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Offset);  // 去除毫秒部分

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreateTime = now;
                entry.Entity.UpdateTime = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdateTime = now;
            }
        }

        return base.SaveChanges();
    }

    // 若需要异步保存重写 SaveChangesAsync
    /// 异步保存时的额外逻辑（异步保存时自动调用SaveChangesAsync()）
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries)
        {
            var now = DateTimeOffset.Now;
            // now = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Offset);  // 去除毫秒部分

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreateTime = now;
                entry.Entity.UpdateTime = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdateTime = now;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}