<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="GrupaEka.Azure.ccproj" generation="1" functional="0" release="0" Id="e077a916-7c52-4a89-b390-032430529ea5" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="GrupaEka.Azure.ccprojGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="GrupaEka:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/GrupaEka.Azure.ccproj/GrupaEka.Azure.ccprojGroup/LB:GrupaEka:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="GrupaEka:?IsSimulationEnvironment?" defaultValue="">
          <maps>
            <mapMoniker name="/GrupaEka.Azure.ccproj/GrupaEka.Azure.ccprojGroup/MapGrupaEka:?IsSimulationEnvironment?" />
          </maps>
        </aCS>
        <aCS name="GrupaEka:?RoleHostDebugger?" defaultValue="">
          <maps>
            <mapMoniker name="/GrupaEka.Azure.ccproj/GrupaEka.Azure.ccprojGroup/MapGrupaEka:?RoleHostDebugger?" />
          </maps>
        </aCS>
        <aCS name="GrupaEka:?StartupTaskDebugger?" defaultValue="">
          <maps>
            <mapMoniker name="/GrupaEka.Azure.ccproj/GrupaEka.Azure.ccprojGroup/MapGrupaEka:?StartupTaskDebugger?" />
          </maps>
        </aCS>
        <aCS name="GrupaEka:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/GrupaEka.Azure.ccproj/GrupaEka.Azure.ccprojGroup/MapGrupaEka:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="GrupaEkaInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/GrupaEka.Azure.ccproj/GrupaEka.Azure.ccprojGroup/MapGrupaEkaInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:GrupaEka:Endpoint1">
          <toPorts>
            <inPortMoniker name="/GrupaEka.Azure.ccproj/GrupaEka.Azure.ccprojGroup/GrupaEka/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapGrupaEka:?IsSimulationEnvironment?" kind="Identity">
          <setting>
            <aCSMoniker name="/GrupaEka.Azure.ccproj/GrupaEka.Azure.ccprojGroup/GrupaEka/?IsSimulationEnvironment?" />
          </setting>
        </map>
        <map name="MapGrupaEka:?RoleHostDebugger?" kind="Identity">
          <setting>
            <aCSMoniker name="/GrupaEka.Azure.ccproj/GrupaEka.Azure.ccprojGroup/GrupaEka/?RoleHostDebugger?" />
          </setting>
        </map>
        <map name="MapGrupaEka:?StartupTaskDebugger?" kind="Identity">
          <setting>
            <aCSMoniker name="/GrupaEka.Azure.ccproj/GrupaEka.Azure.ccprojGroup/GrupaEka/?StartupTaskDebugger?" />
          </setting>
        </map>
        <map name="MapGrupaEka:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/GrupaEka.Azure.ccproj/GrupaEka.Azure.ccprojGroup/GrupaEka/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapGrupaEkaInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/GrupaEka.Azure.ccproj/GrupaEka.Azure.ccprojGroup/GrupaEkaInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="GrupaEka" generation="1" functional="0" release="0" software="D:\PWr\projekt inżynierski\GrupaEKA\GrupaEka.Azure\bin\Debug\GrupaEka.Azure.csx\roles\GrupaEka" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="1792" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="?IsSimulationEnvironment?" defaultValue="" />
              <aCS name="?RoleHostDebugger?" defaultValue="" />
              <aCS name="?StartupTaskDebugger?" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;GrupaEka&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;GrupaEka&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/GrupaEka.Azure.ccproj/GrupaEka.Azure.ccprojGroup/GrupaEkaInstances" />
            <sCSPolicyFaultDomainMoniker name="/GrupaEka.Azure.ccproj/GrupaEka.Azure.ccprojGroup/GrupaEkaFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyFaultDomain name="GrupaEkaFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="GrupaEkaInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="3119b348-a76b-4a54-9a92-a000d43af2af" ref="Microsoft.RedDog.Contract\ServiceContract\GrupaEka.Azure.ccprojContract@ServiceDefinition.build">
      <interfacereferences>
        <interfaceReference Id="219d2578-5741-406c-aa0d-0d4e5b8f657c" ref="Microsoft.RedDog.Contract\Interface\GrupaEka:Endpoint1@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/GrupaEka.Azure.ccproj/GrupaEka.Azure.ccprojGroup/GrupaEka:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>