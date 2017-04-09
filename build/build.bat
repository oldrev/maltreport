echo oFF
setlocal enabledelayedexpansion

set BUILD_SLN_PATH=%~dp0../MaltReport.sln
set BUILD_MAIN_PROJECT_PATH=%~dp0../Sandwych.Reporting\Sandwych.Reporting.csproj

:: one (or more) of these VS2017 editions may be installed
set community="%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MsBuild.exe"
set pro="%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MsBuild.exe"
set ent="%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MsBuild.exe"

if exist %community% (
	%community% %BUILD_SLN_PATH% /t:restore
	::%community% %~dp0build.proj /t:Release /fl /flp:Verbosity=normal
	::%community% %BUILD_MAIN_PROJECT_PATH%  /t:Pack /p:Configuration=Release
) else (
	if exist %ent% (
        %ent% %BUILD_SLN_PATH% /t:restore
        ::%ent% %~dp0build.proj /t:Release /fl /flp:Verbosity=normal
        %ent% %BUILD_MAIN_PROJECT_PATH%  /t:Pack /p:Configuration=Release
	) else (
        %pro% %BUILD_SLN_PATH% /t:restore
        ::%pro% %~dp0build.proj /t:Release /fl /flp:Verbosity=normal
        %pro% %BUILD_MAIN_PROJECT_PATH%  /t:Pack /p:Configuration=Release
	)
)

PAUSE
