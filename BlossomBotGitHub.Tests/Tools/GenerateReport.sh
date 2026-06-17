dotnet run \
    --project BlossomBotGitHub.Tests \
    -- \
    --coverage \
    --coverage-output-format cobertura \
    --coverage-output coverage.cobertura.xml

reportgenerator \
    -reports:BlossomBotGitHub.Tests/bin/Debug/net10.0/TestResults/coverage.cobertura.xml \
    -targetdir:BlossomBotGitHub.Tests/Tools/CoverageReport