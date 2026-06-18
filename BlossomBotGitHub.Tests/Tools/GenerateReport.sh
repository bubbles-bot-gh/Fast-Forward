# Build the testing project
dotnet build BlossomBotGitHub.Tests

# Run xUnit tests with code coverage
dotnet run \
    --project BlossomBotGitHub.Tests \
    -- \
    --coverage \
    --coverage-output-format cobertura \
    --coverage-output coverage.cobertura.xml

# Generate the coverage report
reportgenerator \
    -reports:BlossomBotGitHub.Tests/bin/Debug/net10.0/TestResults/coverage.cobertura.xml \
    -targetdir:BlossomBotGitHub.Tests/Tools/CoverageReport
    
# Echo the coverage report index file for easy access
printf "http://localhost:63341/BlossomBotGitHub/BlossomBotGitHub.Tests/Tools/CoverageReport/index.html"