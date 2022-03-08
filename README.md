# Bus Driver Simulator Multiplayer

## Enjoy the game - now with other players all around the globe!
![bds](https://i.imgur.com/fP6gvsm.jpg)


This is the same Bus Driver Simulator as before, but with Multiplayer.
![bds2](https://i.imgur.com/H4Z0Z8X.png)

## Create your own multiplayer sessions and join others - it's easy!
Just install the mod, set up the server and client configs, and you're ready to go!  
![bds3](https://i.imgur.com/TD835bs.png)


## Feature set
| Feature       | Current state      |
| ------------- |:------------------:|
|Network code: connection between server and clients|✅Works, optimizations are in progress|
|Synchronizing movement of other players' buses|✅Works|
|Synchronizing buses selected by players|✅Works|
|Synchronizing time on the location for everyone in session|✅Works|
|Interface|✅Functional, but design may be improved in the future|
|Synchronizing lights on buses between players|🟨In progress|
|Displaying nicknames above the buses|🟨In progress|
|Synchronizing rotation and state of wheels on buses|🟥Planned|
|Synchronizing installed upgrades for buses|🟥Planned|
|Synchronizing sounds produced by other players' buses|🟥Planned|
|Synchronizing weather|🟥Planned|
|Synchronizing plates with routes on buses|🟥Planned|

## Installing
The first thing you'll need to do is to download the package from Releases. After that, extract contents of the archive into game directory so that ServerConfig.json and ClientConfig.json are next to the game's .exe file (Bus Driver Simulator.exe).

## Configuring
There are two files for configuring server and client part of this mod: ClientConfig.json and ServerConfig.json. If you only want to connect to other servers without hosting yours, you only need to edit the first one. In other case, edit both. Here is a brief description for both of them:
### ClientConfig.json
* "nickname" is, obviously, your nickname which will be shown to other players in session.
* "usePassword" (can be true or false) - this variable defines if your client will use password while connecting to the server. It makes creating private sessions possible.
* "password" - if previous argument is set to true, the client uses what is in this fiend as a password while connecting to the server. If server's and client's passwords match, the connection is established.
* "serverIp" - it's the IP address of the server you're going to connect to.
* "serverPort" - it's the port of the server (by default it's 2022).

### ServerConfig.json
* "serverName" is the name of the server. It will be visible to others.
* "serverPort" is the TCP&UDP port. As previously said, port forwarding and "white" IP address for server may be mandatory for proper functioning of the mod.
* "passwordRequired" determines if the server will require password when a new player connects. If set to true, password will be checked, and new players will only be able to connect using the correct password.
* "password" is the password which new players will need to enter in ClientConfig.json.
* "map" determines the map that server will be running. Can be set to either of these: "serpukhov", "serpukhovWinter" "keln", "murom", "muromWinter", or "solnechnogorsk".
* "playersLimit" is the number of players who can be on your server simultaneously.
* "startAtNight" determines if the game will start at night. If false, the server time will be set to day.

## Compiling from source (for developers)
### Cloning repository
First off, clone this repository using your tool of choice (Visual Studio's git plugin, GitHub Desktop, Sourcetree etc.)
### Preparing your IDE
1. Make sure you have VS 2019 or 2022 with **.NET desktop development workload installed.**
2. Install .NET 6.0 SDK from [here](https://dotnet.microsoft.com/en-us/download) and install it.
3. Launch CMD or Windows Terminal or PowerShell as an Administrator and execute the following command to install templates for BepInEx:
``dotnet new -i BepInEx.Templates —nuget-source https://nuget.bepinex.dev/v3/index.json``
4. Open the project in Visual Studiom right-click on the **project** and select Properties. Go to Build - Events. In Post build events, enter the following:
``copy /Y "X:\...\BDSM\bin\Debug\netstandard2.0\BDSM.dll" "Y:\...\Bus Driver Simulator\BepInEx\plugins"``,
where X:\\...\BDSM is the path to your local copy of this repository and Y:\\...\Bus Driver Simulator\ is the path to your game folder.
5. Press LCtrl+LShift+B to build the mod. It will be automatically copied to the game directory. Now you can start the game.
### If you want to start the game with MVS debugger 
Finally, click the arrow next to the Build button and click ``BDSM Debug Properties``. In the window, click Create a new profile and select Executable. 
In "Executable" field, locate .exe file of the game using Browse button, and locate the game folder using Working directory field. You can rename your new profile to something like "Build and run". Close Launch Profiles window, and make sure that your newly created profile is set as default launch profile (click on the arrow next to Build button again and make sure that your profile is ticked).

If everything is done correctly, pressing F5 should compile the mod, copy it into the game directory and launch the game.

## Authors
The main developer of this mod is [RidgyTheFox](https://github.com/RidgyTheFox).
[Resident007](https://github.com/Resident007) is responsible for English localization of UI and QA/Testing.
