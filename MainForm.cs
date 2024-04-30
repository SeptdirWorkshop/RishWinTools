using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RishWinTools
{
    public partial class MainForm : Form
    {
        private TabControl Tabs;
        private Panel Footer;      

        public MainForm()
        {
            // Window
            Name = "RishWinTools";
            Text = "RishWinTools";
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1024, 600);
            StartPosition = FormStartPosition.CenterScreen;
            Icon = new Icon("icon.ico");

            // Tabs
            Tabs = new TabControl();
            Tabs.Dock = DockStyle.Fill;
            Tabs.Location = new Point(0, 0);
            Tabs.Name = "Tabs";
            Tabs.SelectedIndex = 0;
            Tabs.Size = new Size(800, 450);
            Tabs.SelectedIndexChanged += onTabControlSelectedIndexChanged;
            Tabs.SuspendLayout();
            Controls.Add(Tabs);
            Tabs.Controls.Add(new Servers.ServersTab(Tabs));
            Tabs.Controls.Add(new Keys.KeysTab(Tabs));


            // Footer
            Footer = new Panel
            {
                Dock = DockStyle.Bottom,
                BackColor = Color.White
            };
            Controls.Add(Footer);

            PictureBox footerLogo = new PictureBox
            {
                Image = Image.FromFile("logo.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(100, 80),
                Location = new Point(10, 10)
            };
            Footer.Controls.Add(footerLogo);

            System.Windows.Forms.Label footerTitle = new System.Windows.Forms.Label
            {
                Text = "Rish Windows Tools",
                Location = new Point(footerLogo.Right + 20, 10),
                AutoSize = true
            };
            footerTitle.Font = new Font(footerTitle.Font, FontStyle.Bold);
            Footer.Controls.Add(footerTitle);

            System.Windows.Forms.Label footerAuthor = new System.Windows.Forms.Label
            {
                Text = "by Igor Berdichevskiy",
                Location = new Point(footerLogo.Right + 20, footerTitle.Bottom + 5),
                AutoSize = true
            };
            Footer.Controls.Add(footerAuthor);

            LinkLabel footerLink = new LinkLabel
            {
                Text = "Перейти на сайт rish.su",
                Location = new Point(footerLogo.Right + 20, footerAuthor.Bottom + 5),
                AutoSize = true
            };
            footerLink.LinkClicked += (sender, e) => System.Diagnostics.Process.Start("http://rish.su");
            Footer.Controls.Add(footerLink);

            int footerHeight = 0;
            foreach (Control control in Footer.Controls)
            {
                int controlBottom = control.Bottom;
                if (controlBottom > footerHeight)
                {
                    footerHeight = controlBottom;
                }
            }
            Footer.Height = footerHeight;

            // Layout
            Tabs.ResumeLayout(false);
        }

        private void onTabControlSelectedIndexChanged(object? sender, EventArgs e)
        {
            if (Tabs.SelectedTab is Keys.KeysTab)
            {
                ((Keys.KeysTab)Tabs.SelectedTab).LoadConent(true);
            }
            else if (Tabs.SelectedTab is Servers.ServersTab)
            {
                ((Servers.ServersTab)Tabs.SelectedTab).LoadConent(true);
            }
        }
    }
}
