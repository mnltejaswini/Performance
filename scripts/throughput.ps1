# Coordinating throughput tests
Param(
    [string] $controller,
    [string] $clients,
    [string] $settings,
    [string] $scenario,
    [string] $username,
    [string] $password_file,
    [string] $output)

$plain_password = gc $password_file
$password = ConvertTo-SecureString $plain_password -AsPlainText -Force
$cred = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $username,$password

net use \\$controller\PerfWorkspace $plain_password /USER:$username

Write-Host Clear controller scripts remotely
rm -r -force \\$controller\PerfWorkspace\*

Write-Host Copy scenario
cp $scenario \\$controller\PerfWorkspace\scenario.ubr

Write-Host Copy settings
cp $settings \\$controller\PerfWorkspace\settings.ubr

Invoke-Command -Computer $controller -Credential $cred -Authentication Credssp -ScriptBlock {
   $wcat = "$env:ProgramFiles\wcat\wcat.wsf"
   $clients = "aapt-perf-031,aapt-perf-032,aapt-perf-033,aapt-perf-034"
   cd D:\PerfWorkspace\
   cscript //H:cscript $wcat -terminate -update -clients $clients
   cscript //H:cscript $wcat -terminate -run -clients $clients -f settings.ubr -singleip
}

cp \\$controller\PerfWorkspace\Log.xml $output\

Write-Host Done

