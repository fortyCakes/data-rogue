BUGS

-

0.1

Ordering of messages on kill
Ranged and magic weapons
	"fire" command
	Apply combat stat (aegis)
Caching positions in PositionSystem
Separate out core from implementation
	Make stats window defined in data
	Make Player entity not special (don't allow access from global state)
	Aura and Tilt separation
		Stat window rendering customisation
		Map rendering customisation
Remove RLNet dependencies from core (!)
Event system rework: separate pre- and post-event rules

0.2+

Morgue file format and death screen
Dependency injection? for rules and behaviours
Finish importing all tiers of base items
Smooth rendering while resting
Loading screens and generation on background threads
Picking up items: multiple items per tile
Monsters with skills
	Monsters with targeting
Nonhostile monsters (factions)
Monsters spawn with items and drop them
Rules from file (for monster/cell interactions)
"seen tiles" on the map editor
Dropdown for entity selection
Dropdown for cell selection
Key binding
Mouseover descriptions
Vault based map gen
Mapgen visualiser (debug menu)
Better tunneler
Animation system
Tiles renderer
Use item on item
Use item on entity
Audio system (beep boop)
Random item generation
Shops
Traps
Encounter based map population
View message log scrollback
Remembered state of altered map tiles (e.g. when tile is dug out of player's FoV)
Move RendererFactory into ActivityStack

1.0+

Stock of monsters
Stock of items
Stock of vaults