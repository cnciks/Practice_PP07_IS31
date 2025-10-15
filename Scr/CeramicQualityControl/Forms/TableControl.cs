using CeramicQualityControl.Data;
using CeramicQualityControl.Models;
using Microsoft.EntityFrameworkCore;

namespace CeramicQualityControl.Forms
{
    public partial class TableControl : Form
    {
        private ComboBox cmbTables = null!;
        private DataGridView dataGridView = null!;
        private Button btnAdd = null!;
        private Button btnEdit = null!;
        private Button btnDelete = null!;
        private readonly AppDbContext context;

        public TableControl()
        {
            InitializeComponent();
            context = new AppDbContext();
            SetupForm();
            LoadTables();
        }

        private void SetupForm()
        {
            this.Size = new Size(900, 500);
            this.Text = "Управление таблицами";

            // ComboBox для таблиц
            cmbTables = new ComboBox
            {
                Location = new Point(20, 20),
                Size = new Size(200, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbTables.SelectedIndexChanged += CmbTables_SelectedIndexChanged;

            // DataGridView
            dataGridView = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(840, 350),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true
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

            // ДОБАВЛЯЕМ ОБРАБОТЧИКИ СОБЫТИЙ
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;

            this.Controls.Add(cmbTables);
            this.Controls.Add(dataGridView);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
        }

        private void LoadTables()
        {
            try
            {
                cmbTables.Items.AddRange(new string[] {
                    "Parties", "Products", "Defects", "ResultControls"
                });

                if (cmbTables.Items.Count > 0)
                    cmbTables.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки списка таблиц: {ex.Message}");
            }
        }

        private void CmbTables_SelectedIndexChanged(object? sender, EventArgs e)
        {
            LoadSelectedTable();
        }

        private void LoadSelectedTable()
        {
            if (cmbTables.SelectedItem == null) return;

            try
            {
                var tableName = cmbTables.SelectedItem.ToString();

                switch (tableName)
                {
                    case "Parties":
                        var parties = context.Parties.ToList();
                        dataGridView.DataSource = parties;
                        break;
                    case "Products":
                        var products = context.Products
                            .Include(p => p.Party)
                            .ToList();
                        dataGridView.DataSource = products;
                        break;
                    case "Defects":
                        var defects = context.Defects.ToList();
                        dataGridView.DataSource = defects;
                        break;
                    case "ResultControls":
                        var resultControls = context.ResultControls
                            .Include(rc => rc.Product)
                            .Include(rc => rc.Defect)
                            .ToList();
                        dataGridView.DataSource = resultControls;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки таблицы: {ex.Message}");
            }
        }

        // МЕТОД ДЛЯ ДОБАВЛЕНИЯ ДАННЫХ
        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (cmbTables.SelectedItem == null)
            {
                MessageBox.Show("Выберите таблицу для добавления данных");
                return;
            }

            try
            {
                var tableName = cmbTables.SelectedItem.ToString();

                switch (tableName)
                {
                    case "Parties":
                        AddParty();
                        break;
                    case "Products":
                        AddProduct();
                        break;
                    case "Defects":
                        AddDefect();
                        break;
                    case "ResultControls":
                        AddResultControl();
                        break;
                }

                // Обновляем данные после добавления
                LoadSelectedTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления: {ex.Message}");
            }
        }

        // МЕТОДЫ ДЛЯ ДОБАВЛЕНИЯ КОНКРЕТНЫХ СУЩНОСТЕЙ

        private void AddParty()
        {
            using (var form = new Form())
            {
                form.Text = "Добавить партию";
                form.Size = new Size(350, 300);
                form.StartPosition = FormStartPosition.CenterParent;

                var lblProductionDate = new Label { Text = "Дата производства:", Location = new Point(20, 20), AutoSize = true };
                var dtpProductionDate = new DateTimePicker { Location = new Point(150, 20), Size = new Size(150, 20) };

                var lblChangeNumber = new Label { Text = "Номер смены:", Location = new Point(20, 50), AutoSize = true };
                var txtChangeNumber = new TextBox { Location = new Point(150, 50), Size = new Size(150, 20) };

                var lblProductType = new Label { Text = "Тип продукции:", Location = new Point(20, 80), AutoSize = true };
                var txtProductType = new TextBox { Location = new Point(150, 80), Size = new Size(150, 20) };

                var lblSize = new Label { Text = "Размер:", Location = new Point(20, 110), AutoSize = true };
                var txtSize = new TextBox { Location = new Point(150, 110), Size = new Size(150, 20) };

                var lblColor = new Label { Text = "Цвет:", Location = new Point(20, 140), AutoSize = true };
                var txtColor = new TextBox { Location = new Point(150, 140), Size = new Size(150, 20) };

                var lblNumberOfProducts = new Label { Text = "Количество:", Location = new Point(20, 170), AutoSize = true };
                var numNumberOfProducts = new NumericUpDown { Location = new Point(150, 170), Size = new Size(150, 20), Minimum = 1, Maximum = 10000 };

                var btnOk = new Button { Text = "Добавить", Location = new Point(50, 210), Size = new Size(80, 30) };
                var btnCancel = new Button { Text = "Отмена", Location = new Point(150, 210), Size = new Size(80, 30) };

                btnOk.Click += (s, e) => { form.DialogResult = DialogResult.OK; form.Close(); };
                btnCancel.Click += (s, e) => { form.DialogResult = DialogResult.Cancel; form.Close(); };

                form.Controls.AddRange(new Control[] {
                    lblProductionDate, dtpProductionDate,
                    lblChangeNumber, txtChangeNumber,
                    lblProductType, txtProductType,
                    lblSize, txtSize,
                    lblColor, txtColor,
                    lblNumberOfProducts, numNumberOfProducts,
                    btnOk, btnCancel
                });

                if (form.ShowDialog() == DialogResult.OK)
                {
                    var party = new Party
                    {
                        ProductionDate = dtpProductionDate.Value,
                        ChangeNumber = txtChangeNumber.Text,
                        ProductType = txtProductType.Text,
                        Size = txtSize.Text,
                        Color = txtColor.Text,
                        NumberOfProducts = (int)numNumberOfProducts.Value
                    };

                    context.Parties.Add(party);
                    context.SaveChanges();
                    MessageBox.Show("Партия добавлена успешно!");
                }
            }
        }

        private void AddProduct()
        {
            // Получаем список партий для выбора
            var parties = context.Parties.ToList();

            using (var form = new Form())
            {
                form.Text = "Добавить продукт";
                form.Size = new Size(350, 200);
                form.StartPosition = FormStartPosition.CenterParent;

                var lblParty = new Label { Text = "Партия:", Location = new Point(20, 20), AutoSize = true };
                var cmbParty = new ComboBox { Location = new Point(150, 20), Size = new Size(150, 20), DropDownStyle = ComboBoxStyle.DropDownList };
                cmbParty.DisplayMember = "PartyID";
                cmbParty.ValueMember = "PartyID";
                cmbParty.DataSource = parties;

                var lblProductNumber = new Label { Text = "Номер в партии:", Location = new Point(20, 50), AutoSize = true };
                var numProductNumber = new NumericUpDown { Location = new Point(150, 50), Size = new Size(150, 20), Minimum = 1, Maximum = 10000 };

                var btnOk = new Button { Text = "Добавить", Location = new Point(50, 100), Size = new Size(80, 30) };
                var btnCancel = new Button { Text = "Отмена", Location = new Point(150, 100), Size = new Size(80, 30) };

                btnOk.Click += (s, e) => { form.DialogResult = DialogResult.OK; form.Close(); };
                btnCancel.Click += (s, e) => { form.DialogResult = DialogResult.Cancel; form.Close(); };

                form.Controls.AddRange(new Control[] {
                    lblParty, cmbParty,
                    lblProductNumber, numProductNumber,
                    btnOk, btnCancel
                });

                if (form.ShowDialog() == DialogResult.OK && cmbParty.SelectedItem is Party selectedParty)
                {
                    var product = new Product
                    {
                        PartyID = selectedParty.PartyID,
                        ProductNumberInParty = (int)numProductNumber.Value
                    };

                    context.Products.Add(product);
                    context.SaveChanges();
                    MessageBox.Show("Продукт добавлен успешно!");
                }
            }
        }

        private void AddDefect()
        {
            using (var form = new Form())
            {
                form.Text = "Добавить дефект";
                form.Size = new Size(350, 250);
                form.StartPosition = FormStartPosition.CenterParent;

                var lblName = new Label { Text = "Название дефекта:", Location = new Point(20, 20), AutoSize = true };
                var txtName = new TextBox { Location = new Point(150, 20), Size = new Size(150, 20) };

                var lblDescription = new Label { Text = "Описание:", Location = new Point(20, 50), AutoSize = true };
                var txtDescription = new TextBox { Location = new Point(150, 50), Size = new Size(150, 60), Multiline = true, Height = 60 };

                var lblCriticality = new Label { Text = "Критичность:", Location = new Point(20, 120), AutoSize = true };
                var cmbCriticality = new ComboBox { Location = new Point(150, 120), Size = new Size(150, 20) };
                cmbCriticality.Items.AddRange(new string[] { "Низкая", "Средняя", "Высокая", "Критическая" });
                cmbCriticality.SelectedIndex = 0;

                var btnOk = new Button { Text = "Добавить", Location = new Point(50, 160), Size = new Size(80, 30) };
                var btnCancel = new Button { Text = "Отмена", Location = new Point(150, 160), Size = new Size(80, 30) };

                btnOk.Click += (s, e) => { form.DialogResult = DialogResult.OK; form.Close(); };
                btnCancel.Click += (s, e) => { form.DialogResult = DialogResult.Cancel; form.Close(); };

                form.Controls.AddRange(new Control[] {
                    lblName, txtName,
                    lblDescription, txtDescription,
                    lblCriticality, cmbCriticality,
                    btnOk, btnCancel
                });

                if (form.ShowDialog() == DialogResult.OK)
                {
                    var defect = new Defect
                    {
                        NameOfDefect = txtName.Text,
                        DescriptionOfDefect = txtDescription.Text,
                        Criticality = cmbCriticality.SelectedItem?.ToString() ?? "Низкая"
                    };

                    context.Defects.Add(defect);
                    context.SaveChanges();
                    MessageBox.Show("Дефект добавлен успешно!");
                }
            }
        }

        private void AddResultControl()
        {
            // Получаем данные для выпадающих списков
            var products = context.Products.Include(p => p.Party).ToList();
            var defects = context.Defects.ToList();

            using (var form = new Form())
            {
                form.Text = "Добавить результат контроля";
                form.Size = new Size(400, 400);
                form.StartPosition = FormStartPosition.CenterParent;

                var lblProduct = new Label { Text = "Продукт:", Location = new Point(20, 20), AutoSize = true };
                var cmbProduct = new ComboBox { Location = new Point(150, 20), Size = new Size(200, 20), DropDownStyle = ComboBoxStyle.DropDownList };
                cmbProduct.DisplayMember = "ProductNumberInParty";
                cmbProduct.ValueMember = "ProductID";
                cmbProduct.DataSource = products;

                var lblDefect = new Label { Text = "Дефект:", Location = new Point(20, 50), AutoSize = true };
                var cmbDefect = new ComboBox { Location = new Point(150, 50), Size = new Size(200, 20), DropDownStyle = ComboBoxStyle.DropDownList };
                cmbDefect.DisplayMember = "NameOfDefect";
                cmbDefect.ValueMember = "DefectID";
                cmbDefect.DataSource = defects;

                var lblMonitoringData = new Label { Text = "Дата мониторинга:", Location = new Point(20, 80), AutoSize = true };
                var dtpMonitoringData = new DateTimePicker { Location = new Point(150, 80), Size = new Size(200, 20), Value = DateTime.Now };

                var lblControlOperator = new Label { Text = "Оператор:", Location = new Point(20, 110), AutoSize = true };
                var txtControlOperator = new TextBox { Location = new Point(150, 110), Size = new Size(200, 20) };

                var lblLocation = new Label { Text = "Местоположение дефекта:", Location = new Point(20, 140), AutoSize = true };
                var txtLocation = new TextBox { Location = new Point(150, 140), Size = new Size(200, 20) };

                var lblDefectSize = new Label { Text = "Размер дефекта (мм):", Location = new Point(20, 170), AutoSize = true };
                var numDefectSize = new NumericUpDown
                {
                    Location = new Point(150, 170),
                    Size = new Size(200, 20),
                    Minimum = 0,
                    Maximum = 1000,
                    DecimalPlaces = 2,
                    Increment = 0.1m
                };

                var btnOk = new Button { Text = "Добавить", Location = new Point(50, 210), Size = new Size(80, 30) };
                var btnCancel = new Button { Text = "Отмена", Location = new Point(150, 210), Size = new Size(80, 30) };

                btnOk.Click += (s, e) => { form.DialogResult = DialogResult.OK; form.Close(); };
                btnCancel.Click += (s, e) => { form.DialogResult = DialogResult.Cancel; form.Close(); };

                form.Controls.AddRange(new Control[] {
            lblProduct, cmbProduct,
            lblDefect, cmbDefect,
            lblMonitoringData, dtpMonitoringData,
            lblControlOperator, txtControlOperator,
            lblLocation, txtLocation,
            lblDefectSize, numDefectSize,
            btnOk, btnCancel
        });

                if (form.ShowDialog() == DialogResult.OK &&
                    cmbProduct.SelectedItem is Product selectedProduct &&
                    cmbDefect.SelectedItem is Defect selectedDefect)
                {
                    var resultControl = new ResultControl
                    {
                        ProductID = selectedProduct.ProductID,
                        DefectID = selectedDefect.DefectID,
                        MonitoringData = dtpMonitoringData.Value, // DateTime
                        ControlOperator = txtControlOperator.Text,
                        LocationOfDefect = txtLocation.Text,
                        DefectSize = numDefectSize.Value // decimal
                    };

                    context.ResultControls.Add(resultControl);
                    context.SaveChanges();
                    MessageBox.Show("Результат контроля добавлен успешно!");
                }
            }
        }

        // ЗАГЛУШКИ ДЛЯ РЕДАКТИРОВАНИЯ И УДАЛЕНИЯ
        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Функция редактирования будет реализована позже");
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Функция удаления будет реализована позже");
        }

        // ОСВОБОЖДЕНИЕ РЕСУРСОВ
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            context?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
