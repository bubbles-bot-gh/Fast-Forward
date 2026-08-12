# Generate the coverage report
dotnet reportgenerator \
    -reports:"BubblesBotGitHub.Tests.Unit/bin/Debug/net10.0/TestResults/coverage.cobertura.xml;BubblesBotGitHub.Tests.Integration/bin/Debug/net10.0/TestResults/coverage.cobertura.xml" \
    -targetdir:./Tools/CoverageReport