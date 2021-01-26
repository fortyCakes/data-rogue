using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems;
using data_rogue_core.IOSystems.BLTTiles;
using data_rogue_core.IOSystems.RLNetConsole;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_one.IOSystems;

namespace data_rogue_one
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var dataRogueGame = new DataRogueGame();

            IIOSystem ioSystem = BLTTilesConfig.GetBLTTilesIOSystem();

            var additionalComponents = typeof(Program).Assembly.GetTypes().Where(t => t.IsAssignableFrom(typeof(IEntityComponent))).ToList();

            dataRogueGame.Run("SEED_DEBUG", GameRules.Rules, ioSystem, null, additionalComponents);
        }
    }
}
