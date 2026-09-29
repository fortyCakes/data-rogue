# Architecture

This describes the code **as it is**. Where it differs from the intended design,
there is a `⚠ Dn` marker linking to [`divergences.md`](divergences.md).

## Projects

| Project | Kind | Role | Depends on |
|---|---|---|---|
| `data-rogue-core` | Library | The engine: entity engine, event rules, systems, world/map generation, activities (screens), UI controls, two IO backends, save/load. | BearLib, BLTWrapper, RLNET, OpenTK, NLua/KeraLua, Newtonsoft.Json, WinForms |
| `data-rogue-one` | WinExe | Example game: `Program.cs`, the rule list (`GameRules.cs`), game-specific components/rules/generators, UI layout, and all content under `Data/`. | core, BearLib, BLTWrapper |
| `BearLib` | Library | P/Invoke wrapper for `BearLibTerminal.dll` (checked in). | — |
| `BLTWrapper` | Library | Sprite-sheet loading and tiling helpers for BearLibTerminal. | BearLib |
| `data-rogue-core.UnitTests` | Tests | NUnit tests for core. | core |

## The engine/game boundary

The intent is that core contains nothing specific to one game, and a game supplies
its content, rules and presentation through the extension points below. In
practice core still assumes some of data-rogue-one's content, and some extension
points are closed enums or fixed paths. ⚠ D1–D8

### Extension points

How a game plugs into the engine, and how each kind of plug-in is found:

| Extension | Game supplies | How the engine finds it |
|---|---|---|
| Components | Classes implementing `IEntityComponent` | **Reflection** over every loaded assembly (`EntityEngine` constructor). Name must be unique across assemblies. |
| Behaviours (AI) | Components implementing `IBehaviour`, with `(ISystemContainer)` ctor | Same as components. Run by `TimeSystem`, ordered by `BehaviourPriority`. |
| Event rules | `IEventRule` classes with `(ISystemContainer)` ctor | **Explicit list** passed to `DataRogueGame.Run` (`GameRules.Rules`). |
| Player/UI actions | Subclasses of `ApplyActionRule` | Explicit list (as rules). The action *kind* must be a value of the core `ActionType` enum. ⚠ D3 |
| Map-gen commands | `ICommandExecutor` implementations | **Reflection** over every loaded assembly (`MapGenCommandExecutorFactory`), matched by `CommandType` string. |
| Branch generation | Components on a `Branch` entity: exactly one `BranchMapGenerationStrategy`, any number of `BaseEntityGenerationStrategy` | Read from the branch entity at generation time (`BranchGenerator`). |
| Control renderers | `IDataRogueControlRenderer`s | Core's own are found by reflection **over the core assembly only** (`ReflectiveEnumerator`). A game must pass its own via `IOSystemConfiguration.AdditionalControlRenderers`. |
| UI layout | `IOSystemConfiguration` (controls, window title, etc.) | Passed in with the `IIOSystem` instance (`BLTTilesConfig.cs`). |
| Data sources | `EntityDataProviders` | Optional argument to `Run`. `null` → `EntityDataProviders.Default`, which reads fixed paths. ⚠ D4 |
| Content | `.edt` / `.map` files | Loaded by the data providers; see [Data loading](#data-loading). |

Discovery is inconsistent: some kinds are found by reflection, others must be
listed. When adding one, check the table above rather than assuming.

## Central objects

- **`DataRogueGame`** (`data-rogue-core/DataRogueGame.cs`) owns startup and the two
  loop callbacks (update and render).
- **`SystemContainer`** (`Systems/SystemContainer.cs`) is the service locator. It
  constructs every system, in dependency order, and is passed to nearly everything:
  rules, behaviours, activities, Lua scripts. `Verify()` throws if any system is null.
- **`EntityEngine`** holds all entities. An entity is a name, an id and a list of
  components. Systems that implement `ISystem` declare `RequiredComponents` /
  `ForbiddenComponents`; the engine adds or removes entities from each system
  whenever components change.
- **`EventSystem`** runs rules for an event (see [Events](#events-and-rules)).
- **`ActivitySystem`** is a stack of screens (`IActivity`): gameplay, menus, forms,
  targeting, shop, map editor, loading screens, toasts.

## Startup

1. `Program.Main` builds the BearLibTerminal IO system (`BLTTilesConfig`) and calls
   `DataRogueGame.Run("SEED_DEBUG", GameRules.Rules, ioSystem, null, …)`.
   The seed is hard-coded.
2. The IO system is initialised with the update/render callbacks and the sprite-sheet
   data (`Data/Entities/spritesheet.edt`).
3. `SystemContainer.CreateSystems(seed)` constructs every system, then:
   - `EntityEngine.Initialise` loads everything under `Data/Entities/StaticEntities`
     and marks those entities **static** (they are never saved);
   - `ControlSystem.Initialise` loads `Data/Entities/keybindings.edt`.
4. A `GameplayActivity` (not yet running) and a loading screen are pushed.
5. **On a background thread**, rule instances are built and registered; then the
   loading screen is popped and the main menu (core's `MainMenu`) is pushed. ⚠ D7
6. `ioSystem.Run()` blocks, driving the callbacks until quit.

### New game

Main menu → `CharacterCreationForm` (core) → `SaveSystem.Create` →
`WorldGenerator.Create`:

1. Resets messages and maps, and re-runs `EntityEngine.Initialise`, which reloads
   all static entities.
2. Loads `Data/Entities/World/**` (the world root, branches, branch links). These are
   *not* marked static, so their state (e.g. `Branch.Generated`) is saved.
3. Loads every `Data/Vaults/*.map` into `MapSystem.Vaults`.
4. Reads the `"World"` prototype, and generates its `InitialBranch`. The player
   spawns on a cell with a `SpawnPoint` component in map `<BranchName>:1`.
5. Builds the player from `Data/Entities/player.edt`, then applies the form: name,
   the four core-defined stats, `Class:<name>` skills (`KnownSkill`) and gear
   (`StartsWithItem`). ⚠ D1, D2
6. Sets the gameplay activity running.

### Load game

`SaveSystem.Load` reads `Saves/saveFile.sav`, re-initialises the engine (static
entities), deserialises the saved mutable entities and maps, and restores messages.
It does not reload vaults or restore the seed. ⚠ H5

## Game loop

`IIOSystem` calls two callbacks.

**Update** (`OnRootConsoleUpdate`):
1. Read the key press and mouse, and pass them to `ControlSystem.HandleInput`. This
   maps keys to `ActionEventData` through the keybindings and hands them to the
   top activity that accepts input.
2. Unless a blocking animation is playing, call `TimeSystem.Tick()` repeatedly (up
   to 1000 times) while the top activity is gameplay and the game isn't waiting for
   player input.

**Render** (`OnRootConsoleRender`): tick animation, movement and particle systems;
walk the activity stack down to the first activity that `RendersEntireSpace`; render
those bottom-up through the IO system's `IUnifiedRenderer`; draw.

### Time and turns

`TimeSystem` manages entities with an `Actor` component (excluding prototypes).
Each tick:
- increments `CurrentTime`;
- runs every `ITickUpdate` component;
- for actors whose `NextTick` has arrived *and who are on the player's current map*,
  tries their behaviours in descending priority (each gated by its chance) until one
  spends time.

The player's `PlayerControlledBehaviour` returns `WaitForInput`, which stops ticking
until input arrives. Actors on other maps don't act.

### Input → action → event

Key press → `KeyBinding` (e.g. `Move(0,-1)`) → `ActionEventData { Action, Parameters }`
→ `activity.HandleAction` → typically `EventSystem.Try(EventType.Action, …)` →
the `ApplyActionRule` subclass whose `actionType` and `activityType` match the
current state.

## Events and rules

`EventSystem.Try(eventType, sender, data)` runs the rules registered for that event
type, in phases:

| Phase | Runs when | Return value `false` means |
|---|---|---|
| `BeforeEvent` | always, first | **veto**: stop this phase; event fails |
| `EventResolution` | event not vetoed | recorded, doesn't stop |
| `AfterSuccess` | event not vetoed | — |
| `AfterFailure` | event vetoed | — |
| `Finally` | always, last | — |

Within a phase, rules run in descending `RuleOrder`. Ties run in registration order,
which is the order of `GameRules.Rules`. ⚠ H1

Rules pass information along by mutating the shared `eventData` object (e.g.
`AttackEventData.Damage`). For `Attack`, the `BeforeEvent` phase is really a
pipeline: weapon (1000) → attack class (999) → damage/accuracy/speed (998) → …
→ defences and resistances. Many rules therefore depend on running before or after
others, and that dependency is expressed only through the numbers.

Event types are the closed `EventType` enum in core.

## Entities, prototypes and content

- Every static definition has a `Prototype` component with a `Name` (`Kind:Name`).
  `PrototypeSystem.Get(name)` returns the entity itself if `Singleton: true`,
  otherwise a **deep-ish clone**: component fields are copied, `Counter`s and
  `[AlwaysCreateNewInstance]` types are copied as new instances, `StartsWithItem`
  entries are instantiated and equipped. It then fires `SpawnEntity`.
- Prototypes reference each other by name (skill → script, class → skills and items,
  branch → cells and items, map → entities). Missing names fail at runtime.
- Core itself asks for several specific names. ⚠ D2

### Data loading

All paths are relative to the directory of `data-rogue-core.dll` (i.e. the game's
output folder) and are fixed in core. ⚠ D4

| Provider | Path | Loaded when |
|---|---|---|
| `StaticEntityDataProvider` | `Data/Entities/StaticEntities/**/*.edt` | Startup, and again at every new game / load |
| `KeyBindingsDataProvider` | `Data/Entities/keybindings.edt` | Startup |
| `SpriteSheetDataProvider` | `Data/Entities/spritesheet.edt` | IO system initialisation |
| `WorldEntityDataProvider` | `Data/Entities/World/**/*.edt` | New game |
| `PlayerEntityDataProvider` | `Data/Entities/player.edt` | New game |
| `VaultDataProvider` | `Data/Vaults/*.map` | New game |

Static maps (`Data/StaticMaps/*.map`) are loaded by the static-map branch generator
when its branch is generated.

### File formats

Both formats are shared by content files and save files.

- **`.edt`**:
  - `"EntityName"` (or `"Name":id` in saves) starts an entity.
  - `[ComponentClassName]` starts a component.
  - `Field: value` lines bind public fields; `Field: {` … `}` gives multi-line text.
  - Lines starting with `#` are comments.
  - Values are converted by type: `Color` from HTML hex, enums by name, types
    implementing `ICustomFieldSerialization` (e.g. `Counter` as `8/8`, vector lists)
    by their own parser, everything else by `Convert.ChangeType`.
- **`.map`**: in order,
  - `Map:"name"` and `default:<cell prototype>`;
  - optional `Biomes:` and `VaultWeight:` lines;
  - the origin `x,y`;
  - map-gen commands `x,y: Command(params)`;
  - a glyph legend `c:<cell prototype>`;
  - the ASCII grid (space means no cell);
  - optionally a `Seen:` block (saves only).

## World generation

- The `World` prototype names the initial branch.
- A branch is a `Branch` entity plus generation-strategy components (see
  `Data/Entities/World/Branches/*.edt`).
- `BranchLink` entities connect a level range of one branch to a level range of
  another.

`BranchGenerator.Generate`:
1. Seed an RNG with `seed + branchName`.
2. Run the branch's single map strategy (static map, basic dungeon, or vault-based
   dungeon) to produce the maps.
3. Run each entity-generation strategy (monsters, items, stairs, portals, shops…).
4. Execute the map-gen commands embedded in the maps.
5. Link stairs between consecutive levels (ordered by map-key **string**). ⚠ H4
6. Attach branch-link portals.

Only the initial branch is generated at world creation. Every other branch is
generated **lazily**, the first time a portal into it is used (`BranchGeneratorRule`
vetoes the move, generates on a `Task.Run` thread behind a loading screen, then
retries). ⚠ H3

## Rendering and UI

- `IIOSystem` abstracts window, input and drawing. There are two implementations:
  - `BLTTilesIOSystem` (BearLibTerminal, sprite tiles), used by data-rogue-one;
  - `RLNetConsoleIOSystem` (RLNET, ASCII console), still compiled but not used
    by data-rogue-one.
- Activities expose trees of **controls** (`IDataRogueControl`: map, minimap,
  message log, stat bars, flow containers…). Each IO system has one
  **control renderer** per control type.
- The game lays out gameplay controls in `IOSystemConfiguration.GameplayWindowControls`
  and adds its own renderers through `AdditionalControlRenderers`.
- Screens other than gameplay (main menu, character creation, inventory, equipment,
  skills, shop, morgue, high scores, map editor) are defined in core. ⚠ D7

## Persistence

`SaveSystem` writes the following next to the executable:
- `Saves/saveFile.sav`: one save slot, containing the mutable entities, all maps
  with their seen cells, time and messages;
- `Saves/highscores.txt`;
- `Morgue/morgue-<timestamp>.txt`.

## Scripting

- `Script` components hold Lua. `ScriptExecutor` runs it in a fresh NLua state with:
  - globals `SystemContainer`, `User`, `Entity`;
  - helper functions `withTarget`, `requestTarget`, `onComplete`, `makeAttack`,
    `attackCellsHit`, `attackAllCells`, `isCellBlocked`, `restoreCellsHit`;
  - an `each()` iterator for .NET enumerables.
- `LoadCLRPackage` is enabled, so scripts can reach any .NET type. ⚠ H6
- Only one script may await completion at a time.

## Threading

Most state is unsynchronised and assumed to be touched by the IO loop thread. Two
places break that:
- Rule registration at startup runs on a background thread. The loading screen hides
  it, but input and render callbacks are live while it runs.
- Lazy branch generation runs on a thread-pool task. It mutates the entity engine,
  map collection and activity stack while the render loop keeps running. ⚠ H3
