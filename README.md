# BlazorBoilerplate
A starter project which uses Blazor as Client, an Auth Server and .NET Core as API &amp; Backend


## How to use it?
- Just clone this repository to your local machine,
- Open Solution with Visual Studio. **Boilerplate.AuthServer/Boilerplate.AuthServer.sln**
- In AuthServer project, open appsettings.json and change your connection string according to your DB Server
- From package manager console, select AuthServer as Default Project and run ```update-database -Context ApplicationDbContext```
- Right click on solution and click on ```Select Startup Projects```
- Choose API, AuthServer and at least one of AdminUI and ClientUI projects