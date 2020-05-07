; Script generated by the HM NIS Edit Script Wizard.

Var /GLOBAL IC_DN40 #.Net frameworker 4.0
Var /GLOBAL IC_SCOM #SCOM Server

; HM NIS Edit Wizard helper defines
!define SOURCEDIR "..\Release"
!define PRODUCT_NAME "Huawei eSight For SCOM plugin"
!define PRODUCT_PUBLISHER "Huawei Technologies Co., Ltd."
!define PRODUCT_WEB_SITE "https://www.huawei.com"
!define PRODUCT_UNINST_KEY "Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"
!define PATH_KEY "SYSTEM\CurrentControlSet\Control\Session Manager\Environment"

RequestExecutionLevel admin
Unicode true
; MUI 1.67 compatible ------
!include "MUI.nsh"
!include "WordFunc.nsh"
!include "WinMessages.nsh"
!include "WinCore.nsh"
!include "x64.nsh"
!include "nsProcess.nsh"
!include "StrFunc.nsh"
!include "nsDialogs.nsh"
!include "TextFunc.nsh"
!include "LogicLib.nsh"
!include "Ports.nsh"


; MUI Settings
!define MUI_ABORTWARNING
!define MUI_ICON "${SOURCEDIR}\Configuration\logo.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"

; Language Selection Dialog Settings
!define MUI_LANGDLL_REGISTRY_ROOT "${PRODUCT_UNINST_ROOT_KEY}"
!define MUI_LANGDLL_REGISTRY_KEY "${PRODUCT_UNINST_KEY}"
!define MUI_LANGDLL_REGISTRY_VALUENAME "NSIS:Language"

!define MUI_CUSTOMFUNCTION_UNGUIINIT "un.myUnOnGuiInit"
!define MUI_CUSTOMFUNCTION_GUIINIT ".onMyGuiInit"

; Welcome page
!insertmacro MUI_PAGE_WELCOME


; License page
;!define MUI_LICENSEPAGE_CHECKBOX
;!insertmacro MUI_PAGE_LICENSE "..\Desktop\License.txt"
; Directory page


!insertmacro MUI_PAGE_DIRECTORY 

Page Custom fnc_agreeIISExpress_Show

Page Custom fnc_test_Show checkInput
; Instfiles page
!insertmacro MUI_PAGE_INSTFILES

; Finish page
;!define MUI_FINISHPAGE_RUN "$INSTDIR\Configuration\Huawei.SCOMPlugin.Client"
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_INSTFILES

; Language files
!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_LANGUAGE "SimpChinese"



LangString Lan_CheckNF_Msg ${LANG_ENGLISH} "You must install Microsoft .NET Framework 4.0 or above."
LangString Lan_CheckNF_Msg ${LANG_SIMPCHINESE} "您需要安装 Microsoft .NET Framework 4.0 或以上版本"

LangString Lan_CheckSCOM_Msg ${LANG_ENGLISH} "You must install SCOM 2012 or above."
LangString Lan_CheckSCOM_Msg ${LANG_SIMPCHINESE} "您需要安装 SCOM 2012 或以上版本"

LangString Lan_CheckSCOMIsRuning_Msg ${LANG_ENGLISH} "Microsoft.EnterpriseManagement.Monitoring.Console.exe is running, please close it first."
LangString Lan_CheckSCOMIsRuning_Msg ${LANG_SIMPCHINESE} "Microsoft.EnterpriseManagement.Monitoring.Console.exe 正在运行，请先关闭"

LangString Lan_keepIIS_Msg ${LANG_ENGLISH} "Uninstalling IIS Express may affect the proper running of other software. Are you sure you want to uninstall it?"
LangString Lan_keepIIS_Msg ${LANG_SIMPCHINESE} "卸载IIS Express 可能会影响其他正在运行的软件，确定要卸载？"

LangString Lan_Uninstall_Msg ${LANG_ENGLISH} "has been removed successfully from your computer."
LangString Lan_Uninstall_Msg ${LANG_SIMPCHINESE} "已经从您的计算机中卸载。"


LangString Lan_NotEmptyIP_Msg ${LANG_ENGLISH} "The IP can't be empty!"
LangString Lan_NotEmptyIP_Msg ${LANG_SIMPCHINESE} "IP不能为空！"


LangString Lan_InvalidIP_Msg ${LANG_ENGLISH} "Please enter a valid IP!"
LangString Lan_InvalidIP_Msg ${LANG_SIMPCHINESE} "请输入有效的IP！"

LangString Lan_NotEmptyPort_Msg ${LANG_ENGLISH} "The port can't be empty!"
LangString Lan_NotEmptyPort_Msg ${LANG_SIMPCHINESE} "端口号不能为空！"

LangString Lan_InvalidPortBig_Msg ${LANG_ENGLISH} "The port cannot be greater than 44399!"
LangString Lan_InvalidPortBig_Msg ${LANG_SIMPCHINESE} "端口号不能大于44399！"

LangString Lan_InvalidPortSmall_Msg ${LANG_ENGLISH} "The port cannot be smaller than 44300!"
LangString Lan_InvalidPortSmall_Msg ${LANG_SIMPCHINESE} "端口号不能小于44300！"

LangString Lan_InvalidPortOccupied_Msg ${LANG_ENGLISH} "The port has been occupied, please change it."
LangString Lan_InvalidPortOccupied_Msg ${LANG_SIMPCHINESE} "端口号被占用，请更换。"

LangString Lan_ConfigHeader_Msg ${LANG_ENGLISH} "IP/Port Configuration!"
LangString Lan_ConfigHeader_Msg ${LANG_SIMPCHINESE} "IP/端口号 配置"

LangString Lan_ConfigTip_Msg ${LANG_ENGLISH} "Please enter the FQDN or IP address and port(44300-44399) of your computer which will be used to connect by eSight server."
LangString Lan_ConfigTip_Msg ${LANG_SIMPCHINESE} "请输入可以被eSight访问的本地FQDN或IP地址和端口号(44300-44399)。"

LangString Lan_agreeHeader_Msg ${LANG_ENGLISH} "Select features to install"
LangString Lan_agreeHeader_Msg ${LANG_SIMPCHINESE} "选择功能"

LangString Lan_agreeTip_Msg ${LANG_ENGLISH} "You must select at least one feature to continue the installation."
LangString Lan_agreeTip_Msg ${LANG_SIMPCHINESE} "您需要选择至少一个功能以继续安装。"

LangString Lan_agreeLabel_Msg ${LANG_ENGLISH} "Used to receive alerts from eSight and provide WEB service, It will be install IIS Express on this server."
LangString Lan_agreeLabel_Msg ${LANG_SIMPCHINESE} "用于从eSight接收警报并提供WEB服务，它将在此服务器上安装IIS Express。"

LangString Lan_agreeLeaveAlert_Msg ${LANG_ENGLISH} "You must check the IIS Express feature."
LangString Lan_agreeLeaveAlert_Msg ${LANG_SIMPCHINESE} "您需要选择IIS Express功能。"

LangString Lan_ConfirmUninstall_Msg ${LANG_ENGLISH} "Are you sure you want to remove "
LangString Lan_ConfirmUninstall_Msg ${LANG_SIMPCHINESE} "您确定要卸载"

LangString Lan_ConfirmUpgrade_Msg ${LANG_ENGLISH} "You already installed ${PRODUCT_NAME} $OLD_VER,do you want to uninstall the installed version?$\n(After uninstall successfully, please run the installation package again.)"
LangString Lan_ConfirmUpgrade_Msg ${LANG_SIMPCHINESE} "您已经安装了${PRODUCT_NAME} $OLD_VER，是否卸载已安装版本？$\n（卸载成功后，请手动再次运行安装包）"

LangString Lan_IP_Msg ${LANG_ENGLISH} "FQDN or IP Address:"
LangString Lan_IP_Msg ${LANG_SIMPCHINESE} "FQDN或IP地址："

LangString Lan_Port_Msg ${LANG_ENGLISH} "Port:"
LangString Lan_Port_Msg ${LANG_SIMPCHINESE} "端口："

; MUI end ------

Name "${PRODUCT_NAME} ${VERSION}"
OutFile "Huawei_eSight_For_SCOM_Plugin_${VERSION}.${BUILD_NUMBER}.exe"
InstallDir "$PROGRAMFILES64\Huawei eSight For SCOM plugin"
;InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
ShowInstDetails show
ShowUnInstDetails show
BrandingText "HUAWEI"

Var ip
Var port

/*--------------------Start----------------------------*/

Section "MainSection" SEC01

  SetOutPath "$INSTDIR"
  SetOverwrite ifnewer
  File /r "${SOURCEDIR}\"

  DetailPrint "Start Install Management Packs"
  
  nsExec::ExecToLog '"$INSTDIR\Configuration\Huawei.SCOM.ESightPlugin.PackageHelper.exe" /i /port=$port /ip=$ip'
  
  DetailPrint "Install Management Packs Finish"
  RMDir /r "$INSTDIR\MPFiles\Temp"
  RMDir /r "$INSTDIR\MPResources.resources"
  
  ;ExecWait 'cmd'
 SectionEnd

Section -Post

  WriteUninstaller "$INSTDIR\uninst.exe"
 ; WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\Configuration\Huawei.SCOMPlugin.WindowsService.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${VERSION}.${BUILD_NUMBER}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon"  "$\"$INSTDIR\Configuration\logo.ico$\""
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"  
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PATH_KEY}" "ESIGHTSCOMPLUGIN" "$INSTDIR"
  SendMessage ${HWND_BROADCAST} ${WM_WININICHANGE} 0 "STR:Environment"
SectionEnd

Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) $(Lan_Uninstall_Msg)"
FunctionEnd

var KeepIISExpress

Function un.onInit
  !insertmacro MUI_UNGETLANGUAGE
FunctionEnd


Function un.myUnOnGuiInit
  ${nsProcess::FindProcess} "Microsoft.EnterpriseManagement.Monitoring.Console.exe" $R0
  StrCmp $R0 "0" On_Abort End
  On_Abort:
	MessageBox MB_USERICON|MB_OK|MB_TOPMOST "$(Lan_CheckSCOMIsRuning_Msg)"
	Abort
  End:

  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "$(Lan_ConfirmUninstall_Msg) $(^Name)?" IDYES keep IDNO none
  keep:
       MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "$(Lan_keepIIS_Msg)" IDYES deleteIIS IDNO keepIIS
        keepIIS:
             StrCpy $KeepIISExpress "1"
             goto go
        deleteIIS:
             goto go
  none:
       Abort
  go:

FunctionEnd

Section Uninstall
  DetailPrint "DeleteRegKey...${PRODUCT_UNINST_KEY}"
  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
    ${If} $KeepIISExpress == "1"
        nsExec::ExecToLog '"$INSTDIR\Configuration\Huawei.SCOM.ESightPlugin.PackageHelper.exe" /u /keepIISExpress=true'
    ${Else}
        nsExec::ExecToLog '"$INSTDIR\Configuration\Huawei.SCOM.ESightPlugin.PackageHelper.exe" /u /keepIISExpress=false'
    ${EndIf}

  Delete "$INSTDIR\uninst.exe"
  Delete "$INSTDIR\install.log"
  Delete "$INSTDIR\InstallUtil.InstallLog"
  Delete "$INSTDIR\iisexpress_amd64_en-US.install.log"
  Delete "$INSTDIR\iisexpress_amd64_en-US.uninstall.log"
  RMDir /r "$INSTDIR\WebServer"
  RMDir /r "$INSTDIR\MPFiles"
  RMDir /r "$INSTDIR\Configuration"
  RMDir /r "$INSTDIR\Logs"
 
  RMDir /r "$INSTDIR\KN"
  RMDir /r "$INSTDIR\Certs"


  DetailPrint "ESIGHTSCOMPLUGIN..."
  DeleteRegValue ${PRODUCT_UNINST_ROOT_KEY} "${PATH_KEY}" "ESIGHTSCOMPLUGIN"
  ;ExecWait 'cmd'
  SetAutoClose true
SectionEnd

Function .onInit
  ;Call CheckSCOMPlugin
	!insertmacro mui_langdll_display

FunctionEnd

Function .onMyGuiInit
	Call CheckOS
	Call CheckSCOM
	Call CheckSCOMPluginIsInstall
	Call CheckNF
FunctionEnd

Function .onInstSuccess
	
FunctionEnd

Function .onGUIEnd
     nsExec::ExecToLog 'net start "Huawei SCOM Plugin For eSight.Service"'
FunctionEnd
 

Var hCtl_test
Var hCtl_test_TextBox1
Var hCtl_test_Label1
Var hCtl_test_TextBox2
Var hCtl_test_Label2
; dialog create function
Function fnc_test_Create

  ; === test (type: Dialog) ===
  nsDialogs::Create 1018
  Pop $hCtl_test
  ${If} $hCtl_test == error
    Abort
  ${EndIf}
  !insertmacro MUI_HEADER_TEXT "$(Lan_ConfigHeader_Msg)" "$(Lan_ConfigTip_Msg)"

   ; === TextBox2 (type: Text) ===
  ${NSD_CreateText} 108.4u 33.67u 77.24u 14u ""
  Pop $hCtl_test_TextBox2
  ;${NSD_OnChange} $hCtl_test_TextBox2 ipCheck
  
  ; === Label2 (type: Label) ===
  ${NSD_CreateLabel} 30.4u 35.67u 75u 16u "$(Lan_IP_Msg)"
  Pop $hCtl_test_Label2
  ${NSD_AddStyle} $hCtl_test_Label2 ${SS_RIGHT}
  
  ; === TextBox1 (type: Text) ===
  ${NSD_CreateNumber} 108.4u 55.67u 77.24u 14u "44301"
  Pop $hCtl_test_TextBox1
 StrCpy $port "44301"
  ;${NSD_OnChange} $hCtl_test_TextBox1 portChange
   
  ; === Label1 (type: Label) ===
  ${NSD_CreateLabel} 30.4u 57.67u 75u 16u "$(Lan_Port_Msg)"
  Pop $hCtl_test_Label1
  ${NSD_AddStyle} $hCtl_test_Label1 ${SS_RIGHT}
  

FunctionEnd

Var hCtl_agree
Var hCtl_agree_CheckBox1
Var hCtl_agree_Label1
Var hCtl_agree_Label2

Function fnc_agreeIISExpress_Create

  ; === test (type: Dialog) ===
  nsDialogs::Create 1018
  Pop $hCtl_agree
  ${If} $hCtl_agree == error
    Abort
  ${EndIf}
  !insertmacro MUI_HEADER_TEXT "$(Lan_agreeHeader_Msg)"  "$(Lan_agreeTip_Msg)"

   ; === Checkbox (type: CheckBox) ===
  ${NSD_CreateCheckBox} 10.4u 23.67u 15.24u 14u ""
  Pop $hCtl_agree_CheckBox1
		
  ${NSD_OnClick} $hCtl_agree_CheckBox1 EnDisableButton
  
  ;${NSD_OnChange} $hCtl_test_TextBox2 ipCheck
  ; === Label2 (type: Label) ===
  ${NSD_CreateLabel} 25.8u 26.67u 50.33u 16u "IIS Express"
  Pop $hCtl_agree_Label1
  
    ${NSD_CreateLabel} 25.8u 40.67u 240.33u 16u "$(Lan_agreeLabel_Msg)"
  Pop $hCtl_agree_Label2
  
  IfFileExists "$PROGRAMFILES64\IIS Express\iisexpress.exe" iis_found iis_not_found
		iis_found:
			#锟窖帮拷装选锟叫诧拷锟斤拷锟矫革拷选锟斤拷
			SendMessage $hCtl_agree_CheckBox1 ${BM_SETCHECK} 1 1
			EnableWindow $hctl_agree_checkbox1 0
			goto end_of_find
		iis_not_found:
			#未锟斤拷装默锟较斤拷锟斤拷next
			SendMessage $hCtl_agree_CheckBox1 ${BM_SETCHECK} 1 1
			GetDlgItem $0 $HWNDPARENT 1
			EnableWindow $0 1
		end_of_find:
  
FunctionEnd


; dialog show function
Function fnc_test_Show
  Call fnc_test_Create
  nsDialogs::Show
  
FunctionEnd

; dialog show function
Function fnc_agreeIISExpress_Show
  Call fnc_agreeIISExpress_Create
  nsDialogs::Show
  
FunctionEnd 

Function EnDisableButton
	Pop $hCtl_agree_CheckBox1
	${NSD_GetState} $hCtl_agree_CheckBox1 $0
	${If} $0 == 1
		GetDlgItem $0 $HWNDPARENT 1
		EnableWindow $0 1
	${Else}
		GetDlgItem $0 $HWNDPARENT 1
		EnableWindow $0 0
	${EndIf}
FunctionEnd


Function ipCheck
  ;Pop $1 # $1 == $ Text
  ;${NSD_GetText} $1 $0
  ;StrCpy $port $0
  ${NSD_GetText} $hCtl_test_TextBox2 $ip
  ${if} $ip == ""
     MessageBox MB_USERICON|MB_OK  "$(Lan_NotEmptyIP_Msg)"
       Abort
  ${endif}

FunctionEnd
Function portCheck
  ${NSD_GetText} $hCtl_test_TextBox1 $port
  ${If} $port == ""
     MessageBox MB_USERICON|MB_OK  "$(Lan_NotEmptyPort_Msg)"
       Abort
  ${ElseIf} $port L> 44399
    MessageBox MB_USERICON|MB_OK  "$(Lan_InvalidPortBig_Msg)"
       Abort
 ${ElseIf} $port L< 44300
    MessageBox MB_USERICON|MB_OK  "$(Lan_InvalidPortSmall_Msg)"
      Abort

  ${EndIf}
 ${If} ${TCPPortOpen} $port
      MessageBox MB_OK "The port $port has been occupied, please change it."
      Abort
 ${EndIf}
FunctionEnd

Function checkInput
   Call ipCheck
   Call portCheck
FunctionEnd

Function CheckOS
	${If} ${RunningX64}
		SetRegView 64
	${Else}
		SetRegView 32
	${EndIf}
FunctionEnd

# Function CheckSCOMIsRuning
# 	${nsProcess::FindProcess} "Microsoft.EnterpriseManagement.Monitoring.Console.exe" $R0
# 	StrCmp $R0 "0" On_Abort End
# 	On_Abort:
# 		MessageBox MB_USERICON|MB_OK|MB_TOPMOST "$(Lan_CheckSCOMIsRuning_Msg)"
# 		Abort
# 	End:
		
# FunctionEnd

Function IsNet40Installed
	;LogText "Checking .Net frameworker4.0..."

	Push $R0
	Push $R1
	ReadRegDWORD $R0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" "Install"
	ReadRegDWORD $R1 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" "Version"
	
	;LogText ".Net frameworker install status is: $R0"
;	LogText ".Net frameworker version is: $R1"
	
	${If} $R0 == 1
		${If} $R1 >= "4.0.30319"
			StrCpy $R0 "Yes"
			StrCpy $IC_DN40 $R0
			;LogText "IC_DN40 Value is: $IC_DN40"
			goto lbl_?net40idone
		${EndIf}
	${EndIf}

	StrCpy $R0 "No"
	StrCpy $IC_DN40 $R0
	;LogText "IC_DN40 Value is: $IC_DN40"
	
	lbl_?net40idone:

	Pop $R1
	Exch $R0
FunctionEnd

Function CheckNF
	Call IsNet40Installed
	${If} $IC_DN40 == 'No'
		MessageBox MB_USERICON|MB_OK|MB_TOPMOST "$(Lan_CheckNF_Msg)"
		Abort
	${EndIf}
FunctionEnd

Function IsSCOMInstalled
	;LogText "Checking SCOM..."

	Push $R0
	Push $R1
	ReadRegDWORD $R1 HKLM "SOFTWARE\Microsoft\System Center Operations Manager\12\Setup" "ProductName"
	;LogText "SCOM ProductName is: $R1"
	
	${If} $R1 != ""
		StrCpy $R0 "Yes"
		StrCpy $IC_SCOM $R0
		;LogText "IC_SCOM Value is: $IC_SCOM"
		goto lbl_?sccmdone
	${EndIf}

	StrCpy $R0 "No"
	StrCpy $IC_SCOM $R0
	;LogText "IC_SCOM Value is: $IC_SCOM"
	
	lbl_?sccmdone:

	Pop $R1
	Exch $R0
FunctionEnd

Function CheckSCOM
	;LogText "Checking SCOM ccc..."
	Call IsSCOMInstalled
	${If} $IC_SCOM == 'No'
		MessageBox MB_USERICON|MB_OK|MB_TOPMOST "$(Lan_CheckSCOM_Msg)"
		Abort
	${EndIf}
FunctionEnd

Var UNINSTALL_PROG
Var OLD_VER
;Var OLD_PATH

Function CheckSCOMPluginIsInstall
 
  ReadRegStr $UNINSTALL_PROG ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString"
  ;ReadRegStr $OLD_PATH HKLM "Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}" "UninstallString"
 
  ReadRegStr $OLD_VER ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion"
  ${If} $OLD_VER = ""
	goto norun
  ${EndIf}
  
  MessageBox MB_ICONQuESTION|MB_YESNO "$(Lan_ConfirmUpgrade_Msg)" IDYES keep IDNO none
  keep:
    ExecWait '$UNINSTALL_PROG'
    Abort
  none:
    Quit
  norun:
    
FunctionEnd






