using CeramicQualityControl.Data;
using CeramicQualityControl.Models;

namespace CeramicQualityControl.Forms
{
    public partial class EditForm : Form
    {
        private TableLayoutPanel mainPanel = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;
        private readonly string tableName;
        private object? entity;
        private readonly AppDbContext context;

        public EditForm(string tableName, object? entity, AppDbContext context)
        {
            this.tableName = tableName;
            this.entity = entity;
            this.context = context;
            InitializeComponent();
            CreateForm();
        }

        private void CreateForm()
        {
            this.Text = entity == null ? "Добавить запись" : "Изменить запись";
            this.Size = new Size(500, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Основная панель
            mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(20),
                AutoScroll = true
            };

            // Заголовок
            var lblTitle = new Label
            {
                Text = $"{(entity == null ? "Добавление" : "Изменение")} записи",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 30
            };

            // Поля ввода
            CreateFields();

            // Панель кнопок
            var panelButtons = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50
            };

            btnSave = new Button
            {
                Text = "Сохранить",
                Size = new Size(80, 30),
                Location = new Point(300, 10)
            };

            btnCancel = new Button
            {
                Text = "Отмена",
                Size = new Size(80, 30),
                Location = new Point(390, 10),
                DialogResult = DialogResult.Cancel
            };

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            panelButtons.Controls.Add(btnSave);
            panelButtons.Controls.Add(btnCancel);

            this.Controls.Add(lblTitle);
            this.Controls.Add(mainPanel);
            this.Controls.Add(panelButtons);

            // Назначаем клавиши по умолчанию
            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void CreateFields()
        {
            // Очищаем панель
            mainPanel.Controls.Clear();
            mainPanel.RowCount = 0;

            // Упрощенное создание полей
            var lblInfo = new Label
            {
                Text = $"Таблица: {tableName}\n\nЗдесь будут поля для ввода данных\nна основе структуры таблицы",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Height = 100
            };
            mainPanel.Controls.Add(lblInfo, 0, 0);
            mainPanel.SetColumnSpan(lblInfo, 2);
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                if (entity == null)
                {
                    // Создание новой записи
                    entity = CreateNewEntity();
                    if (entity != null)
                    {
                        context.Add(entity);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось создать новую запись", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

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

        private object? CreateNewEntity()
        {
            return tableName switch
            {
                "Parties" => new Party(),
                "Products" => new Product(),
                "Defects" => new Defect(),
                "ResultControls" => new ResultControl(),
                _ => null
            };
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
