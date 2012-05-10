ClientDiagnostics-HTTP-Module
=============================

Custom HTTP Module to perform basic actions when intercepting Enterprise Vault client web requests.

Installation:
* Build the Class Library and chuck it with the rest of the EV Web binaries in Webapp/bin.
* Modify web.config and add the following by adding our module to the ‘httpModules’ section
* (creating it, if it does not exist):

<configuration>
<system.web>
<httpModules>
<add name="ClientDiagnosticsHandler"type="ClientDiagnosticsHandler.ClientDiagnosticsModule"/>
</httpModules>
</system.web>
</configuration>
