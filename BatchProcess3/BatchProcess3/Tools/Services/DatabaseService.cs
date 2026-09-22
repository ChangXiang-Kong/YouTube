using System;
using BatchProcess3.EntityFramework;
using BatchProcess3.EntityFramework.Entities;

namespace BatchProcess3.Tools.Services;

public class DatabaseService(AppDbContext dbContext) : IDisposable
{
    private readonly AppDbContext _dbContext = dbContext;

    public void ApplyMigrations()
    {
        // TODO: Change to migrations once we start persisting data
        _dbContext.Database.EnsureCreated();
    }
    
    // public SettingsEntity
    
    
    
    
    public void Dispose()
    {
        _dbContext.Dispose();
    }
}