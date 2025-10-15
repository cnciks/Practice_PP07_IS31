using CeramicQualityControl.Data;
using CeramicQualityControl.Models;

namespace CeramicQualityControl.Forms
{
    public partial class RoleEditForm : Form
    {
        private TextBox txtRoleName = null!;
        private TextBox txtDescription = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;
        private Role? role;
        private readonly AppDbContext context;

        public RoleEditForm(Role? role, AppDbContext context)
        {
            this.role = role;
            this.context = context;
            InitializeComponent();
            CreateForm();
        }

        private void CreateForm()
        {
            this.Text = role == null ? "Добавить роль" : "Изменить роль";
            this.Size = new Size(400, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(20),
                RowCount = 3
            };

            // Поля формы
            var lblRoleName = new Label { Text = "Название роли:", AutoSize = true };
            mainPanel.Controls.Add(lblRoleName, 0, 0);
            txtRoleName = new TextBox { Width = 200 };
            mainPanel.Controls.Add(txtRoleName, 1, 0);

            var lblDescription = new Label { Text = "Описание:", AutoSize = true };
            mainPanel.Controls.Add(lblDescription, 0, 1);
            txtDescription = new TextBox { Width = 200, Height = 60, Multiline = true };
            mainPanel.Controls.Add(txtDescription, 1, 1);

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

            LoadRoleData();

            // Назначаем клавиши по умолчанию
            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void LoadRoleData()
        {
            if (role != null)
            {
                txtRoleName.Text = role.RoleName;
                txtDescription.Text = role.Description;
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRoleName.Text))
            {
                MessageBox.Show("Введите название роли", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRoleName.Focus();
                return;
            }

            // Проверка уникальности названия роли
            var roleName = txtRoleName.Text.Trim();
            var currentRoleId = role?.RoleID ?? 0; // Выносим проверку за пределы LINQ

            var existingRole = context.Roles
                .FirstOrDefault(r => r.RoleName == roleName && r.RoleID != currentRoleId);

            if (existingRole != null)
            {
                MessageBox.Show("Роль с таким названием уже существует", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRoleName.Focus();
                return;
            }

            try
            {
                if (role == null)
                {
                    role = new Role();
                    context.Roles.Add(role);
                }

                role.RoleName = roleName;
                role.Description = txtDescription.Text.Trim();

                context.SaveChanges();
                this.DialogResult = DialogResult.OK;
                this.Close();
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

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            // Отписываемся от событий
            btnSave.Click -= BtnSave_Click;
            btnCancel.Click -= BtnCancel_Click;
        }
    }
}
