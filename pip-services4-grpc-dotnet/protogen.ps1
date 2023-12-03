#!/usr/bin/env pwsh

##Set-StrictMode -Version latest
$ErrorActionPreference = "Stop"

# Get component data and set necessary variables
$component = Get-Content -Path "component.json" | ConvertFrom-Json

$docsImage="$($component.registry)/$($component.name):$($component.version)-$($component.build)-proto"
$container=$component.name

# Remove old generate files
if (Test-Path "src/Protos") {
    Remove-Item -Path "src/protos/*" -Force -Include Commandable.cs
}

if (Test-Path "example/Protos") {
    Remove-Item -Path "example/Protos/*" -Force -Include Dummies.cs
}

# Build docker image
docker build -f docker/Dockerfile.proto -t $docsImage .

# Create and copy compiled files, then destroy
docker create --name $container $docsImage
docker cp "$($container):/app/src/Protos" ./src/
docker cp "$($container):/app/example/Protos" ./example/
docker rm $container
