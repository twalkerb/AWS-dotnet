$functionName="samplemigration-lambda"
$zipName="samplemigration.zip"

Push-Location $PSScriptRoot
$project=Split-Path -Path $PSScriptRoot -Leaf
dotnet lambda deploy-serverless xxxxxxxxxxxxxxxx --s3-bucket xxxx----xxxx----xxxx --template serverless.template --region us-east-2
Pop-Location
