using Microsoft.EntityFrameworkCore;
using Homework3.Models;

namespace Homework3.Data;

/// <summary>
/// Контекст базы данных приложения
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Набор сущностей игровых платформ
    /// </summary>
    public DbSet<Platform> Platforms { get; set; }

    /// <summary>
    /// Набор сущностей видеоигр
    /// </summary>
    public DbSet<Game> Games { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=app.db");
}
