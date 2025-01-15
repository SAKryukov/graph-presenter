@echo off

set solution=All.sln

dotnet build %solution% -c Debug
dotnet build %solution% -c Release
