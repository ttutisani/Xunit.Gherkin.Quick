The library supports two types of file lookup as listed below.

* Embedded files, this ensures feature files are always included and available with the test project. They are guaranteed to include the same files during build.
* File system search, assumes the feature files have been copied to the working directory when tests are run and can be updated after build.

In general, embedding files is the easiest way to get started as this removes the file system and ensures related feature files are always present. Both approaches require changes to the `.csproj` to ensure files are present when running tests.

```xml
<ItemGroup>
  <EmbeddedResource Include="**/*.feature" Exclude="bin/**/*.feature;obj/**/*.feature" />
</ItemGroup>
```

```xml
<ItemGroup>
  <None Update="**/*.feature">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

## Test Discovery

Lookup happens regardless, all feature files are browsed and matched with implementing types, the following steps describe how test cases are generated and handled.

1. If the feature file is invalid then the test fails with the parse message, test case display name is the file name.
2. If a feature file does not have a matching implementation then it shows up as a test case and is skipped, the display name is the feature name as described in the related file.
3. If a feature file has a matching implementation then the feature aggregates each scenario which is listed as an individual test case.
4. If a feature contains an outline section then this is treated as a data driven test, each set of inputs generates a test case.
5. If a feature file does not have matching implementation steps then the test case fails with a message indicating the missing step.
6. If any of the steps fail, then the related assertion fails the test case.
7. If all steps in a scenario or scenario outline example pass then the test passes.
8. Features, scenarios and scenario outlines decorated with `@ignore` are skipped.