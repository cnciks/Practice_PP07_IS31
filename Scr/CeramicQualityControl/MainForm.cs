using CeramicQualityControl.Forms;
using System.Windows.Forms;

namespace CeramicQualityControl
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.Text = "Контроль качества керамики - Главная";
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearContentPanel();
            this.Text = "Контроль качества керамики - Настройки";
            ShowFormInPanel(new ConnectionSettingsForm());
        }

        private void таблицыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearContentPanel();
            this.Text = "Контроль качества керамики - Таблицы";
            ShowFormInPanel(new TableControl());
        }

        private void пользователиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearContentPanel();
            this.Text = "Контроль качества керамики - Пользователи";
            ShowFormInPanel(new UsersForm());
        }

        private void ролиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearContentPanel();
            this.Text = "Контроль качества керамики - Роли";
            ShowFormInPanel(new RolesForm());
        }

        private void ShowFormInPanel(Form form)
        {
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(form);
            form.Show();
        }

        private void ClearContentPanel()
        {
            foreach (Control control in contentPanel.Controls)
            {
                if (control is Form form)
                {
                    form.Close();
                }
            }
            contentPanel.Controls.Clear();
        }
    }
}
