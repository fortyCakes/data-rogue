using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using System.Drawing;

namespace data_rogue_core
{
    public class WorldGenerator
    {
        public static WorldState Create(int seed, IEntityEngineSystem entityEngineSystem)
        {
            entityEngineSystem.Initialise();

            var state = new WorldState(entityEngineSystem);

            var emptyCell = state.EntityEngineSystem.New(
                new Appearance { Color = Color.LightGray, Glyph = '.' },
                new Physical { Passable = true, Transparent = true }
                );

            var wallCell = state.EntityEngineSystem.New(
                new Appearance { Color = Color.LightGray, Glyph = '#' },
                new Physical { Passable = false, Transparent = false }
            );

            var testMap = new Map("testMap", wallCell);

            

            testMap.SetCellsInRange(-5, 5, -5, 5, emptyCell);

            state.Maps = new MapCollection();
            state.Maps.AddMap(testMap);
            state.CurrentMap = testMap;

            state.Player = state.EntityEngineSystem.New(
                new Appearance { Color = Color.White, Glyph = '@', ZOrder = int.MaxValue },
                new Position { MapCoordinate = new MapCoordinate(testMap.MapKey, 0, 0) },
                new PlayerControlled(),
                new Physical { Passable = false, Transparent = true }
                );

            return state;
        }
    }
}