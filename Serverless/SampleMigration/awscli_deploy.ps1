
$functionName=""
$zipName=""
$s3Bucket=""

Push-Location $PSScriptRoot
$project=Split-Path -Path $PSScriptRoot -Leaf
dotnet lambda package --configuration Release --framework netcoreapp3.1
aws s3 cp "bin/Release/netcoreapp3.1/$project.zip" "s3://xxxx-xxxx-xxxx/$zipName"
aws lambda create-function-code --function-name  $functionName --s3-bucket $s3Bucket --s3-key $zipName
Pop-Location