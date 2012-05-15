ClientDiagnostics-HTTP-Module
=============================

Custom HTTP Module to perform basic actions when intercepting Enterprise Vault client web requests.

In order to register the module, build the soloution, drop the built ClientDiagnostics DLL into 
Enterprise Vault/WebApp/bin,then edit the web.config file located in Enterprise Vault/WebApp, adding
the following:

<pre>
  &lt;configuration>
  &lt;system.web>
  &lt;httpModules>
  &lt;add name="ClientDiagnosticsHandler"type="ClientDiagnosticsHandler.ClientDiagnosticsModule"/>
  &lt;/httpModules>
  &lt;/system.web>
  &lt;/configuration>
</pre>

It's important to note that since the module will be attempting to write to disk, any users will
need sufficient permissions to write/crate at that location, else the module will serve up a 500 error.