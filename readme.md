
# How to run unit tests

1. Right-click on the `ChessBrowser.Tests` project in the Solution Explorer and select "Manage NuGet Packages."
2. In the NuGet Package Manager, search for "NUnit" and install the `NUnit` and `NUnit3TestAdapter` packages for your project.
3. Open the `ChessBrowser.csproj` file and comment out the line `<TargetFrameworks>net7.0-ios;net7.0-maccatalyst</TargetFrameworks>`.
4. Uncomment the line `<!--<TargetFrameworks>net7.0;net7.0-ios;net7.0-maccatalyst</TargetFrameworks>-->`.
5. Save the changes and reload the project.
6. Run the unit tests.