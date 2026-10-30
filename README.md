# Tic Tac Toe: AI and WiFi LAN Multiplayer

A Tic Tac Toe game made in **Unity 6** with **C#** and **AI**.
You can play against three AI levels, play with a friend on the same device, or play with a friend over WiFi LAN.

## Features

- **Three AI levels**
  - **Easy:** plays a random empty cell.
  - **Smart:** wins if it can, blocks the player if needed, otherwise plays random.
  - **Hard:** uses the **Minimax algorithm**, so it plays the best possible move.
- **Local 2-player mode** (P1 vs P2 on one device)
- **WiFi LAN 2-player mode** with Netcode for GameObjects (Host plays X, Client plays O)
- Home page, mode selection, pause, restart, back and result pages (win / draw)
- Builds for Windows and Android

## Tech Stack

| Item | Version / Name |
|---|---|
| Engine | Unity 6.3 LTS (6000.3.13f1) |
| Language | C# |
| UI | Unity UI + TextMeshPro |
| Multiplayer | Netcode for GameObjects + Unity Transport (LAN only, no Relay or Lobby) |

## How to Run

1. Clone this repository.
2. Open it with **Unity Hub** using Unity **6000.3.13f1**.
3. Open the menu scene from `Assets/Scenes` (the one with "UI" in its name) and press **Play**.
4. Make sure both scenes (menu and game) are in **Build Profiles > Scene List**.

## How to Play on LAN

1. Both players must be on the **same WiFi network**.
2. **Player 1 (Host):** open the Online page and press **Host**. Your IP address is shown on the screen.
3. **Player 2 (Client):** type the Host's IP address and press **Connect**.
4. When the friend has joined, the Host presses **Start**. Both players move to the game scene.
5. Host plays **X** and moves first. Client plays **O**.

Notes:
- The game uses port **7777 (UDP)**. On Windows, allow the game through the **Private network** firewall.
- Some routers block devices from talking to each other (AP isolation). If it does not connect, try a mobile hotspot.
- To test on one PC: run one build as Host, and join from the Editor with IP `127.0.0.1`.


## Code Structure

```
Assets/Scripts/TicTacToe/
├── AI/        IAiStrategy, EasyAi, SmartAi, MinimaxAi, AiFactory
├── Core/      AiMode, PlayerMoves, WinChecker
├── UI/        BoardView, UiManager, OnlineMenuUI
├── Network/   OnlineSession, NetworkGameSync
└── TicTacToeGameController.cs
```

How the code is organized (OOP):

- **Strategy pattern:** every AI level implements `IAiStrategy`. `AiFactory` creates the right one.
  A new AI level can be added without changing the game controller.
- **Single responsibility:** `WinChecker` only checks wins, `BoardView` only handles the board buttons,
  `PlayerMoves` only stores a player's moves, `UiManager` only handles pages and navigation.
- **Host decides moves in LAN mode:** a client sends only the cell number to the Host.
  The Host checks that it is the right turn and the cell is empty, then tells both players.
  `NetworkGameSync` is the only NetworkObject. The NetworkManager is created at runtime by `OnlineSession`,
  and only 2 players are allowed.

## What I Built and What I Used AI For

I want to be clear about my work:

- **Written by me:** the original game logic, the three AI levels, the UI scenes and flow,
  and the local 2-player mode.
- **Built with AI assistance (Claude):** splitting the code into separate OOP classes,
  and the LAN multiplayer structure with Netcode.
- I reviewed and tested this code, and I can explain how it works.

## Known Limitations

- The Host's IP address must be typed by hand (no automatic room finding yet).
- Online play has no reconnect. If a player leaves, the game ends.
- Pause is turned off in online mode.

## Future Plans

- Score board and "your turn" indicator
- Automatic LAN discovery
- Sound effects and animations
- Unit tests for `WinChecker` and the AI classes
- Relay or Lobby
- Voice chate
## Author

**Shubham Gautam**
GitHub: [SG-Game-Dev](https://github.com/SG-Game-Dev)

https://github.com/user-attachments/assets/d02e72a6-972f-44a6-8ff0-ba3fcac7832c

