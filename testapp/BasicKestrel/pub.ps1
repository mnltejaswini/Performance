dotnet restore
dotnet publish -o .\publish\dotnet --framework dnxcore50
cp .\hosting.json .\publish\dotnet\ -force

dnu publish -o .\publish\dnx --no-source --runtime active
