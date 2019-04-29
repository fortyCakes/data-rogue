namespace data_rogue_core.Activities
{
    public delegate void PositionEventHandler(object sender, PositionEventHandlerArgs args);

    public class PositionEventHandlerArgs
    {
        public PositionEventHandlerArgs(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }
}