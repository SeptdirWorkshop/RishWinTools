using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RishWinTools.Keys
{
    internal class KeysTab : TabPage
    {
        private TableLayoutPanel MainPanel;
        private TableLayoutPanel ButtonsPanel;
        private Button CreateButton;
        private Button InsertButton;
        private Button RefreshButton;
        private Button RemoveFromAgentButton;
        private Button SSHFolderButton;
        private DataGridView ContentTable;

        public KeysTab(TabControl Tabs)
        {
            // Tab
            Name = "Keys";
            Text = "Ключи";
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
            ButtonsPanel.ColumnCount = 6;
            MainPanel.Controls.Add(ButtonsPanel, 0, 0);

            // CreateButton          
            CreateButton = new Button();
            CreateButton.Name = "CreateKey";
            CreateButton.Text = "Создать ключ";
            CreateButton.AutoSize = true;
            CreateButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            CreateButton.Click += OnCreateButtonClick;
            ButtonsPanel.Controls.Add(CreateButton, 0, 0);

            // InsertButton          
            InsertButton = new Button();
            InsertButton.Name = "InsertButton";
            InsertButton.Text = "Вставить ключ";
            InsertButton.AutoSize = true;
            InsertButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            InsertButton.Click += OnInsertButtonClick;
            ButtonsPanel.Controls.Add(InsertButton, 1, 0);

            // RefreshButton          
            RefreshButton = new Button();
            RefreshButton.Name = "RefreshButton";
            RefreshButton.Text = "Обновить список";
            RefreshButton.AutoSize = true;
            RefreshButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            RefreshButton.Click += OnRefreshButtonClick;
            ButtonsPanel.Controls.Add(RefreshButton, 2, 0);

            // RemoveFromAgentButton     
            RemoveFromAgentButton = new Button();
            RemoveFromAgentButton.Name = "RemoveFromAgentButton";
            RemoveFromAgentButton.Text = "Очистить SSH агент";
            RemoveFromAgentButton.AutoSize = true;
            RemoveFromAgentButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            RemoveFromAgentButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            RemoveFromAgentButton.Click += OnRemoveFromAgentButtonClick;
            ButtonsPanel.Controls.Add(RemoveFromAgentButton, 4, 0);

            // SSHFolderButton     
            SSHFolderButton = new Button();
            SSHFolderButton.Name = "ButtonSSH";
            SSHFolderButton.Text = "Перейти в папку";
            SSHFolderButton.AutoSize = true;
            SSHFolderButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            SSHFolderButton.Click += OnSSHFolderButtonClick;
            SSHFolderButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            ButtonsPanel.Controls.Add(SSHFolderButton, 5, 0);

            // ButtonsPanel Columns
            ButtonsPanel.ColumnStyles.Clear();
            ButtonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, CreateButton.Width + 5));
            ButtonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, InsertButton.Width + 5));
            ButtonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, RefreshButton.Width + 5));
            ButtonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            ButtonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, RemoveFromAgentButton.Width + 5));
            ButtonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, SSHFolderButton.Width + 5));
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
            ContentTable.Columns.Add("Name", "Имя файла");
            ContentTable.Columns.Add("PublicKey", "Публичный ключ");
            ContentTable.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
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
            Dictionary<string, KeyObject> items = KeysManager.GetKeys(force);
            ContentTable.Rows.Clear();
            foreach (var item in items)
            {
                KeyObject key = item.Value as KeyObject;
                ContentTable.Rows.Add(key.Filename, key.PublicKey);
            }
        }

        protected void OnCreateButtonClick(object? sender, EventArgs e)
        {
            using (CreateKeyForm createKeyForm = new CreateKeyForm())
            {
                if (createKeyForm.ShowDialog() == DialogResult.OK)
                {
                    LoadConent(true);
                }
            }
        }

        protected void OnInsertButtonClick(object? sender, EventArgs e)
        {
            using (InsertKeyForm insertKeyForm = new InsertKeyForm())
            {
                if (insertKeyForm.ShowDialog(this) == DialogResult.OK)
                {
                    LoadConent(true);
                }
            }
        }

        protected void OnRefreshButtonClick(object? sender, EventArgs e)
        {
            LoadConent(true);
        }

        protected void OnRemoveFromAgentButtonClick(object? sender, EventArgs e)
        {
            KeysManager.RemoveKeyFromAgent();
        }

        protected void OnSSHFolderButtonClick(object? sender, EventArgs e)
        {
            string folderPath = Program.SSHFolderPath;

            if (!System.IO.Directory.Exists(folderPath))
            {
                MessageBox.Show($"Папка {folderPath} не найдена.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            System.Diagnostics.Process.Start("explorer.exe", folderPath);
        }

        protected void OnContentTableCellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            string? keyName = ContentTable.Rows[e.RowIndex].Cells[0].Value.ToString();
            if (keyName == null || String.IsNullOrEmpty(keyName))
            {
                return;
            }

            using (KeyForm keyForm = new KeyForm(keyName))
            {
                keyForm.FormClosed += (sender, args) => LoadConent(true);
                keyForm.ShowDialog(this);
            }
        }
    }
}
