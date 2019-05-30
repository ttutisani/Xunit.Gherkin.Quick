# Copying feature files to build output

There are multiple ways to make sure that feature files are copied to the build output directory.

## Manually set the file property

Open the properties window of the feature file and set the value of `Copy to Output Directory` to `Copy Always` or `Copy if Newer`.

The disadvantage of this method is that you need to remember to set this property on each feature file you add to the project.

## Automatically copy feature files

It's also possible to leverage MSBuild to automatically copy any feature files.
Especially the new `Microsoft.NET.Sdk` project file makes this very easy. 

Open the `csproj` file of your test project and add a new `ItemGroup` to search for any feature file in a certain location and copy them to the output directory:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <!-- ... rest of project file omitted for brevity ... -->

  <ItemGroup>
    <None Update="Features\**\*.feature">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>

</Project>
```

This example will automatically copy any files ending with `.feature` from a folder named `Features` (and any of its subfolders).

**Important**: This will only work for project files that have `Sdk="Microsoft.NET.Sdk"` as attribute of the `Project` element.

Older project files can achieve the same effect by using the `AfterBuild` target in the `csproj` file.

```xml
<Target Name="AfterBuild">
     <Copy SourceFiles="*.features" DestinationFolder="$(OutputPath)" ContinueOnError="true" />
</Target>
```