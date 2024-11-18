# Memory Game with Unity and Firebase

## Quick Start Guide

### Prerequisites
- Unity 2021.3.44f1
- Android Unity Module
- Firebase SDK for Unity

### Setup Instructions
1. Clone the repository.
2. Open the project in Unity.
3. Set project target build to Android.
3. Open `Main Menu Scene` and press Play.

## Design Decisions
- Used **ScriptableObjects** for modular card management.
- Implemented **Singleton Pattern** for Firebase management,Audio System, Pooling Manager.
- Utilized Coroutine-based **card flip animation** and **card matched animation**.
- Created reusable **ShuffleUtility** to randomize card order.
- Encapsulation for maintaning clean code
- Object Pooling Pattern for optimization (Cards, Leaderboard Entry, Audio Source)
- Modular Game Manager can add Levels for future improvements without affecting the script.

## Other Details
- intended not to implement show player highscore, basically player can input a name and save highscore with that name same as arcade games leaderboards in amusement parks.
   


### Features
- 7 Difficulty Levels
- Firebase Leaderboard
- Card Animations & VFX
- Music & SFX
 
