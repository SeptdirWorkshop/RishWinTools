using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace RishWinTools.Keys
{
    public partial class KeyForm : Form
    {
        private string? KeyName;
        public KeyObject? Key;
        private TableLayoutPanel MainPanel;
        private TableLayoutPanel ButtonsPanel;
        private DataGridView ContentTable;
        private Button CreatePublicKeyButton;
        private Button RemoveKeyButton;

        public KeyForm(string? key)
        {
            // Window
            KeyName = key;
            Name = "KeyForm";
            Text = "Ключ";
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

            // CreatePublicKeyButton          
            CreatePublicKeyButton = new Button();
            CreatePublicKeyButton.Name = "CreatePublicKeyButton";
            CreatePublicKeyButton.Text = "Создать публичный ключ";
            CreatePublicKeyButton.AutoSize = true;
            CreatePublicKeyButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            CreatePublicKeyButton.Click += OnCreatePublicKeyButtonClick;
            ButtonsPanel.Controls.Add(CreatePublicKeyButton, 0, 0);

            // RemoveKeyButton     
            RemoveKeyButton = new Button();
            RemoveKeyButton.Name = "RemoveKeyButton";
            RemoveKeyButton.Text = "Удалить ключ";
            RemoveKeyButton.AutoSize = true;
            RemoveKeyButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            RemoveKeyButton.Click += OnRemoveKeyButtonClick;
            RemoveKeyButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            ButtonsPanel.Controls.Add(RemoveKeyButton, 5, 0);

            // ButtonsPanel Columns
            ButtonsPanel.ColumnStyles.Clear();
            ButtonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, CreatePublicKeyButton.Width + 5));
            ButtonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            ButtonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, RemoveKeyButton.Width + 5));
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
                Key = null;
            }

            if (KeyName == null)
            {
                MessageBox.Show("Ключ не найден", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Key == null)
            {
                Key = new KeyObject(KeyName);
                Text = "Ключ " + Key.Filename;
                KeyName = Key.Filename;
            }

            ContentTable.Rows.Clear();
            ContentTable.Rows.Add("Имя", Key.Filename);
            ContentTable.Rows.Add("Путь", Key.Path);
            ContentTable.Rows.Add("Публичный ключ", Key.PublicKey);
            ContentTable.Rows.Add("Комманда для сервера", Key.ServerCommand);
        }

        protected void OnCreatePublicKeyButtonClick(object? sender, EventArgs e)
        {
            if (Key == null)
            {
                MessageBox.Show("Ключ не найден", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (KeysManager.CreatePubKey(Key.Filename))
            {
                LoadConent(true);
            }
        }
        protected void OnRemoveKeyButtonClick(object? sender, EventArgs e)
        {
            if (Key == null)
            {
                MessageBox.Show("Ключ не найден", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (KeysManager.RemoveKey(Key.Filename))
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
