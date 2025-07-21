# 🐾 Wildlife Simulator

**Wildlife Simulator** is a 3D Unity game that immerses players into the life of wild animals in a dense, dynamic forest environment. The game simulates 🦌 survival, 🐺 hunting, 🏃 escaping predators, and 🌲 interacting with the ecosystem — all driven by a powerful **Finite State Machine (FSM)**-based AI system. It aims to teach players about 🧬 animal behavior, 🗺️ territory, and 🛡️ survival instincts through interactive simulation.

---

## 🎮 Game Overview

* 🎯 **Genre:** Simulation
* 🧍 **Perspective:** Third Person
* 🖼️ **Graphics:** 3D, realistic environment with dynamic lighting and weather effects
* 🛠️ **Engine:** Unity 6 (6000.0.39f1)

### 🎯 Gameplay Features:

* 🐾 Choose from different wild animals (e.g., deer, wolf, rabbit, bear)
* 🧠 Survive using instincts like hunting, hiding, and exploring
* 🌊 Interact with dynamic elements like water sources, prey, predators, and shelters
* 🌧️ Environmental challenges: Day-night cycle, weather changes, hunger, thirst, and stamina management
* 🧭 Navigate terrain using Unity's NavMesh system for realistic movement

---

## 🧠 AI System: Finite State Machine (FSM)

Wildlife Simulator features intelligent animal behaviors using FSM (Finite State Machine) to switch between multiple states depending on internal and external conditions. Each animal operates independently based on its current state, leading to organic and immersive interactions.

### 🌀 Example FSM States

#### 🐇 Prey Animals (e.g., Deer, Rabbit)

* 💤 **Idle**: Standing, grazing, or looking around.
* 🚶 **Wandering**: Random movement within safe areas.
* 🏃 **Fleeing**: Triggered by nearby predators; movement increases and direction is randomized.
* 🍃 **Eating/Drinking**: Triggered by hunger/thirst meters.
* 👀 **Alert**: Temporarily focused state if predator is nearby but not close enough to flee.
* 💀 **Dead**: Triggered if stamina runs out while fleeing or if caught by predator.

#### 🐺 Predators (e.g., Wolf, Bear)

* 💤 **Idle**: Resting or patrolling territory.
* 🔍 **Searching**: Looking for prey within detection radius.
* 🏃‍♂️ **Chasing**: Actively pursuing prey using NavMesh pathfinding.
* 🍖 **Eating**: Triggered when prey is caught and predator is hungry.
* 🛌 **Resting**: Regenerates stamina after a chase.

### 🔄 FSM Transitions:

* 📍 Proximity to other animals
* ⚡ Current stamina, hunger, and thirst levels
* 🌙 Time of day or 🌧️ weather conditions (e.g., predators more active at night)
* 🎲 Random variation for realism

---

## 👥 Team Members

* 👨‍💻 **Mostafa Mohamed Ali** – AI Programming, FSM Architecture, and Logic Design
* 🧑‍🎨 **Veronia Wagih Sami** – UI/UX, Environment Art, Asset Integration
* 👩‍🎞️ **Mariam Mohamed Monir** – Character Modeling, Animation, and Visual Effects

---

## 🎧 Audio & Music

* 🎵 Background Music: From YouTube Audio Library
  🔗 [https://youtube.com/watch?v=eNUpTV9BGac](https://youtube.com/watch?v=eNUpTV9BGac)
* 🎙️ Animal sounds and ambient forest effects sourced from Mixkit and FreeSound (royalty-free)

---

## 🔗 Additional Links

* 💻 *((https://github.com/Mustafa-Mohamed26/AI-Wildlife-Simulator/new/main?filename=README.md))* Link to GitHub repository with project code
* 📦 *(Coming Soon)* Download executable build for Windows

---

🌿 Feel free to explore the immersive and realistic wildlife world we've built, and experience life through the eyes of a wild animal in nature's most primal form. Whether you're escaping danger or hunting to survive, **Wildlife Simulator** challenges your instincts and strategy every step of the way.
