## dev publish
dotnet publish -f net10.0-android -c Release -p:AndroidPackageFormat=apk -p:AndroidFastDeploymentEnabled=false -p:RuntimeIdentifiers=android-arm64 -p:ApplicationID=com.dakdot.financetracker.beta "-p:ApplicationTitle=Arupiya Beta"
## production publish
dotnet publish -f net10.0-android -c Release -p:AndroidPackageFormat=apk -p:AndroidFastDeploymentEnabled=false -p:ApplicationID=com.dakdot.financetracker -p:ApplicationTitle="Arupiya"