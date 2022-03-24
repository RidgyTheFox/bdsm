# Features status

## Genereal features
Everything that I would like to do in this fashion is described here. The lower the position of the feature, the more time its implementation is likely going to take.
| Feature | Status | Notes |
|---------|------- |------:|
| Basic position and rotation sync. | ✅ Implemented | N/A |
| Basic GUI. | ✅ Implemented | N/A |
| Settings for client and server. | ✅ Implemented | N/A |
| Automatic loading/unloading scenes. | ✅ Implemented | N/A |
| Time syncing. | ✅ Implemented | N/A |
| Basic bus sync. | ✅ Implemented | When you enter the garage you should disappear to other players. Changing bus should also be visible. |
| Advanced bus sync (Stage 1). | ✅ Implemented | (Lights) Required classes and functions, network packets. |
| All buses sync. | 🟨 Work in progress | Bus classes, lighting layouts, wheels and so on. |
| Advanced bus sync (Stage 2).| 🟥 Planned | Upgrades and liveries sync. |
| Route numbers on route plates. | 🟥 Planned | N/A |
| Finish offset, lights and wheels sync, upgrades for buses. | 🟥 Planned | N/A |

I also have a couple of other ideas, but they are very difficult to implement. I'm not sure if I'll be doing this as Bus World is due to be released soon.
Here are my ideas:
* Weather sync.
* Discord Integration. (This one isn't so difficult actually...)
* NPCs sync.
* Traffic lights sync.
* Anti-lag system. Right now movement works good for mod without any anti-lag or prediction system. But still, it would be cool to have one.
* Workshop support.
* Updater for my mod. This is a small program that you can run and it will update your mod to the latest version.
* Cool GUI. Right now, GUI in this mod is based on IMGUI. This is a deprecated system for GUI in Unity. Usually, developers are using this kind of a GUI for debugging. But you know... I dont actually care: it works, and it's enough for me. (Or: It works? Don\`t touch!)
* Random license plates with procedural PBR material. I think i know how to do it, but it requires so many time. 

## Bus features status
Here you will find a table with all the buses in the game. Each bus is controlled by its own class. And writing these classes takes a huge amount of time. To see what has already been done, you can refer to this plate in the indev branch. Lines contain buses. And each column is one or another part of this bus.

| Bus name      | Basic movement| Offset      | Lights        | Wheels Sync   | Upgrades      | Animations    | Sounds         | Passengers |
| ------------- |---------------|-------------|---------------|---------------|---------------|---------------|----------------|-----------:|
| LIAZ 677 | ✅ Implemented | ✅ Implemented | ✅ Implemented | ✅ Implemented | 🟥 Planned | 🟥 Planned | 🟥 Planned |  🟥 Planned |
| Citaro   |✅ Implemented |  🟨 Work in progress |  🟨 Work in progress |  🟨 Work in progress |  🟥 Planned |  🟥 Planned |  🟥 Planned | 🟥 Planned |
| MAN |✅ Implemented |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned | 🟥 Planned |
| MAN 15 |✅ Implemented |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned | 🟥 Planned |
| Citaro L |✅ Implemented |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned | 🟥 Planned |
| Icarus |✅ Implemented |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned | 🟥 Planned |
| LAZ 695 |✅ Implemented |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned | 🟥 Planned |
| Sprinter |✅ Implemented | ✅ Implemented | ✅ Implemented | ✅ Implemented |  🟥 Planned | 🟥 Planned |  🟥 Planned |  🟥 Planned |
| PAZ 672 | ✅ Implemented |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned | 🟥 Planned|
| LAZ 699 |✅ Implemented |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned | 🟥 Planned |
| LIAZ 5292 | ✅ Implemented |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned |  🟥 Planned | 🟥 Planned
