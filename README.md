# Find Maestro

**Find Maestro** is a first‑person exploration game about the conflict between AI‑generated art and human creativity.  
You wake up in an abandoned factory, read environmental clues, collect memory fragments, avoid AI brainwashing rays, and ultimately find who is the true “Maestro” of creation.

---

## Game Overview

- **Genre** : First‑person exploration / environmental narrative / light puzzle / immersive experience
- **Theme** : Exploring genuine creativity and looking for the balance between AI and human creativity 
- **Gameplay** : Walk through a ruined factory, interact with computers, posters, and diaries, collect glowing fragments, evade scanning AI rays, and experience a three‑tier story (Ruins → Awakening → Prosperity).

- **Current Version** : Vertical slice (first two layers complete, third layer in planning)

---

## Controls

| Action | Key / Mouse |
|--------|-------------|
| Move | `W` `A` `S` `D` |
| Look around | Mouse |
| Interact / Read / Talk | `E` |
| Jump | `Space`  |
| Pause (if implemented) | `Esc` |

---

## How to Run

### Requirements
- **Unity 2022.3 LTS** or newer with **Universal 3D (URP)** support.
- Git LFS (for large asset files, if any).

### Steps
1. Clone the repository (make sure Git LFS is installed if needed):
   ```bash
   git clone https://github.com/your-username/FindMaestro.git
   ```
2. Open **Unity Hub** → **Add project** → select the **inner** `FindMaestro` folder (the one containing `Assets/`).
3. Open the main scene:  
   `Assets/_Project/Scenes/First_Level_gameplay.unity` 
4. Press **Play** in the Unity Editor.

> ⚠️ If you see pink materials, use `Edit → Rendering → Materials → Convert Selected Built‑in Materials to URP`.

---

## Project Structure

The repository has two nested folders:

```
FindMaestro/                ← Git root (outer)
├──  .git/
├──  .gitignore
├──  .gitattributes
├──  README.md
├──  FindMaestro/              ← Unity project  folder (inner)
   ├── Assets/
   │   ├──  _Project/             ← Your custom scripts, scenes, materials, prefabs
   │         ├──  Scripts/
   │         ├──  Scenes/
   │         ├──  Materials/
   │         ├──  Prefabs/
   │         ├──  UI/
   │   ├──  StarterAssets/        ← First‑person controller (third‑party)
   │   ├──  AbandonedFactory/      ← Environment models (third‑party)
   │   └── ...
   ├──  Packages/
   └──  ProjectSettings/
```

---

## Development Environment

| Tool | Version / Detail |
|------|------------------|
| **Unity** | 2022.3 LTS (URP) |
| **C#** | .NET 4.x |
| **IDE** | Visual Studio 2022 / VS Code |
| **Version control** | Git + GitHub (Git LFS for large assets) |
| **Render Pipeline** | Universal Render Pipeline |

---

## Asset Credits

All third‑party assets are used under their respective licenses (free for non‑commercial educational use).  
Modifications (material conversion, collision setup, script integration) were done by the developer.

### Scenes and Items

| Asset | Purpose | Source |
|-------|---------|--------|
| **Starter Assets – First Person Controller (URP)** | Player movement, input, basic camera | [Asset Store](https://assetstore.unity.com/packages/essentials/starter-assets-firstperson-urp-196525) |
| **Abandoned Factory (Lite)** | 3D environment (walls, machines, props) | [Asset Store](https://assetstore.unity.com/packages/3d/props/industrial/abandoned-factory-lite-62597) |
| **Free Sci-Fi Office Pack** | Basic office environment (desk, chair, computer, monitor) | [Asset Store](https://assetstore.unity.com/packages/3d/environments/sci-fi/free-sci-fi-office-pack-195067) |
| **Low-poly Office Set #1 [+140 Models][VNB]** | Additional office models (desk, chair, computer, monitor) for the office environment | [Asset Store](https://assetstore.unity.com/packages/3d/props/low-poly-office-set-1-140-models-vnb-327126) |
| **Quick Outline** (optional) | Outline effect for interactable objects | [Asset Store](https://assetstore.unity.com/packages/tools/particles-effects/quick-outline-115488) |

### Unity Official Packages

| Package | Usage |
|---------|-------|
| **Universal Render Pipeline (URP)** | Rendering, post‑processing, lighting |
| **Cinemachine** | Camera transitions (readable objects, cutscenes) |
| **Input System** | Unified input handling |
| **TextMeshPro** | UI and 3D text rendering |

## Audio Credits

All sound effects are used under the **Pixabay License** (royalty‑free, commercial use allowed). Attribution is provided voluntarily; each entry includes the author name and original source link.

| Sound File | Usage | Author (Pixabay) | Source Link |
|------------|-------|------------------|--------------|
| `Beam` | AIScanner envirment sound | *[gustavorezende]* "https://pixabay.com/users/gustavorezende-1488336/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=272990" | *[https://pixabay.com/sound-effects/film-special-effects-beam-272990/]* |
| `Short energy beam shot(3)` | AI scanner hits the player | *[Yodguard]* "https://pixabay.com/users/yodguard-12455005/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=482517" | *[https://pixabay.com/sound-effects/film-special-effects-short-energy-beam-shot-3-482517/]* |
| `Pick up or found it secret item` | Activate storyline | *[freesound_community]* "https://pixabay.com/users/freesound_community-46691455/" | *[https://pixabay.com/sound-effects/technology-pick-up-or-found-it-secret-item-104874/]* |
| `Keys_pickup` | Player picks up creativity fragment | *[freesound_community]* "https://pixabay.com/users/freesound_community-46691455/" | *[https://pixabay.com/sound-effects/household-keys-pickup-27204/]* |
| `sector.mp3` | Total background music | *[SRG774]* | *[https://opengameart.org/content/dark-sci-fi-audio-pack]* |

---

> **Note**: If any sound was modified (e.g., trimmed, pitch‑shifted, layered), it is indicated in the “Usage” column. All other sounds are used as‑is.

### Self‑Generated / Modified Assets

| Asset | Description |
|-------|-------------|
| **All C# scripts** | `ReadableNote`, `NPCDialogue`, `WakeUp`, `DoorLock`, `ComputerTerminal`, `DialogueManager`, `FragmentManager`, `CreativityFragment`, `AIScanner`, `HorizontalMover`, `Level2Exit`, `Billboard`, etc. – fully written by the developer. |
| **UI panels** | Dialogue panel, hint panel, objective panel, fragment counter – created in‑project. |
| **Materials** | Highlight materials (orange emission + Bloom), fragment glow material – created from scratch. |
| **“Press E” 3D text** | Floating interaction hint with Billboard script. |
| **Creativity fragment model** | Simple cube with custom glow material. |
| **Level design & lighting** | All scene layout, lighting adjustments, fog, post‑processing settings. |
| **Narrative texts** | All posters, computer logs, diary entries, NPC dialogues, ending lines – original writing. |

---

## Design Decisions & Key Features

- **Three‑tier reverse narrative** : Ruin (AI‑dominated) → Awakening (collection + evasion) → Prosperity (AI as tool, planning).
- **Exploration‑driven storytelling** : No combat, only reading, observing, and interacting.
- **Player‑triggered tutorial** : Operate‑based steps (WASD → mouse → E → keyboard input) to guide the player through the game.
- **Wake‑up sequence** : Procedural camera animation (blink, head‑shake, stagger) to immerse the player.
- **Interactable system** : Universal `ReadableNote` script for any readable object (supports Cinemachine camera switch, 3D hint, material highlight, player lock).
- **Dialogue system** : `NPCDialogue` / `DialogueManager` with typewriter effect, speaker name, and “Press E to continue” prompts.
- **Collectable fragments** : `FragmentManager` singleton with reset on AI detection and UI counter.
- **AI scanner** : Horizontally moving spotlight + distance‑based detection + raycast obstruction check (optional), resets position and fragments.

---

## Current Vertical Slice Status

### Completed
- [x] Full first layer (ruin) : exploration, readable clues, door password (`20260608`), computer terminal with typewriter AI, teleport to second layer.
- [x] Second layer (awakening) :
  - Auto‑triggered dialogue with speaker name and “Press E to continue”.
  - 5 collectable creativity fragments with UI counter.
  - AI scanning lights (horizontal movement, detection radius adjustable, optional raycast shield).
  - Reset on detection (teleport to start, fragments reset, warning UI).
  - Exit trigger (gather all fragments + step into safe zone) → final dialogue → fade to black → teleport to third layer (placeholder).

### Planned / In Progress
- [ ] Third layer (prosperity) : AI as human tool, final choice (good / bad ending).
- [ ] Sound effects and ambient music.
- [ ] More environment variations.

---

## Testing & Bug Fixes

Key issues resolved during development (see `Testing.md` for full log):
- **Player unable to interact** : Missing `Capsule Collider` on player → added, later replaced by `CharacterController` to avoid flying.
- **Trigger not detected** : `Is Trigger` was too small → increased size and raised center Y.
- **Camera didn’t switch** : Manual transform movement failed → replaced with Cinemachine Virtual Camera.
- **Dialogue text disabled** : External script interfering → forced `enabled = true` and `color = white` in `LateUpdate`.
- **Speaker name not showing** : UI component initially disabled → code forces activation each line.
- **Fragment UI invisible** : UI panel was disabled at start; now enabled only after second‑layer dialogue.
- **AI scanner too fast** : `speed` reduced from 5 to 3.
- **Scanner detection imprecise** : Changed from cone‑based to vertical distance + raycast (optional fallback).

---

## License & Ethical Notes

- This project is created for educational purposes as a coursework assignment.  
- All third‑party assets are used under free / educational licenses. No copyrighted material is redistributed.
- The game explores themes of AI and creativity; no real individuals or organizations are portrayed.

---

## Author

**Qiyin Huang** – Game Design, Programming, Level Design, Writing  
Course: Game Engineering Module  
GitHub: [xXwuyang](https://github.com/xXwuyang)

---

## Acknowledgements

Special thanks to the course instructor for feedback, and to fellow students for playtesting.

---

*README last updated: June 13, 2026*  
*For the latest version, please refer to the GitHub repository.*