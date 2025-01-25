#!/usr/bin/env bash

git fetch upstream
git switch -C master
git reset --hard upstream/master

PATCH_BRANCHES=("patch-windows-installer-nsis" "patch-multistage-cicd" "patch-multistage-cicd-linux" "patch-multistage-cicd-windows-installer-nsis" "patch-multistage-cicd-release-notes" "patch-linux-audio" "patch-linux-pkgbuild" "patch-config-numbers" "patch-readme" "patch-fix-crash-on-bad-config")
for PATCH in "${PATCH_BRANCHES[@]}"; do
  echo "Applying patch: $PATCH"
  if ! git merge --no-ff "$PATCH" -m "Merging $PATCH"; then
    while true; do
      read -rp "Merge conflict, resolve conflicts, git add files, then type 'continue' to proceed or abort: " response
      if [[ "$response" == "continue" ]]; then
        if git diff --name-only --diff-filter=U | grep -q .; then
          echo "Conflicts still exist. Please resolve them first."
        else
          echo "Conflicts resolved. Continuing..."
          git commit -m "Resolved conflicts for $PATCH"
          break
        fi
      else
        echo "Aborting script"
        exit 1
      fi
    done
  else
    echo "Successfully merged $PATCH."
  fi
done
