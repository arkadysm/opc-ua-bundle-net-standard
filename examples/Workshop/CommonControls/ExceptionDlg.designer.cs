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

namespace Technosoftware.CommonControls
{
    partial class ExceptionDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionDlg));
            this.bottomPanel_ = new System.Windows.Forms.Panel();
            this.showStackTracesCheckBox_ = new System.Windows.Forms.CheckBox();
            this.closeButton_ = new System.Windows.Forms.Button();
            this.mainPanel_ = new System.Windows.Forms.Panel();
            this.exceptionBrowser_ = new System.Windows.Forms.WebBrowser();
            this.bottomPanel_.SuspendLayout();
            this.mainPanel_.SuspendLayout();
            this.SuspendLayout();
            // 
            // bottomPanel_
            // 
            this.bottomPanel_.Controls.Add(this.showStackTracesCheckBox_);
            this.bottomPanel_.Controls.Add(this.closeButton_);
            resources.ApplyResources(this.bottomPanel_, "bottomPanel_");
            this.bottomPanel_.Name = "bottomPanel_";
            // 
            // showStackTracesCheckBox_
            // 
            resources.ApplyResources(this.showStackTracesCheckBox_, "showStackTracesCheckBox_");
            this.showStackTracesCheckBox_.Name = "showStackTracesCheckBox_";
            this.showStackTracesCheckBox_.UseVisualStyleBackColor = true;
            this.showStackTracesCheckBox_.CheckedChanged += new System.EventHandler(this.OnShowStackTracesCheckBoxCheckedChanged);
            // 
            // closeButton_
            // 
            resources.ApplyResources(this.closeButton_, "closeButton_");
            this.closeButton_.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton_.Name = "closeButton_";
            this.closeButton_.UseVisualStyleBackColor = true;
            // 
            // mainPanel_
            // 
            resources.ApplyResources(this.mainPanel_, "mainPanel_");
            this.mainPanel_.Controls.Add(this.exceptionBrowser_);
            this.mainPanel_.Name = "mainPanel_";
            // 
            // exceptionBrowser_
            // 
            resources.ApplyResources(this.exceptionBrowser_, "exceptionBrowser_");
            this.exceptionBrowser_.Name = "exceptionBrowser_";
            this.exceptionBrowser_.ScriptErrorsSuppressed = true;
            // 
            // ExceptionDlg
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainPanel_);
            this.Controls.Add(this.bottomPanel_);
            this.Name = "ExceptionDlg";
            this.bottomPanel_.ResumeLayout(false);
            this.bottomPanel_.PerformLayout();
            this.mainPanel_.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel bottomPanel_;
        private System.Windows.Forms.Button closeButton_;
        private System.Windows.Forms.Panel mainPanel_;
        private System.Windows.Forms.WebBrowser exceptionBrowser_;
        private System.Windows.Forms.CheckBox showStackTracesCheckBox_;

    }
}
