using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CleanCode
{
    public class RepairRobot : BaseLocation
    {
        public int Steps { get; set; }
        public IntcodeComputer Computer { get; set; }
        public HashSet<Point> VisitedLocations { get; set; }

        public RepairDroidResponse PerformMovement(Direction direction)
        {
            Location = GetNewLocation(direction);
            VisitedLocations.Add(Location);
            Computer.IntcodeIoHandler.InputList.Add((long) direction);
            Steps += 1;
            Computer.RunUntilAwaitingInput();
            return new RepairDroidResponse
            {
                Status = (RepairDroidStatus) Computer.IntcodeIoHandler.LastOutput,
                Location = Location
            };
        }

        public List<Direction> AvailableDirections()
        {
            return Enum.GetValues(typeof(Direction))
                .Cast<Direction>()
                .Where(IsDirectionOkay).ToList()
                .ToList();
        }

        private bool IsDirectionOkay(Direction direction)
        {
            var newLocation = GetNewLocation(direction);
            return !VisitedLocations.Contains(newLocation);
        }
    }
}