# ARR Server Management Dashboard

## To set up to use your own AAD 
Go to the Azure portal and create a new App Registration and make a note of the domain, tenantId, ClientId and client secret. Find the appsettings.json file (in the root of the project) and enter those values as below:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "yourdomain.onmicrosoft.com",
    "TenantId": "your tenant id",
    "ClientId": "your client id",
    "CallbackPath": "/signin-oidc"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

## To configure for your own Azure Remote Rendering account 
Right-click on the Project in Solution Explorer of Visual Studio. Select the option 'Manage User Secrets' which will open a file called secrets.json and add elements as follows:

```json
{
  "Kestrel:Certificates:Development:Password": "your kestrel pwd",
  "Authentication:AzureAd:ClientSecret": "your client secret",
  "ARR:AccountKey": "your account key",
  "ARR:AccountId": "your account id",
  "Azure:StorageConnectionString": "your azure storage connection string"
}
```

NOTE: This file should never be submitted to source control.

Or, if you are deploying the site to Azure then you can set these values in the portal under App Service > Configuration.

Then log into the website with your Azure subscription account credentials and you will be able to monitor running ARR VMs, start and stop them, launch the inspector (without credentials).
