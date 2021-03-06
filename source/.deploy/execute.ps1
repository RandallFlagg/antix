Param($path,$version,$apikey)

#$packagesSource = "http://localhost:50358/packages"
$packagesSource = "http://nuget.antix.co.uk/packages"

if ($version -Eq $null) {
    $version = Read-Host "Enter Version Number"
}
if ($apikey -Eq $null) {
    $apikey = "00000000-0000-0000-0000-000000000000"
}

if($path -Eq $null){
    $path = Split-Path -parent $PSCommandPath 
    $path = "$path\..\.."

    set-alias msbuild "C:\Program Files (x86)\MSBuild\12.0\Bin\amd64\MSBuild.exe"

    msbuild "$path\source\antix.sln" /t:Build /p:Configuration=Release
}

Write-Output "begin deploy version $version from $path"

set-alias nuget $path\source\.nuget\NuGet.exe

function deploy{
	param($project)

	$packagePath = "$path\source\$project\obj"
	$package = "$packagePath\$project.code.$version.nupkg"

	nuget pack "$path\source\$project\$project.code.nuspec" -Properties version=$version -OutputDirectory "$packagePath"
	
	write-Output "pushing $package"

	nuget push "$package" -ApiKey $apikey -Source "$packagesSource/"
}

deploy "Antix"
deploy "Antix.Data"
deploy "Antix.Data.EF"
deploy "Antix.Data.Keywords"
deploy "Antix.Data.Keywords.EF"
deploy "Antix.Data.Static"
deploy "Antix.Drawing"
deploy "Antix.Html"
deploy "Antix.Http"
deploy "Antix.Net"
deploy "Antix.SignalR"
deploy "Antix.Services"
deploy "Antix.Services.ActionCache"
deploy "Antix.Security"
deploy "Antix.Web"

function deployJS{
	param($project, $name)

	$version = "$version"
	$packagePath = Resolve-Path "$path\source\$project\obj"
	$package = "$packagePath\$name.$version.nupkg"
	$nuspec = "$path\source\$project\$name.nuspec"

	write-Output "packing $nuspec"

	nuget pack $nuspec -Properties version=$version -OutputDirectory "$packagePath" -Verbosity detailed -NonInteractive
	
	write-Output "pushing $package"
	
    nuget push "$package" -ApiKey $apikey -Source $packagesSource -Verbosity detailed -NonInteractive
}

deployJS "Antix.Code.Web" "antix.cellLayout.angularjs"

