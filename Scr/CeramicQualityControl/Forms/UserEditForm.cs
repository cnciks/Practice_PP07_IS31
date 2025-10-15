using CeramicQualityControl.Data;
using CeramicQualityControl.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CeramicQualityControl.Forms
{
    public partial class UserEditForm : Form
    {
        private TextBox txtUsername = null!;
        private TextBox txtPassword = null!;
        private ComboBox cmbRole = null!;
        private CheckBox chkActive = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;
        private User? user;
        private readonly AppDbContext context;

        public UserEditForm(User? user, AppDbContext context)
        {
            this.user = user;
            this.context = context;
            InitializeComponent();
            CreateForm();
        }

        private void CreateForm()
        {
            this.Text = user == null ? "Добавить пользователя" : "Изменить пользователя";
            this.Size = new Size(400, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(20),
                RowCount = 5
            };

            // Поля формы
            var lblUsername = new Label { Text = "Логин:", AutoSize = true };
            mainPanel.Controls.Add(lblUsername, 0, 0);
            txtUsername = new TextBox { Width = 200 };
            mainPanel.Controls.Add(txtUsername, 1, 0);

            var lblPassword = new Label { Text = "Пароль:", AutoSize = true };
            mainPanel.Controls.Add(lblPassword, 0, 1);
            txtPassword = new TextBox { Width = 200, UseSystemPasswordChar = true };
            mainPanel.Controls.Add(txtPassword, 1, 1);

            var lblRole = new Label { Text = "Роль:", AutoSize = true };
            mainPanel.Controls.Add(lblRole, 0, 2);
            cmbRole = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            mainPanel.Controls.Add(cmbRole, 1, 2);

            var lblActive = new Label { Text = "Активен:", AutoSize = true };
            mainPanel.Controls.Add(lblActive, 0, 3);
            chkActive = new CheckBox { Checked = true };
            mainPanel.Controls.Add(chkActive, 1, 3);

            // Кнопки
            var panelButtons = new Panel { Dock = DockStyle.Bottom, Height = 50 };
            btnSave = new Button { Text = "Сохранить", Size = new Size(80, 30), Location = new Point(200, 10) };
            btnCancel = new Button { Text = "Отмена", Size = new Size(80, 30), Location = new Point(290, 10) };

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            panelButtons.Controls.Add(btnSave);
            panelButtons.Controls.Add(btnCancel);

            this.Controls.Add(mainPanel);
            this.Controls.Add(panelButtons);

            LoadRoles();
            LoadUserData();
        }

        private void LoadRoles()
        {
            try
            {
                var roles = context.Roles.ToList();
                cmbRole.DataSource = roles;
                cmbRole.DisplayMember = "RoleName";
                cmbRole.ValueMember = "RoleID";

                if (cmbRole.Items.Count > 0)
                    cmbRole.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки ролей: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadUserData()
        {
            if (user != null)
            {
                txtUsername.Text = user.Username;

                // Безопасная установка значения роли
                if (cmbRole.Items.Count > 0)
                {
                    var roleToSelect = cmbRole.Items.Cast<Role>()
                        .FirstOrDefault(r => r.RoleID == user.RoleID);
                    if (roleToSelect != null)
                    {
                        cmbRole.SelectedItem = roleToSelect;
                    }
                }

                chkActive.Checked = user.IsActive;
                txtPassword.Enabled = false; // Пароль меняется через отдельную форму
                txtPassword.PlaceholderText = "Используйте 'Смена пароля'";
            }
            else
            {
                txtPassword.PlaceholderText = "Введите пароль";
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            // Валидация данных
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Введите логин пользователя", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            if (user == null && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Введите пароль для нового пользователя", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            if (cmbRole.SelectedItem == null)
            {
                MessageBox.Show("Выберите роль пользователя", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (user == null)
                {
                    user = new User();
                    context.Users.Add(user);
                }

                user.Username = txtUsername.Text.Trim();
                user.RoleID = ((Role)cmbRole.SelectedItem).RoleID; // Безопасное преобразование
                user.IsActive = chkActive.Checked;

                // Если это новый пользователь, устанавливаем пароль
                if (user.UserID == 0 && !string.IsNullOrEmpty(txtPassword.Text))
                {
                    user.PasswordHash = HashPassword(txtPassword.Text);
                }

                context.SaveChanges();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка сохранения в базу данных: {ex.InnerException?.Message ?? ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Хеширование пароля с использованием SHA256
        /// </summary>
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            // Отписываемся от событий
            btnSave.Click -= BtnSave_Click;
            btnCancel.Click -= BtnCancel_Click;
        }
    }
}
