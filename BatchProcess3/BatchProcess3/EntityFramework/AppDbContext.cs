using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BatchProcess3.Data;
using BatchProcess3.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace BatchProcess3.EntityFramework;

public class AppDbContext : DbContext
{
    // 每个 DbSet 会映射一张表到数据库中，属性名为表名
    public DbSet<SettingsEntity> Settings { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);     // 调用与否没有影响
        
        // 若在外部已经配置了数据库连接，则不再执行此处的配置
        if (!optionsBuilder.IsConfigured)
        {
            string tableName = $"{ResourceToken.AppName}.db";
            //string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tableName);  // 旧（.NET Framework）
            string dbPath = Path.Combine(AppContext.BaseDirectory, tableName);  // 新（.NET 6+）
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