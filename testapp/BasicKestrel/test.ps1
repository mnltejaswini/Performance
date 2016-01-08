function start_dotnet_web()
{
    start .\publish\dotnet\web.exe
}

function stop_dotnet_web()
{
    ps web | kill
}

function start_dnx_web()
{
    start .\publish\dnx\approot\web.cmd
}

function stop_dnx_web()
{
    ps dnx | kill
} 

function perf_test()
{
    $begin = [System.DateTime]::Now
    invoke-webrequest http://localhost:5000 | out-null
    $end = [System.DateTime]::Now

    Write-Host "Startup in: " + ($end - $begin).TotalMilliseconds

    c:\tools\ab.exe -n5000 -c16 -q -d http://127.0.0.1:5000/
}

# dotnet based Kestrel

start_dotnet_web
perf_test
stop_dotnet_web

# dnx based Kestrel

start_dnx_web
perf_test
stop_dnx_web

