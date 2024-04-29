using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RishWinTools
{
    public partial class MainForm : Form
    {
        private TabControl Tabs;
        private readonly System.ComponentModel.IContainer? Components;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (Components != null))
            {
                Components.Dispose();
            }
            base.Dispose(disposing);
        }

        public MainForm()
        {
            // Window
            Name = "RishWinTools";
            Text = "RishWinTools";
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 600);
            StartPosition = FormStartPosition.CenterScreen;

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

            // Add servers tab
            Tabs.Controls.Add(new Servers.ServersTab(Tabs));

            // Add keys tab
            Tabs.Controls.Add(new Keys.KeysTab(Tabs));

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
