using System.Collections.Generic;
using System.Drawing;
using data_rogue_core;
using data_rogue_core.Activities;
using data_rogue_core.IOSystems;
using data_rogue_core.IOSystems.RLNetConsole;
using data_rogue_core.Renderers;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Systems;

namespace data_rogue_one
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var theGame = new DataRogueGame();


            var ioSystem = new RLNetConsoleIOSystem(RLNetConsoleIOSystem.DefaultConfiguration);

            theGame.Run("SEED_DEBUG", GameRules.Rules, ioSystem);
        }
    }
}
