using RishWinTools.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace RishWinTools.Servers
{
    public partial class CreateServerForm : System.Windows.Forms.Form
    {
        private Label HostLabel;
        private TextBox HostInput;
        public string HostValue => HostInput.Text;
        private Label HostnameLabel;
        private TextBox HostnameInput;
        public string HostnameValue => HostnameInput.Text;
        private Label UserLabel;
        private TextBox UserInput;
        public string UserValue => UserInput.Text;
        private Label KeyLabel;
        private ComboBox KeyInput;
        public string KeyValue
        {
            get
            {
                if (KeyInput.SelectedItem is KeyValuePair<string, string> selectedPair)
                {
                    return selectedPair.Key;
                }
                return null;
            }
        }
        private Button CreateButton;

        public CreateServerForm()
        {
            // Window
            Name = "CreateServerForm";
            Text = "Создание сервера";
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(480, 0);
            StartPosition = FormStartPosition.CenterParent;

            // HostLabel
            HostLabel = new Label();
            HostLabel.Name = "HostLabel";
            HostLabel.Text = "Имя сервера";
            HostLabel.AutoSize = true;
            HostLabel.Location = new Point(10, 20);
            Controls.Add(HostLabel);

            // HostInput
            HostInput = new TextBox();
            HostInput.Name = "HostInput";
            HostInput.Text = "";
            HostInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            HostInput.Size = new Size(ClientSize.Width - 20, 20);
            HostInput.Location = new Point(10, HostLabel.Bottom + 5);
            Controls.Add(HostInput);

            // HostnameLabel
            HostnameLabel = new Label();
            HostnameLabel.Name = "HostnameLabel";
            HostnameLabel.Text = "Адрес сервера";
            HostnameLabel.AutoSize = true;
            HostnameLabel.Location = new Point(10, HostInput.Bottom + 10);
            Controls.Add(HostnameLabel);

            // HostnameInput
            HostnameInput = new TextBox();
            HostnameInput.Name = "HostnameInput";
            HostnameInput.Text = "";
            HostnameInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            HostnameInput.Size = new Size(ClientSize.Width - 20, 20);
            HostnameInput.Location = new Point(10, HostnameLabel.Bottom + 5);
            Controls.Add(HostnameInput);

            // UserLabel
            UserLabel = new Label();
            UserLabel.Name = "UserLabel";
            UserLabel.Text = "Пользователь";
            UserLabel.AutoSize = true;
            UserLabel.Location = new Point(10, HostnameInput.Bottom + 10);
            Controls.Add(UserLabel);

            // UserInput
            UserInput = new TextBox();
            UserInput.Name = "UserInput";
            UserInput.Text = "root";
            UserInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            UserInput.Size = new Size(ClientSize.Width - 20, 20);
            UserInput.Location = new Point(10, UserLabel.Bottom + 5);
            Controls.Add(UserInput);

            // KeyLabel
            KeyLabel = new Label();
            KeyLabel.Name = "KeyLabel";
            KeyLabel.Text = "Ключ";
            KeyLabel.AutoSize = true;
            KeyLabel.Location = new Point(10, UserInput.Bottom + 10);
            Controls.Add(KeyLabel);

            // KeyInput
            KeyInput = new ComboBox();
            KeyInput.Name = "KeyInput";
            KeyInput.Text = "";
            KeyInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            KeyInput.Size = new Size(ClientSize.Width - 20, 20);
            KeyInput.Location = new Point(10, KeyLabel.Bottom + 5);
            KeyInput.DisplayMember = "Value";
            KeyInput.ValueMember = "Key";
            KeyInput.Items.Add(new KeyValuePair<string, string>("", "-Выберите ключ-"));
            KeyInput.Items.Add(new KeyValuePair<string, string>("rwt_generete", "Созать ключ"));
            Dictionary<string, KeyObject> items = KeysManager.GetKeys(true);
            foreach (var item in items)
            {
                KeyObject key = item.Value as KeyObject;
                KeyInput.Items.Add(new KeyValuePair<string, string>(key.Filename, key.Filename));
            }
            KeyInput.SelectedIndex = 0;
            KeyInput.GotFocus += (sender, e) =>
            {
                ComboBox comboBox = sender as ComboBox;
                if (comboBox != null)
                {
                    comboBox.DroppedDown = true;
                }
            };
            Controls.Add(KeyInput);

            // GenerateButton
            CreateButton = new Button();
            CreateButton.Name = "CreateButton";
            CreateButton.Text = "Создать сервер";
            CreateButton.AutoSize = true;
            CreateButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            CreateButton.Location = new Point(10, KeyInput.Bottom + 10);
            CreateButton.UseVisualStyleBackColor = true;
            CreateButton.Click += new EventHandler(OnCreateButtonClick);
            Controls.Add(CreateButton);

            // Layout
            int FormHeight = 0;
            foreach (Control control in this.Controls)
            {
                int controlBottom = control.Bottom;
                if (controlBottom > FormHeight)
                {
                    FormHeight = controlBottom;
                }
            }
            ClientSize = new Size(ClientSize.Width, FormHeight + 20);
        }

        protected void OnCreateButtonClick(object sender, EventArgs e)
        {

            string hostValue = HostValue.Trim();
            if (string.IsNullOrWhiteSpace(hostValue))
            {
                MessageBox.Show("Пожалуйста заполните поле Имя сервера", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string hostnameValue = HostnameValue.Trim();
            if (string.IsNullOrWhiteSpace(hostnameValue))
            {
                MessageBox.Show("Пожалуйста заполните поле Адрес сервера", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string userValue = UserValue.Trim();
            if (string.IsNullOrWhiteSpace(userValue))
            {
                MessageBox.Show("Пожалуйста заполните поле Пользователь", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string keyValue = KeyValue.Trim();
            if (string.IsNullOrWhiteSpace(keyValue))
            {
                MessageBox.Show("Пожалуйста Выберите ключ", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }          

            if (keyValue == "rwt_generete")
            {
                string newKeyName = hostValue + "_server";
                KeyObject key = new KeyObject(newKeyName);
                if (!key.KeyExist)
                {
                    if (!KeysManager.GenerateKey(hostValue + "_server", "RWT." + hostValue, false))
                    {
                        DialogResult = DialogResult.None;
                        return;
                    }
                    key = new KeyObject(newKeyName);
                }

                keyValue = key.Filename;
            }

            if (ServersManager.CreateServer(hostValue, hostnameValue, userValue, keyValue))
            {
                KeysManager.GetKeys(true);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                DialogResult = DialogResult.None;
            }
        }
    }
}
