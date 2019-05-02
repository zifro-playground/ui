
# Builds and pushes the docker images

# THIS SCRIPT IS MEANT TO BE USED FOR DEVELOPMENT PURPOSES
# DO NOT USE IN PRODUCTION

param(
    # Docker image account
    [string]
    $account = "zifrose",
    
    # Docker images
    [string[]]
    $images = @(
        "unity3d-webgl",
        "unity3d"
    ),

    # Dockerfile config to use
    [string]
    $DockerFile = ".Dockerfile",

    # Unity version, also used as Docker image tag
    [string]
    $UnityVersion = "2018.3.11f1"
)

function GetBaseImage ([string] $Image, [string] $Version) {
    if ($Image.Contains('-')) {
        return $Version + $Image.SubString($Image.IndexOf('-'))
    } else {
        return "$Version-unity"
    }
}

$basePath = Resolve-Path $PSCommandPath | Split-Path -Parent

Write-Host ">>> Building" -BackgroundColor Green
$step = 0
$steps = $images.Count * 2

foreach ($image in $images) {
    $step++;
    $imageFullName = "$account/$($image):$UnityVersion"
    $file = Join-Path $basePath $DockerFile
    Write-Host "> Building $imageFullName docker image (step $step/$steps)" -BackgroundColor DarkGreen
    Write-Host ""
    Write-Host "ARG UNITY_VERSION=$(GetBaseImage $image $UnityVersion)"
    docker build . -t $imageFullName -f $file --build-arg UNITY_VERSION=$(GetBaseImage $image $UnityVersion)
    if (-not $?) {
        throw "Failed to build $imageFullName (step $step/$steps)"
    }
    Write-Host ""
}

Write-Host ">>> Pushing" -BackgroundColor Blue

foreach ($image in $images) {
    $step++;
    $imageFullName = "$account/$($image):$UnityVersion"
    Write-Host "> Pushing $imageFullName docker image (step $step/$steps)" -BackgroundColor DarkBlue
    Write-Host ""
    docker push $imageFullName
    if (-not $?) {
        throw "Failed to push $imageFullName (step $step/$steps)"
    }
    Write-Host ""
}

Write-Host "<<< Build and push complete" -BackgroundColor Cyan
