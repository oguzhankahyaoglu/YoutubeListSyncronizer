SET APIURL=http://ytdownloader-api.herokuapp.com/swagger/v1/swagger.json
SET OUTPUT_PATH=Swagger.YTAPI
SET PACKAGE_NAME=Swagger.YTAPI

set executable=swagger-codegen-cli.jar
set JAVA_OPTS=%JAVA_OPTS% -XX:MaxPermSize=256M -Xmx1024M -DloggerPath=conf/log4j.properties
set ags=generate -i %APIURL% -l CsharpDotNet2 -o %OUTPUT_PATH% --additional-properties packageName=%PACKAGE_NAME%,basePath=%APIURL%
java %JAVA_OPTS% -jar %executable% %ags%

REM if not exist ".\nuget.exe" powershell -Command "[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12;(new-object System.Net.WebClient).DownloadFile('https://dist.nuget.org/win-x86-commandline/latest/nuget.exe', '.\nuget.exe')"
REM .\nuget.exe install src\%PACKAGE_NAME%\packages.config -o packages

REM "C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe" src/%PACKAGE_NAME%/%PACKAGE_NAME%.csproj
REM nuget pack src/%PACKAGE_NAME%/%PACKAGE_NAME%.csproj -symbols
