# Install program using `pacman`

The [`PKGBUILD`](./PKGBUILD) file contains 3 different packages (using commented out sections):

1. (Default) A build using a `git` version tag release
2. A build using the latest `git` `master` branch commit
3. A build using the local files

For options 1 and 2 the following command automatically builds the package and asks if it should be installed.

```sh
makepkg -sifrC
#  -s, --syncdeps   Install missing dependencies with pacman
#  -i, --install    Install package after successful build
#  -f, --force      Overwrite existing package
#  -r, --rmdeps     Remove installed dependencies after a successful build
#  -C, --cleanbuild Remove $srcdir/ dir before building the package
```

For option 3 (local files) the option to not extract the local source tree is necessary:

```sh
makepkg -sifrC --noextract
```

If you want multiple `PKGBUILD` files you can select them using:

```sh
makepkg -sifrC -p CUSTOM_PKGBUILD
```

To create the package info [`.SRCINFO`](./.SRCINFO) run:

```sh
makepkg --printsrcinfo > .SRCINFO
```
