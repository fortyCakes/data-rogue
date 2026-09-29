# data-rogue

A C# roguelike **engine** (`data-rogue-core`) plus an example **game** built on it
(`data-rogue-one`). Game content (monsters, items, skills, world structure, maps,
keybindings, sprites) is defined in text files; game logic is C# event rules.

The core/game split is intended to be a real boundary so the engine can host
multiple games. It currently leaks in several places — see
[`docs/divergences.md`](docs/divergences.md) before touching anything near it.

## Read next

- [`docs/architecture.md`](docs/architecture.md) — projects, boundary, startup,
  game loop, extension points. Read this before any non-trivial change.
- [`docs/divergences.md`](docs/divergences.md) — where the code differs from the
  intended design, plus known hazards. Check it before "fixing" something that
  looks wrong; it may already be recorded.

## Build and test

- .NET Framework 4.5.2 (tests: 4.6.1), old-style `.csproj`, `packages.config`,
  WinForms. **Windows + Visual Studio / MSBuild only.** Solution: `data-rogue.sln`.
- Tests: `data-rogue-core.UnitTests` (NUnit, NSubstitute, FluentAssertions).
  The MSTest packages it references are unused.
- **In a Linux container (e.g. a cloud agent session) you cannot build or run
  tests.** Say so explicitly when reporting work; do not claim a change compiles
  or passes. Compensate by reading call sites carefully and keeping changes small.
- Entry point: `data-rogue-one/Program.cs`. Runtime output (`Saves/`, `Morgue/`)
  is written next to the executable.

## Rules for changes

**Boundary**
- `data-rogue-core` must not gain new knowledge of `data-rogue-one`'s content:
  no new hard-coded prototype names, stat names, action types or assets. If you
  can't avoid it, add an entry to `docs/divergences.md`.
- Put a new file in the project that owns the concept, and use that project's
  namespace (`data_rogue_core.*` / `data_rogue_one.*`). Existing files often
  don't (see divergences); don't copy that pattern.

**Data files**
- Every file under `data-rogue-one/Data/` must be listed in
  `data-rogue-one.csproj` with `<CopyToOutputDirectory>Always</CopyToOutputDirectory>`,
  or it is silently absent at runtime. The same applies to images in
  `data-rogue-core.csproj`.
- Prototype names follow `Kind:Name` (`Monster:Orc`, `Item:Staff`,
  `Skill:Fireball`, `Cell:LightStoneFloor`, `Branch:Dungeon`). Cross-references
  between files use these names; a typo fails at runtime, not load time.

**Components**
- Serialization binds **public fields only** (not properties), by exact name.
  Field names must be letters only — the `.edt` parser's key pattern is `[a-zA-Z]*`.
- Class names must be unique across all loaded assemblies (lookup is by
  `Type.Name`).
- Behaviours (`IBehaviour`) need a `(ISystemContainer)` constructor; other
  components need a parameterless one.

**Event rules**
- Constructor must be `(ISystemContainer)`. A rule does nothing until it is added
  to the list in `data-rogue-one/GameRules.cs`.
- Choose `RuleType` and `RuleOrder` deliberately. Higher `RuleOrder` runs first;
  ties run in `GameRules.cs` list order. Don't reorder that list casually.

**Lua scripts** (in `.edt` `Script` components) have full .NET access via
`SystemContainer`. Treat them as code, not data.

## Layout

```
data-rogue-core/            engine (library)
data-rogue-one/             example game (WinExe): rules, components, Data/
  Data/Entities/            .edt entity files (StaticEntities/, World/, player, keybindings, spritesheet)
  Data/StaticMaps, Vaults/  .map files
BearLib/, BLTWrapper/       BearLibTerminal bindings + sprite-sheet helpers
data-rogue-core.UnitTests/  NUnit tests for core
docs/                       design docs (this file is the index)
```
