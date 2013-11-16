# AzureFabric Task for MSBuild
#### An Azure dev fabric MSBuild task

This is an Azure dev fabric task for MSBuild, which can be used to start the
Windows Azure emulator during build. The emulator will only be started if it 
is not already.

## Usage

To use, first include the task in your MSBuild project file:

    <UsingTask AssemblyFile="AzureFabricTask.dll" TaskName="AzureFabric" />

The task will automatically try to find the Windows Azure SDK installation on
your machine via the Windows Registry. If you'd like to use a custom install
location, you can specify one via the `SdkDir` property.

The task requires MSBuild v4.0 or later.

### Minimal Example

    <Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
      <UsingTask AssemblyFile="AzureFabricTask.dll" TaskName="FxCop"/>
      
      <Target Name="StartFabric">
        <AzureFabric />
      </Target>
    </Project>

## License

Licensed under the [MIT](http://www.opensource.org/licenses/mit-license.html) license. See LICENSE.txt.

Copyright (c) 2013 Chad Burggraf.