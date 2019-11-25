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

using System.Windows.Forms;

#endregion

namespace Technosoftware.CommonControls
{
    partial class TitleBarControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.logoPictureBox_ = new System.Windows.Forms.PictureBox();
            this.labelProduct_ = new System.Windows.Forms.Label();
            this.linkLabel_ = new System.Windows.Forms.LinkLabel();
            this.ApplicationName = new System.Windows.Forms.Label();
            this.opcMemberLogoPictureBox_ = new System.Windows.Forms.PictureBox();
            this.labelVersion_ = new System.Windows.Forms.Label();
            this.labelAddress_ = new System.Windows.Forms.Label();
            this.Licensee = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox_)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.opcMemberLogoPictureBox_)).BeginInit();
            this.SuspendLayout();
            // 
            // logoPictureBox_
            // 
            this.logoPictureBox_.Image = global::Technosoftware.CommonControls.Properties.Resources.Logo;
            this.logoPictureBox_.Location = new System.Drawing.Point(15, 3);
            this.logoPictureBox_.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.logoPictureBox_.Name = "logoPictureBox_";
            this.logoPictureBox_.Size = new System.Drawing.Size(285, 102);
            this.logoPictureBox_.TabIndex = 1;
            this.logoPictureBox_.TabStop = false;
            this.logoPictureBox_.Click += new System.EventHandler(this.OnLogoPictureBoxClick);
            // 
            // labelProduct_
            // 
            this.labelProduct_.AutoSize = true;
            this.labelProduct_.BackColor = System.Drawing.Color.White;
            this.labelProduct_.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelProduct_.Location = new System.Drawing.Point(309, 3);
            this.labelProduct_.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelProduct_.Name = "labelProduct_";
            this.labelProduct_.Size = new System.Drawing.Size(488, 29);
            this.labelProduct_.TabIndex = 11;
            this.labelProduct_.Text = "OPC UA Client/Server SDK .NET Standard";
            // 
            // linkLabel_
            // 
            this.linkLabel_.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel_.AutoSize = true;
            this.linkLabel_.BackColor = System.Drawing.Color.White;
            this.linkLabel_.Location = new System.Drawing.Point(808, 80);
            this.linkLabel_.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkLabel_.Name = "linkLabel_";
            this.linkLabel_.Size = new System.Drawing.Size(190, 20);
            this.linkLabel_.TabIndex = 10;
            this.linkLabel_.TabStop = true;
            this.linkLabel_.Text = "www.technosoftware.com";
            this.linkLabel_.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkLabelClick);
            // 
            // ApplicationName
            // 
            this.ApplicationName.BackColor = System.Drawing.Color.White;
            this.ApplicationName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.ApplicationName.Location = new System.Drawing.Point(309, 58);
            this.ApplicationName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ApplicationName.Name = "ApplicationName";
            this.ApplicationName.Size = new System.Drawing.Size(490, 20);
            this.ApplicationName.TabIndex = 12;
            this.ApplicationName.Text = "Sample Application";
            // 
            // opcMemberLogoPictureBox_
            // 
            this.opcMemberLogoPictureBox_.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.opcMemberLogoPictureBox_.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.opcMemberLogoPictureBox_.Image = global::Technosoftware.CommonControls.Properties.Resources.OPCMember;
            this.opcMemberLogoPictureBox_.Location = new System.Drawing.Point(1011, 8);
            this.opcMemberLogoPictureBox_.Margin = new System.Windows.Forms.Padding(4, 9, 4, 5);
            this.opcMemberLogoPictureBox_.Name = "opcMemberLogoPictureBox_";
            this.opcMemberLogoPictureBox_.Size = new System.Drawing.Size(180, 92);
            this.opcMemberLogoPictureBox_.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.opcMemberLogoPictureBox_.TabIndex = 9;
            this.opcMemberLogoPictureBox_.TabStop = false;
            // 
            // labelVersion_
            // 
            this.labelVersion_.Font = new System.Drawing.Font("Arial", 8F);
            this.labelVersion_.Location = new System.Drawing.Point(309, 32);
            this.labelVersion_.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelVersion_.Name = "labelVersion_";
            this.labelVersion_.Size = new System.Drawing.Size(297, 23);
            this.labelVersion_.TabIndex = 17;
            this.labelVersion_.Text = "Version: 1.2.0";
            // 
            // labelAddress_
            // 
            this.labelAddress_.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelAddress_.Font = new System.Drawing.Font("Arial", 8F);
            this.labelAddress_.Location = new System.Drawing.Point(808, 8);
            this.labelAddress_.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelAddress_.Name = "labelAddress_";
            this.labelAddress_.Size = new System.Drawing.Size(183, 72);
            this.labelAddress_.TabIndex = 18;
            this.labelAddress_.Text = "Technosoftware GmbH\r\nWindleweg 3\r\nCH-5325 Rüfenach";
            // 
            // Licensee
            // 
            this.Licensee.BackColor = System.Drawing.Color.White;
            this.Licensee.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.Licensee.Location = new System.Drawing.Point(309, 83);
            this.Licensee.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Licensee.Name = "Licensee";
            this.Licensee.Size = new System.Drawing.Size(490, 20);
            this.Licensee.TabIndex = 19;
            // 
            // TitleBarControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.Licensee);
            this.Controls.Add(this.labelAddress_);
            this.Controls.Add(this.labelVersion_);
            this.Controls.Add(this.labelProduct_);
            this.Controls.Add(this.linkLabel_);
            this.Controls.Add(this.ApplicationName);
            this.Controls.Add(this.opcMemberLogoPictureBox_);
            this.Controls.Add(this.logoPictureBox_);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(1200, 108);
            this.Name = "TitleBarControl";
            this.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Size = new System.Drawing.Size(1200, 108);
            this.Load += new System.EventHandler(this.OnTitleBarControlLoad);
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox_)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.opcMemberLogoPictureBox_)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox logoPictureBox_;
        private System.Windows.Forms.Label labelProduct_;
        private System.Windows.Forms.LinkLabel linkLabel_;
        private System.Windows.Forms.PictureBox opcMemberLogoPictureBox_;
        private System.Windows.Forms.Label labelVersion_;
        private System.Windows.Forms.Label labelAddress_;
        public Label Licensee;
        public Label ApplicationName;
    }
}
