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

using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

using Opc.Ua;

#endregion

namespace Technosoftware.CommonControls
{
    public partial class TitleBarControl : UserControl
    {
        public TitleBarControl()
        {
            InitializeComponent();
        }

        private void OnLogoPictureBoxClick(object sender, EventArgs eventArgs)
        {
            string url = "https://technosoftware.com";
            try
            {
                Process.Start(url);
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Unexpected error opening the URL " + url + ".");
            }
        }

        private void OnLinkLabelClick(object sender, EventArgs eventArgs)
        {
            try
            {
                Process.Start(linkLabel_.Text);
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Unexpected error opening the URL " + linkLabel_.Text + ".");
            }
        }

        private void OnTitleBarControlLoad(object sender, EventArgs e)
        {
            ApplicationName.Text = Parent.Text;
            string version;

            try
            {
                version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            }
            catch
            {
                version = "1.2.0.0";
            }
            labelVersion_.Text = String.Format("Version: {0}", version);
        }
    }
}