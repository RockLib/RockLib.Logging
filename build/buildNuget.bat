msbuild /p:Configuration=Release ..\Rock.Logging\Rock.Logging.csproj
nuget pack ..\Rock.Logging\Rock.Logging.csproj -Properties Configuration=Release