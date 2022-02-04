$functionName="xxxxxxxxxxxxxxxx-lambda"
$zipName="xxxxxxxxxxxxxxxx.zip"

Push-Location $PSScriptRoot
$project=Split-Path -Path $PSScriptRoot -Leaf
dotnet lambda package --configuration Release --framework netcoreapp3.1
aws s3 cp "bin/Release/netcoreapp3.1/$project.zip" "s3://xxxxxxxxxxxxxxxx/$zipName"
aws lambda update-function-code --function-name  $functionName --s3-bucket xxxxxxxxxxxxxxxx --s3-key $zipName
Pop-Location
