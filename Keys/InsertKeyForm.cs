using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RishWinTools.Keys
{
    public partial class InsertKeyForm : System.Windows.Forms.Form
    {
        private Label NameLabel;
        private TextBox NameInput;
        public string NameValue => NameInput.Text;
        private Label SecretKeyLabel;
        private TextBox SecretKeyInput;
        public string SecretKeyValue => SecretKeyInput.Text;
        private Button InsertButton;

        public InsertKeyForm()
        {
            // Window
            Name = "InsertKeyForm";
            Text = "Вставка SSH ключа";
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(640, 0);
            StartPosition = FormStartPosition.CenterParent;

            // NameLabel
            NameLabel = new Label();
            NameLabel.Name = "NameLabel";
            NameLabel.Text = "Имя ключа";
            NameLabel.AutoSize = true;
            NameLabel.Location = new Point(10, 20);
            Controls.Add(NameLabel);

            // NameInput
            NameInput = new TextBox();
            NameInput.Name = "NameInput";
            NameInput.Text = "id_ed25519";
            NameInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            NameInput.Size = new Size(ClientSize.Width - 20, 20);
            NameInput.Location = new Point(10, 40);
            Controls.Add(NameInput);

            // SecretKeyLabel
            SecretKeyLabel = new Label();
            SecretKeyLabel.Name = "SecretKeyLabel";
            SecretKeyLabel.Text = "Секретный ключ";
            SecretKeyLabel.AutoSize = true;
            SecretKeyLabel.Location = new Point(10, NameInput.Bottom + 10);
            Controls.Add(SecretKeyLabel);

            // SecretKeyInput
            SecretKeyInput = new TextBox();
            SecretKeyInput.Name = "SecretKeyInput";
            SecretKeyInput.Text = "";
            SecretKeyInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;         
            SecretKeyInput.Size = new Size(ClientSize.Width - 20, 140);
            SecretKeyInput.Location = new Point(10, SecretKeyLabel.Bottom + 5);
            SecretKeyInput.Multiline = true;
            Controls.Add(SecretKeyInput);

            // InsertButton
            InsertButton = new Button();
            InsertButton.Name = "InsertButton";
            InsertButton.Text = "Вставить ключ";           
            InsertButton.AutoSize = true;
            InsertButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            InsertButton.Location = new Point(10, SecretKeyInput.Bottom + 10);
            InsertButton.UseVisualStyleBackColor = true;
            InsertButton.Click += OnInsertButtonClick;
            Controls.Add(InsertButton);

            // Layout
            int FormHeight = 0;
            foreach (Control control in Controls)
            {
                int controlBottom = control.Bottom;
                if (controlBottom > FormHeight)
                {
                    FormHeight = controlBottom;
                }
            }
            ClientSize = new Size(ClientSize.Width, FormHeight + 20);           
        }

        protected void OnInsertButtonClick(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameValue))
            {
                MessageBox.Show("Пожалуйста заполните поле Имя", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(SecretKeyValue))
            {
                MessageBox.Show("Пожалуйста заполните поле Секретный ключ.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (KeysManager.InsertKey(NameValue, SecretKeyValue))
            {
                KeysManager.GetKeys(true);
                if (KeysManager.CreatePubKey(NameValue, false, true))
                {
                    KeysManager.GetKeys(true);
                }
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
