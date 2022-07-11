using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Furtiv
{
    internal class FurtivAppContext : ApplicationContext
    {
        //Component declarations
        private NotifyIcon TrayIcon;
        private ContextMenuStrip TrayIconContextMenu;
        private ToolStripMenuItem QuitMenuItem;
        private ToolStripMenuItem LogsMenuItem;

        public FurtivAppContext()
        {
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);

            InitializeComponent();

            RunConsoleApp();
        }

        private void InitializeComponent()
        {
            TrayIcon = new NotifyIcon();

            TrayIcon.BalloonTipIcon = ToolTipIcon.Info;
            TrayIcon.Text = "Furtiv Console Wrapper";

            TrayIcon.Icon = Properties.Resources.PowerShellIcon;

            TrayIconContextMenu = new ContextMenuStrip();
            QuitMenuItem = new ToolStripMenuItem();
            LogsMenuItem = new ToolStripMenuItem();
            TrayIconContextMenu.SuspendLayout();

            // 
            // Tray Icon Context Menu
            // 
            this.TrayIconContextMenu.Items.AddRange(new ToolStripItem[] { this.LogsMenuItem });
            this.TrayIconContextMenu.Items.AddRange(new ToolStripItem[] { this.QuitMenuItem });
            this.TrayIconContextMenu.Name = "TrayIconContextMenu";
            this.TrayIconContextMenu.Size = new Size(101, 70);

            // 
            // View Logs Menu Item
            // 
            this.LogsMenuItem.Name = "LogsMenuItem";
            this.LogsMenuItem.Size = new Size(100, 22);
            this.LogsMenuItem.Text = "View logs";
            this.LogsMenuItem.Click += new EventHandler(this.CloseMenuItem_Click);

            // 
            // Quit Menu Item
            // 
            this.QuitMenuItem.Name = "QuitMenuItem";
            this.QuitMenuItem.Size = new Size(100, 22);
            this.QuitMenuItem.Text = "Quit";
            this.QuitMenuItem.Click += new EventHandler(this.CloseMenuItem_Click);

            TrayIconContextMenu.ResumeLayout(false);

            TrayIcon.ContextMenuStrip = TrayIconContextMenu;

            TrayIcon.Visible = true;
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            //Cleanup so that the icon will be removed when the application is closed
            TrayIcon.Visible = false;
        }

        private void CloseMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    
        private void RunConsoleApp()
        {
            Process process = new Process();
            // Configure the process using the StartInfo properties.
            process.StartInfo.FileName = "powershell.exe";
            process.StartInfo.Arguments = "-ExecutionPolicy Unrestricted -NoLogo -WindowStyle Hidden -Command C:\\Temp\\Test.ps1";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();
        }
    }
}
