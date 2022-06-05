using System;
using System.Drawing;

namespace CleanCode
{
    public class BaseLocation
    {
        public Point Location { get; set; }
        
        public Point GetNewLocation(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return new Point(Location.X, 1 + Location.Y);
                case Direction.East:
                    return new Point(Location.X + 1, Location.Y);
                case Direction.South:
                    return new Point(Location.X, Location.Y - 1);
                case Direction.West:
                    return new Point(Location.X - 1, Location.Y);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
    }
}