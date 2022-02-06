param($target="default", [int]$buildNumber)

. .\build-helpers

help {
    write-help "build (default)" "Local development build."
    write-help "build ci 1234" "Continuous integration build, applying the build counter to assembly versions."
}

main {
    validate-target "default" "ci"

    $targetFramework = "netcoreapp3.1"
    $configuration = 'Release'

    task ".NET SDK" { dotnet --version }
    task "Restore Tools" { dotnet tool restore }
    task "Project Properties" { project-properties "3.0" $buildNumber } src
    task "Clean" { dotnet clean --configuration $configuration --nologo -v minimal } src
    task "Restore (Solution)" { dotnet restore } src
    task "Build" { dotnet build --configuration $configuration --no-restore --nologo } src
    task "Test" { dotnet fixie *.Tests --configuration $configuration --no-build }
}