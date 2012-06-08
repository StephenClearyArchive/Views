@echo off
if not exist ..\..\Binaries mkdir ..\..\Binaries
devenv Nito.Views.sln /rebuild Release
.nuget\nuget.exe pack -sym Views.nuspec -o ..\..\Binaries
