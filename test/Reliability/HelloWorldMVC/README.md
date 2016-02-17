# Description
This setup is intended for reliability or performance tests for Kestrel and ASP.NET core on Linux

# Server Setup
- Refer to [Wcat Server Setup](..\Readme.md)
- Run testapp\HelloWorldMVC application on a specific port, say http://*:5000
        
        cd testapp\HelloWorldMvc
        dotnet restore
        dotnet run server.urls=http://*:5000

# Client Setup - Wcat
- Refer to [Wcat Client Setup](..\Readme.md)
- Update scenario.ubr with the duration needed (in seconds).
- Run 
        
        wcat.cmd




