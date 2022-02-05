$functionName=""
$template=""
$s3Bucket=""
$awsregion=""

Push-Location $PSScriptRoot
dotnet lambda deploy-serverless $functionName --s3-bucket $s3Bucket --template $template --region $awsregion
Pop-Location