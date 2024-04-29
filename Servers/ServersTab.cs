using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RishWinTools.Servers
{
    internal class ServersTab : TabPage
    {
        private TableLayoutPanel MainPanel;
        private TableLayoutPanel ButtonsPanel;
        private Button AddButton;
        private Button RefreshButton;
        private Button RemoveKnownHostsButton;
        private DataGridView ContentTable;

        public ServersTab(TabControl Tabs)
        {
            // Tab
            Name = "Servers";
            Text = "Сервера";
            Location = new Point(4, 22);
            Padding = new Padding(3);
            Size = new Size(792, 422);
            TabIndex = 0;
            UseVisualStyleBackColor = true;

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
            ButtonsPanel.ColumnCount = 4;
            MainPanel.Controls.Add(ButtonsPanel, 0, 0);

            // AddButton          
            AddButton = new Button();
            AddButton.Name = "AddButton";
            AddButton.Text = "Добавить сервер";
            AddButton.AutoSize = true;
            AddButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            AddButton.Click += OnAddButtonClick;
            ButtonsPanel.Controls.Add(AddButton, 0, 0);

            // RefreshButton          
            RefreshButton = new Button();
            RefreshButton.Name = "RefreshButton";
            RefreshButton.Text = "Обновить список";
            RefreshButton.AutoSize = true;
            RefreshButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            RefreshButton.Click += OnRefreshButtonClick;
            ButtonsPanel.Controls.Add(RefreshButton, 1, 0);

            // RemoveKnownHostsButton     
            RemoveKnownHostsButton = new Button();
            RemoveKnownHostsButton.Name = "RemoveKnownHostsButton";
            RemoveKnownHostsButton.Text = "Очистить хосты";
            RemoveKnownHostsButton.AutoSize = true;
            RemoveKnownHostsButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            RemoveKnownHostsButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            RemoveKnownHostsButton.Click += OnRemoveKnownHostsButtonClick;
            ButtonsPanel.Controls.Add(RemoveKnownHostsButton, 3, 0);

            // ButtonsPanel Columns
            ButtonsPanel.ColumnStyles.Clear();
            ButtonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, AddButton.Width + 5));
            ButtonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, RefreshButton.Width + 5));
            ButtonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            ButtonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, RemoveKnownHostsButton.Width + 5));
            ButtonsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

            // ContentTable
            ContentTable = new DataGridView();
            ContentTable.Dock = DockStyle.Fill;
            ContentTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ContentTable.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            ContentTable.AllowUserToAddRows = false;
            ContentTable.AllowUserToDeleteRows = false;
            ContentTable.ReadOnly = true;
            ContentTable.RowHeadersVisible = false;
            ContentTable.Columns.Add("Host", "Сервер");
            ContentTable.Columns.Add("Hostname", "Адрес");
            ContentTable.Columns.Add("User", "Пользователь");
            ContentTable.Columns.Add("Key", "Ключ");
            ContentTable.Columns.Add("PublicKey", "Публичный ключ");
            ContentTable.Columns["Host"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ContentTable.Columns["Hostname"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ContentTable.Columns["User"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ContentTable.Columns["Key"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ContentTable.Columns["PublicKey"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ContentTable.CellDoubleClick += OnContentTableCellDoubleClick;
            MainPanel.Controls.Add(ContentTable, 0, 1);

            // Load content          
            LoadConent();
            Tabs.SelectedIndexChanged += (sender, e) =>
            {
                if (Tabs.SelectedTab == this)
                {
                    this.LoadConent();
                }
            };

            // Layout
            MainPanel.SuspendLayout();
            SuspendLayout();
            ButtonsPanel.ResumeLayout(false);
            MainPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        public void LoadConent(bool force = false)
        {
            Dictionary<string, ServerObject> items = ServersManager.GetServers(force);
            ContentTable.Rows.Clear();
            foreach (var item in items)
            {
                ServerObject server = item.Value as ServerObject;
                ContentTable.Rows.Add(server.Host, server.Hostname, server.User, server.KeyName, server.PublicKey);
            }
        }

        protected void OnAddButtonClick(object? sender, EventArgs e)
        {
            using (CreateServerForm createServerForm = new CreateServerForm())
            {
                if (createServerForm.ShowDialog(this) == DialogResult.OK)
                {
                    LoadConent(true);
                }
            }
        }

        protected void OnRefreshButtonClick(object? sender, EventArgs e)
        {
            LoadConent(true);
        }

        protected void OnRemoveKnownHostsButtonClick(object? sender, EventArgs e)
        {
            ServersManager.RemoveKnownHosts();
        }

        protected void OnContentTableCellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            string? serverName = ContentTable.Rows[e.RowIndex].Cells[0].Value.ToString();
            if (serverName == null)
            {
                return;
            }

            using (ServerForm serverForm = new ServerForm(serverName))
            {
                serverForm.FormClosed += (sender, args) => LoadConent(true);
                serverForm.ShowDialog(this);
            }
        }
    }
}
