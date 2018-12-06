using data_rogue_core.Data;
using data_rogue_core.EntitySystem;

namespace data_rogue_core.Components
{
    public class Position : IEntityComponent
    {
        public MapCoordinate MapCoordinate;

        public void Move(Vector vector)
        {
            MapCoordinate.X += vector.X;
            MapCoordinate.Y += vector.Y;
        }

        public static Position operator+(Position position, Vector vector)
        {
            var newPosition = new Position
            {
                MapCoordinate = new MapCoordinate(
                    mapKey: position.MapCoordinate.Key,
                    x: position.MapCoordinate.X + vector.X,
                    y: position.MapCoordinate.Y + vector.Y
                )
            };

            return newPosition;
        }
    }
}
