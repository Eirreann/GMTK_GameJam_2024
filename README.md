# *Escalation Protocol* (GMTK Game Jam 2024)

A tower defence game with a city-builder twist, made in four days for the GMTK Game Jam 2024 (theme: "Built to Scale"), in Unity 2022.3 with a team of four. I was the lead developer, sole programmer, and co-designer.

You play as the CEO of a self-driving car company whose entire fleet, after a firmware update goes horribly wrong, is heading straight for your HQ. Build a city of heavily armed structures to defend it, and use your Scale-O-Matic 9000 ray gun to grow them into a metropolis.

The game placed in the top 14% for Style and the top 19% overall, out of 7,511 entries.

Play it in your browser or download it [on itch.io](https://iains.itch.io/escalation-protocol), or read more on the [*Escalation Protocol* page on my website](https://iain-skaggs.com/escalation-protocol-game-jam-project).

## In the code

All of the game's code is in `Assets/Scripts`:
- `Buildings/TowerBase` and `Interfaces/IScrollInteractable`: the core scaling mechanic. Scrolling over a tower grows or shrinks it, trading resources for damage, fire rate, and range, with a smooth scaling animation and live cost readouts.
- `Buildings/`: four tower types on the shared base class (basic, cannon, inferno, and ricochet, whose shots split into sub-bullets), each with a pooled set of projectiles (`ProjectilePool`).
- `AI/EnemyBase` and its subclasses: four enemy types (basic, tank, spawner, and exploder), each with an elite variant, following the roads to the player's HQ.
- `Scriptables/EnemyWavesObject` and `AI/EnemySpawnManager`: a designer-friendly wave system. Each wave in the ScriptableObject sets its batches of enemies, their timings, whether it unlocks a new path, and its tooltip text, so the whole team could balance every wave directly in data.
- `Game/PlaceBuildingHandler`: building placement, with raycast checks, rotation by scroll wheel, and no placement through the UI.
- `UI/`: the tower shop with tooltips, the intro screen, the options menu, and the scale bar.
- `Player/PlayerController`: camera movement and zoom.

Git tags mark each build uploaded to itch.io, up to the post-jam `v0.1.0` update, which rebalanced the waves and polished the UI.

## Opening the project

- Open the project in Unity Hub with Unity 2022.3.21f1, available from [Unity's download archive](https://unity.com/releases/editor/archive), and play from `Assets/Scenes/Final/00_Landing`.
- The project uses URP, the Input System, Cinemachine, and AI Navigation.

## A note on the code

This is four-day jam code, with the shortcuts that come with it. For more recent work, see my GMTK Game Jam [2025](https://github.com/Eirreann/GMTK_GameJam_2025) and [2026](https://github.com/Eirreann/GMTK_GameJam_2026) repos.

## Team

- Lithium: 3D modelling and art direction
- Raccoon-Pedro: 2D art and sound effects
- Jordan Skaggs (BlueJayProductions): music and sound effects