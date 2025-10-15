using Microsoft.EntityFrameworkCore;
using CeramicQualityControl.Models;

namespace CeramicQualityControl.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Party> Parties { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Defect> Defects { get; set; }
        public DbSet<ResultControl> ResultControls { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // ПРОБУЕМ РАЗНЫЕ ВАРИАНТЫ ПОДКЛЮЧЕНИЯ
                string connectionString = @"Server=DESKTOP-OR4548Q;Database=Ceramic;Trusted_Connection=true;TrustServerCertificate=true;";

                optionsBuilder.UseSqlServer(connectionString);
                optionsBuilder.EnableSensitiveDataLogging(); // Для отладки
                optionsBuilder.LogTo(Console.WriteLine); // Логируем SQL запросы
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // УБИРАЕМ ВСЕ СЛОЖНЫЕ НАСТРОЙКИ - ДЕЛАЕМ ПРОСТО
            base.OnModelCreating(modelBuilder);
        }
    }
}
