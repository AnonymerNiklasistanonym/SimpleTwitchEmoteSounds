# NSIS Windows installer

> [!NOTE]
> When creating the program binary add the constant `CUSTOM_FEATURE_INSTALLED` (`-p:DefineConstants="CUSTOM_FEATURE_INSTALLED"'`) so that the Settings are stored in a separate application data directory.

## Features

- Installs application to `%LocalAppData%/SimpleTwitchEmoteSounds` and a program data directory in `%AppData%/SimpleTwitchEmoteSounds`
  - Meaning the installer does only need user level privileges
- Creates Windows start menu shortcuts to:
  - Start the binary executable
  - Open the `Settings` directory
  - Uninstall the application (Remove all files, optionally remove settings via a dialog)
- Registers Windows registry keys to:
  - List it as installed application
  - List uninstall option to installed application
  - Recognize existing previous installations and automatically uninstall them before installing the current (new) version
- Optionally adds a binary executable desktop shortcut if user checks box

## Setup

1. Install [NSIS](https://nsis.sourceforge.io/Download) (e.g. `winget install -e --id NSIS.NSIS`)
2. Add `makensis` to the terminal by adding TODO to the environment PATH variable

## Build

> [!IMPORTANT]
> This installer requires a built version of the program binary: `./publish/SimpleTwitchEmoteSounds.exe`

```sh
# This creates ./publish/SimpleTwitchEmoteSounds_installer.exe
makensis windows_installer.nsi
```