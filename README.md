# GLITCHVERSE: ECHO PROTOCOL

A cinematic 2.5D puzzle-platformer for Artificial Arcade VI, PS01 (GlitchVerse).

**Core hook:** Every escape leaves an echo. Player actions recorded in earlier rooms become interactive puzzle elements in later rooms.

## Technical direction
- Unity 6, 2D URP template, C#.
- 2D physics and movement with layered parallax, camera effects, shader-driven glitches and lighting for a 2.5D presentation.
- Four handcrafted, independently solvable chambers.
- Three environment abilities: gravity inversion, time freeze, dimension swap.
- Echo recorder/replayer; deterministic replay with validation and rewind/re-record fallback.
- Optional behavior-driven Architect director (offline, rule-based; never modifies arbitrary geometry).
- No external API required to finish or play the game.

## Project setup
1. Install the SAME Unity 6 editor version for all three team members using Unity Hub.
2. Shlok creates a **Universal 2D** project named `GlitchVerse` at the cloned repo root. If Unity Hub creates an extra directory, move its contents to the repo root before committing.
3. Make a first commit containing `Assets/`, `Packages/`, and `ProjectSettings/`. Never commit `Library/`, `Temp/`, `Obj/`, `Build/`, or `UserSettings/`.
4. In Unity: Edit > Project Settings > Editor > Asset Serialization = Force Text and Version Control = Visible Meta Files.
5. Teammates fetch and branch from the first Unity project commit. Do not create three independent Unity projects.

## Division of work
- Shlok (Mac, Antigravity): project owner, architecture/contracts, global game state, echo recording/replay, integration, builds and QA.
- Lalit (Windows, Antigravity): player controller, gravity invert, time freeze, dimension swap, interactables and gameplay tests.
- Soham (Windows, Claude): level scenes, art/UI, audio/VFX, narrative and tutorial; integrate via defined interfaces.
- ChatGPT: technical lead, implementation reviews, acceptance tests, debugging, integration plans and scope control.

See `docs/PRODUCTION_PLAN.md`, `docs/INTERFACES.md` and `docs/TEAM_PROMPTS.md`.
