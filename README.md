# Description

This is a web app to show Power BI and other reports. Using app owns data.

# Prerequisites

* Microsoft [.net 7.0.102](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) SDK
* [Power BI](https://powerbi.microsoft.com/en-us/) account
* [Power BI workspace](https://docs.microsoft.com/en-us/power-bi/service-create-the-new-workspaces) with reports
* Azure active directory group.
* Enable the [power bi service admin settings and allow service principals to use Power BI API](https://learn.microsoft.com/en-us/power-bi/developer/embedded/embed-service-principal#step-3---enable-the-power-bi-service-admin-settings)
* Azure registered app. [Register app](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app) and [create a client secret](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app#add-a-client-secret)

# Installation
* Clone the repository
```powershell
git clone https://github.com/MarkoTagoma/SiseveebRaportid
```
* Change the principal context ip , domain, username and password in code at [Homecontroller](/Controllers/HomeController.cs) line 47.

* Fill the [appsettings.json](/appsettings.json) example below with your own data
```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "",
   "TenantId": "",
    "ClientId": "",
    "ClientSecret": ""
  },
  "PowerBi": {
    "ServiceRootUrl": "https://api.powerbi.com",
   "WorkspaceId": ""
  },
  "CurrentUser": {
    "Region": ""
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```
* To get [TenantID](https://learn.microsoft.com/en-us/azure/active-directory/fundamentals/active-directory-how-to-find-tenant)
* To get [ClientID](https://learn.microsoft.com/en-us/azure/active-directory/develop/howto-create-service-principal-portal#sign-in-to-the-application)
* To get [ClientSecret](https://learn.microsoft.com/en-us/azure/active-directory/develop/howto-create-service-principal-portal#option-2-create-a-new-application-secret)
* To get WorkspaceID go to the workspace in Power BI and copy the id from the url
* Add the Azure app to the Azure active directory group that was mentioned in the prerequisites.
* Add the Azure app to the [Power BI workspace](https://learn.microsoft.com/en-us/power-bi/collaborate-share/service-give-access-new-workspaces#give-access-to-your-workspace).
```
dotnet build
``` 

# Usage
* To start the project run in the root directory terminal
```
dotnet run
``` 
* Go to [localhost:5249](http://localhost:5249/)
* Click on "Raportid" and log in with a valid active directory user.



# Move to production reports
* Remove the commented line in [ApiService.cs](Services/ApiService.cs) line 106 and comment out the hard coded one.
* Make sure a capacity is assigned to the workspace.

# To add more
* If it is a website/jpg/pdf copy from [Raportid.cshtml](/Views/Home/Raportid.cshtml) line 14 and change the Id to match the website link or pdf/jpg path.

* If it is a PowerBI report copy from [Raportid.cshtml](/Views/Home/Raportid.cshtml) line 24 and change the Id to match the report name as it is in powerbi.

* If there are more groups copy one of the if-s and change the group name then add the reports inside it
# Troubleshooting

## Forbidden
* If you have run out of automatically generated tokens you will get "Forbidden" and have to  generate a token at [Microsoft's page](https://learn.microsoft.com/en-us/rest/api/power-bi/embed-token/generate-token?tryIt=true&source=docs#code-try-0) and paste it to [ApiService.cs](Services/ApiService.cs) line 107 or purchase capacity.