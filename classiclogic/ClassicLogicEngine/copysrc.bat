@SET SRC_DIR="ClassicLogicEngine\"
@SET COPY_TO="..\..\game\Assets\ThirdParty\ClassicLogicEngine\src\"

@xcopy %SRC_DIR%Engine %COPY_TO%Engine\ /S
@xcopy %SRC_DIR%Outputs %COPY_TO%Outputs\ /S
@xcopy %SRC_DIR%Utils %COPY_TO%Utils\ /S