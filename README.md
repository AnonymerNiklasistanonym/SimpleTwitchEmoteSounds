### 🐮 Simple Twitch Emote Sounds

This application was created as a simple, easy-to-use, quick to set up, sound trigger. The goal is to reduce the barrier
of entry and enable creators to add an Emote Sound within seconds, instead of the minute(s)-long process with current
mainstream options like MixItUp and Streamer.bot.

> [!NOTE]
> This is a fork to enable audio playback on Linux and a simple Windows installer that everyone can create which includes the following patches on top of the original code by Ganom:
>
> - `patch-linux-audio-v2`: Run this program natively on Linux by using an audio workaround since the default audio API `NAudio` is not available
> - `feature-disable-update-service-v2`: Disable the update service
> - `feature-win-installer-v2`: Add a simple Windows installer based on NSIS
> - `feature-cicd-v2`: Replace original CI/CD with simple multi stage script that supports disabling the update service and the NSIS Windows installer that runs without any variables needed for the original CI/CD
> - `feature-linux-pkgbuild-v2`: Add a `pacman` `PKGBUILD` file to easily install it on Arch Linux derivatives using the native package manager
>
> **Run**:
>
> Install SDK and runtime dependencies (e.g. `sudo pacman -S dotnet-sdk mpv` on Linux), then run:
>
> ```sh
> dotnet run --project SimpleTwitchEmoteSounds/SimpleTwitchEmoteSounds.csproj
> ```
>
> **Build**:
>
> Build a single program binary to the directory `publish` using:
>
> ```sh
> dotnet publish SimpleTwitchEmoteSounds/SimpleTwitchEmoteSounds.csproj -o publish -c Release -p:PublishSingleFile=true -p:DebugType=none -p:PublishReadyToRun=false -p:IncludeNativeLibrariesForSelfExtract=true --self-contained false -p:DefineConstants="DISABLE_UPDATE_SERVICE"
> ```
>
> Run the built application after making sure the runtime dependencies are installed (e.g. `sudo pacman -S dotnet-runtime mpv` on Linux):
>
> ```sh
> ./publish/SimpleTwitchEmoteSounds
> ```

- [📝 FAQ](#-faq)
    - [🤔 What is an Emote Sound?](#-what-is-an-emote-sound)
    - [❔ Why do that?](#-why-do-that)
    - [📁 What do categories do?](#-what-do-categories-do)
    - [💅 Customizing](#-customizing)
    - [🔊 Can you show me an example of a multi-sound trigger?](#-can-you-show-me-an-example-of-a-multi-sound-trigger)

## 📝 FAQ

#### 🤔 What is an Emote Sound?

Emote sounds are triggered when a user types a certain phrase into Twitch chat, playing a sound effect. For example, if
there is a sound for the word `hiii` and the user types `hiii hello streamer`, it will trigger the sound set for `hiii`.

#### ❔ Why do that?

It is a fun way for chatters to directly interact with the stream. The emotes/phrases are typically associated with an
emotion or response. For example, `no` or `yes` sounds, or `xdx` being a trolling/gremlin response.

#### 📁 What do categories do?

Categories exist solely for organization currently, in the future they may be expanded to have toggle hotkeys or
exclusive hotkeys.

#### 💅 Customizing

The app allows you to upload as many sounds to a single phrase as you want. You can set the volume for each phrase, but
not for specific sounds. There is an option to change the play rate; say you want to only have a 20% chance for a sound
to trigger because you have a very active chat, this will allow you to throttle it. You can also have multiple sounds in
that trigger with different chances. Think sub alerts: you may want a special unique sound that has a 1% chance.

A feature not prominently shown is that you can split the name of the trigger with a comma for multiple valid phrases.
For example, name:`hi,hii,hiii` would be a valid phrase for each of those words.

#### 🔊 Can you show me an example of a multi-sound trigger?

Sure! We'll do a plink multi-sound trigger using these sounds:

- [plink](https://github.com/Ganom/SimpleTwitchEmoteSounds/raw/refs/heads/master/example/plink.mp3)
- [plonk](https://github.com/Ganom/SimpleTwitchEmoteSounds/raw/refs/heads/master/example/plonk.mp3)

For the name, we'll set it to `plink`, then we press ok followed by ctrl-clicking both files plink and plonk. Once it's
created, we'll set the percentage to 50% and 50%. See the image below for how it should look.

![plink plonk example](https://raw.githubusercontent.com/Ganom/SimpleTwitchEmoteSounds/refs/heads/master/example/example-image.png)
