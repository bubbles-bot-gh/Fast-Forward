#!/bin/bash

sh .devcontainer/se-linux-podman/import-gpg-keys.sh "${1}"

dotnet tool install --global dotnet-reportgenerator-globaltool
dotnet restore
