; Windows Installer

;--------------------------------
;Include Modern UI

  !include "MUI2.nsh"

;--------------------------------
;Include External Config File (that contains name, version, ...)

  !include "windows_installer_config.nsi"

;--------------------------------
;Include External Logic File To Run Previous Uninstaller If Detected

  !include "windows_installer_run_previous_uninstaller.nsi"

;--------------------------------
;General Settings

  ;Properly display all languages
  Unicode true

  ;Show 'console' in installer and uninstaller
  ShowInstDetails "show"
  ShowUninstDetails "show"

  ;Set name and output file
  Name "${PRODUCT}"
  OutFile "..\publish\${PRODUCT}_installer.exe"

  ;Set the default installation directory
  InstallDir "$LOCALAPPDATA\${PRODUCT}"

  ;Create separate installation directory for binary files that can be added to the path
  !define INSTDIR_BIN "bin"

  ;Overwrite $InstallDir value when a previous installation directory was found
  InstallDirRegKey HKCU "Software\${PRODUCT}" ""

  ;Set execution level to 'user' to avoid requiring admin privileges
  RequestExecutionLevel user

;--------------------------------
;Interface Settings

  ;Use a custom welcome page title
  !define MUI_WELCOMEPAGE_TITLE "${PRODUCT_DISPLAY_NAME} ${PRODUCT_VERSION}"

  ;Show warning if user wants to abort
  !define MUI_ABORTWARNING

  ;Show all languages, despite user's codepage
  !define MUI_LANGDLL_ALLLANGUAGES

  ;Use a custom (un-)install icon
  !define MUI_ICON "..\${PRODUCT}\Assets\cow.ico"
  ;!define MUI_UNICON ".\resources\${PRODUCT}_greyscale.ico"

  ;Use custom image files for the (un-)installer
  ;!define MUI_HEADERIMAGE_RIGHT
  ;!define MUI_WELCOMEFINISHPAGE_BITMAP "..\res\installer\picture_left_installer.bmp"
  ;!define MUI_UNWELCOMEFINISHPAGE_BITMAP "..\res\installer\picture_left_uninstaller.bmp"

  ;Add a Desktop shortcut if the user wants to enable it on the finish page
  ;(https://stackoverflow.com/a/1517851)
  !define MUI_FINISHPAGE_SHOWREADME ""
  !define MUI_FINISHPAGE_SHOWREADME_NOTCHECKED
  !define MUI_FINISHPAGE_SHOWREADME_TEXT $(LangStringCreateDesktopShortcut)
  !define MUI_FINISHPAGE_SHOWREADME_FUNCTION createDesktopShortcut

;--------------------------------
;Pages

  ;For the installer:
  ;------------------------------
  ;Welcome page with name and version
  !insertmacro MUI_PAGE_WELCOME
  ;License page
  ;!insertmacro MUI_PAGE_LICENSE "..\LICENSE"
  ;Component selector
  ;!insertmacro MUI_PAGE_COMPONENTS
  ;Set install directory
  !insertmacro MUI_PAGE_DIRECTORY
  ;Show progress while installing/copying the files
  !insertmacro MUI_PAGE_INSTFILES
  ;Show final finish page
  !insertmacro MUI_PAGE_FINISH

  ;For the uninstaller:
  ;------------------------------
  ;Welcome page to uninstaller
  !insertmacro MUI_UNPAGE_WELCOME
  ;Confirm the uninstall with the install directory shown
  !insertmacro MUI_UNPAGE_CONFIRM
  ;Show progress while uninstalling/removing the files
  !insertmacro MUI_UNPAGE_INSTFILES
  ;Show final finish page
  !insertmacro MUI_UNPAGE_FINISH

;--------------------------------
;Include External Languages File

  !include "windows_installer_languages.nsi"

;--------------------------------
;Before Installer Section

Function .onInit
  ;If a previous installation was found ask the user via a popup if they want to
  ;uninstall it before running the installer
  ; Get uninstall information from HKCU instead of HKLM
  ReadRegStr $0 HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "UninstallString"
  ReadRegStr $1 HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "DisplayName"
  ${If} $0 != ""
  ${AndIf} ${Cmd} `MessageBox MB_YESNO|MB_ICONQUESTION "$(LangStrUninstallTheCurrentlyInstalled1)$1$(LangStrUninstallTheCurrentlyInstalled2)${PRODUCT_DISPLAY_NAME} ${PRODUCT_VERSION}$(LangStrUninstallTheCurrentlyInstalled3)" /SD IDYES IDYES`
    ;Use the included macro to uninstall the existing installation if the user
    ;selected yes
    !insertmacro UninstallExisting $0 $0
    ;If the uninstall failed show an additional popup window asking if the
    ;installation should be aborted or not
    ${If} $0 <> 0
      MessageBox MB_YESNO|MB_ICONSTOP "$(LangStrFailedToUninstallContinue)" /SD IDYES IDYES +2
        Abort
    ${EndIf}
  ${EndIf}

FunctionEnd

;--------------------------------
;Installer Section > Main Component

Section "${PRODUCT_DISPLAY_NAME} ($(LangStrRequired))" Section1

  DetailPrint "$(LangStrInstall) ${PRODUCT_DISPLAY_NAME} ${PRODUCT_VERSION}"

  ;This will prevent this component from being disabled on the selection page
  SectionIn RO

  ;Set output path to the installation directory and list the files that should
  ;be put into it
  SetOutPath "$INSTDIR"
  ;Icon for shortcuts
  File "/oname=${PRODUCT}.ico" "..\${PRODUCT}\Assets\cow.ico"
  ;Create a bin directory for binaries that should be made available via a PATH
  ;entry
  CreateDirectory "$INSTDIR\${INSTDIR_BIN}"
  SetOutPath "$INSTDIR\${INSTDIR_BIN}"
  ;Binary of program
  File "..\publish\${PRODUCT}.exe"
  SetOutPath "$INSTDIR"

  ;Store installation folder in registry for future installs under HKCU
  WriteRegStr HKCU "Software\${PRODUCT}" "" "$INSTDIR"

  ;Register the application in Add/Remove Programs under HKCU
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "DisplayName" "${PRODUCT_DISPLAY_NAME} ${PRODUCT_VERSION}"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "UninstallString" "$\"$INSTDIR\${PRODUCT}_uninstaller.exe$\""
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "QuietUninstallString" "$\"$INSTDIR\${PRODUCT}_uninstaller.exe$\" /S"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "URLInfoAbout" "$\"${PRODUCT_URL}$\""
  WriteRegDWORD HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "NoModify" 1
  WriteRegDWORD HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "NoRepair" 1

  ;Create default settings directory
  CreateDirectory "$AppData\${PRODUCT}"
  CreateDirectory "$AppData\${PRODUCT}\Settings"

  ;Create start menu shortcut for program, settings directory, and uninstaller
  CreateDirectory "$SMPROGRAMS\${PRODUCT}"
  CreateShortCut "$SMPROGRAMS\${PRODUCT}\${PRODUCT_DISPLAY_NAME} ${PRODUCT_VERSION}.lnk" "$INSTDIR\${INSTDIR_BIN}\${PRODUCT}.exe" "" "$INSTDIR\${PRODUCT}.ico" 0
  CreateShortCut "$SMPROGRAMS\${PRODUCT}\${PRODUCT_DISPLAY_NAME} $(LangStrSettings).lnk" "$AppData\${PRODUCT}\Settings"
  CreateShortCut "$SMPROGRAMS\${PRODUCT}\$(LangStrUninstall) ${PRODUCT_DISPLAY_NAME} ${PRODUCT_VERSION}.lnk" "$INSTDIR\${PRODUCT}_uninstaller.exe" "" "$INSTDIR\${PRODUCT}_uninstaller.exe" 0

  ;Create uninstaller
  WriteUninstaller "${PRODUCT}_uninstaller.exe"

SectionEnd

;--------------------------------
;Uninstaller Section

Section "Uninstall"

  DetailPrint "$(LangStrUninstall) ${PRODUCT_DISPLAY_NAME} ${PRODUCT_VERSION}"

  ;Remove registry keys
  DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}"
  DeleteRegKey HKCU "Software\${PRODUCT}"

  ;Remove the installation directory and all files within it
  RMDir /r "$INSTDIR\${INSTDIR_BIN}\*.*"
  RMDir "$INSTDIR\${INSTDIR_BIN}"
  RMDir /r "$INSTDIR\*.*"
  RMDir "$INSTDIR"

  ;Remove the start menu directory and all shortcuts within it
  Delete "$SMPROGRAMS\${PRODUCT}\*.*"
  RmDir  "$SMPROGRAMS\${PRODUCT}"

  ;Confirm whether to delete the settings directory
  MessageBox MB_YESNO|MB_ICONQUESTION "$(LangStrRemoveSettings)" IDYES +2
  Goto skip_delete_directory

  ;Remove the settings directory and its contents
  RMDir /r "$AppData\${PRODUCT}\Settings\*.*"
  RMDir /r "$AppData\${PRODUCT}\*.*"
  RMDir "$AppData\${PRODUCT}"

  skip_delete_directory:

SectionEnd

;--------------------------------
;After Installation Function (is triggered after a successful installation)

Function .onInstSuccess

  ;Open the settings directory
  ExecShell "open" "$INSTDIR\Settings"

FunctionEnd

;--------------------------------
;Custom Function To Create A Desktop Shortcut

Function createDesktopShortcut

  ;Reset output file path to installation directory because CreateShortCut needs
  ;that information (https://nsis-dev.github.io/NSIS-Forums/html/t-299421.html)
  SetOutPath "$INSTDIR\${INSTDIR_BIN}"

  ;Create Desktop shortcut to main component
  CreateShortCut "$DESKTOP\${PRODUCT_DISPLAY_NAME} ${PRODUCT_VERSION}.lnk" "$INSTDIR\${INSTDIR_BIN}\${PRODUCT}.exe" "" "$INSTDIR\${PRODUCT}.ico" 0

FunctionEnd
