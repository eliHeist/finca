## dev publish
dotnet publish -f net10.0-android -c Release -p:AndroidPackageFormat=apk -p:AndroidFastDeploymentEnabled=false -p:AndroidSupportedAbis=arm64-v8a -p:ApplicationID=com.dakdot.financetracker.beta -p:ApplicationTitle="Arupiya Beta"
## production publish
dotnet publish -f net10.0-android -c Release -p:AndroidPackageFormat=apk -p:AndroidFastDeploymentEnabled=false -p:ApplicationID=com.dakdot.financetracker -p:ApplicationTitle="Arupiya"