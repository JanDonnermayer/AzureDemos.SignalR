# AzureDemos.SignalR

A simple example on how different agents can communicate in an event-based manner using Microsoft Azure SignalR.

## Setup

1. Clone this repository
1. Go to the [Azure portal](https://portal.azure.com/#create/hub) and create a SignalR service instance.
1. Copy the connection string.
1. Apply the connection string as user-secrets to the apps:

```powershell
dotnet user-secrets set "SignalR:ConnectionString" "<Your con. string>" --project "app\AzureDemos.SignalR.PublisherApp"
dotnet user-secrets set "SignalR:ConnectionString" "<Your con. string>" --project "app\AzureDemos.SignalR.ConsumerApp"
```

Run the apps:

```powershell
dotnet run "app\AzureDemos.SignalR.PublisherApp"
```

```powershell
dotnet run "app\AzureDemos.SignalR.ConsumerApp"
```
