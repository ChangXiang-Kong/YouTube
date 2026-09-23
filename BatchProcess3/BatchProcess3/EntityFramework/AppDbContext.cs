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
            
            optionsBuilder.UseSqlite($"Data Source={dbPath}")
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