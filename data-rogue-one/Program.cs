using data_rogue_core;
using data_rogue_core.Systems;

namespace data_rogue_one
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var theGame = new DataRogueGame();

            theGame.Run(DataRogueGame.DEBUG_SEED, GameRules.Rules);
        }
    }
}
