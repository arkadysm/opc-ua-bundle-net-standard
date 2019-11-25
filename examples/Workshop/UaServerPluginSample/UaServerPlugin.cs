#region Copyright (c) 2011-2019 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2019 Technosoftware GmbH. All rights reserved
// Web: http://www.technosoftware.com
//
// Purpose:
//
//
// The Software is subject to the Technosoftware GmbH Software License Agreement,
// which can be found here:
// http://www.technosoftware.com/documents/Technosoftware_SLA.pdf
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2019 Technosoftware GmbH. All rights reserved

#region Using Directives

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using Opc.Ua;

using Technosoftware.UaServer;
using Technosoftware.UaServer.Controls;

using LicenseHandler = Opc.Ua.LicenseHandler;
using Timer = System.Threading.Timer;

#endregion

namespace UaServerPlugin
{
    /// <summary>
    ///   <para>OPC UA Server Configuration and IO Handling</para>
    ///   <para>This C# based plugin for the OPC UA Server SDK .NET Standard shows a base OPC UA 1.01, OPC UA 1.02, OPC UA 1.03 and OPC UA 1.04 compliant OPC UA server
    /// implementation.</para>
    ///   <para>At startup items with several data types and access rights are statically defined. The RefreshThread simulates signal changes for items like /Dynamic/All
    /// Profiles/Scalar/Double /Dynamic/All Profiles/Scalar/String /Dynamic/DA Profile/DataItem/UInt64 and writes the changed values into the internal cache. Item
    /// values written by a client are written into the local buffer only.</para>
    /// </summary>
    public class UaServerPlugin : IUaServerPlugin, IDisposable
    {
        #region Constants

        #endregion

        #region Fields

        private readonly object lockDisposable_ = new object();
        private bool disposed_;
        private List<BaseDataVariableState> dynamicNodes_;
        IUaServer opcServer_;

        private Timer simulationTimer_;

        #endregion

        #region Constructors, Destructor, Initialization

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~UaServerPlugin()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        #endregion

        #region General Methods (not related to an OPC specification)

        //---------------------------------------------------------------------
        // OPC UA Server SDK .NET Interface 
        // (Called by the generic server)
        //---------------------------------------------------------------------

        /// <summary>
        /// This method is called from the generic server to get the license information. 
        /// </summary>
        /// <param name="productEdition">Product Edition</param>
        /// <param name="serialNumber">Serial Number</param>
        /// <remarks>Returning empty strings activates the evaluation version of the OPC UA Server SDK .NET. The evaluation allows the usage of the full product for 30 days.</remarks>
        public void OnGetLicenseInformation(out Opc.Ua.LicenseHandler.LicenseEdition productEdition, out string serialNumber)
        {
            productEdition = Opc.Ua.LicenseHandler.LicenseEdition.Evaluation;
            serialNumber = "";
        }

        /// <summary>This method is the first method called from the generic server at the startup.</summary>
        /// <param name="args">
        ///     String array with the command line parameters as they were specified when the server was being
        ///     started.
        /// </param>
        /// <remarks>
        ///     <para>The following command line parameters are handled by the generic server:</para>
        ///     <list type="bullet">
        ///         <item>
        ///             /silent<br />
        ///             No output is done during startup of the server
        ///         </item>
        ///         <item>
        ///             /configfile<br />
        ///             Allows the definition of a specific application configuration file
        ///         </item>
        ///         <item>
        ///             /install<br />
        ///             Installs and configures the application according the configuration file, e.g. certificates are created and
        ///             firewall configured.
        ///         </item>
        ///         <item>
        ///             /uninstall<br />
        ///             Uninstalls the application by removing the changes made during installation.
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <returns>
        ///     A <see cref="StatusCode" /> code with the result of the operation. Returning an error code stops the further
        ///     server execution.
        /// </returns>
        public StatusCode OnStartup(string[] args)
        {
            return StatusCodes.Good;
        }

        /// <summary>
        ///     Defines namespaces used by the application.
        /// </summary>
        /// <returns>Array of namespaces that are used by the application.</returns>
        public string[] OnGetNamespaceUris()
        {
            // set one namespace for the type model.
            var namespaceUrls = new string[1];
            namespaceUrls[0] = "http://technosoftware.com/SampleServer";
            return namespaceUrls;
        }

        /// <summary>This method is called after the node manager is initialized.</summary>
        /// <param name="opcServer">The generic server object. Used to call methods the generic server provides.</param>
        /// <param name="configuration">The application configuration</param>
        public void OnInitialized(IUaServer opcServer, ApplicationConfiguration configuration)
        {
            opcServer_ = opcServer;
        }

        /// <summary>
        ///     <para>
        ///         This method is called from the generic server at the startup; when the first client connects or the service is
        ///         started. All items supported by the server need to be defined by calling the methods provided by the
        ///         <see cref="IUaServer">IUaServer</see> interface for each item.
        ///     </para>
        /// </summary>
        /// <param name="externalReferences">The externalReferences allows the generic server to link to the general nodes.</param>
        /// <returns>The root folder.</returns>
        public NodeState OnCreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            dynamicNodes_ = new List<BaseDataVariableState>();
            IList<IReference> references;
            if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out references))
            {
                externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
            }

            FolderState root = opcServer_.CreateFolder(null, "/CTT", "CTT");
            references.Add(new NodeStateReference(ReferenceTypes.Organizes, false, root.NodeId));
            root.EventNotifier = EventNotifiers.SubscribeToEvents;
            opcServer_.AddRootNotifier(root);

            var variables = new List<BaseDataVariableState>();
            variables.Add(opcServer_.CreateBaseDataVariable(root, "/Trigger", "Trigger", BuiltInType.UInt16,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            variables[0].OnSimpleWriteValue = OnWriteTrigger;

            FolderState folder1 = opcServer_.CreateFolder(root, "/Static", "Static");
            FolderState folder2 = opcServer_.CreateFolder(folder1, "/Static/All Profiles/", "All Profiles");

            #region /Static/All Profiles/Scalar/

            // create a folder called "Scalar" and place it under the "Static/All Profiles" folder
            FolderState folder3 = opcServer_.CreateFolder(folder2, "/Static/All Profiles/Scalar", "Scalar");
            // create our Static Scalar nodes within this folder
            variables.Add(opcServer_.CreateBaseDataVariable(folder3, "/Static/All Profiles/Scalar/Double", "Double",
                BuiltInType.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder3, "/Static/All Profiles/Scalar/Boolean", "Boolean",
                BuiltInType.Boolean, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder3, "/Static/All Profiles/Scalar/Byte", "Byte",
                BuiltInType.Byte, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder3, "/Static/All Profiles/Scalar/DateTime", "DateTime",
                BuiltInType.DateTime, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder3, "/Static/All Profiles/Scalar/Float", "Float",
                BuiltInType.Float, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder3, "/Static/All Profiles/Scalar/Guid", "Guid",
                BuiltInType.Guid, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder3, "/Static/All Profiles/Scalar/Int16", "Int16",
                BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder3, "/Static/All Profiles/Scalar/Int32", "Int32",
                BuiltInType.Int32, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder3, "/Static/All Profiles/Scalar/Int64", "Int64",
                BuiltInType.Int64, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder3, "/Static/All Profiles/Scalar/String", "String",
                BuiltInType.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder3, "/Static/All Profiles/Scalar/SByte", "SByte",
                BuiltInType.SByte, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder3, "/Static/All Profiles/Scalar/ByteString",
                "ByteString", BuiltInType.ByteString, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder3, "/Static/All Profiles/Scalar/UInt16", "UInt16",
                BuiltInType.UInt16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder3, "/Static/All Profiles/Scalar/UInt32", "UInt32",
                BuiltInType.UInt32, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder3, "/Static/All Profiles/Scalar/UInt64", "UInt64",
                BuiltInType.UInt64, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder3, "/Static/All Profiles/Scalar/XmlElement",
                "XmlElement", BuiltInType.XmlElement, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder3, "/Static/All Profiles/Scalar/Numeric", "Number",
                BuiltInType.Number, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder3, "/Static/All Profiles/Scalar/Integer", "Integer",
                BuiltInType.Integer, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder3, "/Static/All Profiles/Scalar/UInteger", "UInteger",
                BuiltInType.UInteger, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));

            #endregion

            #region /Static/All Profiles/Structures

            // create a folder called "structures" under the "all profiles" folder
            FolderState structuresFolder = opcServer_.CreateFolder(folder2, "/Static/All Profiles/Structures",
                "Structures");
            // now create the variables beneath
            variables.Add(opcServer_.CreateBaseDataVariable(structuresFolder,
                "/Static/All Profiles/Structures/LocalizedText", "LocalizedText", BuiltInType.LocalizedText,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(structuresFolder,
                "/Static/All Profiles/Structures/QualifiedName", "QualifiedName", BuiltInType.QualifiedName,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));

            #endregion

            #region /Static/All Profiles/Access Right

            // create folder called "access right" under the "all profiles" folder
            FolderState accessRightFolder = opcServer_.CreateFolder(folder2, "/Static/All Profiles/Access Right",
                "Access Right");
            // create some nodes
            BaseDataVariableState arROnode = opcServer_.CreateBaseDataVariable(accessRightFolder,
                "/Static/All Profiles/Access Right/ReadOnly", "ReadOnly", BuiltInType.Int32, ValueRanks.Scalar,
                AccessLevels.CurrentReadOrWrite, null);
            arROnode.AccessLevel = AccessLevels.CurrentRead;
            arROnode.UserAccessLevel = AccessLevels.CurrentRead;
            variables.Add(arROnode);
            BaseDataVariableState arWOnode = opcServer_.CreateBaseDataVariable(accessRightFolder,
                "/Static/All Profiles/Access Right/WriteOnly", "WriteOnly", BuiltInType.Int32, ValueRanks.Scalar,
                AccessLevels.CurrentReadOrWrite, null);
            arWOnode.AccessLevel = AccessLevels.CurrentWrite;
            arWOnode.UserAccessLevel = AccessLevels.CurrentWrite;
            variables.Add(arWOnode);

            #endregion

            #region /Static/All Profiles/Scalar Mass/

            // create 100 instances of each static scalar type
            FolderState scalarMass = opcServer_.CreateFolder(folder2, "/Static/All Profiles/Scalar Mass", "Scalar Mass");
            variables.AddRange(CreateBaseDataVariables(scalarMass, "/Static/All Profiles/Scalar Mass/Boolean", "Boolean",
                BuiltInType.Boolean, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, 100));
            variables.AddRange(CreateBaseDataVariables(scalarMass, "/Static/All Profiles/Scalar Mass/Byte", "Byte",
                BuiltInType.Byte, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, 100));
            variables.AddRange(CreateBaseDataVariables(scalarMass, "/Static/All Profiles/Scalar Mass/ByteString",
                "ByteString", BuiltInType.ByteString, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, 100));
            variables.AddRange(CreateBaseDataVariables(scalarMass, "/Static/All Profiles/Scalar Mass/DateTime",
                "DateTime", BuiltInType.DateTime, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, 100));
            variables.AddRange(CreateBaseDataVariables(scalarMass, "/Static/All Profiles/Scalar Mass/Float", "Float",
                BuiltInType.Float, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, 100));
            variables.AddRange(CreateBaseDataVariables(scalarMass, "/Static/All Profiles/Scalar Mass/Guid", "Guid",
                BuiltInType.Guid, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, 100));
            variables.AddRange(CreateBaseDataVariables(scalarMass, "/Static/All Profiles/Scalar Mass/Int16", "Int16",
                BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, 100));
            variables.AddRange(CreateBaseDataVariables(scalarMass, "/Static/All Profiles/Scalar Mass/Int32", "Int32",
                BuiltInType.Int32, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, 100));
            variables.AddRange(CreateBaseDataVariables(scalarMass, "/Static/All Profiles/Scalar Mass/Int64", "Int64",
                BuiltInType.Int64, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, 100));
            variables.AddRange(CreateBaseDataVariables(scalarMass, "/Static/All Profiles/Scalar Mass/String", "String",
                BuiltInType.String, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, 100));
            variables.AddRange(CreateBaseDataVariables(scalarMass, "/Static/All Profiles/Scalar Mass/SByte", "SByte",
                BuiltInType.SByte, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, 100));
            variables.AddRange(CreateBaseDataVariables(scalarMass, "/Static/All Profiles/Scalar Mass/UInt16", "UInt16",
                BuiltInType.UInt16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, 100));
            variables.AddRange(CreateBaseDataVariables(scalarMass, "/Static/All Profiles/Scalar Mass/UInt32", "UInt32",
                BuiltInType.UInt32, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, 100));
            variables.AddRange(CreateBaseDataVariables(scalarMass, "/Static/All Profiles/Scalar Mass/UInt64", "UInt64",
                BuiltInType.UInt64, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, 100));
            variables.AddRange(CreateBaseDataVariables(scalarMass, "/Static/All Profiles/Scalar Mass/XmlElement",
                "XmlElement", BuiltInType.XmlElement, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, 100));

            #endregion

            #region /Static/All Profiles/Arrays

            // create the folder "Arrays"
            FolderState folder5 = opcServer_.CreateFolder(folder2, "/Static/All Profiles/Arrays", "Arrays");
            // create our scalar array types in this folder
            variables.Add(opcServer_.CreateBaseDataVariable(folder5, "/Static/All Profiles/Arrays/Double", "Double",
                BuiltInType.Double, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder5, "/Static/All Profiles/Arrays/Boolean", "Boolean",
                BuiltInType.Boolean, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder5, "/Static/All Profiles/Arrays/Byte", "Byte",
                BuiltInType.Byte, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder5, "/Static/All Profiles/Arrays/DateTime", "DateTime",
                BuiltInType.DateTime, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder5, "/Static/All Profiles/Arrays/Float", "Float",
                BuiltInType.Float, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder5, "/Static/All Profiles/Arrays/Guid", "Guid",
                BuiltInType.Guid, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder5, "/Static/All Profiles/Arrays/Int16", "Int16",
                BuiltInType.Int16, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder5, "/Static/All Profiles/Arrays/Int32", "Int32",
                BuiltInType.Int32, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder5, "/Static/All Profiles/Arrays/Int64", "Int64",
                BuiltInType.Int64, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder5, "/Static/All Profiles/Arrays/String", "String",
                BuiltInType.String, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder5, "/Static/All Profiles/Arrays/SByte", "SByte",
                BuiltInType.SByte, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder5, "/Static/All Profiles/Arrays/ByteString",
                "ByteString", BuiltInType.ByteString, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder5, "/Static/All Profiles/Arrays/UInt16", "UInt16",
                BuiltInType.UInt16, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder5, "/Static/All Profiles/Arrays/UInt32", "UInt32",
                BuiltInType.UInt32, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder5, "/Static/All Profiles/Arrays/UInt64", "UInt64",
                BuiltInType.UInt64, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder5, "/Static/All Profiles/Arrays/XmlElement",
                "XmlElement", BuiltInType.XmlElement, ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, null));

            #endregion

            #region /Static/All Profiles/Scalar Set 1/

            // create a folder "Scalar Set 1"
            FolderState folder6 = opcServer_.CreateFolder(folder2, "/Static/All Profiles/Scalar Set 1", "Scalar Set 1");
            // store the nodes within this folder
            int nodeId1Pos = variables.Count;
            variables.Add(opcServer_.CreateBaseDataVariable(folder6, "/Static/All Profiles/Scalar Set 1/NodeId1",
                "NodeId1", BuiltInType.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder6, "/Static/All Profiles/Scalar Set 1/NodeId2",
                "NodeId2", BuiltInType.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder6, "/Static/All Profiles/Scalar Set 1/NodeId3",
                "NodeId3", BuiltInType.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            variables.Add(opcServer_.CreateBaseDataVariable(folder6, "/Static/All Profiles/Scalar Set 1/NodeId4",
                "NodeId4", BuiltInType.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null));
            opcServer_.CreateMeshVariable(folder6, "/Static/All Profiles/Scalar Set 1/NodeId5", "NodeId5",
                BuiltInType.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, variables[nodeId1Pos],
                variables[nodeId1Pos + 1], variables[nodeId1Pos + 2], variables[nodeId1Pos + 3]);

            #endregion

            FolderState folder07 = opcServer_.CreateFolder(folder1, "/Static/DA Profile/", "DA Profile");

            #region /Static/DA Profile/DataItem

            FolderState folder08 = opcServer_.CreateFolder(folder07, "/Static/DA Profile/DataItem", "DataItem");

            opcServer_.CreateDataItem(folder08, "/Static/DA Profile/DataItem/SByte", "SByte", BuiltInType.SByte,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, 2);
            opcServer_.CreateDataItem(folder08, "/Static/DA Profile/DataItem/Byte", "Byte", BuiltInType.Byte,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, 2);
            BaseDataVariableState parentType1 = opcServer_.CreateDataItem(folder08, "/Static/DA Profile/DataItem/Int16",
                "Int16", BuiltInType.Int16, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, 2);
            opcServer_.CreateDataItem(folder08, "/Static/DA Profile/DataItem/UInt16", "UInt16", BuiltInType.UInt16,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, 2);
            opcServer_.CreateDataItem(folder08, "/Static/DA Profile/DataItem/Int32", "Int32", BuiltInType.Int32,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, 2);
            opcServer_.CreateDataItem(folder08, "/Static/DA Profile/DataItem/UInt32", "UInt32", BuiltInType.UInt32,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, 2);
            opcServer_.CreateDataItem(folder08, "/Static/DA Profile/DataItem/Int64", "Int64", BuiltInType.Int64,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, 2);
            opcServer_.CreateDataItem(folder08, "/Static/DA Profile/DataItem/UInt64", "UInt64", BuiltInType.UInt64,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, 2);
            opcServer_.CreateDataItem(folder08, "/Static/DA Profile/DataItem/Double", "Double", BuiltInType.Double,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, 2);
            opcServer_.CreateDataItem(folder08, "/Static/DA Profile/DataItem/Float", "Float", BuiltInType.Float,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, 2);
            opcServer_.CreateDataItem(folder08, "/Static/DA Profile/DataItem/String", "String", BuiltInType.String,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, 2);
            opcServer_.CreateDataItem(folder08, "/Static/DA Profile/DataItem/DateTime", "DateTime", BuiltInType.DateTime,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, 2);

            #endregion

            #region /Static/DA Profile/AnalogType

            FolderState folder09 = opcServer_.CreateFolder(folder07, "/Static/DA Profile/AnalogType", "AnalogType");

            opcServer_.CreateAnalogItem(folder09, "/Static/DA Profile/AnalogType/SByte", "SByte", BuiltInType.SByte,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, 2, null, null, null);
            opcServer_.CreateAnalogItem(folder09, "/Static/DA Profile/AnalogType/Byte", "Byte", BuiltInType.Byte,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, 2, null, null, null);
            BaseDataVariableState parentType2 = opcServer_.CreateAnalogItem(folder09,
                "/Static/DA Profile/AnalogType/Int16", "Int16", BuiltInType.Int16, ValueRanks.Scalar,
                AccessLevels.CurrentReadOrWrite, null, 2, null, null, null);
            opcServer_.CreateAnalogItem(folder09, "/Static/DA Profile/AnalogType/UInt16", "UInt16", BuiltInType.UInt16,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, 2, null, null, null);
            opcServer_.CreateAnalogItem(folder09, "/Static/DA Profile/AnalogType/Int32", "Int32", BuiltInType.Int32,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, 2, null, null, null);
            opcServer_.CreateAnalogItem(folder09, "/Static/DA Profile/AnalogType/UInt32", "UInt32", BuiltInType.UInt32,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, 2, null, null, null);
            opcServer_.CreateAnalogItem(folder09, "/Static/DA Profile/AnalogType/Int64", "Int64", BuiltInType.Int64,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, 2, null, null, null);
            opcServer_.CreateAnalogItem(folder09, "/Static/DA Profile/AnalogType/UInt64", "UInt64", BuiltInType.UInt64,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, 2, null, null, null);
            opcServer_.CreateAnalogItem(folder09, "/Static/DA Profile/AnalogType/Double", "Double", BuiltInType.Double,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, 2, null, new Range(102, -1), null);
            opcServer_.CreateAnalogItem(folder09, "/Static/DA Profile/AnalogType/Float", "Float", BuiltInType.Float,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, 2, null, null, null);

            #endregion

            #region /Static/DA Profile/AnalogType Arrays

            const string pathAtArrays = "/Static/DA Profile/AnalogType Arrays";
            FolderState folderAnalogTypeArrays = opcServer_.CreateFolder(folder07, pathAtArrays, "AnalogType Arrays");

            opcServer_.CreateAnalogItem(folderAnalogTypeArrays, pathAtArrays + "/Int16", "Int16", BuiltInType.Int16,
                ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, new Int16[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 2,
                null, null, null);
            opcServer_.CreateAnalogItem(folderAnalogTypeArrays, pathAtArrays + "/Int32", "Int32", BuiltInType.Int32,
                ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, new[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 },
                2, null, null, null);
            opcServer_.CreateAnalogItem(folderAnalogTypeArrays, pathAtArrays + "/UInt16", "UInt16", BuiltInType.UInt16,
                ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite,
                new UInt16[] { 20, 21, 22, 23, 24, 25, 26, 27, 28, 29 }, 2, null, null, null);
            opcServer_.CreateAnalogItem(folderAnalogTypeArrays, pathAtArrays + "/UInt32", "UInt32", BuiltInType.UInt32,
                ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite,
                new UInt32[] { 30, 31, 32, 33, 34, 35, 36, 37, 38, 39 }, 2, null, null, null);
            opcServer_.CreateAnalogItem(folderAnalogTypeArrays, pathAtArrays + "/Float", "Float", BuiltInType.Float,
                ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite,
                new[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 1.1f, 2.2f, 3.3f, 4.4f, 5.5f }, 2, null, null, null);
            opcServer_.CreateAnalogItem(folderAnalogTypeArrays, pathAtArrays + "/Double", "Double", BuiltInType.Double,
                ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite,
                new[] { 9.00001d, 9.0002d, 9.003d, 9.04d, 9.5d, 9.06d, 9.007d, 9.008d, 9.0009d }, 2, null, null, null);

            #endregion

            #region /Static/DA Profile/DiscreteType

            FolderState folder10 = opcServer_.CreateFolder(folder07, "/Static/DA Profile/DiscreteType", "DiscreteType");
            BaseDataVariableState parentType3 = opcServer_.CreateMultiStateDiscreteItem(folder10,
                "/Static/DA Profile/DiscreteType/MultiState1", "MultiState1", AccessLevels.CurrentReadOrWrite, null,
                "red", "green", "blue");
            opcServer_.CreateMultiStateDiscreteItem(folder10, "/Static/DA Profile/DiscreteType/MultiState2",
                "MultiState2", AccessLevels.CurrentReadOrWrite, null, "open", "close", "ajar");
            opcServer_.CreateMultiStateDiscreteItem(folder10, "/Static/DA Profile/DiscreteType/MultiState3",
                "MultiState3", AccessLevels.CurrentReadOrWrite, null, "up", "down", "level");
            opcServer_.CreateMultiStateDiscreteItem(folder10, "/Static/DA Profile/DiscreteType/MultiState4",
                "MultiState4", AccessLevels.CurrentReadOrWrite, null, "left", "right", "center");
            opcServer_.CreateMultiStateDiscreteItem(folder10, "/Static/DA Profile/DiscreteType/MultiState5",
                "MultiState5", AccessLevels.CurrentReadOrWrite, null, "circle", "cross", "triangle");
            opcServer_.CreateMultiStateDiscreteItem(folder10, "/Static/DA Profile/DiscreteType/TwoState1", "TwoState1",
                AccessLevels.CurrentReadOrWrite, null, "red", "blue");
            opcServer_.CreateMultiStateDiscreteItem(folder10, "/Static/DA Profile/DiscreteType/TwoState2", "TwoState2",
                AccessLevels.CurrentReadOrWrite, null, "open", "close");
            opcServer_.CreateMultiStateDiscreteItem(folder10, "/Static/DA Profile/DiscreteType/TwoState3", "TwoState3",
                AccessLevels.CurrentReadOrWrite, null, "up", "down");
            opcServer_.CreateMultiStateDiscreteItem(folder10, "/Static/DA Profile/DiscreteType/TwoState4", "TwoState4",
                AccessLevels.CurrentReadOrWrite, null, "left", "right");
            opcServer_.CreateMultiStateDiscreteItem(folder10, "/Static/DA Profile/DiscreteType/TwoState5", "TwoState5",
                AccessLevels.CurrentReadOrWrite, null, "circle", "cross");

            #endregion

            #region /Static/DA Profile/Deadband

            FolderState folder11 = opcServer_.CreateFolder(folder07, "/Static/DA Profile/Deadband", "Deadband");

            opcServer_.CreateAnalogItem(folder11, "/Static/DA Profile/Deadband/Int16", "Int16", BuiltInType.Int16,
                ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, new Int16[] { 5, 20, 50, 90, 95 }, 2, null, null,
                null);
            opcServer_.CreateAnalogItem(folder11, "/Static/DA Profile/Deadband/UInt16", "UInt16", BuiltInType.UInt16,
                ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, new UInt16[] { 1, 2, 3, 88, 99 }, 2, null, null,
                null);
            opcServer_.CreateAnalogItem(folder11, "/Static/DA Profile/Deadband/Int32", "Int32", BuiltInType.Int32,
                ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, new[] { 10, 20, 30, 40, 50 }, 2, null, null,
                null);
            opcServer_.CreateAnalogItem(folder11, "/Static/DA Profile/Deadband/UInt32", "UInt32", BuiltInType.UInt32,
                ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, new UInt32[] { 5, 6, 7, 66, 77 }, 2, null, null,
                null);
            opcServer_.CreateAnalogItem(folder11, "/Static/DA Profile/Deadband/Float", "Float", BuiltInType.Float,
                ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, new[] { 0.01f, 1.23f, 3.45f, 76.54f, 99.875f },
                2, null, null, null);
            opcServer_.CreateAnalogItem(folder11, "/Static/DA Profile/Deadband/Double", "Double", BuiltInType.Double,
                ValueRanks.OneDimension, AccessLevels.CurrentReadOrWrite, new[] { 0, 1.23d, 2.34d, 6.78d, 7.89d }, 2, null,
                new Range(101, -1), null);

            #endregion

            FolderState folder1a = opcServer_.CreateFolder(root, "/Dynamic", "Dynamic");
            FolderState folder2a = opcServer_.CreateFolder(folder1a, "/Dynamic/All Profiles/", "All Profiles");

            #region /Dynamic/All Profiles/Scalar

            // create a folder for our dynamic scalar nodes
            FolderState folder3a = opcServer_.CreateFolder(folder2a, "/Dynamic/All Profiles/Scalar", "Scalar");
            // put our scalar nodes within this folder, these are dynamic, i.e. will change values automatically
            CreateDynamicVariable(folder3a, "/Dynamic/All Profiles/Scalar/NodeId1", "NodeId1", BuiltInType.Double,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite);
            CreateDynamicVariable(folder3a, "/Dynamic/All Profiles/Scalar/NodeId2", "NodeId2", BuiltInType.Int32,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite);
            CreateDynamicVariable(folder3a, "/Dynamic/All Profiles/Scalar/Double", "Double", BuiltInType.Double,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite);
            CreateDynamicVariable(folder3a, "/Dynamic/All Profiles/Scalar/Boolean", "Boolean", BuiltInType.Boolean,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite);
            CreateDynamicVariable(folder3a, "/Dynamic/All Profiles/Scalar/Byte", "Byte", BuiltInType.Byte,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite);
            CreateDynamicVariable(folder3a, "/Dynamic/All Profiles/Scalar/DateTime", "DateTime", BuiltInType.DateTime,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite);
            CreateDynamicVariable(folder3a, "/Dynamic/All Profiles/Scalar/Float", "Float", BuiltInType.Float,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite);
            CreateDynamicVariable(folder3a, "/Dynamic/All Profiles/Scalar/Guid", "Guid", BuiltInType.Guid,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite);
            CreateDynamicVariable(folder3a, "/Dynamic/All Profiles/Scalar/Int16", "Int16", BuiltInType.Int16,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite);
            CreateDynamicVariable(folder3a, "/Dynamic/All Profiles/Scalar/Int32", "Int32", BuiltInType.Int32,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite);
            CreateDynamicVariable(folder3a, "/Dynamic/All Profiles/Scalar/Int64", "Int64", BuiltInType.Int64,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite);
            CreateDynamicVariable(folder3a, "/Dynamic/All Profiles/Scalar/String", "String", BuiltInType.String,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite);
            CreateDynamicVariable(folder3a, "/Dynamic/All Profiles/Scalar/SByte", "SByte", BuiltInType.SByte,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite);
            CreateDynamicVariable(folder3a, "/Dynamic/All Profiles/Scalar/ByteString", "ByteString",
                BuiltInType.ByteString, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite);
            CreateDynamicVariable(folder3a, "/Dynamic/All Profiles/Scalar/UInt16", "UInt16", BuiltInType.UInt16,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite);
            CreateDynamicVariable(folder3a, "/Dynamic/All Profiles/Scalar/UInt32", "UInt32", BuiltInType.UInt32,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite);
            CreateDynamicVariable(folder3a, "/Dynamic/All Profiles/Scalar/UInt64", "UInt64", BuiltInType.UInt64,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite);
            CreateDynamicVariable(folder3a, "/Dynamic/All Profiles/Scalar/XmlElement", "XmlElement",
                BuiltInType.XmlElement, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite);

            #endregion

            #region /Dynamic/All Profiles/Scalar Mass/

            // create 100 instances of each dynamic scalar type
            // create our folder
            FolderState dynamicScalarMass = opcServer_.CreateFolder(folder2a, "/Dynamic/All Profiles/Scalar Mass",
                "Scalar Mass");
            // create our nodes within this folder.
            // HOW THIS WORKS:
            // 1. a folder will be created of the named type, e.g. "Boolean"
            // 2. within the newly created folder a specified number of nodes will be created of that type
            CreateDynamicVariables(dynamicScalarMass, "/Dynamic/All Profiles/Scalar Mass/Boolean", "Boolean",
                BuiltInType.Boolean, ValueRanks.Scalar, 100);
            CreateDynamicVariables(dynamicScalarMass, "/Dynamic/All Profiles/Scalar Mass/Byte", "Byte", BuiltInType.Byte,
                ValueRanks.Scalar, 100);
            CreateDynamicVariables(dynamicScalarMass, "/Dynamic/All Profiles/Scalar Mass/ByteString", "ByteString",
                BuiltInType.ByteString, ValueRanks.Scalar, 100);
            CreateDynamicVariables(dynamicScalarMass, "/Dynamic/All Profiles/Scalar Mass/DateTime", "DateTime",
                BuiltInType.DateTime, ValueRanks.Scalar, 100);
            CreateDynamicVariables(dynamicScalarMass, "/Dynamic/All Profiles/Scalar Mass/Float", "Float",
                BuiltInType.Float, ValueRanks.Scalar, 100);
            CreateDynamicVariables(dynamicScalarMass, "/Dynamic/All Profiles/Scalar Mass/Guid", "Guid", BuiltInType.Guid,
                ValueRanks.Scalar, 100);
            CreateDynamicVariables(dynamicScalarMass, "/Dynamic/All Profiles/Scalar Mass/Int16", "Int16",
                BuiltInType.Int16, ValueRanks.Scalar, 100);
            CreateDynamicVariables(dynamicScalarMass, "/Dynamic/All Profiles/Scalar Mass/Int32", "Int32",
                BuiltInType.Int32, ValueRanks.Scalar, 100);
            CreateDynamicVariables(dynamicScalarMass, "/Dynamic/All Profiles/Scalar Mass/Int64", "Int64",
                BuiltInType.Int64, ValueRanks.Scalar, 100);
            CreateDynamicVariables(dynamicScalarMass, "/Dynamic/All Profiles/Scalar Mass/String", "String",
                BuiltInType.String, ValueRanks.Scalar, 100);
            CreateDynamicVariables(dynamicScalarMass, "/Dynamic/All Profiles/Scalar Mass/SByte", "SByte",
                BuiltInType.SByte, ValueRanks.Scalar, 100);
            CreateDynamicVariables(dynamicScalarMass, "/Dynamic/All Profiles/Scalar Mass/UInt16", "UInt16",
                BuiltInType.UInt16, ValueRanks.Scalar, 100);
            CreateDynamicVariables(dynamicScalarMass, "/Dynamic/All Profiles/Scalar Mass/UInt32", "UInt32",
                BuiltInType.UInt32, ValueRanks.Scalar, 100);
            CreateDynamicVariables(dynamicScalarMass, "/Dynamic/All Profiles/Scalar Mass/UInt64", "UInt64",
                BuiltInType.UInt64, ValueRanks.Scalar, 100);
            CreateDynamicVariables(dynamicScalarMass, "/Dynamic/All Profiles/Scalar Mass/XmlElement", "XmlElement",
                BuiltInType.XmlElement, ValueRanks.Scalar, 100);

            #endregion

            #region /Dynamic/All Profiles/Scalar Set 1/

            // create our folder
            FolderState folder6a = opcServer_.CreateFolder(folder2a, "/Dynamic/All Profiles/Scalar Set 1",
                "Scalar Set 1");
            // add the nodes to the new folder
            CreateDynamicVariable(folder6a, "/Dynamic/All Profiles/Scalar Set 1/NodeId1", "NodeId1", BuiltInType.Double,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite);
            CreateDynamicVariable(folder6a, "/Dynamic/All Profiles/Scalar Set 1/NodeId2", "NodeId2", BuiltInType.Double,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite);
            CreateDynamicVariable(folder6a, "/Dynamic/All Profiles/Scalar Set 1/NodeId3", "NodeId3", BuiltInType.Double,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite);
            CreateDynamicVariable(folder6a, "/Dynamic/All Profiles/Scalar Set 1/NodeId4", "NodeId4", BuiltInType.Double,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite);
            CreateDynamicVariable(folder6a, "/Dynamic/All Profiles/Scalar Set 1/NodeId5", "NodeId5", BuiltInType.Double,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite);

            #endregion

            #region /NodeClasses

            FolderState folder4 = opcServer_.CreateFolder(root, "/NodeClasses", "NodeClasses");
            opcServer_.CreateBaseObject(folder4, "/NodeClasses/Object1", "Object1");
            opcServer_.CreateBaseObject(folder4, "/NodeClasses/Object2", "Object2");
            opcServer_.CreateBaseObject(folder4, "/NodeClasses/Object3", "Object3");
            opcServer_.CreateMethod(folder4, "/NodeClasses/Method1", "Method1", null);
            opcServer_.CreateMethod(folder4, "/NodeClasses/Method2", "Method2", null);
            opcServer_.CreateMethod(folder4, "/NodeClasses/Method3", "Method3", null);
            opcServer_.CreateBaseDataVariable(folder4, "/NodeClasses/Variable1", "Variable1", BuiltInType.Double,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
            opcServer_.CreateBaseDataVariable(folder4, "/NodeClasses/Variable2", "Variable2", BuiltInType.Double,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
            opcServer_.CreateBaseDataVariable(folder4, "/NodeClasses/Variable3", "Variable3", BuiltInType.Double,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
            opcServer_.CreateBaseObjectType(folder4, externalReferences, "/NodeClasses/ObjectType1", "ObjectType1");
            opcServer_.CreateBaseObjectType(folder4, externalReferences, "/NodeClasses/ObjectType2", "ObjectType2");
            opcServer_.CreateBaseObjectType(folder4, externalReferences, "/NodeClasses/ObjectType3", "ObjectType3");
            opcServer_.CreateBaseVariableType(folder4, externalReferences, "/NodeClasses/VariableType1", "VariableType1",
                BuiltInType.Boolean, ValueRanks.Scalar);
            opcServer_.CreateBaseVariableType(folder4, externalReferences, "/NodeClasses/VariableType2", "VariableType2",
                BuiltInType.Boolean, ValueRanks.Scalar);
            opcServer_.CreateBaseVariableType(folder4, externalReferences, "/NodeClasses/VariableType3", "VariableType3",
                BuiltInType.Boolean, ValueRanks.Scalar);
            opcServer_.CreateReferenceType(folder4, externalReferences, "/NodeClasses/ReferenceType1", "ReferenceType1");
            opcServer_.CreateReferenceType(folder4, externalReferences, "/NodeClasses/ReferenceType2", "ReferenceType2");
            opcServer_.CreateReferenceType(folder4, externalReferences, "/NodeClasses/ReferenceType3", "ReferenceType3");
            opcServer_.CreateDataType(folder4, externalReferences, "/NodeClasses/DataType1", "DataType1");
            opcServer_.CreateDataType(folder4, externalReferences, "/NodeClasses/DataType2", "DataType2");
            opcServer_.CreateDataType(folder4, externalReferences, "/NodeClasses/DataType3", "DataType3");
            opcServer_.CreateView(folder4, externalReferences, "/NodeClasses/View1", "View1");
            opcServer_.CreateView(folder4, externalReferences, "/NodeClasses/View2", "View2");
            opcServer_.CreateView(folder4, externalReferences, "/NodeClasses/View3", "View3");

            #endregion

            #region NodeIds > References

            FolderState referencesFolder = opcServer_.CreateFolder(root, "/References", "References");
            BaseDataVariableState has3ForwardReferences1 = opcServer_.CreateMeshVariable(referencesFolder,
                "/References/Has 3 Forward References 1", "Has 3 Forward References 1", BuiltInType.Double,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
            has3ForwardReferences1.AddReference(ReferenceTypes.HasCause, false, variables[0].NodeId);
            BaseDataVariableState has3ForwardReferences2 = opcServer_.CreateMeshVariable(referencesFolder,
                "/References/Has 3 Forward References 2", "Has 3 Forward References 2", BuiltInType.Double,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
            has3ForwardReferences2.AddReference(ReferenceTypes.HasCause, false, variables[0].NodeId);
            BaseDataVariableState has3ForwardReferences3 = opcServer_.CreateMeshVariable(referencesFolder,
                "/References/Has 3 Forward References 3", "Has 3 Forward References 3", BuiltInType.Double,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
            has3ForwardReferences3.AddReference(ReferenceTypes.HasCause, false, variables[0].NodeId);
            BaseDataVariableState has3ForwardReferences4 = opcServer_.CreateMeshVariable(referencesFolder,
                "/References/Has 3 Forward References 4", "Has 3 Forward References 4", BuiltInType.Double,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
            has3ForwardReferences4.AddReference(ReferenceTypes.HasCause, false, variables[0].NodeId);
            BaseDataVariableState has3ForwardReferences5 = opcServer_.CreateMeshVariable(referencesFolder,
                "/References/Has 3 Forward References 5", "Has 3 Forward References 5", BuiltInType.Double,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
            has3ForwardReferences5.AddReference(ReferenceTypes.HasCause, false, variables[0].NodeId);
            BaseDataVariableState hasInverseAndForwardReferences = opcServer_.CreateMeshVariable(referencesFolder,
                "/References/Has Inverse and Forward References", "Has Inverse and Forward References",
                BuiltInType.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null, has3ForwardReferences1,
                has3ForwardReferences2, has3ForwardReferences3, has3ForwardReferences4, has3ForwardReferences5);
            BaseDataVariableState hasReferencesWithDiffParentTypes = opcServer_.CreateMeshVariable(referencesFolder,
                "/References/Has References With Different Parent Types", "Has References With Different Parent Types",
                BuiltInType.Double, ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
            hasReferencesWithDiffParentTypes.AddReference(ReferenceTypes.HasChild, true, parentType1.NodeId);
            hasReferencesWithDiffParentTypes.AddReference(ReferenceTypes.HasChild, true, parentType2.NodeId);
            hasReferencesWithDiffParentTypes.AddReference(ReferenceTypes.HasChild, true, parentType3.NodeId);
            BaseDataVariableState has3InverseReferences1 = opcServer_.CreateMeshVariable(referencesFolder,
                "/References/Has 3 Inverse References 1", "Has 3 Inverse References 1", BuiltInType.Double,
                ValueRanks.Scalar, AccessLevels.CurrentReadOrWrite, null);
            has3InverseReferences1.AddReference(ReferenceTypes.HasEffect, true, variables[0].NodeId);
            has3InverseReferences1.AddReference(ReferenceTypes.HasEffect, true, variables[1].NodeId);
            has3InverseReferences1.AddReference(ReferenceTypes.HasEffect, true, variables[2].NodeId);

            #endregion

            #region Dynamic adding/deleting of nodes

            FolderState folderDynamic1 = opcServer_.CreateFolder(root, "/Analog", "Analog");
            FolderState folderDynamic2 = opcServer_.CreateFolder(root, "/Discrete", "Discrete");

            #region AnalogItem's

            opcServer_.CreateAnalogItem(folderDynamic1, "/Analog/Item1", "Item1", BuiltInType.Int64, ValueRanks.Scalar,
                AccessLevels.CurrentReadOrWrite, null, 2, null, null, null);
            opcServer_.CreateAnalogItem(folderDynamic1, "/Analog/Item2", "Item2", BuiltInType.Double, ValueRanks.Scalar,
                AccessLevels.CurrentReadOrWrite, null, 2, null, new Range(102, -1), null);
            opcServer_.CreateAnalogItem(folderDynamic1, "/Analog/Item3", "Item3", BuiltInType.Float, ValueRanks.Scalar,
                AccessLevels.CurrentReadOrWrite, null, 2, null, null, null);

            #endregion

            #region DiscreteItem's

            opcServer_.CreateMultiStateDiscreteItem(folderDynamic2, "/Discrete/Sensor1", "Sensor1",
                AccessLevels.CurrentReadOrWrite, null, "left", "right", "center");
            opcServer_.CreateTwoStateDiscreteItem(folderDynamic2, "/Discrete/Sensor2", "Sensor2",
                AccessLevels.CurrentReadOrWrite, false, "open", "close");

            #endregion

            #endregion

            simulationTimer_ = new Timer(DoSimulation, null, 1000, 1000);

            return root;
        }

        /// <summary>
        ///     This method is called from the generic server when the server was successfully started and is running.
        /// </summary>
        /// <remarks>This method should only return if the server should be stopped.</remarks>
        /// <returns>
        ///     A <see cref="StatusCode" /> code with the result of the operation. Returning from this method stops the
        ///     further server execution.
        /// </returns>
        public StatusCode OnRunning()
        {
            // Initialize the user interface.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // run the application interactively.
            Application.Run(new ServerForm(opcServer_));

            return StatusCodes.Good;
        }

        /// <summary>
        ///     This method is called from the generic server when a Shutdown is executed.
        ///     To ensure proper process shutdown, any communication channels should be closed and all threads terminated before
        ///     this method returns.
        /// </summary>
        /// <param name="opcServerState">The current state of the server.</param>
        /// <param name="reason">The reason why the server shutdowns.</param>
        /// <param name="exception">The exception which caused an error. null if not an exception.</param>
        /// <returns>A <see cref="StatusCode" /> code with the result of the operation.</returns>
        public StatusCode OnShutdown(ServerState opcServerState, string reason, Exception exception)
        {
            return StatusCodes.Good;
        }

        /// <summary>The OPC Server Properties of the current server instance.</summary>
        /// <returns>A <see cref="ServerProperties" /> object.</returns>
        public ServerProperties OnGetServerProperties()
        {
            var properties = new ServerProperties
            {
                ManufacturerName = "Technosoftware GmbH",
                ProductName = "OPC UA Server SDK .NET Sample Server",
                ProductUri = "http://technosoftware.com/TechnosoftwareSampleServer/v1.0",
                SoftwareVersion = GetAssemblySoftwareVersion(),
                BuildNumber = GetAssemblyBuildNumber(),
                BuildDate = GetAssemblyTimestamp()
            };

            return properties;
        }

        #endregion

        #region Core Server Facet

        //---------------------------------------------------------------------
        // OPC UA Server SDK .NET Interface 
        // (Called by the generic server)
        //---------------------------------------------------------------------

        /// <summary>
        ///     This method is called when a client executes a 'write' server call. The <see cref="BaseDataVariableState" />
        ///     includes all information required to identify the item
        ///     as well as original (unmodified) value, timestamp and quality. Depending on the returned result code the cache is
        ///     updated or not in the generic server after returning from this method.
        /// </summary>
        /// <param name="originalVariableState">
        ///     The <see cref="BaseVariableState" /> of the variable including the original state
        ///     of the variable. Can be used to check what changes happens
        /// </param>
        /// <param name="value">
        ///     The value which should be written. The returned value is used for updating the cache depending on
        ///     the returned result code.
        /// </param>
        /// <param name="statusCode">A <see cref="StatusCode" /> code which should be used as new status code for the value.</param>
        /// <param name="timestamp">
        ///     The timestamp the value was written. The returned value is used for updating the cache
        ///     depending on the returned result code.
        /// </param>
        /// <returns>A <see cref="StatusCode" /> code with the result of the operation.</returns>
        /// <remarks>
        ///     <para>Rules for updating the cache:</para>
        ///     <list type="number">
        ///         <item>
        ///             If the returned <see cref="StatusCode" /> is Bad (something like Bad...) the cache is not updated with
        ///             timestamp and value.
        ///         </item>
        ///         <item>
        ///             If the returned <see cref="StatusCode" /> is
        ///             <see cref="StatusCodes.GoodCompletesAsynchronously">OpcStatusCodes.GoodCompletesAsynchronously</see> the
        ///             cache is not updated with
        ///             timestamp and value. After the customization DLL has finished its operation it can use
        ///             <see cref="IUaServer.WriteBaseVariable">WriteBaseVariable</see> to update the cache.
        ///         </item>
        ///         <item>In all other cases the cache is updated with timestamp and value.</item>
        ///         <item>In all cases the status code is updated with the status code set in the 'statusCode' parameter.</item>
        ///     </list>
        /// </remarks>
        public StatusCode OnWriteBaseVariable(BaseVariableState originalVariableState, ref object value,
            ref StatusCode statusCode, ref DateTime timestamp)
        {
            if (timestamp == DateTime.MinValue)
            {
                timestamp = DateTime.UtcNow;
            }
            return StatusCodes.Good;
        }

        private ServiceResult OnWriteTrigger(ISystemContext context, NodeState node, ref object value)
        {
            return ServiceResult.Good;
        }

        #endregion

        #region Helper Methods

        private BaseDataVariableState[] CreateBaseDataVariables(NodeState parent, string path, string name,
            BuiltInType dataType, int valueRank, byte accessLevel, UInt16 numVariables)
        {
            // first, create a new Parent folder for this data-type
            FolderState newParentFolder = opcServer_.CreateFolder(parent, path, name);

            var itemsCreated = new List<BaseDataVariableState>();
            // now to create the remaining NUMBERED items
            for (uint i = 0; i < numVariables; i++)
            {
                string newName = string.Format("{0}{1}", name, i.ToString("000"));
                string newPath = string.Format("{0}/{1}", path, newName);
                itemsCreated.Add(opcServer_.CreateBaseDataVariable(newParentFolder, newPath, newName, dataType,
                    valueRank, accessLevel, null));
            } //for i
            return (itemsCreated.ToArray());
        }

        /// <summary>
        ///     Creates a new variable and adds it to the list of dynamic variables
        /// </summary>
        private BaseDataVariableState CreateDynamicVariable(FolderState parent, string path, string name,
            BuiltInType dataType, int valueRank, byte accessLevel)
        {
            BaseDataVariableState deviceItem = opcServer_.CreateBaseDataVariable(parent, path, name, dataType,
                valueRank, accessLevel, null);
            dynamicNodes_.Add(deviceItem);
            return deviceItem;
        }

        private BaseDataVariableState[] CreateDynamicVariables(NodeState parent, string path, string name,
            BuiltInType dataType, int valueRank, uint numVariables)
        {
            // first, create a new Parent folder for this data-type
            FolderState newParentFolder = opcServer_.CreateFolder(parent, path, name);

            var itemsCreated = new List<BaseDataVariableState>();
            // now to create the remaining NUMBERED items
            for (uint i = 0; i < numVariables; i++)
            {
                string newName = string.Format("{0}{1}", name, i.ToString("000"));
                string newPath = string.Format("{0}/{1}", path, newName);
                itemsCreated.Add(CreateDynamicVariable(newParentFolder, newPath, newName, dataType, valueRank,
                    AccessLevels.CurrentReadOrWrite));
            } //for i
            return (itemsCreated.ToArray());
        }

        private void DoSimulation(object state)
        {
            try
            {
                foreach (BaseDataVariableState deviceItem in dynamicNodes_)
                {
                    opcServer_.WriteBaseVariable(deviceItem, opcServer_.GetNewSimulatedValue(deviceItem),
                        StatusCodes.Good, DateTime.UtcNow);
                }

                #region Dynamic adding/deleting of nodes

                // find the analog sensor node
                var analogSensor =
                    (BaseObjectState)opcServer_.FindNode(new NodeId("Analog Sensor 1", opcServer_.NamespaceIndexes[0]));

                if (analogSensor == null)
                {
                    // The analog sensor doesn't exists, so we create it

                    var rootObj =
                        (BaseObjectState)
                            opcServer_.FindNode(new NodeId("/CTT/DynamicObject", opcServer_.NamespaceIndexes[0]));

                    BaseObjectState childAnalogSensor1 = opcServer_.CreateBaseObject(rootObj, "Analog Sensor 1",
                        "Analog Sensor 1");
                    opcServer_.CreateProperty(childAnalogSensor1, "Value", "Value", BuiltInType.Float, ValueRanks.Scalar,
                        AccessLevels.CurrentRead, null);
                    opcServer_.CreateProperty(childAnalogSensor1, "HighSet", "HighSet", BuiltInType.Float,
                        ValueRanks.Scalar, AccessLevels.CurrentRead, null);
                    opcServer_.CreateProperty(childAnalogSensor1, "LowSet", "LowSet", BuiltInType.Float,
                        ValueRanks.Scalar, AccessLevels.CurrentRead, null);

                    opcServer_.AddNode(childAnalogSensor1);
                }
                else
                {
                    // The analog sensor exists, so we delete it
                    opcServer_.DeleteNode(new NodeId("Analog Sensor 1", opcServer_.NamespaceIndexes[0]));
                }

                #endregion
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Returns the linker timestamp for an assembly.
        /// </summary>
        private static DateTime GetAssemblyTimestamp()
        {
            const int peHeaderOffset = 60;
            const int linkerTimestampOffset = 8;

            var bytes = new byte[2048];

            using (Stream istrm = new FileStream(Assembly.GetCallingAssembly().Location, FileMode.Open, FileAccess.Read)
                )
            {
                istrm.Read(bytes, 0, bytes.Length);
            }

            // get the location of te PE header.
            int index = BitConverter.ToInt32(bytes, peHeaderOffset);

            // get the timestamp from the linker.
            int secondsSince1970 = BitConverter.ToInt32(bytes, index + linkerTimestampOffset);

            // convert to DateTime value.
            var timestamp = new DateTime(1970, 1, 1, 0, 0, 0);
            timestamp = timestamp.AddSeconds(secondsSince1970);
            return timestamp;
        }

        /// <summary>
        ///     Returns the major/minor version number for an assembly formatted as a string.
        /// </summary>
        private static string GetAssemblySoftwareVersion()
        {
            Version version = Assembly.GetCallingAssembly().GetName().Version;
            return Format("{0}.{1}", version.Major, version.Minor);
        }

        /// <summary>
        ///     Returns the build/revision number for an assembly formatted as a string.
        /// </summary>
        private static string GetAssemblyBuildNumber()
        {
            Version version = Assembly.GetCallingAssembly().GetName().Version;
            return Format("{0}.{1}", version.Build, (version.MajorRevision << 16) + version.MinorRevision);
        }

        /// <summary>
        ///     Formats a message using the default locale.
        /// </summary>
        private static string Format(string text, params object[] args)
        {
            return String.Format(CultureInfo.InvariantCulture, text, args);
        }

        public StatusCode OnCallMethod(object sender, MethodCallEventArgs e)
        {
            return StatusCodes.Good;
        }

        #endregion

        #region IDisposable

        /// <summary>
        ///     Implement IDisposable.
        ///     Do not make this method virtual.
        ///     A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        /// <summary>
        /// </summary>
        /// <param name="disposing">
        ///     If disposing equals true, the method has been called directly
        ///     or indirectly by a user's code. Managed and unmanaged resources
        ///     can be disposed.
        ///     If disposing equals false, the method has been called by the
        ///     runtime from inside the finalizer and you should not reference
        ///     other objects. Only unmanaged resources can be disposed.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!disposed_)
            {
                lock (lockDisposable_)
                {
                    // If disposing equals true, dispose all managed
                    // and unmanaged resources.
                    if (disposing)
                    {
                        // Dispose managed resources.
                        if (simulationTimer_ != null)
                        {
                            simulationTimer_.Dispose();
                        }
                    }

                    // Call the appropriate methods to clean up
                    // unmanaged resources here.
                    // If disposing is false,
                    // only the following code is executed.

                    // Disposing has been done.
                    disposed_ = true;
                }
            }
        }

        #endregion
    }
}