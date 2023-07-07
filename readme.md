# How to run unit tests

## Set up project config
1. Right-click on the `ChessBrowser.Tests` project in the Solution Explorer and select "Manage NuGet Packages."
2. In the NuGet Package Manager, search for "NUnit" and install the `NUnit` and `NUnit3TestAdapter` packages for your project.
3. Open the `ChessBrowser.csproj` file and comment out the line `<TargetFrameworks>net7.0-ios;net7.0-maccatalyst</TargetFrameworks>`.
4. Uncomment the line `<!--<TargetFrameworks>net7.0;net7.0-ios;net7.0-maccatalyst</TargetFrameworks>-->`.

## How to use config file to run unit tests

1. Open the terminal and navigate to the `ChessBrowser.Tests` directory.
2. Run the following command to untrack changes to the config file:
   ```
   git update-index --assume-unchanged appsettings.json
   ```

3. Open the `appsettings.json` file folder and replace `your_uid` and `your_password` with your own UID and password:
   ```json
   {
     "UID": "your_uid",
     "Password": "your_password"
   }
   ```

4. Right-click on the `ChessBrowser.Tests` project in the Solution Explorer and select "Manage NuGet Packages."
5. Add the `Microsoft.Extensions.Configuration` and `Microsoft.Extensions.Configuration.Json` packages to the project.

6. Right-click the `appsettings.json` file, select "Properties" from the context menu, and set the "Copy to Output Directory" property to "Copy if newer".

7. Save the changes and reload the project.
8. Run the unit tests.

