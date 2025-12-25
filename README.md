# MyFirstFPS

![Status](https://img.shields.io/badge/Status-In--Development-yellow)
![Platform](https://img.shields.io/badge/Platform-Windows-blue)
![Engine](https://img.shields.io/badge/Engine-Unity%20/%20C%23-green)

A modular First-Person Shooter framework built with a focus on clean architecture and scalable game logic. This project serves as a practical implementation of professional GitFlow workflows and MVP (Model-View-Presenter) design patterns.

---

## 🚀 Overview
**MyFirstFPS** is more than just a shooter; it is an exploration into professional software engineering within game development. The goal is to create a robust system for weapon handling, player movement, and AI interaction that is easy to extend and test.

### Key Features
* **Modular Movement System:** Physics-based player controller.
* **Weapon Engine:** Scalable system for hitscan and projectile-based weapons.
* **Event-Driven UI:** Decoupled UI components using an Observer pattern.
* **Clean Architecture:** Strict separation between Game Core, UI, and Input.

---

## 🛠 Tech Stack
* **Language:** C#
* **Game Engine:** Unity (v2022.3+ recommended)
* **Version Control:** GitHub (Following GitFlow standards)
* **Patterns:** MVP, Singleton, Command Pattern.

---

## 📂 Project Structure
Following enterprise standards, the source code is organized by responsibility:

```text
MyFirstFPS/
├── Assets/
│   ├── _Project/            # All custom code/assets
│   │   ├── Scripts/
│   │   │   ├── Core/        # Game loop & Managers
│   │   │   ├── Player/      # Movement & Input
│   │   │   ├── Weapons/     # Combat logic
│   │   │   └── UI/          # View controllers
│   │   ├── Prefabs/
│   │   └── Textures/
├── Docs/                    # Design specifications
└── Tests/                   # Unit & Integration tests
