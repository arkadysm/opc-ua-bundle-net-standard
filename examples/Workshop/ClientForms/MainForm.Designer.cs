#region Copyright (c) 2011-2019 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2019 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
// 
// License: 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// SPDX-License-Identifier: MIT
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2019 Technosoftware GmbH. All rights reserved

#region Using Directives

#endregion

namespace WorkshopClient
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuBar_ = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem_ = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem_ = new System.Windows.Forms.ToolStripMenuItem();
            this.serverMenuItem_ = new System.Windows.Forms.ToolStripMenuItem();
            this.serverDiscoverMenuItem_ = new System.Windows.Forms.ToolStripMenuItem();
            this.serverConnectMenuItem_ = new System.Windows.Forms.ToolStripMenuItem();
            this.serverDisconnectMenuItem_ = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenuItem_ = new System.Windows.Forms.ToolStripMenuItem();
            this.helpContentsMenuItem_ = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBar_ = new System.Windows.Forms.StatusStrip();
            this.connectServerControl_ = new Technosoftware.UaClient.Controls.ConnectServerCtrl();
            this.browseControl_ = new Technosoftware.UaClient.Controls.BrowseNodeCtrl();
            this.titleBarControl_ = new Technosoftware.CommonControls.TitleBarControl();
            this.menuBar_.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuBar_
            // 
            this.menuBar_.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem_,
            this.serverMenuItem_,
            this.helpMenuItem_});
            this.menuBar_.Location = new System.Drawing.Point(0, 0);
            this.menuBar_.Name = "menuBar_";
            this.menuBar_.Size = new System.Drawing.Size(884, 24);
            this.menuBar_.TabIndex = 1;
            this.menuBar_.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem_
            // 
            this.fileToolStripMenuItem_.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem_});
            this.fileToolStripMenuItem_.Name = "fileToolStripMenuItem_";
            this.fileToolStripMenuItem_.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem_.Text = "&File";
            // 
            // exitToolStripMenuItem_
            // 
            this.exitToolStripMenuItem_.Name = "exitToolStripMenuItem_";
            this.exitToolStripMenuItem_.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem_.Text = "E&xit";
            this.exitToolStripMenuItem_.Click += new System.EventHandler(this.OnExitToolStripMenuItemClick);
            // 
            // serverMenuItem_
            // 
            this.serverMenuItem_.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serverDiscoverMenuItem_,
            this.serverConnectMenuItem_,
            this.serverDisconnectMenuItem_});
            this.serverMenuItem_.Name = "serverMenuItem_";
            this.serverMenuItem_.Size = new System.Drawing.Size(51, 20);
            this.serverMenuItem_.Text = "Server";
            // 
            // serverDiscoverMenuItem_
            // 
            this.serverDiscoverMenuItem_.Name = "serverDiscoverMenuItem_";
            this.serverDiscoverMenuItem_.Size = new System.Drawing.Size(133, 22);
            this.serverDiscoverMenuItem_.Text = "Discover...";
            this.serverDiscoverMenuItem_.Click += new System.EventHandler(this.OnServerDiscoverMenuItemClick);
            // 
            // serverConnectMenuItem_
            // 
            this.serverConnectMenuItem_.Name = "serverConnectMenuItem_";
            this.serverConnectMenuItem_.Size = new System.Drawing.Size(133, 22);
            this.serverConnectMenuItem_.Text = "Connect";
            this.serverConnectMenuItem_.Click += new System.EventHandler(this.OnServerConnectMenuItemClick);
            // 
            // serverDisconnectMenuItem_
            // 
            this.serverDisconnectMenuItem_.Name = "serverDisconnectMenuItem_";
            this.serverDisconnectMenuItem_.Size = new System.Drawing.Size(133, 22);
            this.serverDisconnectMenuItem_.Text = "Disconnect";
            this.serverDisconnectMenuItem_.Click += new System.EventHandler(this.OnServerDisconnectMenuItemClick);
            // 
            // helpMenuItem_
            // 
            this.helpMenuItem_.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpContentsMenuItem_});
            this.helpMenuItem_.Name = "helpMenuItem_";
            this.helpMenuItem_.Size = new System.Drawing.Size(44, 20);
            this.helpMenuItem_.Text = "Help";
            // 
            // helpContentsMenuItem_
            // 
            this.helpContentsMenuItem_.Name = "helpContentsMenuItem_";
            this.helpContentsMenuItem_.Size = new System.Drawing.Size(122, 22);
            this.helpContentsMenuItem_.Text = "Contents";
            this.helpContentsMenuItem_.Click += new System.EventHandler(this.OnHelpContentsMenuItemClick);
            // 
            // statusBar_
            // 
            this.statusBar_.Location = new System.Drawing.Point(0, 524);
            this.statusBar_.Name = "statusBar_";
            this.statusBar_.Size = new System.Drawing.Size(884, 22);
            this.statusBar_.TabIndex = 2;
            // 
            // connectServerControl_
            // 
            this.connectServerControl_.Configuration = null;
            this.connectServerControl_.DisableDomainCheck = false;
            this.connectServerControl_.Dock = System.Windows.Forms.DockStyle.Top;
            this.connectServerControl_.Location = new System.Drawing.Point(0, 99);
            this.connectServerControl_.MaximumSize = new System.Drawing.Size(2048, 23);
            this.connectServerControl_.MinimumSize = new System.Drawing.Size(500, 23);
            this.connectServerControl_.Name = "connectServerControl_";
            this.connectServerControl_.PreferredLocales = null;
            this.connectServerControl_.ServerUrl = "";
            this.connectServerControl_.SessionName = null;
            this.connectServerControl_.Size = new System.Drawing.Size(884, 23);
            this.connectServerControl_.StatusStrip = this.statusBar_;
            this.connectServerControl_.TabIndex = 6;
            this.connectServerControl_.UserIdentity = null;
            this.connectServerControl_.UseSecurity = true;
            this.connectServerControl_.ReconnectStarting += new System.EventHandler(this.OnServerReconnectStarting);
            this.connectServerControl_.ReconnectComplete += new System.EventHandler(this.OnServerReconnectComplete);
            this.connectServerControl_.ConnectComplete += new System.EventHandler(this.OnServerConnectComplete);
            // 
            // browseControl_
            // 
            this.browseControl_.AttributesListCollapsed = false;
            this.browseControl_.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browseControl_.Location = new System.Drawing.Point(0, 122);
            this.browseControl_.Name = "browseControl_";
            this.browseControl_.Size = new System.Drawing.Size(884, 402);
            this.browseControl_.SplitterDistance = 387;
            this.browseControl_.TabIndex = 5;
            this.browseControl_.View = null;
            // 
            // titleBarControl_
            // 
            this.titleBarControl_.BackColor = System.Drawing.Color.White;
            this.titleBarControl_.Dock = System.Windows.Forms.DockStyle.Top;
            this.titleBarControl_.ForeColor = System.Drawing.SystemColors.ControlText;
            this.titleBarControl_.Location = new System.Drawing.Point(0, 24);
            this.titleBarControl_.MinimumSize = new System.Drawing.Size(500, 75);
            this.titleBarControl_.Name = "titleBarControl_";
            this.titleBarControl_.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.titleBarControl_.Size = new System.Drawing.Size(884, 75);
            this.titleBarControl_.TabIndex = 7;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 562);
            this.Controls.Add(this.browseControl_);
            this.Controls.Add(this.connectServerControl_);
            this.Controls.Add(this.statusBar_);
            this.Controls.Add(this.titleBarControl_);
            this.Controls.Add(this.menuBar_);
            this.MainMenuStrip = this.menuBar_;
            this.Name = "MainForm";
            this.Text = "Technosoftware OPC UA Workshop Client Sample";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnMainFormFormClosing);
            this.menuBar_.ResumeLayout(false);
            this.menuBar_.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuBar_;
        private System.Windows.Forms.StatusStrip statusBar_;
        private System.Windows.Forms.ToolStripMenuItem serverMenuItem_;
        private System.Windows.Forms.ToolStripMenuItem serverDiscoverMenuItem_;
        private System.Windows.Forms.ToolStripMenuItem serverConnectMenuItem_;
        private System.Windows.Forms.ToolStripMenuItem serverDisconnectMenuItem_;
        private System.Windows.Forms.ToolStripMenuItem helpMenuItem_;
        private System.Windows.Forms.ToolStripMenuItem helpContentsMenuItem_;
        private Technosoftware.UaClient.Controls.ConnectServerCtrl connectServerControl_;
        private Technosoftware.UaClient.Controls.BrowseNodeCtrl browseControl_;
        private Technosoftware.CommonControls.TitleBarControl titleBarControl_;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem_;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem_;
    }
}
