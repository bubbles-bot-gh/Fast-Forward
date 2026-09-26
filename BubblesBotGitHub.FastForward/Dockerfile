# Set the base image as .NET 10.0 SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 as build-env

# Copy everything and publish the release
WORKDIR /app
COPY . ./
RUN dotnet publish ./BubblesBotGitHub.FastForward/BubblesBotGitHub.FastForward.csproj -c Release -o out \
    --no-self-contained 

# Label the container
LABEL maintainer="Lunei Solei"
LABEL repository="https://github.com/bubbles-bot-gh/fast-forward"
LABEL homepage="https://github.com/bubbles-bot-gh/fast-forward"

# Label as GitHub action
LABEL com.github.actions.name="BubblesBot Fast-Forward"

# Description limit is 160 characters
LABEL com.github.actions.description="Allows for fast-forwarding of pull requests."

# Branding
# https://docs.github.com/actions/creating-actions/metadata-syntax-for-github-actions#branding
LABEL com.github.actions.icon="fast-forward"
LABEL com.github.actions.color="blue"

# Relayer the .NET SDK, anew with the build output
FROM mcr.microsoft.com/dotnet/sdk:10.0
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "BubblesBotGitHub.FastForward.dll" ]