# Divergences and hazards

This is a register of the places where the code differs from the intended design,
plus known hazards. Other docs link here by ID (`⚠ D3`). IDs are permanent: when
an item is resolved, mark it **Resolved** with the commit; don't delete or reuse it.

Unless stated otherwise, every entry was found by reading the code. None has been
confirmed by running the game.

- **D-entries** are divergences from stated intent. The core/game split is meant to
  be a real boundary, so that core can host multiple games.
- **H-entries** are hazards and probable bugs. Their intent is unknown or they look
  unintentional.

---

## D — Divergences from intended design

### D1 — Game-specific stat names hard-coded in core
- **Current:** core hard-codes `Muscle`, `Agility`, `Intellect` and `Willpower`:
  - the character-creation form defines them, with their limits;
  - `WorldGenerator.SetInitialStats` applies them to the player;
  - `FighterSystem.BasicAttack` uses `Muscle` as base damage.
- **Intended:** stat names and character creation are defined by the game.
- **Where:** `data-rogue-core/Forms/StaticForms/CharacterCreationForm.cs:37-59`,
  `data-rogue-core/World/WorldGenerator.cs:147-153`,
  `data-rogue-core/Systems/FighterSystem.cs:34`.

### D2 — Core requires specific prototype and entity names
- **Current:** core asks for these by literal name, so any game must define them:
  - `World`, and `Class:<name>`, where `<name>` is the class's `Description.Name`;
  - `Props:StairsDown`, `Props:StairsUp`, `Props:Portal`, `Props:Shop`;
  - `Cell:Empty`, `Cell:Wall`, `Cell:Void`;
  - `Item:UnarmedWeapon`.
- Save loading identifies the player as the entity named `Player`.
- **Intended:** the game supplies these names, or they are part of a documented
  engine contract.
- **Where:** `WorldGenerator.cs:78,188`; `BaseBranchGenerator.cs:192-211`;
  `BaseShopGenerator.cs:17`; `VaultBasedDungeonMapGenerator.cs:98-99`;
  `MapEditorActivity.cs:36-37,105`; `SetWeaponOnAttackRule.cs:57`; `SaveSystem.cs:44`.
- **Note:** until this is fixed, the list above *is* the contract. Update it when
  you add or remove one.

### D3 — Closed enums include game-specific values
- **Current:** `ActionType` is an enum in core that includes values only
  data-rogue-one uses (`Rest`, `Morgue`, `PlayerStatus`, `Examine`). A new game
  cannot add an action kind without editing core. The same applies to `EventType`,
  and probably to `ActivityType` (not checked in detail).
- **Intended:** games can define their own actions, and possibly their own events,
  without changing core.
- **Where:** `data-rogue-core/EventSystem/EventData/ActionType.cs`,
  `EventSystem/EventType.cs`, `Activities/ActivityType.cs`.

### D4 — Data layout and paths fixed in core
- **Current:** the default data providers hard-code paths under `Data/`, relative to
  the directory of `data-rogue-core.dll`. `Run` accepts custom
  `EntityDataProviders`, but data-rogue-one passes `null`. Save, morgue and
  high-score paths are also fixed and sit next to the executable.
- **Intended:** all of these paths are configurable by the game. The current
  layout can remain as the default.
- **Notes:** `EntityDataProviders` covers most data paths already. Three are not
  covered:
  - static maps, loaded through `DataFileLoader` with a hard-coded `Data/` root;
  - the save, morgue and high-score locations in `SaveSystem`;
  - image paths inside `spritesheet.edt`, which are passed to BearLibTerminal
    unmodified. They are probably resolved against the process working directory
    rather than the executable's directory; this hasn't been checked.
- **Where:** `data-rogue-core/EntityEngineSystem/*DataProvider.cs`,
  `Data/DataFileLoader.cs`, `Systems/SaveSystem.cs`.

### D5 — Game art lives in the core project
- **Current:** all sprites that data-rogue-one's `spritesheet.edt` references
  (creatures, items, props, UI) sit in `data-rogue-core/Images/` and are copied to
  output by `data-rogue-core.csproj`.
- **Intended:** game assets belong to the game project. Core keeps only what the
  engine itself needs, such as the font and default UI sprites.
- **Where:** `data-rogue-core/Images/**`, and the `<Content>` items in
  `data-rogue-core.csproj`.

### D6 — Namespaces don't match projects
- **Current:** many `data-rogue-one` files declare `data_rogue_core.*` namespaces,
  e.g. most of `Rules/`, several `Components/`, and `Maps/MapGenCommands/`. Some
  game utilities use `data_rogue_one.EventSystem.Utils`. You can't tell which side
  of the boundary a type is on from its namespace.
- **Intended:** `data_rogue_one.*` for everything in the game project.
- **Where:** run `grep -rn '^namespace data_rogue_core' data-rogue-one/`.

### D7 — Game flow and screens defined in core
- **Current:** core defines the main menu, the character-creation form,
  new-game/load flow, the morgue screen and high scores. `DataRogueGame` always
  pushes core's `MainMenu`.
- **Intended:** core provides default screens, and a game can override any of them.
- **Notes:** screens are currently constructed directly with `new`, from many places
  (menus push other menus, forms push the main menu on cancel). Making them
  overridable needs one place to resolve screens, such as a registry keyed by screen
  identity. Overriding a single class won't be enough. Character creation is also
  tied up with D1.
- **Where:** `data-rogue-core/Menus/StaticMenus/MainMenu.cs`,
  `Forms/StaticForms/CharacterCreationForm.cs`, `DataRogueGame.cs:97-100`.

### D8 — Both IO backends compiled into core
- **Current:** core contains both the RLNET console and the BearLibTerminal tile
  backends, and references both libraries plus WinForms and OpenTK.
  data-rogue-one uses only BearLibTerminal. Default control renderers are
  discovered only within the core assembly (`ReflectiveEnumerator`), so game
  renderers must be registered explicitly.
- **Intended:** each renderer is a plugin in its own assembly. Core has no dependency
  on any rendering or windowing library. RLNET and BearLibTerminal both remain
  supported.
- **Notes:** this goes deeper than moving `IOSystems/` out. Supposedly
  backend-neutral code references backend and windowing types directly. The counts
  below are of `using` directives outside `IOSystems/`, so some may be unused:
  - **WinForms** in 27 files. Controls, activities and layout use its `Padding`,
    `FlowDirection` and similar types as the core layout model.
  - **OpenTK** in 7 files. `KeyCombination.Key` is an OpenTK `Key`, so the engine's
    input model is OpenTK's.
  - **RLNET** in 5 files, including `DataRogueGame.cs`, `ColorExtensions.cs` and
    `ControlSystem.cs`.
  - **BLTWrapper** in 5 files, including vault generation
    (`VaultConnectionPoint.cs`, `VaultPossibleConnection.cs`), probably for its
    `TileDirections` type.

  Core needs its own neutral types for keys, layout and directions before the
  backends can be split out.
- **Where:** `data-rogue-core/IOSystems/`, `IOSystems/KeyCombination.cs`,
  `Controls/`, `Activities/`, `Utils/ReflectiveEnumerator.cs`.

### D9 — "Entirely configured in text files" is partial
- **Current:** the README's goal holds for content: entities, world structure,
  generator parameters, maps, keybindings and sprites. It does not hold for:
  - which rules are active (`GameRules.cs`);
  - new component, behaviour or rule types;
  - map-gen command types;
  - UI layout (`BLTTilesConfig.cs`);
  - the RNG seed (hard-coded `"SEED_DEBUG"` in `Program.cs`).
- **Intended:** the active rule list and the UI layout should be data, so that
  different games can be assembled mostly from data. Adding new component,
  behaviour, rule and command *types* still requires C#. The seed is not yet decided.
- **Notes:**
  - **Rule list:** rule order is currently implicit, from `RuleOrder` ties broken by
    list order (H1). If the list moves to data, that tie-breaking becomes invisible
    and fragile. Rule order should be made explicit first, or as part of the move.
    Rules would also have to be resolved by type name, which brings the same
    uniqueness concern as H9.
  - **UI layout:** controls are configured through object initialisers with WinForms
    layout types (D8). A data format for layout depends on the neutral layout types
    that D8 needs anyway, so doing D8 first avoids designing it twice.

---

## H — Hazards and probable bugs

### H1 — Rule order depends on registration order
About 30 rules share `RuleOrder => 0`. Within a phase, ties run in `GameRules.Rules`
list order (the sort is stable). The `Attack` pipeline depends on a hand-maintained
number sequence (1000 → 999 → 998 → 500 → 100 …). Reordering `GameRules.cs`, or
choosing a new rule's `RuleOrder` carelessly, can silently change combat.

### H2 — Parsers fail late and without useful messages
In `.edt` files:
- an unknown component name throws from `.Single()`;
- an unknown field throws a `NullReferenceException`;
- an unparseable line throws `NotImplementedException`;
- field names containing digits don't match the key pattern.

In `.map` files, `VaultWeight` must contain a decimal point (`2.0`, not `2`).

A misspelt prototype reference is only caught when it is first used, which may be
deep into a run.
- **Where:** `EntityEngineSystem/ComponentSerializer.cs`, `Maps/MapSerializer.cs:111`.

### H3 — Lazy branch generation runs off the main thread
`BranchGeneratorRule` generates a new branch on a `Task.Run` thread. That thread
mutates the entity engine, map collection and activity stack while the IO loop keeps
rendering and polling input. None of these structures is thread-safe. Exceptions on
that task are unobserved, so a generation failure may just leave the loading screen
up.
- **Where:** `EventSystem/Rules/BranchGeneratorRule.cs:41`.

### H4 — Levels are ordered by string map key
Stairs are linked between consecutive maps ordered by `MapKey.Key` as a string, so
`…:10` sorts before `…:2`. This is harmless while branches have at most 9 levels.
- **Where:** `World/GenerationStrategies/MapGeneration/BaseBranchGenerator.cs:31,175`.

### H5 — Save/load doesn't restore vaults or seed
- `SaveSystem.GetSaveState` never sets `Seed`, so a save records an empty seed, and
  `Load` sets `SystemContainer.Seed` to `""`. Branches generated after loading use a
  different RNG seed from the original run.
- `MapSystem.Initialise` (called by `Load`) clears the vault list, and only
  `WorldGenerator.Create` reloads it. A vault-based branch first generated after
  loading a save would therefore have no vaults.
- Neither has been reproduced by running the game.
- **Where:** `Systems/SaveSystem.cs:26-65,101-110`, `Systems/MapSystem.cs:13-17`,
  `World/WorldGenerator.cs:42-53`.

### H6 — Lua scripts have unrestricted .NET access
`ScriptExecutor` enables `LoadCLRPackage` and passes in the whole `SystemContainer`.
Any content file can run arbitrary code. This is fine for first-party content;
it is a problem if third-party data files are ever supported.
- **Where:** `Systems/ScriptExecutor.cs:84-95`.

### H7 — Data files missing from build output
`Data/Vaults/glass_junction.map` and `Data/Vaults/smoke_chamber.map` are in
`data-rogue-one.csproj` without `CopyToOutputDirectory`, so they are probably never
loaded. It's not known whether that is deliberate (e.g. disabled vaults).
- **Where:** `data-rogue-one/data-rogue-one.csproj:472,500`.

### H8 — Dead component-discovery code in `Program.cs`
`typeof(Program).Assembly.GetTypes().Where(t => t.IsAssignableFrom(typeof(IEntityComponent)))`
has the check backwards and always yields an empty list. It has no effect because
`EntityEngine` scans all assemblies anyway, but it misleads readers about how
components are found.
- **Where:** `data-rogue-one/Program.cs:25`.

### H9 — Component type names must be unique across assemblies
Component lookup is `ComponentTypes.Single(t => t.Name == name)` across every loaded
assembly. A game component with the same class name as a core one, or as a type in
any referenced library that implements `IEntityComponent`, breaks deserialisation.
- **Where:** `EntityEngineSystem/ComponentSerializer.cs:76`.

### H10 — Unused test framework references
The test project references MSTest packages, but every test uses NUnit. This is
harmless, but it confuses anyone choosing which attributes to write.
- **Where:** `data-rogue-core.UnitTests/packages.config`.
