#!/bin/bash

sh .devcontainer/se-linux-podman/import-gpg-keys.sh "${1}"

dotnet restore
