# New Version (New Release) Instructions

1. Create a new MD file under /versions directory.
   - For major or significantly backward incompatible changes, increment the major version number; otherwise, a minor version number increment will suffice.
   - List all enhancements and bug fixes this release addresses, linking each as an issue where applicable. Additionally, include a link to the documentation explaining the changed/added features.
   - Describe backward compatibility.
3. Change "Xunit.Gherkin.Quick" project attributes to reflect the new release information.
   - This action can be done either via Visual Studio (right-click the project and Properties) or in a text editor by modifying the .csproj file.
4. Run "pack.cmd" under /Xunit.Gherkin.Quick folder.
   - This will generate a new .nupkg filder under /bin/release folder.
   - Upload the newly created .nupkg file into Nuget.

Done!
