
# Opens the current folder with the docker image

# THIS SCRIPT IS MEANT TO BE USED FOR DEVELOPMENT PURPOSES
# DO NOT USE IN PRODUCTION

param(
    # Unity license
    [ValidateScript({
        if(-Not ($_ | Test-Path) ){
            throw "File or folder ""$_"" does not exist"
        }
        if(-Not ($_ | Test-Path -PathType Leaf) ){
            throw "The Path argument must be a file. Folder paths are not allowed."
        }
        if($_ -notmatch "(\.ulf)$"){
            throw "The file specified in the path argument must be filetype .ulf"
        }
        return $true 
    })]
    [System.IO.FileInfo]
    $UnityLicenseULF = "C:\ProgramData\Unity\Unity_lic.ulf",

    # Docker image
    [string]
    $DockerImage = "zifrose/unity3d"
)

Write-Output "Using Unity license $UnityLicenseULF"
Write-Output "Using Docker image $DockerImage"
Write-Output ""

$UnityLicenseContent = Get-Content -Path $UnityLicenseULF -Raw
$UnityLicenseBytes = [System.Text.Encoding]::UTF8.GetBytes($UnityLicenseContent)
$UnityLicenseB64 = [Convert]::ToBase64String($UnityLicenseBytes)

$UnityID = Get-Credential -Message "Enter UnityID login"

docker run -it --rm `
    -e "UNITY_USERNAME=$($UnityID.UserName)" `
    -e "UNITY_PASSWORD=$($UnityID.GetNetworkCredential().Password)" `
    -e "UNITY_LICENSE_CONTENT_B64=$UnityLicenseB64" `
    -e "TEST_PLATFORM=linux" `
    -e "WORKDIR=/root/repo" `
    -v "$(Get-Location):/root/repo" `
    $DockerImage `
    '/bin/bash'

# To generate the .alf file run this:
#
# xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
#  /opt/Unity/Editor/Unity \
#  -logFile \
#  -batchmode \
#  -username "$UNITY_USERNAME" -password "$UNITY_PASSWORD"

# Then copy the XML and paste into a new file and name if unity3d.alf
# Upload the .alf to https://license.unity3d.com/manual
