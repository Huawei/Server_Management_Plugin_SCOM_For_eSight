set PATH=C:\Program Files (x86)\MSBuild\14.0\Bin\;%PATH%

call D:\plugins\CodeDEX\tool\fortify\bin\sourceanalyzer -b scom msbuild /t:rebuild %WORKSPACE%\src\Huawei.SCOM.ESightPlugin\Huawei.SCOM.ESightPlugin.sln