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
using System.IO;

using Opc.Ua;
using Technosoftware.UaClient;
using Technosoftware.UaClient.Controls;

#endregion

namespace WorkshopClient
{
    /// <summary>
    /// The main form for the Reference Client application.
    /// </summary>
    public partial class MainForm : Form
    {
        #region Constructors
        /// <summary>
        /// Creates an empty form.
        /// </summary>
        private MainForm()
        {
            InitializeComponent();
            Icon = ClientUtils.GetAppIcon();
        }

        /// <summary>
        /// Creates a form which uses the specified client configuration.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        public MainForm(ApplicationConfiguration configuration)
        {
            InitializeComponent();
            Icon = ClientUtils.GetAppIcon();
            connectServerControl_.Configuration = configuration_ = configuration;
            connectServerControl_.ServerUrl = "opc.tcp://localhost:55552/TechnosoftwareWorkshopServerForms";
            base.Text = configuration_.ApplicationName;
        }
        #endregion

        #region Private Fields
        private ApplicationConfiguration configuration_;
        private Session session_;
        private bool connectedOnce_;
        #endregion

        #region Private Methods
        #endregion

        #region Event Handlers

        /// <summary>
        /// Connects to a server.
        /// </summary>
        private async void OnServerConnectMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                await connectServerControl_.Connect();
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(base.Text, exception);
            }
        }

        /// <summary>
        /// Disconnects from the current session.
        /// </summary>
        private void OnServerDisconnectMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                connectServerControl_.Disconnect();
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(base.Text, exception);
            }
        }

        /// <summary>
        /// Prompts the user to choose a server on another host.
        /// </summary>
        private void OnServerDiscoverMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                connectServerControl_.Discover(null);
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(base.Text, exception);
            }
        }

        /// <summary>
        /// Updates the application after connecting to or disconnecting from the server.
        /// </summary>
        private void OnServerConnectComplete(object sender, EventArgs e)
        {
            try
            {
                session_ = connectServerControl_.Session;

                // set a suitable initial state.
                if (session_ != null && !connectedOnce_)
                {
                    connectedOnce_ = true;
                }

                // browse the instances in the server.
                browseControl_.Initialize(session_, ObjectIds.ObjectsFolder, ReferenceTypeIds.Organizes, ReferenceTypeIds.Aggregates);
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(base.Text, exception);
            }
        }

        /// <summary>
        /// Updates the application after a communicate error was detected.
        /// </summary>
        private void OnServerReconnectStarting(object sender, EventArgs e)
        {
            try
            {
                browseControl_.ChangeSession(null);
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(base.Text, exception);
            }
        }

        /// <summary>
        /// Updates the application after reconnecting to the server.
        /// </summary>
        private void OnServerReconnectComplete(object sender, EventArgs e)
        {
            try
            {
                session_ = connectServerControl_.Session;
                browseControl_.ChangeSession(session_);
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(base.Text, exception);
            }
        }

        /// <summary>
        /// Cleans up when the main form closes.
        /// </summary>
        private void OnMainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            connectServerControl_.Disconnect();
        }
#endregion

        private void OnExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (MessageBox.Show("Exit this application?", "Technosoftware OPC UA Workshop Client Sample", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void OnHelpContentsMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start( Path.GetDirectoryName(Application.ExecutablePath) + "\\WebHelp\\overview_-_reference_client.htm");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to launch help documentation. Error: " + ex.Message);
            }
        }
    }
}
