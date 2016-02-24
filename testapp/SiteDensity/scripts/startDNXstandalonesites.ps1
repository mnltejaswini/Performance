Param(
[string] $apppath,
[string] $clr,
[int32] $count

)

$scriptPath = 'D:\aspcore\startsite.ps1'


For($i=1;$i -le $count;$i++)
{
  $port = 5100 + $i;
$commandLine = "-NoExit & $scriptPath $apppath $clr $port"
  Start-Process powershell.exe -ArgumentList $commandLine 
}
