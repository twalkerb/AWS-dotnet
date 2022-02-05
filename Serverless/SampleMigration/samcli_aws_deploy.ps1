$stackname=""
$awsregion=""
$template=""
$buildtemplate=".aws-sam\build\template.yaml"
$s3Bucket=""
$capabilitiesIAM="CAPABILITY_IAM" 
$configFile="samconfig.toml" 
$configEnv="default"


Push-Location $PSScriptRoot
sam build --template $template
sam deploy --template-file $buildtemplate --stack-name $stackname --region $awsregion --s3-bucket  $s3Bucket --no-disable-rollback --capabilities $capabilitiesIAM --config-file $configFile --config-env $configEnv
Pop-Location