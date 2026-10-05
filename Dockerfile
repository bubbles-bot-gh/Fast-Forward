# Set the base image as the .NET 10.0 SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 as build-env

# Copy everything and publish the release
WORKDIR /app
COPY . ./
RUN dotnet publish ./BubblesBotGitHub.FastForward/BubblesBotGitHub.csproj -c Release -o out --no-self-contained

# Label the container
LABEL maintainer="Lunei Solei"
LABEL repository="https://github.com/bubbles-bot-gh/fast-forward"
LABEL homepage="https://github.com/bubbles-bot-gh/fast-forward"

# Label as GitHub Action
LABEL com.github.actions.name="Fast-Forward"
# 160 character limit
LABEL com.github.actions.description="Allows for verifying and/or performing a fast-forward on a pull request."
LABEL com.github.actions.icon="fast-forward"
LABEL com.github.actions.color="blue"

# Re-layer the .NET SDK, anew with the build output
FROM mcr.microsoft.com/dotnet/sdk:10.0
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "/BubblesBotGitHub.FastForward.dll" ]