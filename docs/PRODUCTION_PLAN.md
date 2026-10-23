# Production plan — GlitchVerse: Echo Protocol

## North star
A judge must understand and enjoy a playable puzzle within 30 seconds; a second reveal shows the room replaying the player's recorded actions. All systems prioritize working gameplay over unnecessary features.

## Required gameplay
- Four authored levels with escalating interactions and explicit success criteria.
- Three reality manipulation mechanics (gravity inversion, time freeze, dimension switching).
- Checkpoint/restart and no soft locks; death/resets never destroy necessary echo data without an alternate way to re-record.
- Timer or score, visible win/loss feedback, and clean navigation.
- First 60 seconds contain at least one aha moment.

## Four chambers
1. **The First Lie:** Push a cube onto a plate; teach movement and interaction; record player trajectory for later use.
2. **Gravity Is a Suggestion:** Revisit familiar space, reverse gravity, and make a recorded echo activate a switch.
3. **The Past Is Your Enemy:** Freeze moving hazards; cooperate with (or avoid) an echo sequence.
4. **Break the System:** Shift between two curated dimension states and combine the prior abilities. Finale must be solvable with an accessible re-record/restore path.

## Architecture
Unity 6 Universal 2D; Unity C# scripts; Rigidbody2D/Collider2D physics. Use a persistent GameSession and scene-contained LevelDirector. Input action mapping must stay stable. Behavior-driven Architect may select only predefined, solvability-tested variations. Avoid Unity ML-Agents training during the event. No API or internet dependency during judging.

## 24-hour gates (elapsed time)
- H0–H2: Unity baseline boots on all devices; controls, repo, responsibilities, graybox room and scripted interfaces finalized.
- H2–H6: Player movement, object interaction, exit logic; first complete room; stable test build.
- H6–H10: Gravity, time freeze, dimension switch independently work; echo samples and replays.
- H10–H15: Four connected level scenes; puzzle validation; checkpoints and failure recovery.
- H15–H19: Lighting, audio, animation, story, menu, adaptive Architect dialogue.
- H19–H22: Integration freeze; complete playthrough, bug fixing, Windows build, test on target hardware.
- H22–H24: Demo rehearsal and submission buffer; no new features.

## Team workflow
Branches: `feature/shlok-core`, `feature/lalit-mechanics`, `feature/soham-levels`; PR into main. Only one owner may edit shared project settings and the initial main scene. Prefer additive scripts/prefabs and separately owned scenes; never manually delete Unity .meta files. Commit working increments frequently. Lead reviews every merge after Unity opens without compiler errors.

## Quality gates
- Level is completable with keyboard; no reliance on editor-only setup.
- Echo replay has stable coordinate/time basis; deterministic-enough physics and playback.
- All powers show cooldown/state feedback and restore safely after reset.
- Game can restart from any level without losing required state.
- Build launches offline on target Windows PC; frame pacing acceptable; readable UI.

## Stretch ONLY if baseline stable
Adaptive Architect narration, optional speech, branching endings, advanced VFX. Never let generative AI author untested obstacles or require internet to solve a puzzle.
