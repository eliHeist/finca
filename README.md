## debug build
dotnet publish -f net10.0-android -c Release -p:ApplicationId=com.dakdot.financetracker.beta "-p:ApplicationTitle=Finance Tracker Beta" -p:AndroidPackageFormat=apk -p:AndroidFastDeploymentEnabled=false
## prod build
dotnet publish -f net10.0-android -c Release -p:AndroidPackageFormat=apk -p:AndroidFastDeploymentEnabled=false