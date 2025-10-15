using CeramicQualityControl.Data;
using CeramicQualityControl.Models;
using System.Security.Cryptography;
using System.Text;

namespace CeramicQualityControl.Forms
{
    public partial class ChangePasswordForm : Form
    {
        private TextBox txtNewPassword = null!;
        private TextBox txtConfirmPassword = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;
        private readonly int userID;
        private readonly AppDbContext context;

        public ChangePasswordForm(int userID, AppDbContext context)
        {
            this.userID = userID;
            this.context = context;
            InitializeComponent();
            CreateForm();
        }

        private void CreateForm()
        {
            this.Text = "Смена пароля";
            this.Size = new Size(350, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Основная панель
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(20),
                RowCount = 2
            };

            // Поле нового пароля
            var lblNewPassword = new Label
            {
                Text = "Новый пароль:",
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            mainPanel.Controls.Add(lblNewPassword, 0, 0);

            txtNewPassword = new TextBox
            {
                Width = 150,
                UseSystemPasswordChar = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            mainPanel.Controls.Add(txtNewPassword, 1, 0);

            // Поле подтверждения пароля
            var lblConfirmPassword = new Label
            {
                Text = "Подтверждение:",
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            mainPanel.Controls.Add(lblConfirmPassword, 0, 1);

            txtConfirmPassword = new TextBox
            {
                Width = 150,
                UseSystemPasswordChar = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            mainPanel.Controls.Add(txtConfirmPassword, 1, 1);

            // Панель кнопок
            var panelButtons = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(10)
            };

            btnSave = new Button
            {
                Text = "Сохранить",
                Size = new Size(80, 30),
                Location = new Point(150, 10)
            };

            btnCancel = new Button
            {
                Text = "Отмена",
                Size = new Size(80, 30),
                Location = new Point(240, 10)
            };

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            panelButtons.Controls.Add(btnSave);
            panelButtons.Controls.Add(btnCancel);

            this.Controls.Add(mainPanel);
            this.Controls.Add(panelButtons);

            // Назначаем клавишу Enter для сохранения
            this.AcceptButton = btnSave;
            // Назначаем клавишу Escape для отмены
            this.CancelButton = btnCancel;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            // Валидация пароля
            if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                MessageBox.Show("Введите новый пароль", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPassword.Focus();
                return;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                return;
            }

            if (txtNewPassword.Text.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPassword.Focus();
                return;
            }

            try
            {
                // Находим пользователя
                var user = context.Users.Find(userID);
                if (user != null)
                {
                    user.PasswordHash = HashPassword(txtNewPassword.Text);
                    context.SaveChanges();

                    MessageBox.Show("Пароль успешно изменен", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Пользователь не найден", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при изменении пароля: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }

        public static bool VerifyPassword(string password, string passwordHash)
        {
            return HashPassword(password) == passwordHash;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            btnSave.Click -= BtnSave_Click;
            btnCancel.Click -= BtnCancel_Click;
        }
    }
}
