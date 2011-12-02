<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="FoundOps.Core.Azure" generation="1" functional="0" release="0" Id="2d706a5c-c40d-4c88-9a75-9497d0b5910a" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="FoundOps.Core.AzureGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="FoundOps.Server:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/LB:FoundOps.Server:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="FoundOps.Server:?IsSimulationEnvironment?" defaultValue="">
          <maps>
            <mapMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/MapFoundOps.Server:?IsSimulationEnvironment?" />
          </maps>
        </aCS>
        <aCS name="FoundOps.Server:?RoleHostDebugger?" defaultValue="">
          <maps>
            <mapMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/MapFoundOps.Server:?RoleHostDebugger?" />
          </maps>
        </aCS>
        <aCS name="FoundOps.Server:?StartupTaskDebugger?" defaultValue="">
          <maps>
            <mapMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/MapFoundOps.Server:?StartupTaskDebugger?" />
          </maps>
        </aCS>
        <aCS name="FoundOps.Server:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/MapFoundOps.Server:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="FoundOps.ServerInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/MapFoundOps.ServerInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:FoundOps.Server:Endpoint1">
          <toPorts>
            <inPortMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/FoundOps.Server/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapFoundOps.Server:?IsSimulationEnvironment?" kind="Identity">
          <setting>
            <aCSMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/FoundOps.Server/?IsSimulationEnvironment?" />
          </setting>
        </map>
        <map name="MapFoundOps.Server:?RoleHostDebugger?" kind="Identity">
          <setting>
            <aCSMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/FoundOps.Server/?RoleHostDebugger?" />
          </setting>
        </map>
        <map name="MapFoundOps.Server:?StartupTaskDebugger?" kind="Identity">
          <setting>
            <aCSMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/FoundOps.Server/?StartupTaskDebugger?" />
          </setting>
        </map>
        <map name="MapFoundOps.Server:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/FoundOps.Server/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapFoundOps.ServerInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/FoundOps.ServerInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="FoundOps.Server" generation="1" functional="0" release="0" software="C:\FoundOps\Agile5\Source-DEV\Server\FoundOps.Azure\bin\Debug\FoundOps.Azure.csx\roles\FoundOps.Server" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="768" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="?IsSimulationEnvironment?" defaultValue="" />
              <aCS name="?RoleHostDebugger?" defaultValue="" />
              <aCS name="?StartupTaskDebugger?" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;FoundOps.Server&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;FoundOps.Server&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/FoundOps.ServerInstances" />
            <sCSPolicyFaultDomainMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/FoundOps.ServerFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyFaultDomain name="FoundOps.ServerFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="FoundOps.ServerInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="e5182d96-3bbd-4848-bee8-390c93a1d9d4" ref="Microsoft.RedDog.Contract\ServiceContract\FoundOps.Core.AzureContract@ServiceDefinition.build">
      <interfacereferences>
        <interfaceReference Id="764ad255-0c2d-404a-bccc-230a6ddd1e38" ref="Microsoft.RedDog.Contract\Interface\FoundOps.Server:Endpoint1@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/FoundOps.Server:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>