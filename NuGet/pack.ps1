$root = (split-path -parent $MyInvocation.MyCommand.Definition) + '\..'
$version = [System.Reflection.Assembly]::LoadFile("$root\FakeDbSet\bin\Release\AnotherFakeDbSet.dll").GetName().Version
$versionStr = "{0}.{1}.{2}" -f ($version.Major, $version.Minor, $version.Build)

Write-Host "Setting .nuspec version tag to $versionStr"

$content = (Get-Content $root\NuGet\InMemoryDbSet.nuspec)
$content = $content -replace '\$version\$',$versionStr

$content | Out-File $root\NuGet\InMemoryDbSet.compiled.nuspec

& $root\NuGet\NuGet.exe pack $root\NuGet\InMemoryDbSet.compiled.nuspec