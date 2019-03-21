== BUGS ==

- 

== 0.1 ==

- Stackable items
- Ranged and magic weapons
- 	"fire" command
- 	Apply combat stat (aegis)
- Separate out core from implementation
- 	Make stats window defined in data
- 	Aura and Tilt separation 
-		(remove AuraFighter/TiltFighter?)
-		Stat window rendering customisation
-		Map rendering customisation
- Move RendererFactory into ActivityStack
- Remove RLNet dependencies from core (!)
- Decouple dataloading from test entity engine
-	In fact generally decouple the system testing, don't create a full systemcontainer
- Event system rework: separate pre- and post-event rules

== 0.1.5 ==
- Tiles renderer

== 0.2 ==
- Morgue file format and death screen
- Dependency injection? for rules and behaviours
- Finish importing all tiers of base items
- Smooth rendering while resting
- Loading screens and generation on background threads
- Picking up items: multiple items per tile
- Monsters with skills
- 	Monsters with targeting
- Nonhostile monsters (factions)
- Monsters spawn with items and drop them
- Messages update
- 	-- More... -- when many messages occur
- 	channels of messages: configure colour + on/off
- Rules from file (for monster/cell interactions)
- "seen tiles" on the map editor
- Dropdown for entity selection
- Dropdown for cell selection
- Mouseover descriptions
- Vault based map gen
- Mapgen visualiser (debug menu)
- Better tunneler
- Animation system
- 	Toast messages (e.g. when entering a floor)
- 	Convert combat information to animations (floating numbers for damage, etc);
- Use item on item
- Use item on entity
- 	Already doable? Just call targeting system?
- Audio system (beep boop)
- Random item generation
- Shops
- Traps
- Encounter based map population
- View message-log history
- Remembered state of altered map tiles (e.g. when tile is dug out of player's FoV) - ghost images

== 1.0+ ==

- Stock of monsters
- Stock of items
- Stock of vaults