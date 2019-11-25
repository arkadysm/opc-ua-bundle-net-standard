-------------------------------------------------------------------------------------------------------------
## OPC UA Client SDK .NET Standard - 1.2.1125 (Release Date 25-NOV-2019)

###	Enhancement
- Less logging for Info messages. Moved them to OperationDetail

## OPC UA Server SDK .NET Standard - 1.2.1125 (Release Date 25-NOV-2019)

###	Enhancement
- Less logging for Info messages. Moved them to OperationDetail

## OPC UA Core .NET Standard - 1.2.1125 (Release Date 25-NOV-2019)

###	Enhancement
- Less logging for Info messages. Moved them to OperationDetail

-------------------------------------------------------------------------------------------------------------
## OPC UA Client SDK .NET Standard - 1.2.1124 (Release Date 24-NOV-2019)

###	Issues
- Add server nonce validation in Client session.

## OPC UA Server SDK .NET Standard - 1.2.1124 (Release Date 24-NOV-2019)

###	Issues
- Added user authentication handling for UaMonitoredItem.

## OPC UA Core .NET Standard - 1.2.1124 (Release Date 24-NOV-2019)

###	Enhancement
- Preparation for Complex Types

###	Issues
- Avoid deep recursion in TcpMessageSocket.ReadNextBlock()
- Certificate Validation fix.
- Remove BadCertificateUriInvalid from list of certificate validation errors that may be suppressed.
- Add server nonce validation in Client session.
- Length validation in user token decryption.
- Stability in BinaryDecoder improved.
- Fix in DiscoveryClient.Create()

-------------------------------------------------------------------------------------------------------------

## OPC UA Server SDK .NET Standard - 1.2.1101 (Release Date 01-NOV-2019)

###	Breaking Changes
- Changed license handling. You will get some errors using this version now. Please see the new license certificate. The new method to use is 
        void OnGetLicenseInformation(out Opc.Ua.LicenseHandler.LicenseEdition productEdition, out string serialNumber);

###	Enhancement
- Added CreateBaseDataVariable(NodeState parent, string path, string name, ExpandedNodeId expandedNodeId, int valueRank, byte accessLevel, object initialValue) method to IUaServer

## OPC UA Client SDK .NET Standard - 1.2.1101 (Release Date 01-NOV-2019)

###	Breaking Changes
- Changed license handling. You will get some errors using this version now. Please see the new license certificate. The new method to use is 
        public static bool Validate(Opc.Ua.LicenseHandler.LicenseEdition productEdition, string serialNumber)

###	Issues
- Add check for some configuration fields and return some additional errors.

## OPC UA Core .NET Standard - 1.2.1101 (Release Date 01-NOV-2019)

###	Issues
- ExpandedNodeId.CompareTo contract violation
- fix schema validator and dictionary loader (only binary)
- fix schema validator and dictionary loader (only binary scheme)
- include some .bsd files for validation

-------------------------------------------------------------------------------------------------------------

## OPC UA Server SDK .NET Standard - 1.2.1013 (Release Date 13-OCT-2019)

###	Breaking Changes
- Several synchronous methods marked as obsolete in 1.1 are now giving an error while building with 1.2. Use the asynchronous versions mentioned in the error instead.
- Updated .NET 4.6.1 version to .NET 4.6.2
  
###	Enhancement
- Add IsNodeAccessibleForUser() and IsReferenceAccessibleForUser() to UaBaseNodeManager().
  This allows to hide nodes and references for specific users. 
  The ServerForms and ServerConsole sample shows the usage if these new methods.

###	Issues
- Added several XSD schema's to the schema folder. The Workshop server sample ModelDesign.xml files references the UAModelDesign.xsd. 
  Visual Studio can be used to change the Design files while giving context sensitive and popups. 

## OPC UA Client SDK .NET Standard - 1.2.1013 (Release Date 13-OCT-2019)

###	Breaking Changes
- Several synchronous methods marked as obsolete in 1.1 are now giving an error while building with 1.2. Use the asynchronous versions mentioned in the error instead.
- Updated .NET 4.6.1 version to .NET 4.6.2
- OPC UA Client Gateway is no longer maintained and source code is no longer delivered.

###	Issues
- The Session Method "ReadValue" delivers old values. 
- Using IUaServerData.Status can be made thread safe by using Status.Lock
- Client keep-alive fix - Outstanding publish requests should not stop keep alive reads.

## OPC UA Core .NET Standard - 1.2.1013 (Release Date 13-OCT-2019)

###	Enhancement
- Synchronize with newest UA Nodeset

###	Issues
- TraceEventHandler event does not publish exception information
- Possible handle leak in stress test for TCP communication 
- Interoperability fixes for HTTPS
- Improvement to enable UANodeSet to import/export AccessLevelEx as uint32

-------------------------------------------------------------------------------------------------------------

## OPC UA Server SDK .NET Standard - 1.1.815 (Release Date 15-AUG-2019)

###	Changes
Version numbering scheme changed. Version numbers are used with Major.Minor.Build, where
- Major: is incremented when we make incompatible API changes
- Minor: is incremented when we add functionality in a backwards-compatible manner
- Build: is incremented when we make backwards-compatible bug fixes and is the date (mmyy) the build was triggered

###	Updates / Fixes
- Enhanced documentation and added example usage on Linux, macOS
- CertificateValidation event not fired for BadCertificateChainIncomplete. 
- Cleaned up source and added build related property files (Source Distribution only)

## OPC UA Client SDK .NET Standard - 1.1.824 (Release Date 24-AUG-2019)

###	Updates / Fixes
- Enhanced documentation and added example usage on Linux, macOS
- CertificateValidation event not fired for BadCertificateChainIncomplete. 
- Cleaned up source and added build related property files (Source Distribution only)

-------------------------------------------------------------------------------------------------------------

## OPC UA Server SDK .NET Standard - 1.1.0 (Release Date 22-JUN-2019)

###	Highlights
- Enhanced documentation regarding certificate and configuration tool handling

###	Updates / Fixes
- Removed WindowsCertificateStore support
- Fixed several screens of the Configuration Tool
- Fix for abandoned socket connection in UaSCUaBinaryTransportChannel.Reconnect()
- Fix to avoid timer interval higher than Int32.MaxValue in ClientSubscription.StartKeepAliveTimer()
- Updated Local Discovery Server to 1.03.401.438

## OPC UA Client SDK .NET Standard - 1.1.0 (Release Date 22-JUN-2019)

###	Highlights
- Enhanced documentation regarding certificate and configuration tool handling
- Fixed several screens of the Configuration Tool

###	Updates / Fixes
- Removed WindowsCertificateStore support
- Fix for abandoned socket connection in UaSCUaBinaryTransportChannel.Reconnect()
- Updated Local Discovery Server to 1.03.401.438

-------------------------------------------------------------------------------------------------------------

## OPC UA Server SDK .NET Standard - 1.0.9 (Release Date 31-MAY-2019)

###	Updates / Fixes
- Compliance fixes in CertificateValidator.
  Added setting in SecurityConfiguration to require revocation lists for all CAs
- Updated documentation

## OPC UA Client SDK .NET Standard - 1.0.9 (Release Date 31-MAY-2019)

###	Updates / Fixes
- Updated documentation

-------------------------------------------------------------------------------------------------------------

## OPC UA Server SDK .NET Standard - 1.0.8 (Release Date 22-MAY-2019)

###	Highlights
- Enhanced documentation
- .NET 4.6.1 sample applications and installer added
- Visual Studio 2013, Visual Studio 2015 added
- New examples added compiling for .NET 4.6.1, .NET 4.7.2 and .NET Core 2.0 with one solution
- Source Code Distribution now available

###	Updates / Fixes
- Cleanup of namespaces (Technosoftware.UaServer.Base no longer exists) and code
- GenericServer class is now in namespace Technosoftware.UaServer.Server
- Session related classes are now in namespace Technosoftware.UaServer.Sessions
- Renamed IAggregateCalculator to IUaAggregateCalculator
- Renamed CertificateUpdateCallbackHandler to CertificateUpdateEvent
- Renamed FirewallUpdateCallbackHandler to FirewallUpdateEvent
- Several session related events of IUaSessionManager changed in naming to fit general concept:
     - SessionCreated to SessionCreatedEvent
	 - SessionActivated to SessionActivatedEvent
	 - SessionClosing to SessionClosingEvent
	 - ImpersonateUser to ImpersonateUserEvent
	 - ValidateSessionLessRequest to ValidateSessionLessRequestEvent

## OPC UA Client SDK .NET Standard - 1.0.8 (Release Date 22-MAY-2019)

###	Highlights
- Enhanced documentation
- .NET 4.6.1 sample applications and installer added
- Visual Studio 2013, Visual Studio 2015 added
- New examples added compiling for .NET 4.6.1, .NET 4.7.2 and .NET Core 2.0 with one solution
- Source Code Distribution now available

###	Updates / Fixes
- Cleanup of code
- Renamed CertificateUpdateCallbackHandler to CertificateUpdateEvent
- Renamed FirewallUpdateCallbackHandler to FirewallUpdateEvent

-------------------------------------------------------------------------------------------------------------

## OPC UA Server SDK .NET Standard - 1.0.7 (Release Date 06-MAY-2019)

###	Highlights
- First version of OPC UA Server SDK .NET Standard. Compatible to API of OPC UA Server SDK .NET
- Supports .NET Standard 2.0. New WorkshopServerConsole is now available for 
     - .NET 4.6.1, .NET 4.7.2, .NET Core 2.0 (\examples\Workshop\ServerConsole)
	 - .NET 4.7.2 (\examples\Workshop\ServerForms)
  Both samples has the same functionality but the first one is a console application and the second one a Wiondows Form based application.

###	Updates / Fixes
- Update 1.04 NodeSets and generated code
- CTT Compliance fixes
- CertificateValidator update

## OPC UA Client SDK .NET Standard - 1.0.7 (Release Date 06-MAY-2019)

###	Highlights
- Supports .NET Standard 2.0. New WorkshopServerConsole is now available for 
     - .NET 4.6.1, .NET 4.7.2, .NET Core 2.0 (\examples\Workshop\ClientConsole)
	 - .NET 4.7.2 (\examples\Workshop\ClientForms)

###	Updates / Fixes
- Update 1.04 NodeSets and generated code

-------------------------------------------------------------------------------------------------------------

## OPC UA Server SDK .NET Standard - 1.0.6 (Release Date 05-APR-2019)

###	Highlights
- First version of OPC UA Server SDK .NET Standard. Compatible to API of OPC UA Server SDK .NET
- Supports .NET 4.6.1 and .NET 4.7.2. 

###	Known Issaues
- .NET Standard 2.0 not yet supported

## OPC UA Client SDK .NET Standard - 1.0.6 (Release Date 05-APR-2019)

###	Highlights
- Added more compatibility to OPC UA Client SDK .NET (InstallConfig, Services and command line parameters supported)
- Supports .NET 4.6.1, .NET 4.7.2. and .NET Standard 2.0

-------------------------------------------------------------------------------------------------------------

## OPC UA Client SDK .NET Standard - 1.0.5 (Release Date 16-FEB-2019)

###	Highlights
- OPC UA Configuration Client is now able to handle also clients based on this SDK (.NET 4.6.1 only)

-------------------------------------------------------------------------------------------------------------

## OPC UA Client SDK .NET Standard - 1.0.4 (Release Date 05-JAN-2019)

###	Highlights
- Support for AES security policies
- 1.04 Specification NodeSets and generated code
- Custom configuration settings for user certificate stores

###	Updates / Fixes
- Security updates
- Certificate stores renamed according to specification recommendation

-------------------------------------------------------------------------------------------------------------

## OPC UA Client SDK .NET Standard - 1.0.3 (Release Date 08-DEC-2018)
- Added Professional license for .NET 4.6.1 usage only

-------------------------------------------------------------------------------------------------------------

## OPC UA Client SDK .NET Standard - 1.0.1 (Release Date 10-NOV-2018)
- Updated brochures

-------------------------------------------------------------------------------------------------------------

## OPC UA Client SDK .NET Standard - 1.0.0 (Release Date 13-OCT-2018)

###	Highlights
- .NET Standard 2.0 version tested on macOS 10.14

## OPC UA Stack .NET Standard - 1.0.0 (Release Date 13-OCT-2018)
- .NET Standard 2.0 version tested on macOS 10.14

-------------------------------------------------------------------------------------------------------------

## OPC UA Client SDK .NET Standard - 0.2.0 (Release Date 03-OCT-2018)

###	Highlights
- Licensing mechanism added
- Certificate stores renamed according to Part 12 specification recommendation.

## OPC UA Stack .NET Standard - 0.2.0 (Release Date 03-OCT-2018)
- Updated stack with OPC Foundation Stack changes until 13-SEP-2018

-------------------------------------------------------------------------------------------------------------

## OPC UA Client SDK .NET Standard - 0.1.0 (Release Date 17-SEP-2018)
- Initial beta version

## OPC UA Stack .NET Standard - 0.1.0 (Release Date 17-SEP-2018)
- Initial beta version