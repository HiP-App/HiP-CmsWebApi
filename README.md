HiPCMS
======

HiP-CmsWebApi is a content management system which is developed by the project group [History in 
Paderborn](http://is.uni-paderborn.de/fachgebiete/fg-engels/lehre/ss15/hip-app/pg-hip-app.html).
It is developed to fill the system 'History in Paderborn' with data. This is only a REST API with service end points. We 
also develop [client application](https://github.com/HiP-App/HiP-CmsAngularApp) to consume this REST API. The client application is built on AngularJS2. For Authentication and Authorization we are using a Microservice [HiP-Auth](https://github.com/HiP-App/HiP-Auth).

In another team of the project group, an Android app is developed that will 
make the content of HiPCMS accessable to the public. Information about the app 
will be added as soon as it is available.

HiPCMS will replace the original project which was known as [HiPBackend](https://hip.upb.de/).
HiPBackend's code unfortunately was not maintainable anymore and a rewrite was decided. 

See the LICENSE file for licensing information.

See [the graphs page](https://github.com/HiP-App/HiP-CmsWebApi/graphs/contributors) 
for a list of code contributions.

## Technolgies and Requirements:
HiP-CmsWebApi is a REST API built on .NET Core 1.0 with C# 6.0. Below are the requirements needed to build and develop this project,
 * [.NET Core](https://www.microsoft.com/net/core#windows) for Windows, Linux or macOS.
 * [PostgreSQL](http://www.postgresql.org/download/)
 
## IDE Options
 * Visual Studio 2015 with Update 3 and [NuGet Package Manager](https://www.nuget.org/). 
 * Visual Studio Code with [C# extention](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp).

## Getting started

 * Clone the repository.
 * Create a new file `appsettings.Development.json` at `scr/Api`. (See `src/Api/appsettings.Development.json.example`).
 * Update the new `appsettings.Development.json` file to match your needs.
 * To run using Visual Studio, just start the app with/without debugging.
 * To run through terminal,
   * Navigate to `src/Api`
   * Set Environment Variable 
		* Windows: `set ASPNETCORE_ENVIRONMENT=Development`
		* Linux/macOS: `export ASPNETCORE_ENVIRONMENT=Development`
   * Before your first run, execute `dotnet restore`
   * Execute `dotnet run`
   * `{{BaseUrl}}/swagger/ui` will give information about the service endpoints.

### VS Code Setup

For getting the project to run with Visual Studio Code, you will have to execute a few more steps:

 * go to the Debug view and click the run button - a prompt will appear asking for the launch configuration's target -- choose .NET
 * in the created `launch.json`:
   * replace every occurence of `${workspaceRoot}` with `${workspaceRoot}/src/Api`
   * add `"env": { "ASPNETCORE_ENVIRONMENT": "Development" }` to your run configurations
 * click run again, which will complain about no task runner being configured -- choose .NET as your task runner, which will create a `tasks.json` file
 * in the created `tasks.json`, add the following line: `"options": { "cwd": "${workspaceRoot}/src/Api" },`

### Removing .NET packages on Linux / macOS

If you are experiencing issues with your .NET installation on Linux or macOS, you can use the script at https://github.com/dotnet/cli/blob/rel/1.0.0/scripts/obtain/uninstall/dotnet-uninstall-pkgs.sh to remove all .NET packages (i.e. the SDK and runtime) in order to perform a fresh install afterwards.

## How to develop

 * You can [fork](https://help.github.com/articles/fork-a-repo/) or [clone](https://help.github.com/articles/cloning-a-repository/) our repo.
   * To submit patches you should fork and then [create a Pull Request](https://help.github.com/articles/using-pull-requests/)
  
## How to test

 * Create a new file `testconfig.json` at `test/Api.Tests`. (See `test/Api.Tests/testconfig.json.example`).
 * Update the new `testconfig.json` file with an valid MyTested Licence.
 * Navigate to `test/Api.Tests` folder and run `dotnet test`.


## How to submit Defects and Feature Proposals

Please write an email to [hip-app@campus.upb.de](mailto:hip-app@campus.upb.de).

## Contact

> HiP (History in Paderborn) ist eine Plattform der:
> Universität Paderborn
> Warburger Str. 100
> 33098 Paderborn
> http://www.uni-paderborn.de
> Tel.: +49 5251 60-0

You can also [write an email](mailto:hip-app@campus.upb.de).
