function validate-target([Parameter(ValueFromRemainingArguments)]$expectedTargets) {
    if ( ($expectedTargets -eq $null) -or (!$expectedTargets.Contains($target)) ) {

        if ($global:helpBlock) {
            &$global:helpBlock
        }

        throw "Invalid Target: $target`r`nValid Targets: $expectedTargets"
    }
}

function write-help($example, $description) {
    write-host
    write-host "$example" -foregroundcolor GREEN
    write-host "    $description"
}

function project-properties($majorMinorVersion, $buildNumber) {

    $versionPrefix = "$majorMinorVersion.$buildNumber"
    $versionSuffix = if ($buildNumber -eq "") { "dev" } else { "" }

    if ($versionSuffix) {
        write-host "Building Version $versionPrefix-$versionSuffix"
    } else {
        write-host "Building Version $versionPrefix"
    }

    regenerate-file (resolve-path "Directory.Build.props") @"
<Project>
    <PropertyGroup>
        <VersionPrefix>$versionPrefix</VersionPrefix>
        <VersionSuffix>$versionSuffix</VersionSuffix>
        <Copyright>(c) 2021 Headspring</Copyright>
        <LangVersion>8.0</LangVersion>
    </PropertyGroup>
</Project>
"@
}

function regenerate-file($path, $newContent) {
    $oldContent = [IO.File]::ReadAllText($path)

    if ($newContent -ne $oldContent) {
        write-host "Generating $path"
        [System.IO.File]::WriteAllText($path, $newContent, [System.Text.Encoding]::UTF8)
    }
}

function task($heading, $command, $path) {
    write-host
    write-host $heading -fore CYAN
    execute $command $path
}

function execute($command, $path) {
    if ($path -eq $null) {
        $global:lastexitcode = 0
        & $command
    } else {
        Push-Location $path
        $global:lastexitcode = 0
        & $command
        Pop-Location
    }

    if ($lastexitcode -ne 0) {
        throw "Error executing command:$command"
    }
}

function help($helpBlock) {
    $global:helpBlock = $helpBlock
}

function main($mainBlock) {
    if ($target -eq "help") {
         if ($global:helpBlock) {
            &$global:helpBlock
            return
         }
    }

    try {
        &$mainBlock
        write-host
        write-host "Build Succeeded" -fore GREEN
        exit 0
    } catch [Exception] {
        write-host
        write-host $_.Exception.Message -fore DARKRED
        write-host
        write-host "Build Failed" -fore DARKRED
        exit 1
    }
}