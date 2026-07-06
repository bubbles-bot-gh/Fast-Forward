

# Generate the coverage report
dotnet \
    ~/.nuget/packages/reportgenerator/5.5.10/tools/net10.0/ReportGenerator.dll \
    -reports:BubblesBotGitHub.Tests/bin/Debug/net10.0/TestResults/coverage.cobertura.xml \
    -targetdir:BubblesBotGitHub.Tests/Tools/CoverageReport