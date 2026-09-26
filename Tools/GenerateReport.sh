# Generate the coverage report
dotnet reportgenerator \
    -reports:"BubblesBotGitHub.Tests/bin/Debug/net10.0/TestResults/coverage.cobertura.xml" \
    -targetdir:./Tools/CoverageReport