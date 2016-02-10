ASP.NET Performance Tests
=========================

AppVeyor: [![AppVeyor](https://ci.appveyor.com/api/projects/status/8e82uv0i2xi3dnv7/branch/dev?svg=true)](https://ci.appveyor.com/project/aspnetci/Performance/branch/dev)

Travis:   [![Travis](https://travis-ci.org/aspnet/Performance.svg?branch=dev)](https://travis-ci.org/aspnet/Performance)

Performance tests and infrastructure for ASP.NET.

In order to execute the tests:
  * Ensure that the peformance dashboard database has been created in your system by running the PerformanceDashboard web project. Once the database is generated then we can run the performance tests and see the results logged.
  * The performance tests depend on npm, bower and gulp being installed on your system. Ensure you have them.

The tests can be run by executing .\build.cmd from the project root. At this point this repository only works under Windows.

This project is part of ASP.NET 5. You can find samples, documentation and getting started instructions for ASP.NET 5 at the [Home](https://github.com/aspnet/home) repo.


## [Microbenchmark tests](testapp)
These are targeted tests covering specific feature areas. The tests are quite self explanatory. You can use the `loadtests.ps1` script to load test the specific scenario. We use the [loadtest npm module](https://www.npmjs.com/package/loadtest) in these scripts to keep them as simple as possible.

For e.g. here is how to run the Hello World MVC microbenchmark.

```powershell
cd testapp\HelloWorldMvc
dotnet restore
dotnet run
```

Run the load test client
```powershell
.\loadtest.ps1
```

A `loadtest.sh` script will also be added to cover cross platform testing.
