using CeramicQualityControl.Data;
using CeramicQualityControl.Models;
using Microsoft.EntityFrameworkCore;

namespace CeramicQualityControl.Forms
{
    public partial class UsersForm : Form
    {
        private DataGridView dataGridView = null!;
        private Button btnAdd = null!;
        private Button btnEdit = null!;
        private Button btnDelete = null!;
        private Button btnChangePassword = null!;
        private readonly AppDbContext context;

        public UsersForm()
        {
            InitializeComponent();
            context = new AppDbContext();
            SetupForm();
            LoadUsers();
        }

        private void SetupForm()
        {
            this.Size = new Size(800, 500);
            this.Text = "Управление пользователями";
            this.StartPosition = FormStartPosition.CenterParent;

            // Заголовок
            var lblTitle = new Label
            {
                Text = "Режим: Пользователи",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Таблица
            dataGridView = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(740, 350),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false
            };

            // Кнопки
            btnAdd = new Button
            {
                Location = new Point(20, 420),
                Size = new Size(80, 30),
                Text = "Добавить"
            };
            btnEdit = new Button
            {
                Location = new Point(110, 420),
                Size = new Size(80, 30),
                Text = "Изменить"
            };
            btnDelete = new Button
            {
                Location = new Point(200, 420),
                Size = new Size(80, 30),
                Text = "Удалить"
            };
            btnChangePassword = new Button
            {
                Location = new Point(290, 420),
                Size = new Size(120, 30),
                Text = "Смена пароля"
            };

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnChangePassword.Click += BtnChangePassword_Click;

            // Упрощенная инициализация коллекции
            this.Controls.Add(lblTitle);
            this.Controls.Add(dataGridView);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
            this.Controls.Add(btnChangePassword);
        }

        private void LoadUsers()
        {
            try
            {
                dataGridView.DataSource = context.Users
                    .Include(u => u.Role)
                    .Select(u => new {
                        u.UserID,
                        u.Username,
                        Role = u.Role.RoleName,
                        u.IsActive,
                        u.CreatedDate
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пользователей: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            try
            {
                using var editForm = new UserEditForm(null, context);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadUsers();
                    MessageBox.Show("Пользователь успешно добавлен", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении пользователя: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите пользователя для редактирования", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // Безопасное извлечение UserID
                var selectedRow = dataGridView.SelectedRows[0];
                var userIDValue = selectedRow.Cells["UserID"].Value;

                if (userIDValue == null)
                {
                    MessageBox.Show("Не удалось получить ID пользователя", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var userID = (int)userIDValue;
                var user = context.Users.Find(userID);

                if (user == null)
                {
                    MessageBox.Show("Выбранный пользователь не найден", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using var editForm = new UserEditForm(user, context);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadUsers();
                    MessageBox.Show("Пользователь успешно изменен", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании пользователя: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите пользователя для удаления", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Безопасное извлечение данных
            var selectedRow = dataGridView.SelectedRows[0];
            var userIDValue = selectedRow.Cells["UserID"].Value;
            var usernameValue = selectedRow.Cells["Username"].Value;

            if (userIDValue == null)
            {
                MessageBox.Show("Не удалось получить ID пользователя", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var userID = (int)userIDValue;
            var username = usernameValue?.ToString() ?? "неизвестный пользователь";

            // Нельзя удалять администратора
            if (userID == 1) // ID администратора
            {
                MessageBox.Show("Нельзя удалить администратора системы", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = MessageBox.Show($"Вы уверены, что хотите удалить пользователя '{username}'?",
                "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var user = context.Users.Find(userID);
                    if (user != null)
                    {
                        context.Users.Remove(user);
                        context.SaveChanges();
                        LoadUsers();

                        MessageBox.Show("Пользователь успешно удален", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении пользователя: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnChangePassword_Click(object? sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите пользователя для смены пароля", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // Безопасное извлечение UserID
                var selectedRow = dataGridView.SelectedRows[0];
                var userIDValue = selectedRow.Cells["UserID"].Value;

                if (userIDValue == null)
                {
                    MessageBox.Show("Не удалось получить ID пользователя", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var userID = (int)userIDValue;
                using var changePasswordForm = new ChangePasswordForm(userID, context);
                changePasswordForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при смене пароля: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод для обновления состояния кнопок
        private void UpdateButtonStates()
        {
            bool hasSelection = dataGridView.SelectedRows.Count > 0;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
            btnChangePassword.Enabled = hasSelection;

            if (hasSelection)
            {
                // Безопасная проверка на администратора
                var selectedRow = dataGridView.SelectedRows[0];
                var userIDValue = selectedRow.Cells["UserID"].Value;

                if (userIDValue != null)
                {
                    var userID = (int)userIDValue;
                    btnDelete.Enabled = userID != 1; // Запрещаем удаление администратора
                }
                else
                {
                    btnDelete.Enabled = false;
                }
            }
        }

        private void DataGridView_SelectionChanged(object? sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateButtonStates();

            // Подписываемся на событие изменения выбора
            dataGridView.SelectionChanged += DataGridView_SelectionChanged;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            // Отписываемся от событий
            dataGridView.SelectionChanged -= DataGridView_SelectionChanged;
            btnAdd.Click -= BtnAdd_Click;
            btnEdit.Click -= BtnEdit_Click;
            btnDelete.Click -= BtnDelete_Click;
            btnChangePassword.Click -= BtnChangePassword_Click;

            context?.Dispose();
        }
    }
}
