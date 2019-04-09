
# Builds and pushes the docker images

# THIS SCRIPT IS MEANT TO BE USED FOR DEVELOPMENT PURPOSES
# DO NOT USE IN PRODUCTION


param(
    # Docker image account
    [string]
    $Account = "zifrose"
)

Write-Output ">>> Building $Account/ui-upm docker image (1/4)"
docker build . -t $Account/ui-upm -f ui-upm.Dockerfile
Write-Output "<<< Successfully built $Account/ui-upm docker image"
Write-Output ""

Write-Output ">>> Building $Account/ui-testrunner docker image (2/4)"
docker build . -t $Account/ui-testrunner -f ui-testrunner.Dockerfile
Write-Output "<<< Successfully built $Account/ui-testrunner docker image"
Write-Output ""

Write-Output ">>> Uploading $Account/ui-upm docker image (3/4)"
docker push $Account/ui-upm
Write-Output "<<< Successfully uploaded $Account/ui-upm docker image"
Write-Output ""

Write-Output ">>> Uploading $Account/ui-testrunner docker image (4/4)"
docker push $Account/ui-testrunner
Write-Output "<<< Successfully uploaded $Account/ui-testrunner docker image"
Write-Output ""

Write-Output "Build and push complete"