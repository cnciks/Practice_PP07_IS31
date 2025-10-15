using CeramicQualityControl.Data;
using CeramicQualityControl.Models;
using Microsoft.EntityFrameworkCore;

namespace CeramicQualityControl.Forms
{
    public partial class RolesForm : Form
    {
        private DataGridView dataGridView = null!;
        private Button btnAdd = null!;
        private Button btnEdit = null!;
        private Button btnDelete = null!;
        private Button btnRefresh = null!;
        private AppDbContext _context = null!;

        public RolesForm()
        {
            InitializeComponent();
            _context = new AppDbContext(); // СОЗДАЕМ КОНТЕКСТ ОДИН РАЗ
            SetupForm();
            LoadRoles();
        }

        private void SetupForm()
        {
            this.Size = new Size(700, 400);
            this.Text = "Управление ролями - ТЕСТ";
            this.StartPosition = FormStartPosition.CenterParent;

            // Заголовок
            var lblTitle = new Label
            {
                Text = "ТЕСТ: Управление ролями",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true,
                ForeColor = Color.Blue
            };

            // Таблица
            dataGridView = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(640, 250),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true
            };

            // Кнопки
            btnAdd = new Button
            {
                Location = new Point(20, 320),
                Size = new Size(80, 30),
                Text = "Добавить"
            };
            btnEdit = new Button
            {
                Location = new Point(110, 320),
                Size = new Size(80, 30),
                Text = "Изменить"
            };
            btnDelete = new Button
            {
                Location = new Point(200, 320),
                Size = new Size(80, 30),
                Text = "Удалить"
            };
            btnRefresh = new Button
            {
                Location = new Point(290, 320),
                Size = new Size(80, 30),
                Text = "Обновить",
                BackColor = Color.LightGreen
            };

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefresh.Click += BtnRefresh_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(dataGridView);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
            this.Controls.Add(btnRefresh);
        }

        private void LoadRoles()
        {
            try
            {
                // Очищаем отслеживание изменений и загружаем заново
                _context.ChangeTracker.Clear();
                var roles = _context.Roles.ToList();

                dataGridView.DataSource = roles;

                // УБИРАЕМ сообщение при каждой загрузке (опционально)
                // или оставляем для отладки
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Ошибка загрузки ролей: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            try
            {
                // ИСПОЛЬЗУЕМ _context вместо создания нового
                var newRole = new Role
                {
                    RoleName = $"ТестРоль_{DateTime.Now:HHmmss}",
                    Description = "Тестовая роль"
                };

                _context.Roles.Add(newRole);
                var result = _context.SaveChanges();

                MessageBox.Show($"✅ Роль добавлена! Сохранено записей: {result}\n" +
                               $"ID: {newRole.RoleID}, Name: {newRole.RoleName}",
                               "Успех",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadRoles(); // Перезагружаем данные
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Ошибка добавления: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите роль для редактирования", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // ИСПОЛЬЗУЕМ _context вместо создания нового
                var selectedRow = dataGridView.SelectedRows[0];
                var roleIDValue = selectedRow.Cells["RoleID"].Value;

                if (roleIDValue == null)
                {
                    MessageBox.Show("Не удалось получить ID выбранной роли", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var roleID = (int)roleIDValue;
                var role = _context.Roles.Find(roleID);
                if (role != null)
                {
                    role.RoleName = $"Измененная_{DateTime.Now:HHmmss}";
                    role.Description = "Измененное описание";

                    var result = _context.SaveChanges();

                    MessageBox.Show($"✅ Роль изменена! Сохранено: {result}", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadRoles();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Ошибка редактирования: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите роль для удаления", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // ИСПОЛЬЗУЕМ _context вместо создания нового
                var selectedRow = dataGridView.SelectedRows[0];
                var roleIDValue = selectedRow.Cells["RoleID"].Value;

                if (roleIDValue == null)
                {
                    MessageBox.Show("Не удалось получить ID выбранной роли", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var roleID = (int)roleIDValue;
                var roleName = selectedRow.Cells["RoleName"].Value?.ToString();

                var role = _context.Roles.Find(roleID);
                if (role != null)
                {
                    var confirmResult = MessageBox.Show($"Вы уверены, что хотите удалить роль '{roleName ?? "неизвестная роль"}'?",
                        "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirmResult == DialogResult.Yes)
                    {
                        _context.Roles.Remove(role);
                        var result = _context.SaveChanges();

                        MessageBox.Show($"✅ Роль удалена! Удалено записей: {result}", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadRoles();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Ошибка удаления: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadRoles();
        }
    protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _context?.Dispose(); // ОСВОБОЖДАЕМ РЕСУРСЫ
            base.OnFormClosing(e);
        }
    }
}
