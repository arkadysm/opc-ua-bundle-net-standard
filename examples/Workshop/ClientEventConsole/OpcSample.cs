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
using System.Collections.Generic;
using System.Reflection;

using Opc.Ua;

using Technosoftware.UaClient;
using Technosoftware.UaConfiguration;

#endregion

namespace Technosoftware.WorkshopClientEventConsole
{
    public class OpcSample
    {
        #region Fields

        private WorkshopClientEventConsoleConfiguration configuration_;

        private const string WorkshopUaServerSampleUri = "opc.tcp://localhost:55554/TechnosoftwareWorkshopServerConsole";

        private Session mySessionSampleServer_;
        private Dictionary<NodeId, NodeId> eventTypeMappings_;

        #endregion

        #region Public Methods

        public void Run()
        {
            ApplicationInstance application = new ApplicationInstance { ApplicationType = ApplicationType.Client };

            try
            {
                // a table used to track event types.
                eventTypeMappings_ = new Dictionary<NodeId, NodeId>();

                // process and command line arguments.
                if (application.ProcessCommandLine())
                {
                    return;
                }

                // Load the Application Configuration and use the specified config section "WorkshopClientEventConsole"
                application.LoadConfigurationAsync("Technosoftware.WorkshopClientEventConsole").Wait();

                // Install a certificate validator callback.
                if (!application.Configuration.SecurityConfiguration.AutoAcceptUntrustedCertificates)
                {
                    application.Configuration.CertificateValidator.CertificateValidationEvent += OnCertificateValidation;
                }

                // Check the Application Certificate.
                application.CheckCertificateAsync().Wait();

                // get the configuration for the extension. In case no configuration exists use suitable defaults.
                configuration_ = application.ApplicationConfig.ParseExtension<WorkshopClientEventConsoleConfiguration>() ??
                                 new WorkshopClientEventConsoleConfiguration();

                string configurationFile = configuration_.ConfigurationFile;

                Console.WriteLine();
                Console.WriteLine("OPC Unified Architecture (DataAccess) client console application");
                Console.WriteLine("----------------------------------------------------------------");
                Console.Write("   Press <Enter> to connect to "); Console.WriteLine(WorkshopUaServerSampleUri);
                Console.ReadLine();
                Console.WriteLine("   Please wait...");

                #region Connect to the OPC UA Sample Server

                List<EndpointDescription> endpointDescriptions = Discover.GetEndpointDescriptions(application.Configuration, WorkshopUaServerSampleUri);

                ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, endpointDescriptions[0]);

                mySessionSampleServer_ = Session.CreateAsync(application.Configuration,
                                            endpoint,
                                            false,
                                            application.Configuration.ApplicationName,
                                            60000,
                                            new UserIdentity(),
                                            null).GetAwaiter().GetResult();
                #endregion

                #region Use OPC UA Sample Server

                if (mySessionSampleServer_ != null && mySessionSampleServer_.Connected)
                {

                    #region Add a Subscription

                    // Create a new subscription
                    Subscription mySubscription = new Subscription
                    {
                        DisplayName = "My Subscription",
                        PublishingEnabled = true,
                        PublishingInterval = 500,
                        KeepAliveCount = 10,
                        LifetimeCount = 100,
                        MaxNotificationsPerPublish = 1000,
                        TimestampsToReturn = TimestampsToReturn.Both
                    };

                    #endregion

                    #region Create a MonitoredItem

                    List<MonitoredItem> list = new List<MonitoredItem>();
                    MonitoredItem monitoredItem = new MonitoredItem(mySubscription.DefaultItem)
                    {
                        DisplayName = "Server",
                        StartNodeId = "i=" + Objects.Server.ToString(),
                        NodeClass = NodeClass.Object,
                        AttributeId = Attributes.Value,
                        SamplingInterval = 0,
                        QueueSize = 1,
                    };

                    // add condition fields to any event filter.
                    EventFilter filter = monitoredItem.Filter as EventFilter;

                    if (filter != null)
                    {
                        monitoredItem.AttributeId = Attributes.EventNotifier;
                        monitoredItem.QueueSize = 0;
                    }
                    list.Add(monitoredItem);

                    list.ForEach(i => i.MonitoredItemNotificationEvent += OnMonitoredEventItemNotification);

                    #endregion

                    #region Add item to subscription and subscription to session

                    // Add the item to the subscription
                    mySubscription.AddItems(list);

                    // Add the subscription to the session
                    mySessionSampleServer_.AddSubscription(mySubscription);

                    #endregion

                    #region Create subscription and apply changes

                    // Create the subscription. Must be done after adding the subscription to a session
                    mySubscription.Create();

                    // Apply all changes on the subscription
                    mySubscription.ApplyChanges();

                    #endregion

                    #region Add item to subscription and apply changes

                    //mySubscription.AddItem(monitoredEventItem);
                    mySubscription.ApplyChanges();
                    mySubscription.ConditionRefresh();

                    #endregion

                    Console.WriteLine();
                    Console.WriteLine("Now you can just let run the client for a while or pres enter to stop the demo.");
                }

                #endregion

                // Read line and then finish
                Console.ReadLine();

            }
            catch (ServiceResultException se)
            {
                Console.WriteLine("   {0}", se.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("   {0}", e.Message);
            }
            finally
            {
                // Close an opened session
                if (mySessionSampleServer_ != null)
                {
                     mySessionSampleServer_.Close();
                }
                Console.WriteLine("   Disconnected from the server.");
                Console.WriteLine();
            }
        }

        #endregion

        #region OPC Event Handlers

        private void OnCertificateValidation(object sender, CertificateValidationEventArgs e)
        {
            e.Accept = true;
        }

        /// <summary>
        /// Updates the display with a new value for a monitored variable. 
        /// </summary>
        private void OnMonitoredEventItemNotification(object sender, MonitoredItemNotificationEventArgs e)
        {
            try
            {
                var monitoredItem = sender as MonitoredItem;
                var notification = e.NotificationValue as EventFieldList;
                string eventTypeName = "";

                if (notification == null)
                {
                    return;
                }

                // check the type of event.
                NodeId eventTypeId = UtilityFunctions.FindEventType((MonitoredItem)sender, notification);

                // ignore unknown events.
                if (NodeId.IsNull(eventTypeId))
                {
                    return;
                }

                // check for refresh start.
                if (eventTypeId == ObjectTypeIds.RefreshStartEventType)
                {
                    eventTypeName = "RefreshStartEventType";
                }

                // check for refresh end.
                if (eventTypeId == ObjectTypeIds.RefreshEndEventType)
                {
                    eventTypeName = "RefreshEndEventType";
                }

                // check for refresh end.
                if (eventTypeId == ObjectTypeIds.SystemEventType)
                {
                    eventTypeName = "SystemEventType";
                }

                // check for autdit event.
                if (eventTypeId == ObjectTypeIds.AuditEventType)
                {
                    eventTypeName = "AuditEventType";
                }

                // construct the base event object.
                var baseEvent = UtilityFunctions.ConstructEvent(
                    mySessionSampleServer_,
                    (MonitoredItem)sender,
                    notification,
                    eventTypeMappings_);

                // Time
                if (baseEvent.Time != null)
                {
                    Console.Write(Utils.Format("Time: {0:HH:mm:ss.fff} ", baseEvent.Time.Value.ToLocalTime()));
                }

                // Severity
                if (baseEvent.Severity != null)
                {
                    Console.Write(Utils.Format("Severity: {0} ", (EventSeverity)baseEvent.Severity.Value));
                }

                // DisplayName
                if (monitoredItem.DisplayName != null)
                {
                    var message = String.Format("DisplayName: {0} ", monitoredItem.DisplayName);
                    Console.Write(message);
                }

                // SourceName
                if (baseEvent.SourceName != null)
                {
                    var message = String.Format("SourceName: {0} ", baseEvent.SourceName.Value);
                    Console.Write(message);
                }

                // Message
                if (baseEvent.Message != null)
                {
                    Console.Write(Utils.Format("Message: {0} ", baseEvent.Message.Value));
                }

                // EventType
                Console.Write(Utils.Format("EventType: {0} ", eventTypeName));
 
                Console.WriteLine();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        #endregion

    }
}
