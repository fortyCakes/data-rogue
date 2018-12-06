using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using System.Drawing;
using data_rogue_core.Systems;

namespace data_rogue_core
{
    public class WorldGenerator
    {
        public static WorldState Create(int seed)
        {
            var state = new WorldState(new EntityEngine());

            InitialiseSystems(state);

            var emptyCell = state.EntityEngine.New(
                new Appearance { Color = Color.LightGray, Glyph = '.' },
                new Terrain { Passable = true, Transparent = true }
                );

            var wallCell = state.EntityEngine.New(
                new Appearance { Color = Color.LightGray, Glyph = '#' },
                new Terrain { Passable = false, Transparent = false }
            );

            var testMap = new Map("testMap", wallCell);

            

            testMap.SetCellsInRange(-5, 5, -5, 5, emptyCell);

            state.Maps = new MapCollection();
            state.Maps.AddMap(testMap);
            state.CurrentMap = testMap;

            state.Player = state.EntityEngine.New(
                new Appearance { Color = Color.White, Glyph = '@', ZOrder = int.MaxValue },
                new Position { MapCoordinate = new MapCoordinate(testMap.MapKey, 0, 0) },
                new PlayerControlled()
                );

            return state;
        }

        private static void InitialiseSystems(WorldState state)
        {
            state.PositionSystem = new PositionSystem();
            state.EntityEngine.Register(state.PositionSystem);

            state.PlayerControlSystem = new PlayerControlSystem(state.PositionSystem, Game.EventSystem);
            state.EntityEngine.Register(state.PlayerControlSystem);
        }
    }
}