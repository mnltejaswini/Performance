# Description
This setup is intended for reliability or performance tests for Kestrel and ASP.NET core

# Server Setup
- Refer to [Server Setup](../README.md)
- Run testapp\HelloWorldMVC application on a specific port, say http://*:5000
        
        cd testapp\HelloWorldMvc
        dotnet restore
        dotnet run server.urls=http://*:5000

# Client Setup - Wcat
- Refer to [Wcat Client Setup](../README.md)
- Update scenario.ubr with the duration needed (in seconds).
- Run 
        
        wcat.cmd




