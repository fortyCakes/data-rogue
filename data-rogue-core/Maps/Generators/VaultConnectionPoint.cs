using BLTWrapper;

namespace data_rogue_core.Maps.Generators
{
    public class VaultConnectionPoint
    {
        public VaultConnectionPoint(MapCoordinate position, Vector vector, IMap vault)
        {
            var origin = vault.Origin;
            Coordinate = new MapCoordinate(position.Key, position.X - origin.X + vector.X, position.Y - origin.Y + vector.Y);

            var size = vault.GetSize();

            if (vector.X == origin.X) Facing = TileDirections.Left;
            if (vector.X == origin.X + size.Width) Facing = TileDirections.Right;
            if (vector.Y == origin.Y) Facing = TileDirections.Up;
            if (vector.Y == origin.Y + size.Height) Facing = TileDirections.Down;
        }

        public MapCoordinate Coordinate { get; }
        public TileDirections Facing { get; set; }
    }
}
