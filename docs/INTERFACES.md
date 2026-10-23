# Shared interface contract (freeze after first integration)

## Ownership
- Shlok: `Assets/_Game/Scripts/Core/`, `Assets/_Game/Scripts/Echo/`, `Assets/_Game/Scenes/Bootstrap.unity`.
- Lalit: `Assets/_Game/Scripts/Player/`, `Assets/_Game/Scripts/Mechanics/`, `Assets/_Game/Prefabs/Player/`.
- Soham: `Assets/_Game/Scenes/Levels/`, `Assets/_Game/Art/`, `Assets/_Game/Audio/`, `Assets/_Game/UI/`.

## Interfaces
- `GameSession` stores completed level, score/timer, unlocked abilities and echo snapshots. Persistent singleton if needed; reset and scene loads must not duplicate it.
- `LevelDirector` owns puzzle state, spawn, win/fail conditions; publishes `OnLevelComplete`, `OnPlayerDied`.
- `PlayerController2D` owns Rigidbody2D motion and emits fixed-step movement snapshots; provide position, facing, grounded state, movement input and interaction event.
- `AbilityController` owns gravity/time/dimension abilities with explicit readiness/cooldown state.
- `EchoRecorder` captures sampled player transforms/actions, with timestamp from a consistent simulation clock and an upper bound on samples.
- `EchoPlayback` replays recorded samples with visual ghost + limited, deliberately specified puzzle interactions; it is NOT a fully physical second player.
- `IInteractable` offers `Interact()` or equivalent for switches and carryables. Keep interaction semantics consistent across levels.
- `DimensionState` toggles predefined level objects and colliders; ensure no trapping player inside newly solid geometry.
- UI listens to published state rather than owning mechanics.

## Scene conventions
- Unity Physics2D: explicit layer mask for Player, Ground, Interactive, Echo, Hazard.
- World units: one Unity unit per tile; initial player height roughly 1.6 units; camera orthographic.
- Controls (keyboard first): A/D move, Space jump, E interact, G gravity invert, F freeze/unfreeze, Q dimension toggle, R restart, Esc pause.
- All scenes designed to run from Bootstrap or directly for local developer testing.

## Integration rule
If a teammate needs to change an interface/owner file, make a proposal in the PR; do not independently rewrite it. Resolve compilation errors before taking new work. No force pushing shared branches.
