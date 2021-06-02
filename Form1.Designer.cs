
namespace ActionCopy
{
    partial class ActionCopyForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.startServerButton = new System.Windows.Forms.Button();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.startClientButton = new System.Windows.Forms.Button();
            this.hostAddressBox = new System.Windows.Forms.TextBox();
            this.clientSetupPanel = new System.Windows.Forms.Panel();
            this.serversListBox = new System.Windows.Forms.ListBox();
            this.actionCopyNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifyContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.stopButton = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshServersButton = new System.Windows.Forms.Button();
            this.clientSetupPanel.SuspendLayout();
            this.notifyContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // startServerButton
            // 
            this.startServerButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.startServerButton.Location = new System.Drawing.Point(0, 0);
            this.startServerButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.startServerButton.Name = "startServerButton";
            this.startServerButton.Size = new System.Drawing.Size(384, 44);
            this.startServerButton.TabIndex = 0;
            this.startServerButton.Text = "run server";
            this.startServerButton.UseVisualStyleBackColor = true;
            this.startServerButton.Click += new System.EventHandler(this.startServerButton_Click);
            // 
            // updateTimer
            // 
            this.updateTimer.Interval = 10;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // startClientButton
            // 
            this.startClientButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.startClientButton.Location = new System.Drawing.Point(240, 0);
            this.startClientButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.startClientButton.Name = "startClientButton";
            this.startClientButton.Size = new System.Drawing.Size(144, 60);
            this.startClientButton.TabIndex = 1;
            this.startClientButton.Text = "run client";
            this.startClientButton.UseVisualStyleBackColor = true;
            this.startClientButton.Click += new System.EventHandler(this.startClientButton_Click);
            // 
            // hostAddressBox
            // 
            this.hostAddressBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.hostAddressBox.Location = new System.Drawing.Point(14, 16);
            this.hostAddressBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.hostAddressBox.Name = "hostAddressBox";
            this.hostAddressBox.Size = new System.Drawing.Size(219, 27);
            this.hostAddressBox.TabIndex = 2;
            this.hostAddressBox.Text = "127.0.0.1";
            // 
            // clientSetupPanel
            // 
            this.clientSetupPanel.BackColor = System.Drawing.SystemColors.Control;
            this.clientSetupPanel.Controls.Add(this.hostAddressBox);
            this.clientSetupPanel.Controls.Add(this.startClientButton);
            this.clientSetupPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.clientSetupPanel.Location = new System.Drawing.Point(0, 44);
            this.clientSetupPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.clientSetupPanel.Name = "clientSetupPanel";
            this.clientSetupPanel.Size = new System.Drawing.Size(384, 60);
            this.clientSetupPanel.TabIndex = 3;
            // 
            // serversListBox
            // 
            this.serversListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serversListBox.FormattingEnabled = true;
            this.serversListBox.ItemHeight = 20;
            this.serversListBox.Location = new System.Drawing.Point(0, 104);
            this.serversListBox.Name = "serversListBox";
            this.serversListBox.Size = new System.Drawing.Size(384, 87);
            this.serversListBox.TabIndex = 4;
            this.serversListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.serversListBox_DoubleClick);
            // 
            // actionCopyNotifyIcon
            // 
            this.actionCopyNotifyIcon.ContextMenuStrip = this.notifyContextMenuStrip;
            this.actionCopyNotifyIcon.Text = "notifyIcon";
            // 
            // notifyContextMenuStrip
            // 
            this.notifyContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stopButton});
            this.notifyContextMenuStrip.Name = "notifyContextMenuStrip";
            this.notifyContextMenuStrip.Size = new System.Drawing.Size(99, 26);
            // 
            // stopButton
            // 
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(98, 22);
            this.stopButton.Text = "Stop";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // refreshServersButton
            // 
            this.refreshServersButton.Location = new System.Drawing.Point(297, 111);
            this.refreshServersButton.Name = "refreshServersButton";
            this.refreshServersButton.Size = new System.Drawing.Size(75, 31);
            this.refreshServersButton.TabIndex = 5;
            this.refreshServersButton.Text = "refresh";
            this.refreshServersButton.UseVisualStyleBackColor = true;
            this.refreshServersButton.Click += new System.EventHandler(this.refreshServersButton_Click);
            // 
            // ActionCopyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(384, 191);
            this.Controls.Add(this.refreshServersButton);
            this.Controls.Add(this.serversListBox);
            this.Controls.Add(this.clientSetupPanel);
            this.Controls.Add(this.startServerButton);
            this.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(400, 230);
            this.Name = "ActionCopyForm";
            this.ShowIcon = false;
            this.Text = "ActionCopy";
            this.Load += new System.EventHandler(this.ActionCopyForm_Load);
            this.clientSetupPanel.ResumeLayout(false);
            this.clientSetupPanel.PerformLayout();
            this.notifyContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button startServerButton;
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.Button startClientButton;
        private System.Windows.Forms.TextBox hostAddressBox;
        private System.Windows.Forms.Panel clientSetupPanel;
        private System.Windows.Forms.ListBox serversListBox;
        private System.Windows.Forms.NotifyIcon actionCopyNotifyIcon;
        private System.Windows.Forms.ContextMenuStrip notifyContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem stopButton;
        private System.Windows.Forms.Button refreshServersButton;
    }
}

