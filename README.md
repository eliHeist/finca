## prod build
dotnet publish -f net10.0-android -c Release -p:ApplicationId=com.dakdot.financetracker -p:AndroidPackageFormat=apk -p:AndroidFastDeploymentEnabled=false
## debug build
dotnet publish -f net10.0-android -c Release "-p:ApplicationTitle=FT Beta" -p:AndroidPackageFormat=apk -p:AndroidFastDeploymentEnabled=false