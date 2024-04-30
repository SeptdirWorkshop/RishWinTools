using RishWinTools.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace RishWinTools.Servers
{
    public partial class ServerForm : Form
    {
        private string? ServerName;
        private ServerObject? Server;
        private TableLayoutPanel MainPanel;
        private TableLayoutPanel ButtonsPanel;
        private Button ConnectServerButton;
        private Button RemoveServerButton;
        private DataGridView ContentTable;

        public ServerForm(string? server)
        {
            // Window
            ServerName = server;
            Name = "ServerForm";
            Text = "Сервер";
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(640, 480);
            StartPosition = FormStartPosition.CenterParent;

            // MainPanel
            MainPanel = new TableLayoutPanel();
            MainPanel.Dock = DockStyle.Fill;
            MainPanel.ColumnCount = 1;
            MainPanel.RowCount = 2;
            MainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            MainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            MainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            MainPanel.RowStyles[0] = new RowStyle(SizeType.Absolute, 40);
            Controls.Add(MainPanel);

            // ButtonsPanel
            ButtonsPanel = new TableLayoutPanel();
            ButtonsPanel.Dock = DockStyle.Top;
            ButtonsPanel.Height = 40;
            ButtonsPanel.AutoSize = true;
            ButtonsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ButtonsPanel.Padding = new Padding(0, 0, 0, 0);
            ButtonsPanel.ColumnCount = 3;
            MainPanel.Controls.Add(ButtonsPanel, 0, 0);

            // ConnectServerButton          
            ConnectServerButton = new Button();
            ConnectServerButton.Name = "ConnectServerButton";
            ConnectServerButton.Text = "Подключится";
            ConnectServerButton.AutoSize = true;
            ConnectServerButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ConnectServerButton.Click += OnConnectServerButtonClick;
            ButtonsPanel.Controls.Add(ConnectServerButton, 0, 0);

            // RemoveServerButton     
            RemoveServerButton = new Button();
            RemoveServerButton.Name = "RemoveServerButton";
            RemoveServerButton.Text = "Удалить сервер";
            RemoveServerButton.AutoSize = true;
            RemoveServerButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            RemoveServerButton.Click += OnRemoveServerButtonClick;
            RemoveServerButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            ButtonsPanel.Controls.Add(RemoveServerButton, 5, 0);

            // ButtonsPanel Columns
            ButtonsPanel.ColumnStyles.Clear();
            ButtonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, ConnectServerButton.Width + 5));
            ButtonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            ButtonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, RemoveServerButton.Width + 5));
            ButtonsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

            // ContentTable
            ContentTable = new DataGridView();
            ContentTable.Dock = DockStyle.Fill;
            ContentTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ContentTable.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            ContentTable.AllowUserToAddRows = false;
            ContentTable.AllowUserToDeleteRows = false;
            ContentTable.AllowUserToResizeColumns = true;
            ContentTable.ReadOnly = true;
            ContentTable.RowHeadersVisible = false;
            ContentTable.Columns.Add("Key", "Свойство");
            ContentTable.Columns.Add("Value", "Значение");
            ContentTable.Columns["Key"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ContentTable.Columns["Value"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ContentTable.CellDoubleClick += OnContentTableCellDoubleClick;
            MainPanel.Controls.Add(ContentTable, 0, 1);

            // Load content
            LoadConent();

            // Layout
            MainPanel.SuspendLayout();
            SuspendLayout();
            ButtonsPanel.ResumeLayout(false);
            MainPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        protected void LoadConent(bool force = false)
        {
            if (force)
            {
                Server = null;
            }

            if (ServerName == null)
            {
                MessageBox.Show("Сервер не найден", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Server == null)
            {
                Server = new ServerObject(ServerName);
                Text = "Сервер " + Server.Host;
                ServerName = Server.Host;
            }

            ContentTable.Rows.Clear();
            ContentTable.Rows.Add("Сервер", Server.Host);
            ContentTable.Rows.Add("Адрес", Server.Hostname);
            ContentTable.Rows.Add("Пользователь", Server.User);
            if (Server.Key != null && Server.Key.KeyExist)
            {
                ContentTable.Rows.Add("Ключ", Server.Key.Filename);
                ContentTable.Rows.Add("Публичный ключ", Server.Key.PublicKey);
                ContentTable.Rows.Add("Комманда для сервера", Server.Key.ServerCommand);
            }
            if (Server.Additions.Count > 0)
            {
                foreach (var item in Server.Additions)
                {
                    ContentTable.Rows.Add(item.Key, item.Value);
                }
            }
        }

        protected void OnConnectServerButtonClick(object? sender, EventArgs e)
        {
            if (Server == null || Server.Host == null)
            {
                MessageBox.Show("Сервер не найден", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ServersManager.ConnectToServer(Server.Host);
        }

        protected void OnRemoveServerButtonClick(object? sender, EventArgs e)
        {
            if (Server == null || Server.Host == null)
            {
                MessageBox.Show("Сервер не найден", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (ServersManager.RemoveServer(Server.Host))
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        protected void OnContentTableCellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            string? name = ContentTable.Rows[e.RowIndex].Cells[0].Value.ToString();
            string? value = ContentTable.Rows[e.RowIndex].Cells[1].Value.ToString();

            if (value != null)
            {
                try
                {
                    Clipboard.SetText(value);

                    MessageBox.Show($"{name} скопировано в буфер обмена", "Копирование в буфер обмена", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось скопировать значение: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}