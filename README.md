# Birdie
Chat room and a bot to retrieve stock information.

## Technologies
* DotNetCore 5
* WebApi
* PostgreSQL
* EntityFrameworkCore
* Redis
* RabbitMQ
* WebSockets
* JWT

##### Birdie.API
This project has all endpoints for authentication, room and socket management.
##### Birdie.Data
This project has the DbContext for entity management and repositories.
##### Birdie.Processor
This project has HostedService for handling rabbitMQ messages.
##### Birdie.Service
This project has all services required (business layer, DTOs, message producers, socket message handling).
##### Birdie.UnitTests
This project has the unit tests for the Business layer.

##### Configuration
In order to run this project is required to have the following settings:
```
{
  "ConnectionString": "postgresql",
  "PrivateKey": "",
  "RabbitMQ:HostName": "",
  "RabbitMQ:UserName": "",
  "RabbitMQ:Password": "",
  "RabbitMQ:VirtualHost": "",
  "RabbitMQ:Port": -1,
  "Queue:stocks": "stocks-queue",
  "Queue:chat": "chat-queue",
  "JwtKey": ""
}
```

Redis is being used as a secret repository, its address is retrieved by environment variables:
```
"environmentVariables": {
    "RedisHost": "",
    "RedisPass": ""
}
```
Have them in your launchSettings or as env variables (which is better). The key being read on Redis is "secrets". 

**TL;DR;** _just_have the settings above as value of a "secrets" name key-value pair on Redis._

##### Migrations
Open a terminal and navigate to root path of the project (e.g. /dev/birdie; c:\dev\birdie)

_dotnet ef migrations add Start -s .\Birdie.API\ -p .\Birdie.Data\ --verbose_
_dotnet ef database update -s .\Birdie.API\ -p .\Birdie.Data\ --verbose_

--verbose is optional

**Note:** migrations won't work if your Redis isn't setup properly

##### Start
###### Services
dotnet run for both executable (Birdie.API and Birdie.Processor) - both projects require the Redis configuration above
###### Web
Under _/birdie/Birdie.API/web_ you will find the html files to execute the chat application

To access just hit http://localhost:5000/web/login.html in the browser once the API project is running
