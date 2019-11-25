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

namespace Technosoftware.WorkshopClient
{
    public class OpcSample
    {
        #region Fields

        private WorkshopClientConfiguration configuration_;

        private const string WorkshopUaServerSampleUri = "opc.tcp://localhost:55554/TechnosoftwareWorkshopServerConsole";

        private readonly NodeId simulatedDataNodeId_ = new NodeId("ns=2;s=Scalar_Simulation_Number");
        private readonly NodeId staticDataNodeId1_ = new NodeId("ns=2;s=Scalar_Static_Integer");
        private readonly NodeId staticDataNodeId2_ = new NodeId("ns=2;s=Scalar_Static_Double");

        private readonly NodeId methodsNodeId_ = new NodeId("ns=2;s=Methods");
        private readonly NodeId callHelloMethodNodeId_ = new NodeId("ns=2;s=Methods_Hello");

        private readonly NodeId dynamicHistoricalAccessNodeId_ = new NodeId("ns=2;s=1:HistoricalAccessServer.Data.Dynamic.Double.txt");

        private Session mySessionSampleServer_;
        private Dictionary<NodeId, NodeId> eventTypeMappings_;

        #endregion

        #region KeepAlive and ReConnect handling

        private SessionReconnectHandler reconnectHandler_;
        private int reconnectPeriod_ = 10;

        #endregion

        #region Public Methods

        public void Run()
        {
            ApplicationInstance application = new ApplicationInstance { ApplicationType = ApplicationType.Client };

            try
            {
                // process and command line arguments.
                if (application.ProcessCommandLine())
                {
                    return;
                }

                // Load the Application Configuration and use the specified config section "WorkshopClientConsole"
                application.LoadConfigurationAsync("Technosoftware.WorkshopClientConsole").Wait();

                // Install a certificate validator callback.
                if (!application.Configuration.SecurityConfiguration.AutoAcceptUntrustedCertificates)
                {
                    application.Configuration.CertificateValidator.CertificateValidationEvent += OnCertificateValidation;
                }

                // Check the Application Certificate.
                application.CheckCertificateAsync().Wait();

                // get the configuration for the extension. In case no configuration exists use suitable defaults.
                configuration_ = application.ApplicationConfig.ParseExtension<WorkshopClientConfiguration>() ??
                                 new WorkshopClientConfiguration();

                string configurationFile = configuration_.ConfigurationFile;

                Console.WriteLine();
                Console.WriteLine("OPC Unified Architecture (DataAccess) client console application");
                Console.WriteLine("----------------------------------------------------------------");
                Console.Write("   Press <Enter> to connect to "); Console.WriteLine(WorkshopUaServerSampleUri);
                Console.ReadLine();
                Console.WriteLine("   Please wait...");

                #region Discover OPC UA Servers

                // Discover all local UA servers
                List<string> servers = Discover.GetUaServers(application.Configuration);

                Console.WriteLine("Found local OPC UA Servers:");
                foreach (var server in servers)
                {
                    Console.WriteLine("{0}", server);
                }

                // Discover all remote UA servers
                //Uri discoveryUri = new Uri("opc.tcp://technosoftware:4840/");
                //servers = Discover.GetUaServers(application.Configuration, discoveryUri);

                //Console.WriteLine("Found remote OPC UA Servers:");
                //foreach (var server in servers)
                //{
                //    Console.WriteLine(String.Format("{0}", server));
                //}

                #endregion

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

                    #region KeepAlive and ReConnect handling

                    mySessionSampleServer_.SessionKeepAliveEvent += OnSessionKeepAliveEvent;

                    #endregion

                    #region Browse the OPC UA Server

                    // Create the browser
                    var browser = new Browser(mySessionSampleServer_)
                    {
                        BrowseDirection = BrowseDirection.Forward,
                        ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
                        IncludeSubtypes = true,
                        NodeClassMask = 0,
                        ContinueUntilDone = false
                    };

                    // Browse from the RootFolder
                    ReferenceDescriptionCollection references = browser.Browse(Objects.ObjectsFolder);

                    GetElements(mySessionSampleServer_, browser, 0, references);

                    #endregion

                    #region Read a single value

                    DataValue simulatedDataValue = mySessionSampleServer_.ReadValue(simulatedDataNodeId_);

                    Console.WriteLine("Node Value:" + simulatedDataValue.Value);

                    #endregion

                    #region Read multiple values

                    // The input parameters of the ReadValues() method
                    List<NodeId> variableIds = new List<NodeId>();
                    List<Type> expectedTypes = new List<Type>();

                    // The output parameters of the ReadValues() method
                    List<object> values;
                    List<ServiceResult> errors;

                    // Add a node to the list
                    variableIds.Add(simulatedDataNodeId_);
                    // Add an expected type to the list (null means we get the original type from the server)
                    expectedTypes.Add(null);

                    // Add another node to the list
                    variableIds.Add(staticDataNodeId1_);
                    // Add an expected type to the list (null means we get the original type from the server)
                    expectedTypes.Add(null);

                    // Add another node to the list
                    variableIds.Add(staticDataNodeId2_);

                    // Add an expected type to the list (null means we get the original type from the server)
                    expectedTypes.Add(null);

                    mySessionSampleServer_.ReadValues(variableIds, expectedTypes, out values, out errors);

                    #endregion

                    #region Read multiple values asynchronous

                    // start reading the value (setting a 10 second timeout).
                    mySessionSampleServer_.BeginReadValues(
                        variableIds,
                        0,
                        TimestampsToReturn.Both,
                        OnReadComplete,
                        new UserData { Session = mySessionSampleServer_, NodeIds = variableIds });

                    #endregion

                    #region Write a value

                    short writeInt = 1234;

                    Console.WriteLine("Write Value: " + writeInt);
                    StatusCode result = mySessionSampleServer_.WriteValue(staticDataNodeId1_, new DataValue(writeInt));

                    // read it again to check the new value
                    Console.WriteLine("Node Value (should be {0}): {1}", mySessionSampleServer_.ReadValue(staticDataNodeId1_).Value, writeInt);

                    #endregion

                    #region Write multiple values at once

                    writeInt = 5678;
                    Double writeDouble = 1234.1234;

                    List<NodeId> nodeIds = new List<NodeId>();
                    List<DataValue> dataValues = new List<DataValue>();

                    nodeIds.Add(staticDataNodeId1_);
                    nodeIds.Add(staticDataNodeId2_);

                    dataValues.Add(new DataValue(writeInt));
                    dataValues.Add(new DataValue(writeDouble));

                    Console.WriteLine("Write Values: {0} and {1}", writeInt, writeDouble);
                    List<StatusCode> statusCodes = mySessionSampleServer_.WriteValues(nodeIds, dataValues);

                    // read it again to check the new value
                    Console.WriteLine("Node Value (should be {0}): {1}", mySessionSampleServer_.ReadValue(staticDataNodeId1_).Value, writeInt);
                    Console.WriteLine("Node Value (should be {0}): {1}", mySessionSampleServer_.ReadValue(staticDataNodeId2_).Value, writeDouble);

                    #endregion

                    #region Write multiple values asynchronous

                    // start writing the values.
                    mySessionSampleServer_.BeginWriteValues(
                        nodeIds,
                        dataValues,
                        OnWriteComplete,
                        new UserData { Session = mySessionSampleServer_, NodeIds = nodeIds });

                    #endregion

                    #region Create a MonitoredItem

                    // Create a MonitoredItem
                    MonitoredItem monitoredItem = new MonitoredItem
                    {
                        StartNodeId = new NodeId(simulatedDataNodeId_),
                        AttributeId = Attributes.Value,
                        DisplayName = "Simulated Data Value",
                        MonitoringMode = MonitoringMode.Reporting,
                        SamplingInterval = 1000,
                        QueueSize = 0,
                        DiscardOldest = true
                    };

                    #endregion

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

                    #region Subscribe to data changes

                    // Establish the notification event to get changes on the MonitoredItem
                    monitoredItem.MonitoredItemNotificationEvent += OnMonitoredItemNotificationEvent;

                    #endregion

                    #region Add item to subscription and subscription to session

                    // Add the item to the subscription
                    mySubscription.AddItem(monitoredItem);

                    // Add the subscription to the session
                    mySessionSampleServer_.AddSubscription(mySubscription);

                    #endregion

                    #region Subscribe to event changes

                    // a table used to track event types.
                    eventTypeMappings_ = new Dictionary<NodeId, NodeId>();

                    List<MonitoredItem> list = new List<MonitoredItem>();
                    monitoredItem = new MonitoredItem(mySubscription.DefaultItem)
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

                    // Add the item to the subscription
                    mySubscription.AddItems(list);

                    #endregion

                    #region Create subscription and apply changes

                    // Create the subscription. Must be done after adding the subscription to a session
                    mySubscription.Create();

                    // Apply all changes on the subscription
                    mySubscription.ApplyChanges();

                    #endregion

                    #region Call a Method

                    INode node = mySessionSampleServer_.ReadNode(callHelloMethodNodeId_);


                    if (node is MethodNode)
                    {
                        var methodNode = node as MethodNode;
                        NodeId methodId = callHelloMethodNodeId_;

                        NodeId objectId = methodsNodeId_;

                        VariantCollection inputArguments = new VariantCollection();

                        Argument argument = new Argument();

                        inputArguments.Add(new Variant("from Technosoftware"));

                        var request = new CallMethodRequest { ObjectId = objectId, MethodId = methodId, InputArguments = inputArguments };

                        var requests = new CallMethodRequestCollection { request };

                        CallMethodResultCollection results;
                        DiagnosticInfoCollection diagnosticInfos;

                        ResponseHeader responseHeader = mySessionSampleServer_.Call(
                            null,
                            requests,
                            out results,
                            out diagnosticInfos);

                        if (StatusCode.IsBad(results[0].StatusCode))
                        {
                            throw new ServiceResultException(new ServiceResult(results[0].StatusCode, 0, diagnosticInfos,
                                responseHeader.StringTable));
                        }

                        Console.WriteLine("{0}", results[0].OutputArguments[0]);

                    }

                    #endregion

                    Console.WriteLine();
                    Console.WriteLine("Now you can just let run the client for a while, test the reconnect or pres enter to start the next demo.");
                    Console.ReadLine();
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
                    #region  KeepAlive and ReConnect handling

                    if (reconnectHandler_ != null)
                    {
                        reconnectHandler_.Dispose();
                        reconnectHandler_ = null;
                    }

                    #endregion

                    mySessionSampleServer_.Close();
                }
                Console.WriteLine("   Disconnected from the server.");
                Console.WriteLine();
            }
        }

        #endregion

        #region Private Methods (Browse related)

        /// <summary>
        /// Gets all elements for the specified references
        /// </summary>
        /// <param name="session">The session to use</param>
        /// <param name="browser">The browser to use</param>
        /// <param name="level">The level</param>
        /// <param name="references">The references to browse</param>
        private static void GetElements(Session session, Browser browser, uint level, ReferenceDescriptionCollection references)
        {
            var spaces = "";
            for (int i = 0; i <= level; i++)
            {
                spaces += "   ";
            }
            // Iterate through the references and print the variables
            foreach (ReferenceDescription reference in references)
            {
                // make sure the type definition is in the cache.
                session.NodeCache.Find(reference.ReferenceTypeId);

                switch (reference.NodeClass)
                {
                    case NodeClass.Object:
                        Console.WriteLine(spaces + "+ " + reference.DisplayName);
                        break;

                    default:
                        Console.WriteLine(spaces + "- " + reference.DisplayName);
                        break;
                }
                var subReferences = browser.Browse((NodeId)reference.NodeId);
                level += 1;
                GetElements(session, browser, level, subReferences);
                level -= 1;
            }
        }

        #endregion

        #region KeepAlive and ReConnect handling

        private void OnSessionKeepAliveEvent(object sender, SessionKeepAliveEventArgs e)
        {
            Session session = (Session)sender;
            if (sender != null && session.Endpoint != null)
            {
                Console.WriteLine(Utils.Format(
                    "{0} ({1}) {2}",
                    session.Endpoint.EndpointUrl,
                    session.Endpoint.SecurityMode,
                    (session.EndpointConfiguration.UseBinaryEncoding) ? "UABinary" : "XML"));
            }

            if (e != null && mySessionSampleServer_ != null)
            {
                if (ServiceResult.IsGood(e.Status))
                {
                    Console.WriteLine(Utils.Format(
                        "Server Status: {0} {1:yyyy-MM-dd HH:mm:ss} {2}/{3}",
                        e.CurrentState,
                        e.CurrentTime.ToLocalTime(),
                        mySessionSampleServer_.OutstandingRequestCount,
                        mySessionSampleServer_.DefunctRequestCount));
                }
                else
                {
                    Console.WriteLine("{0} {1}/{2}", e.Status, mySessionSampleServer_.OutstandingRequestCount, mySessionSampleServer_.DefunctRequestCount);

                    if (reconnectPeriod_ <= 0)
                    {
                        return;
                    }

                    if (reconnectHandler_ == null && reconnectPeriod_ > 0)
                    {
                        reconnectHandler_ = new SessionReconnectHandler();
                        reconnectHandler_.BeginReconnect(mySessionSampleServer_, reconnectPeriod_ * 1000, OnServerReconnectComplete);
                    }
                }
            }
        }

        private void OnServerReconnectComplete(object sender, EventArgs e)
        {

            try
            {
                // ignore callbacks from discarded objects.
                if (!ReferenceEquals(sender, reconnectHandler_))
                {
                    return;
                }

                mySessionSampleServer_ = reconnectHandler_.Session;
                reconnectHandler_.Dispose();
                reconnectHandler_ = null;
                OnSessionKeepAliveEvent(mySessionSampleServer_, null);
            }
            catch (Exception exception)
            {
                // GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }

        #endregion

        #region OPC Event Handlers

        private void OnCertificateValidation(object sender, CertificateValidationEventArgs e)
        {
            e.Accept = true;
        }

        private void OnMonitoredItemNotificationEvent(object sender, MonitoredItemNotificationEventArgs e)
        {
            var notification = e.NotificationValue as MonitoredItemNotification;
            if (notification == null)
            {
                return;
            }
            if (sender is MonitoredItem)
            {
                var monitoredItem = sender as MonitoredItem;
                var message = String.Format("OnMonitoredItemNotificationEvent called for Variable \"{0}\" with Value = {1}.", monitoredItem.DisplayName, notification.Value);
                Console.WriteLine(message);
            }
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
                    return;
                }

                // check for refresh end.
                if (eventTypeId == ObjectTypeIds.RefreshEndEventType)
                {
                    return;
                }

                // check for autdit event.
                if (eventTypeId == ObjectTypeIds.AuditEventType)
                {
                    return;
                }

                // construct the base event object.
                var baseEvent = UtilityFunctions.ConstructEvent(
                    mySessionSampleServer_,
                    (MonitoredItem)sender,
                    notification,
                    eventTypeMappings_);

                // Source
                if (baseEvent.SourceName != null)
                {
                    var message = String.Format("OnMonitoredEventItemNotification called for Source \"{0}\".", baseEvent.SourceName.Value);
                    Console.Write(message);
                }

                // Severity
                if (baseEvent.Severity != null)
                {
                    Console.Write(Utils.Format("Severity: {0} ", (EventSeverity)baseEvent.Severity.Value));
                }

                // Time
                if (baseEvent.Time != null)
                {
                    Console.Write(Utils.Format("Time: {0:HH:mm:ss.fff} ", baseEvent.Time.Value.ToLocalTime()));
                }

                // Message
                if (baseEvent.Message != null)
                {
                    Console.Write(Utils.Format("Message: {0} ", baseEvent.Message.Value));
                }

                Console.WriteLine();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        #endregion

        #region Asynchronous related classes and Handlers

        /// <summary>
        /// A object used to pass state with an asynchronous write call.
        /// </summary>
        private class UserData
        {
            public Session Session { get; set; }
            public List<NodeId> NodeIds { get; set; }
        }

        /// <summary>
        /// Finishes an asynchronous read request.
        /// </summary>
        private void OnReadComplete(IAsyncResult result)
        {
            // get the session used to send the request which was passed as the userData in the BeginWriteValues call.
            UserData userData = (UserData)result.AsyncState;

            try
            {
                // get the results.
                List<DataValue> results = userData.Session.EndReadValues(result);

                // write the result to the console.
                for (int i = 0; i < results.Count; i++)
                {
                    Console.WriteLine("Status of Read of Node {0} is: {1}", userData.NodeIds[i].ToString(), results[i].Value.ToString());
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Finishes an asynchronous read request.
        /// </summary>
        private void OnWriteComplete(IAsyncResult result)
        {
            // get the session used to send the request which was passed as the userData in the BeginWriteValues call.
            UserData userData = (UserData)result.AsyncState;

            try
            {
                // get the results.
                List<StatusCode> results = userData.Session.EndWriteValues(result);

                // write the result to the console.
                for (int i = 0; i < results.Count; i++)
                {
                    Console.WriteLine("Status of Write to Node {0} is: {1}", userData.NodeIds[i].ToString(), results[i].ToString());
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion
    }
}
