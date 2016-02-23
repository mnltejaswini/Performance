# Description
This setup is intended for reliability or performance tests for Kestrel and ASP.NET core 

# Server Setup - Windows
- Create A2 Standard Windows 2012 R2 Server VM on Azure
- Install git - http://git-scm.com/download for example
- Clone the Performance repo - git clone https://github.com/aspnet/Performance.git
- Install dotnet cli - http://dotnet.github.io
- Install ASP.NET 5 - https://go.microsoft.com/fwlink/?LinkId=627627
- Install IIS from server manager -> Manage -> Add Roles and features
- Install HttpPlatformHandler from http://www.iis.net/downloads/microsoft/httpplatformhandler
- In IIS manager, Open Configuration editor and make sure system.webserver/handlers section is unlocked (i.e. write access is enabled).
- Upgrade to latest clr
    - set DNX_FEED=https://www.myget.org/F/aspnetcidev/api/v2dnvm
    - dnvm upgrade -r CoreCLR
- Publish the binaries
    - Go to Permformance repo
    - cd testapp\HelloWorldMvc
    - dnu restore
    - dnu publish --runtime dnx-coreclr-win-x64.&lt;your version&gt; --configuration release
- Create new site (not application or vdir) in IIS; set its Physical Path to testapp\HelloWorldMvc\bin\Output\wwwroot and
  the Host Name to the machine name.
- Make sure user for site AppPool has full permissions to this folder. 
- In IIS manager, edit the site Application Pool to set '.NET CLR version' to  'No Managed Code'.
- Open the Site's port in the fire wall
- Test the new  site with a browser specifying the machine name in the URL both locally and remotely
- Create perfmon user defined report as following:
    - Run perfmon
    - Go to Data Collector Sets, right click on user defined -> New -> Data Collector set
    - Choose "create from template"
    - click next -> browse and choose the test\Reliability\HelloWorldMVC\perfmon.xml file
    - click on Finish and the new Data collector set should be created 
    - modify the duration in the DataCollector properties stop condition to match the run duration.
- Start perfmon data collector on server         
- Execute your test run     

# Server Setup - Ubuntu

- Create A2 Standard Ubuntu VM on Azure
- install an SSH clinet such Putty from http://www.putty.org/ on a Windows machine and connect to the linux VM
- Install dotnet cli - http://dotnet.github.io
- Install libuv

        sudo apt-get install make automake libtool curl
        curl -sSL https://github.com/libuv/libuv/archive/v1.8.0.tar.gz | sudo tar zxfv - -C /usr/local/src
        cd /usr/local/src/libuv-1.8.0
        sudo sh autogen.sh
        sudo ./configure
        sudo make
        sudo make install
        sudo rm -rf /usr/local/src/libuv-1.8.0 && cd ~/
        sudo ldconfig
- Install Nginx
```sh
$ sudo apt-get install nginx
```
- Install GIT
```sh
$ sudo apt-get install git
```
- Clone the Performance Repo
```sh
$  git clone https://github.com/aspnet/Performance.git
```
- Run one of the Performance repo test applications on a specific port, say http://*:5000 
        
        cd Performance/testapp/BasicViews
        dotnet restore
        dotnet run server.urls=http://*:5000
        
        On another machine, test the site with a browser ( http://serverName:5000 )
       
- Create and enable the site/application in ngnix.  Ngnix is a popular proxy for exposing endpoints on a linux server.
    - Open a new SSH session
    - Create a configuration file for a new site to enable the reverse proxy in the nginx sites-available dir.  You can call the file anything.  We'll call it Performance here.  We'll edit it usng the nano editor.
    
            cd /
            cd etc/nginx/sites-available/
            sudo touch Performance
            sudo nano Peformance
            
       Paste the following into nano and save.      
              
            server {
                listen 8080;		 
                location / {
                        proxy_set_header    Host $host;
                        proxy_set_header    X-Real-IP   $remote_addr;
                        proxy_set_header    X-Forwarded-For $proxy_add_x_forwarded_for;
                        proxy_pass  http://localhost:5000;
                }
            }
    - Activate the host in by creating a symbolic link between the nginx sites-available directory and the sites-enabled directory:
            
            sudo ln -s /etc/nginx/sites-available/Performance /etc/nginx/sites-enabled/Performance
    - Restart nginx
            
            sudo service nginx restart
        Now all requests to 8080 will be proxied to 5000 where the Kestrel server listens
        On another machine, test the site with a browser (http://serverName:8080)
        
        Note that if the nginx restart fails, a likely reason is that some other service, such as apache is using port 80.  
        Nginx logs are at:  /var/log/nginx
        To stop Apache,  run the command:  
        
             sudo /etc/init.d/apache2 stop

- Collect server perf counters for process dnx an nginx:
       	
           top | grep 'dnx\|nginx' > cpumemlog.log
- If want to aggregate the data of the same process name (like aggregate for multiple nginx), we can post-process above log generated by top, or we can use atop:
        
        sudo apt-get install atop
        atop -M -m -p 1 >atoplog.log

# Client Setup - Wcat

- Create A2 Standard windows VM on Azure
- Install wcat - http://www.iis.net/downloads/community/2007/05/wcat-63-x86
- Copy the contents of the test\Reliability\AnyTestApp folder to %programfiles(x86)%\wcat 
- Replace the server name/IP in settings.ubr file with the server name/ip that you created during "Server Setup" steps above. 
- Update scenario.ubr with the duration needed (in seconds).
- Run 
        
        wcat.cmd




