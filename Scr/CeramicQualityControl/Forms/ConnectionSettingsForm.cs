using Microsoft.Data.SqlClient;
using System.Configuration;

namespace CeramicQualityControl.Forms
{
    public partial class ConnectionSettingsForm : Form
    {
        private TextBox txtConnectionString = null!;
        private Button btnTest = null!;
        private Button btnSave = null!;
        private Label lblStatus = null!;

        public ConnectionSettingsForm()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.Size = new Size(800, 300);
            this.Text = "Настройки подключения";
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Заголовок
            var lblTitle = new Label
            {
                Text = "Настройки подключения к базе данных",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Поле ввода
            var lblConnection = new Label
            {
                Text = "Строка подключения:",
                Location = new Point(20, 70),
                AutoSize = true
            };

            txtConnectionString = new TextBox
            {
                Location = new Point(20, 95),
                Size = new Size(600, 23),
                Text = @"Server=DESKTOP-OR4548Q;Database=Ceramic;Trusted_Connection=true;TrustServerCertificate=true;"
            };

            // Кнопки
            btnTest = new Button
            {
                Location = new Point(630, 95),
                Size = new Size(120, 23),
                Text = "Тест подключения"
            };
            btnTest.Click += BtnTest_Click;

            btnSave = new Button
            {
                Location = new Point(20, 140),
                Size = new Size(100, 30),
                Text = "Сохранить"
            };
            btnSave.Click += BtnSave_Click;

            // Статус
            lblStatus = new Label
            {
                Location = new Point(20, 180),
                Size = new Size(400, 20),
                Text = "Статус: не подключено",
                ForeColor = Color.Red
            };

            // Упрощенная инициализация коллекции
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblConnection);
            this.Controls.Add(txtConnectionString);
            this.Controls.Add(btnTest);
            this.Controls.Add(btnSave);
            this.Controls.Add(lblStatus);

            // Назначаем клавиши по умолчанию
            this.AcceptButton = btnTest;
            this.CancelButton = btnSave;
        }

        private void BtnTest_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtConnectionString.Text))
            {
                lblStatus.Text = "Ошибка: введите строку подключения";
                lblStatus.ForeColor = Color.Red;
                txtConnectionString.Focus();
                return;
            }

            try
            {
                using var connection = new SqlConnection(txtConnectionString.Text);
                connection.Open();

                // Проверяем, что соединение действительно работает
                using var command = new SqlCommand("SELECT 1", connection);
                var result = command.ExecuteScalar();

                lblStatus.Text = "Статус: подключение успешно";
                lblStatus.ForeColor = Color.Green;

                connection.Close();
            }
            catch (SqlException ex)
            {
                lblStatus.Text = $"Ошибка SQL: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Ошибка: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtConnectionString.Text))
            {
                MessageBox.Show("Введите строку подключения", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConnectionString.Focus();
                return;
            }

            try
            {
                // Сохраняем в App.config
                SaveConnectionStringToConfig(txtConnectionString.Text);

                MessageBox.Show("Настройки подключения сохранены", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении настроек: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveConnectionStringToConfig(string connectionString)
        {
            try
            {
                // Получаем конфигурацию приложения
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                // Безопасная проверка ConnectionStrings
                var connectionStringsSection = config.ConnectionStrings;
                if (connectionStringsSection == null)
                {
                    // Создаем секцию если её нет
                    config.Sections.Add("connectionStrings", new ConnectionStringsSection());
                    // Обновляем ссылку после добавления
                    connectionStringsSection = config.ConnectionStrings;
                }

                // Безопасное сохранение строки подключения
                if (connectionStringsSection != null)
                {
                    var defaultConnection = connectionStringsSection.ConnectionStrings["DefaultConnection"];
                    if (defaultConnection != null)
                    {
                        defaultConnection.ConnectionString = connectionString;
                    }
                    else
                    {
                        connectionStringsSection.ConnectionStrings.Add(new ConnectionStringSettings
                        {
                            Name = "DefaultConnection",
                            ConnectionString = connectionString,
                            ProviderName = "System.Data.SqlClient"
                        });
                    }

                    // Сохраняем изменения
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("connectionStrings");
                }
            }
            catch (Exception ex)
            {
                // Если не удалось сохранить в config, используем простой файл
                SaveConnectionStringToFile(connectionString);
                // Перебрасываем исключение чтобы показать сообщение в BtnSave_Click
                throw new InvalidOperationException($"Не удалось сохранить в config: {ex.Message}");
            }
        }

        private void SaveConnectionStringToFile(string connectionString)
        {
            try
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var appFolder = Path.Combine(appDataPath, "CeramicQualityControl");

                // Создаем папку если не существует
                if (!Directory.Exists(appFolder))
                {
                    Directory.CreateDirectory(appFolder);
                }

                var configFile = Path.Combine(appFolder, "connection.config");
                File.WriteAllText(configFile, connectionString);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Не удалось сохранить в файл: {ex.Message}");
            }
        }

        private string? LoadConnectionStringFromFile()
        {
            try
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var configFile = Path.Combine(appDataPath, "CeramicQualityControl", "connection.config");

                if (File.Exists(configFile))
                {
                    return File.ReadAllText(configFile);
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не прерываем выполнение
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки из файла: {ex.Message}");
            }

            return null;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Загружаем сохраненные настройки если есть
            try
            {
                // Безопасная загрузка из App.config
                var connectionStringsSection = ConfigurationManager.ConnectionStrings;
                if (connectionStringsSection != null)
                {
                    var defaultConnection = connectionStringsSection["DefaultConnection"];
                    if (defaultConnection != null && !string.IsNullOrEmpty(defaultConnection.ConnectionString))
                    {
                        txtConnectionString.Text = defaultConnection.ConnectionString;
                        return;
                    }
                }

                // Пробуем загрузить из файла
                var fileConnectionString = LoadConnectionStringFromFile();
                if (!string.IsNullOrEmpty(fileConnectionString))
                {
                    txtConnectionString.Text = fileConnectionString;
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не прерываем выполнение
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки настроек: {ex.Message}");
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            // Отписываемся от событий
            btnTest.Click -= BtnTest_Click;
            btnSave.Click -= BtnSave_Click;
        }
    }
}
