dotnet restore
dotnet publish -o .\publish\dotnet --framework dnxcore50 --configuration release
cp .\hosting.json .\publish\dotnet\ -force

dnu publish -o .\publish\dnx --no-source --framework dnxcore50 --configuration release

