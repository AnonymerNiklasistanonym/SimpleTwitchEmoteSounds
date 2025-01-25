### 🐮 Simple Twitch Emote Sounds

This application was created as a simple, easy-to-use, quick to set up, sound trigger. The goal is to reduce the barrier
of entry and enable creators to add an Emote Sound within seconds, instead of the minute(s)-long process with current
mainstream options like MixItUp and Streamer.bot.

> [!NOTE]
> This is a fork which includes a list of patches:
>
> - `patch-windows-installer-nsis`: Add a Windows installer based on NSIS
> - `patch-multistage-cicd`: Add multi stage GitHub Actions CI/CD where the automatic GitHub release gets its artifact from the build step
>   - `patch-multistage-cicd-linux`: Add GitHub Actions CI/CD Linux build support
>   - `patch-multistage-cicd-windows-installer-nsis`: Add GitHub Actions CI/CD Windows installer using NSIS build support
>   - `patch-multistage-cicd-release-notes`: Add GitHub Actions CI/CD release notes support
> - `patch-linux-audio`: Run this program natively on Linux by using an audio workaround since the default audio API `NAudio` is not available
> - `patch-linux-pkgbuild`: Add a `pacman` `PKGBUILD` file to easily install it on Arch Linux derivatives using the native package manager
> - `patch-config-numbers`: Make configurations number strings to actual numbers that are parsed differently depending on the runtime locale
> - `patch-readme`: Instructions about the patch list
> - `patch-avalonia-tab-navigation`: Fix broken tab navigation on the main menu when cycling through added sounds
> - `patch-fix-crash-on-bad-config`: Fix crash on bad configurations and fallback to the default configuration instead
>
> **Run**:
>
> ```sh
> dotnet run --project SimpleTwitchEmoteSounds
> ```
>
> **Build**:
>
> Build a single program binary to the directory `publish` using:
>
> ```sh
> dotnet publish SimpleTwitchEmoteSounds -o publish -c Release -p:PublishSingleFile=true -p:DebugType=none -p:PublishReadyToRun=false -p:IncludeNativeLibrariesForSelfExtract=true --self-contained false`.
> ```
>
> [**Update fork:**](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/working-with-forks/syncing-a-fork)
>
> ```sh
> git fetch upstream
> git checkout master
> git merge upstream/master
> ```
>
> **Create patch branch:**
>
> ```sh
> git fetch upstream
> git checkout -b patch-NAME upstream/master
> git push -u origin patch-NAME
> ```
>
> **Apply patches:**
>
> In case of updates that require more than just merging upstream modify the patches and then merge all of them into the latest upstream state:
>
> ```sh
> git checkout patch-readme
> cp merge.sh ../merge.sh
> ../merge.sh
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

For the name, we'll set it to `plink`, then we press ok followed by ctrl-clicking both files plink and plonk. Once it's created, we'll set the percentage to 50% and 50%. See the image below for how it should look.

![plink plonk example](https://raw.githubusercontent.com/Ganom/SimpleTwitchEmoteSounds/refs/heads/master/example/example-image.png)
