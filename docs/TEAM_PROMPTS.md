# Copy/paste kickoff prompts for AI coding agents

All agents must read README.md, docs/PRODUCTION_PLAN.md and docs/INTERFACES.md first. Work against the shared Unity project created on main. Never silently introduce packages or change Unity editor versions. Do not implement another teammate's owned code unless explicitly agreed. Complete and verify one small working feature before starting the next.

## Shlok — Antigravity on Mac
You own architecture/integration. Establish the Unity 6 Universal 2D project baseline in this repo, initialize GameSession/LevelDirector, design a bounded EchoRecorder and EchoPlayback with events and test/re-record fallback, and ensure a simple playable test scene demonstrates prior trajectory replay. Own Bootstrap scene and build settings. Coordinate interfaces with other agents. Commit exclusively to feature/shlok-core after the baseline is pushed to main. Include exact Unity editor test steps and known limitations in PR.

## Lalit — Antigravity on Windows
Implement modular Rigidbody2D player movement (walk/jump/grounded), interaction, and three powers (gravity invert, temporary freeze of tagged actors, predefined dimension switching). Respect docs/INTERFACES.md and own only Player/Mechanics scripts and Player prefabs. Start with placeholder visuals. The controls must feel responsive, each ability must provide feedback and reset safely. Do not alter Shlok's core scripts or Soham's level scenes. Commit to feature/lalit-mechanics with test scene or self-contained prefab and a PR checklist.

## Soham — Claude on Windows
Create cinematic but readable four-room 2D/2.5D environments using Unity 6 2D URP: high-contrast dark industrial aesthetic, cyan interactables, red anomalies, parallax and minimal UI. Own level scenes, art/UI/audio folders. First deliver a graybox Level01 with collision, spawn, pressure plate, movable cube and exit; then Levels02–04 with adjustable mechanism slots, visual effect prefabs, tutorial prompts, timer/score and Architect captions. Scripts must use events/interfaces agreed with lead. Avoid editing PlayerController2D, GameSession, or global project settings. All puzzles must have an alternate recovery route and readable instructions. Commit to feature/soham-levels; show screenshots and test steps.
