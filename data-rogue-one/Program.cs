using data_rogue_core;

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
