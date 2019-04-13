namespace data_rogue_core.IOSystems.BLTTiles
{
    public class GameLoopEventArgs
    {
        public GameLoopEventArgs(long deltaMilliseconds)
        {
            Delta = deltaMilliseconds;
        }

        public long Delta { get; }
    }
}