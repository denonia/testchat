# TestChat

Real-time chat with sentiment analysis, made with SignalR and Azure Cognitive Services. The client application was made
with Blazor WebAssembly.

Supports public and private messaging between users. Saves all messages and sentiment analysis results to database.

### Client life cycle

* Client connects to the SignalR hub located at `/chathub`. Upon connection the server sends information about all
  currently online users, their IDs and names with **UserOnline** messages.
* Client may send a **SendMessage** message with target's ID for a private message or **SendPublicMessage**.
* Client may change their name with **ChangeName** message. In response, server sends **ChangeNameResult** with a
  boolean value indicating whether the change was successful.
* When someone connects/disconnects, all other users get the **UserJoined**/**UserLeft** message with their ID.
* When someone successfully changes name, all users get the **UserChangeName** message with the new name.
* When someone sends a public message, all users get the **ReceivePublicMessage** message with its text and sentiment
  analysis results.
* When someone sends a private message, only the sender and the target get the **ReceiveMessage** message.

### Deploying to Azure

1. Go to [Azure Portal](https://portal.azure.com/) and create a group with the following resources:
    * Web Apps for client and server
    * Azure SQL Server with a single database (use SQL authentication)
    * Azure SignalR Service
    * Azure Language Service
2. Configure access to the SQL Server: set up a private network or go to Security -> Networking -> Check Allow Azure
   services and resources to access this server
3. Configure the server (TestChat.Server/appsettings.Production.json):
   ```json

   {
     "Azure": {
       "SignalR": {
         "Enabled": "true",
         "ConnectionString": "Endpoint=https://xxxxx.service.signalr.net;AccessKey=xxxxx=;Version=1.0;"
       }
     },
     "ClientUrl": "https://xxxxx.azurewebsites.net",
     "AzureTextAnalyticsServiceUrl": "https://xxxxx.cognitiveservices.azure.com/",
     "AzureTextAnalyticsServiceApiKey": "xxxxx",
     "ConnectionStrings": {
       "AzureSQLDatabase": "Server=tcp:xxxxx.database.windows.net,1433;Initial Catalog=xxxxx;Persist Security Info=False;User ID=xxxxx;Password=xxxxx;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
     }
   }
   ```
   Copy the following data for created services from Azure Portal:
    * Connection strings for **SignalR Service** and **Azure SQL Database**
    * **Language Service** endpoint and key
    * Client URL
4. Configure the client (TestChat.Client/wwwroot/appsettings.Production.json) with a URL to the server chat hub:
   ```json
   {
     "ChatHubUrl": "https://xxxxx.azurewebsites.net/chathub"
   }
   ```
5. Publish the client and server with your preferred IDE (for JetBrains Rider: make sure Settings -> Configuration ->
   Enable SCM Basic Auth Publishing Credentials is checked in both apps)

### Running a local instance

1. For local environment, you only need **Azure Language Service** and optionally **Azure SQL Server** (or just use a
   local Microsoft SQL Server instance)
2. For Azure SQL, make sure your IP address is added in firewall rules in Security -> Networking
3. TestChat.Server/appsettings.Development.json example:
   ```json
   {
      "ClientUrl": "https://localhost:7059",
      "AzureTextAnalyticsServiceUrl": "https://xxxxx.cognitiveservices.azure.com/",
      "AzureTextAnalyticsServiceApiKey": "xxxxx",
      "ConnectionStrings": {
       "AzureSQLDatabase": "Server=tcp:xxxxx.database.windows.net,1433;Initial Catalog=xxxxx;Persist Security Info=False;User ID=xxxxx;Password=xxxxx;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
      }
   }
   ```