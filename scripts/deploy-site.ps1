# Deploy a test web site to remote server
Param(
    [Parameter(Mandatory=$True)]
    [string] $server,
    [Parameter(Mandatory=$True)]
    [string] $appsource,
    [Parameter(Mandatory=$True)]
    [string] $username,
    [Parameter(Mandatory=$True)]
    [string] $password_file,
    [Parameter(Mandatory=$True)]
    [string] $framework,
    [Parameter(Mandatory=$True)]
    [string] $temp)

function pack($source, $output) {
    Write-Host Publish web site locally ...
    $begin = [System.DateTime]::Now

    if (Test-Path $output) {
	rm -r -force $output
    }

    dnvm use default -r $framework

    pushd $source
    dnu restore
    dnu publish --out $output --runtime active --no-source --configuration Release
    popd

    $duration = [System.DateTime]::Now - $begin
    Write-Host Total publish time: $duration.TotalMilliseconds ms
    Write-Host
}

function zip($source, $output) {
    Write-Host Compressing web site locally ...
    $begin = [System.DateTime]::Now

    if (Test-Path $output) {
	rm $output
    }

    Add-Type -Assembly System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::CreateFromDirectory($source, $output)

    $duration = [System.DateTime]::Now - $begin
    Write-Host Total compression time: $duration.TotalMilliseconds ms
    Write-Host
}

Write-Host Deploy web stie to server $server under credential $username.
$password = gc $password_file | ConvertTo-SecureString -AsPlainText -Force
$cred = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $username,$password
Write-Host Credential created.

Invoke-Command -ComputerName $server -Credential $cred -ScriptBlock {
    Write-Host Stopping site 'Perf' remotely ...
    Stop-Website Perf -ErrorAction SilentlyContinue;
    Write-Host Stopping app pool 'DefaultAppPool' remotely ...
    Stop-WebAppPool DefaultAppPool -ErrorAction SilentlyContinue;
    
    # sleep for 5 seconds to ensure files are released
    Start-Sleep -s 5
    Write-Host Clean files under \\$env:COMPUTERNAME\inetperf ...
    rm -r -force \\$env:computername\inetperf\*
}

pack $appsource "$temp\publish"
zip "$temp\publish" "$temp\package.zip"

$plain_password = gc $password_file
net use \\$server\inetperf $plain_password /USER:$username
cp $temp\package.zip \\$server\inetperf\package.zip

Invoke-Command -ComputerName $server -Credential $cred -ScriptBlock {
    $work_dir = "$env:HOMEDRIVE\inetperf";
    Add-Type -Assembly System.IO.Compression.FileSystem;
    
    Write-Host Extracting files on $env:COMPUTERNAME
    $begin = [System.DateTime]::Now

    [System.IO.Compression.ZipFile]::ExtractToDirectory("$work_dir\package.zip", "$work_dir");

    $duration = [System.DateTime]::Now - $begin
    Write-Host Extracting time: $duration.TotalMilliseconds ms
    Write-Host
}

Write-Host Start web site remotely ...
Invoke-Command -ComputerName $server -Credential $cred -ScriptBlock {
    Start-Website Perf;
    Start-WebAppPool DefaultAppPool;
}

Write-Host Done!
