<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="FoundOps.Core.Azure" generation="1" functional="0" release="0" Id="66d6bec4-33b7-4228-962b-8f143440b171" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="FoundOps.Core.AzureGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="FoundOps.Server:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/LB:FoundOps.Server:Endpoint1" />
          </inToChannel>
        </inPort>
        <inPort name="FoundOps.Server:Endpoint2" protocol="https">
          <inToChannel>
            <lBChannelMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/LB:FoundOps.Server:Endpoint2" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="Certificate|FoundOps.Server:test.foundops.com" defaultValue="">
          <maps>
            <mapMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/MapCertificate|FoundOps.Server:test.foundops.com" />
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
        <lBChannel name="LB:FoundOps.Server:Endpoint2">
          <toPorts>
            <inPortMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/FoundOps.Server/Endpoint2" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapCertificate|FoundOps.Server:test.foundops.com" kind="Identity">
          <certificate>
            <certificateMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/FoundOps.Server/test.foundops.com" />
          </certificate>
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
          <role name="FoundOps.Server" generation="1" functional="0" release="0" software="C:\FoundOps\GitHub\Source\Server\FoundOps.Azure\csx\TestRelease\roles\FoundOps.Server" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="1792" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
              <inPort name="Endpoint2" protocol="https" portRanges="443">
                <certificate>
                  <certificateMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/FoundOps.Server/test.foundops.com" />
                </certificate>
              </inPort>
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;FoundOps.Server&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;FoundOps.Server&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;e name=&quot;Endpoint2&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
            <storedcertificates>
              <storedCertificate name="Stored0test.foundops.com" certificateStore="My" certificateLocation="System">
                <certificate>
                  <certificateMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/FoundOps.Server/test.foundops.com" />
                </certificate>
              </storedCertificate>
            </storedcertificates>
            <certificates>
              <certificate name="test.foundops.com" />
            </certificates>
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
    <implementation Id="afdc31f0-28c3-4c93-8e49-dfc61e546e8d" ref="Microsoft.RedDog.Contract\ServiceContract\FoundOps.Core.AzureContract@ServiceDefinition.build">
      <interfacereferences>
        <interfaceReference Id="bf719f2d-2a40-454c-8eed-fb11e38ef680" ref="Microsoft.RedDog.Contract\Interface\FoundOps.Server:Endpoint1@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/FoundOps.Server:Endpoint1" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="2ed3e6c6-12d8-4ff7-9b3d-c3cf84108b77" ref="Microsoft.RedDog.Contract\Interface\FoundOps.Server:Endpoint2@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/FoundOps.Core.Azure/FoundOps.Core.AzureGroup/FoundOps.Server:Endpoint2" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>