# Roguelike Arena – Technical Evaluation Project
Overview

This project is a roguelike arena game developed as part of a technical evaluation. The core design focuses on system reusability, clean architecture, and extensibility, while delivering a complete and playable experience within a limited timeframe.

The game challenges the player to survive a sequence of enemy waves while progressively improving their character through equipment and stat upgrades. Although designed with replayability in mind, the scope was intentionally constrained to prioritize stability, code quality, and core gameplay systems.

# Game Objective

The primary objective of the game is to survive all five combat waves inside the arena.

To achieve this, the player must:

Defeat enemies across five fixed waves

Collect and equip better items to improve stats and combat effectiveness

Manage equipment and inventory between waves

Adapt combat strategy based on available weapons and item modifiers

The game is designed so that different item combinations and weapon behaviors can result in varied playstyles, even with a limited number of waves.

# Controls
Action	|  Input

Move	|  W / A / S / D

Attack	|	 Left Mouse Button

Open Inventory	|	Tab

Interact	|  E

# Core Systems

Entity System
Shared base architecture for players, enemies, NPCs, and interactable objects.

Stats & Items
Modular stat system allowing items to dynamically modify entity attributes, including support for exclusive or newly introduced stats.

Combat System
Weapon behavior is defined via ComboData objects, enabling flexible attack patterns and progression without modifying core systems.

Inventory System
Built using Scriptable Objects for clean data organization, easy expansion, and automatic save persistence.

# Assets & External Resources

The following asset packs were used:

POLYGON – Pirate Pack

POLYGON – Fantasy Rivals Pack

POLYGON – Fantasy Characters Pack

Animations were sourced from Mixamo.

# Notes

Some initially planned features (such as wave modifiers, enchanted items, and a final boss) were intentionally removed to ensure a stable and complete core experience within the evaluation timeframe. Despite this, the project architecture was designed to allow these features to be added later with minimal refactoring.
