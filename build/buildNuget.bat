
nuget restore -SolutionDirectory ../  ../Rock.Logging/Rock.Logging.csproj

msbuild /p:Configuration=Release /t:Clean;Rebuild ..\Rock.Logging\Rock.Logging.csproj

nuget pack ..\Rock.Logging\Rock.Logging.csproj -Properties Configuration=Release