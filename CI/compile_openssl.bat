@echo off
set cmd=win32
set opensslpath=%WORKSPACE%\third_party\Src\OpenSSL\openssl-1.0.2n
xcopy %WORKSPACE%\src\Openssl\bad_dtls_test.c %WORKSPACE%\third_party\Src\OpenSSL\openssl-1.0.2n\ssl /E /Y /I /R

echo %cmd%
echo %opensslpath%

if {%cmd%} equ {32} goto :win32
if {%cmd%} equ {64} goto :win64

:win32
echo "win32"
cd /d C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\bin
call vcvars32
call vcvars32.bat
cd /d %opensslpath%
perl configure VC-WIN32
goto :complie


:win64
echo "win64"
cd /d C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\bin\amd64
echo 1

echo 2
call vcvars64.bat
echo "dir"
cd  /d %opensslpath%
perl configure VC-WIN64
goto :complie


:complie
echo "complie"
call ms\do_nasm
call nmake -f ms\ntdll.mak
call editbin.exe /rebase:base=0x11000000 out32dll\libeay32.dll
call editbin.exe /rebase:base=0x12000000 out32dll\ssleay32.dll

mkdir %WORKSPACE%\third_party\Lib\SSL
xcopy %WORKSPACE%\third_party\Src\OpenSSL\openssl-1.0.2n\out32dll\libeay32.dll %WORKSPACE%\third_party\Lib\SSL /E /Y /I /R 
xcopy %WORKSPACE%\third_party\Src\OpenSSL\openssl-1.0.2n\out32dll\libeay32.dll %WORKSPACE%\src_bin /E /Y /I /R 