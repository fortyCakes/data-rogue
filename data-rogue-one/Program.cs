using data_rogue_core;
using data_rogue_core.EventSystem.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_one
{
    public class Program
    {
        static void Main(string[] args)
        {
            var theGame = new DataRogueGame();

            theGame.Run(DataRogueGame.DEBUG_SEED, GameRules.Rules);
        }
    }
}
