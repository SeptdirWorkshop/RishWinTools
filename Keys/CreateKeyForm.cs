using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RishWinTools.Keys
{
    public partial class CreateKeyForm : System.Windows.Forms.Form
    {
        private Label NameLabel;
        private TextBox NameInput;
        public string NameValue => NameInput.Text;
        private Label CommentLabel;
        private TextBox CommentInput;
        public string CommentValue => CommentInput.Text;
        private Button GenerateButton;

        public CreateKeyForm()
        {
            // Window
            Name = "CreateKeyForm";
            Text = "Создание SSH ключа";
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
            NameInput.Location = new Point(10, NameLabel.Bottom + 5);
            Controls.Add(NameInput);

            // CommentLabel
            CommentLabel = new Label();
            CommentLabel.Name = "CommentLabel";
            CommentLabel.Text = "Комментарий к ключу";
            CommentLabel.AutoSize = true;
            CommentLabel.Location = new Point(10, NameInput.Bottom + 10);
            Controls.Add(CommentLabel);

            // CommentInput
            CommentInput = new TextBox();
            CommentInput.Name = "CommentInput";
            CommentInput.Text = "Youre.Name";
            CommentInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            CommentInput.Size = new Size(ClientSize.Width - 20, 20);
            CommentInput.Location = new Point(10, CommentLabel.Bottom + 5);
            Controls.Add(CommentInput);

            // GenerateButton
            GenerateButton = new Button();
            GenerateButton.Name = "GenerateButton";
            GenerateButton.Text = "Сгенерировать ключ";
            GenerateButton.AutoSize = true;
            GenerateButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            GenerateButton.Location = new Point(10, CommentInput.Bottom + 10);
            GenerateButton.UseVisualStyleBackColor = true;
            GenerateButton.Click += OnGenerateButtonClick;
            Controls.Add(GenerateButton);

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

        protected void OnGenerateButtonClick(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameValue))
            {
                MessageBox.Show("Пожалуйста заполните поле Имя", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(CommentValue))
            {
                MessageBox.Show("Пожалуйста заполните поле Комментарий.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (KeysManager.GenerateKey(NameValue, CommentValue))
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
