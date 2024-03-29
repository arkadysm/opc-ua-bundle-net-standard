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
using System.Windows.Forms;

using Opc.Ua;

using Technosoftware.UaConfiguration;
using Technosoftware.CommonControls;
using Technosoftware.UaClient.Controls;

#endregion

namespace WorkshopClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Initialize the user interface.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ApplicationInstance.MessageDlg = new ApplicationMessageDlg();
            ApplicationInstance application = new ApplicationInstance();
            application.ApplicationType   = ApplicationType.Client;
            application.ConfigSectionName = "WorkshopClientForms";

            try
            {
                // process and command line arguments.
                if (application.ProcessCommandLine())
                {
                    return;
                }

                // load the application configuration.
                application.LoadApplicationConfigurationAsync(false).Wait();

                // check the application certificate.
                application.CheckApplicationInstanceCertificateAsync(false, 0).Wait();

                // run the application interactively.
                Application.Run(new MainForm(application.ApplicationConfiguration));
            }
            catch (Exception e)
            {
                ExceptionDlg.Show(application.ApplicationName, e);
                return;
            }
        }
    }

    /// <summary>
    /// The <b>WorkshopClient</b> namespace contains classes which implement a Workshop Client Sample.
    /// </summary>
    /// <exclude/>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class NamespaceDoc
    {
    }
}
