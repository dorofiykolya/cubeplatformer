@SET MSBUILD="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
@SET PROJECT="ClassicLogicEngine\ClassicLogicEngine.csproj"
@SET DLL_DIR="ClassicLogicEngine\bin"
@SET DLL="ClassicLogicEngine.dll"
@SET COPY_TO="..\..\game\Assets\ThirdParty\ClassicLogicEngine\ClassicLogicEngine.dll"
@SET DEBUG=Debug
@SET RELEASE=Release

::CONFIGURATION (DEBUG|RELEASE)
@SET CONFIG=%DEBUG%

::CLEAN PROJECT
%MSBUILD% /t:Clean %PROJECT%

::BUILD PROJECT
@CALL %MSBUILD% /p:Configuration=%CONFIG% %PROJECT%

::COPY DLL
@copy %DLL_DIR%\%CONFIG%\%DLL% %COPY_TO%
