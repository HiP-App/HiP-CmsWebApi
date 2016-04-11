HiPCMS
======

HiP-CmsWebApi is a content management system which is developed by the project group [History in 
Paderborn](http://is.uni-paderborn.de/fachgebiete/fg-engels/lehre/ss15/hip-app/pg-hip-app.html).
It is developed to fill the system 'History in Paderborn' with data. This is only a REST API with service end points. We 
also develop client applicaion to consume this REST API. The client application is built on AngularJS.

In another team of the project group, an Android app is developed that will 
make the content of HiPCMS accessable to the public. Information about the app 
will be added as soon as it is available.

HiPCMS will replace the original project which was known as [HiPBackend](https://hip.upb.de/).
HiPBackend's code unfortunately was not maintainable anymore and a rewrite was decided. 

See the LICENSE file for licensing information.

See [the graphs page](https://github.com/HiP-App/HiP-CmsWebApi/graphs/contributors) 
for a list of code contributions.

## Requirements:

 * [Visual Studio 2015](https://www.visualstudio.com/en-us/products/vs-2015-product-editions.aspx)
 * [MySQL](https://www.mysql.de/)
 * [NuGet Package Manager](https://www.nuget.org/), an extension of Visual Studio.
 

## Technolgies and Frameworks

HiP-CmsWebApi is a REST API built on .NET Framework 4.5.2


## Getting started

 * Clone the repository.
 * Update the database connection string in the connections.config file.
 * Now open NuGet package manager console in visual studio (Tools - NuGet Package Manager - Package Manager Console) and run 'Install-Package MySql.Data.Entity -Pre' which is a MySQL provider to communicate with Entity Framework.
 * Then run 'Enable-Migrations' which enables migrations to the applicaiton. 
 * Run 'Add-Migration anyname' and 'Update-Database' for applying changes to database.
 * Use ```setup.sh``` to install all dependencies.
 * Use ```test.sh``` to execute the tests

Note: the scripts setup.sh and test.sh are also used by TravisCI; they were 
created to keep setup and test-execution independent of a CI server.



## How to develop

 * The latest code is available on [the project's Github-page](https://github.com/HiP-App/HiP-CmsWebApi/)
 * You can [fork the repo](https://help.github.com/articles/fork-a-repo/) or [clone our repo](https://help.github.com/articles/cloning-a-repository/)
   * To submit patches you should fork and then [create a Pull Request](https://help.github.com/articles/using-pull-requests/)
   * If you are part of the project group, you can create new branches on the main repo as described [in our internal
     Confluence](http://atlassian-hip.cs.upb.de:8090/display/DCS/Conventions+for+git)

We are using [Visual Studio 2015](https://www.visualstudio.com/en-us/products/vs-2015-product-editions.aspx). 


## How to submit Defects and Feature Proposals

Please write an email to [hip-app@campus.upb.de](mailto:hip-app@campus.upb.de).

## Documentation

Documentation is currently collected in our [internal Confluence](http://atlassian-hip.cs.upb.de:8090/dashboard.action). If something is missing in 
this README, just [send an email](mailto:hip-app@campus.upb.de).


## Contact

> HiP (History in Paderborn) ist eine Plattform der:
> Universität Paderborn
> Warburger Str. 100
> 33098 Paderborn
> http://www.uni-paderborn.de
> Tel.: +49 5251 60-0

You can also [write an email](mailto:hip-app@campus.upb.de).