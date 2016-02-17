taskkill /f /t /im wcctl
taskkill /f /t /im wcctl.exe
taskkill /f /t /im wcclient.exe
taskkill /f /t /im wcclient
start .\wcclient.exe localhost -b
.\wcctl.exe -t .\scenario.ubr -f .\settings.ubr -p 5000 -c 1 -v 500 -o .\out.xml -x
