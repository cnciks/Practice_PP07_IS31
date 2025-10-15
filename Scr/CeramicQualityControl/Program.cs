using CeramicQualityControl.Data;
using Microsoft.EntityFrameworkCore;

namespace CeramicQualityControl
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // ДИАГНОСТИКА
            TestDatabaseConnection();

            Application.Run(new MainForm());
        }

        static void TestDatabaseConnection()
        {
            try
            {
                using var context = new AppDbContext();

                // 1. Проверяем подключение
                var canConnect = context.Database.CanConnect();
                MessageBox.Show($"🔍 Подключение к базе: {canConnect}", "Диагностика");

                if (canConnect)
                {
                    // 2. Проверяем таблицы
                    var roles = context.Roles.ToList();
                    var users = context.Users.ToList();

                    MessageBox.Show($"📊 Данные в базе:\n" +
                                   $"Ролей: {roles.Count}\n" +
                                   $"Пользователей: {users.Count}\n" +
                                   $"Таблицы: Parties, Products, Defects, ResultControls",
                                   "Диагностика базы");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Ошибка диагностики: {ex.Message}", "Ошибка");
            }
        }
    }
}
