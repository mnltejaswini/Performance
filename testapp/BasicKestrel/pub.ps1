dotnet restore
dotnet publish -o ..\publish\BasicKestrel\dotnet --framework dnxcore50 --configuration release
cp .\hosting.json ..\publish\BasicKestrel\dotnet\ -force

dnu publish -o ..\publish\BasicKestrel\dnx --no-source --framework dnxcore50 --configuration release

