# Online Pişti Game (LiteNetLib + Unity)

An online multiplayer Pişti (card game) built using Unity and a dedicated .NET Console Server powered by [LiteNetLib](https://github.com/RevenantX/LiteNetLib).

## 🎮 About the Game

This is a classic Pişti card game with online multiplayer capabilities. Players can:

- Connect to the dedicated server using their nickname
- Create or join game rooms
- See all active players and available rooms in the game scene
- Play by clicking on cards, following traditional Pişti rules
- Compete for the highest score, which is displayed at the end of the game
- Return to the main lobby after each match

## ⚙️ Features

- **Dedicated Server:** .NET ConsoleApp using LiteNetLib for all networking
- **Lobby System:** Includes room creation, joining, and player listing
- **No Backend Auth Yet:** Players connect by entering a nickname; no authentication code or backend integration currently
- **Real-Time Room and Player List:** All players and rooms are listed live in the game scene
- **Room Settings:** Configurable room sizes; full rooms are locked for new entries
- **Card Game Mechanics:** Classic Pişti gameplay with simple point-and-click interaction

## 🧠 Architecture

### 🧱 Server-Side

- **LiteNetLib:** Lightweight, high-performance networking for reliable UDP communication
- **ObjectManager:** Generic object pooling system to manage reusable instances efficiently
- **Room System:** Supports dynamic room creation and player management without external services

### 🎮 Client-Side (Unity)

- **AssetManager:** Generic support for Unity Addressables for efficient asset loading
- **PlayerController:** Built using SOLID principles, with an abstract base class to manage both player and AI behavior
- **Singleton Pattern:** Used for dependency management (e.g., managers) to keep things simple and clean

## 🛠️ Tech Stack

- Unity (Client)
- .NET Console App (Server)
- LiteNetLib (Networking)
- Addressables (Asset Management)
- C# (Game Logic and Infrastructure)

## 🚀 Getting Started

> Note: Authentication is not implemented yet. Players join with a nickname only.

1. **Run the server**  
   Open the `.NET ConsoleApp` server project and run it. Make sure ports are open if testing over network.

2. **Open the Unity project**  
   Launch the Unity client. Enter a nickname to connect to the server.

3. **Create or Join a Room**  
   You can either create a new room or join an existing one from the lobby screen.

4. **Play the Game!**  
   Once in a room with the minimum required players, the game begins automatically.

## 📌 Roadmap

- Add backend support for authentication and account management
- Improve room and matchmaking UX
- Add AI opponents for solo play
- Polish UI/UX and add animations
- Deploy server for public access

## 📄 License

This project is licensed under the MIT License.
