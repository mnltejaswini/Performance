# dotnet based Kestrel

$begin = [System.DateTime]::Now

start .\publish\dotnet\web.exe
invoke-webrequest http://localhost:5000 | out-null

$end = [System.DateTime]::Now

Write-Host "Startup in: " + ($end - $begin).TotalMilliseconds

c:\tools\ab.exe -n5000 -c16 -q -d http://127.0.0.1:5000/

ps web | kill

# dnx based Kestrel

$begin = [System.DateTime]::Now

start .\publish\dnx\approot\web.cmd
invoke-webrequest http://localhost:5000 | out-null

$end = [System.DateTime]::Now

Write-Host "Startup in: " + ($end - $begin).TotalMilliseconds

c:\tools\ab.exe -n5000 -c16 -q -d http://127.0.0.1:5000/

ps dnx | kill
