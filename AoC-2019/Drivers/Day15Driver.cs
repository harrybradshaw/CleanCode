using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace CleanCode
{
    public class Day15Driver
    {
        private readonly RepairRobotFactory _repairRobotFactory;
        private List<MapLocation> Map { get; set; } = new List<MapLocation>();

        public Day15Driver()
        {
            _repairRobotFactory = new RepairRobotFactory();
        }

        public void Run()
        {
            //Part1();
            Part2();
        }

        private void Part1()
        {
            const string inputString =
                "3,1033,1008,1033,1,1032,1005,1032,31,1008,1033,2,1032,1005,1032,58,1008,1033,3,1032,1005,1032,81,1008,1033,4,1032,1005,1032,104,99,101,0,1034,1039,1002,1036,1,1041,1001,1035,-1,1040,1008,1038,0,1043,102,-1,1043,1032,1,1037,1032,1042,1105,1,124,1001,1034,0,1039,1001,1036,0,1041,1001,1035,1,1040,1008,1038,0,1043,1,1037,1038,1042,1105,1,124,1001,1034,-1,1039,1008,1036,0,1041,1001,1035,0,1040,101,0,1038,1043,102,1,1037,1042,1106,0,124,1001,1034,1,1039,1008,1036,0,1041,1001,1035,0,1040,101,0,1038,1043,1001,1037,0,1042,1006,1039,217,1006,1040,217,1008,1039,40,1032,1005,1032,217,1008,1040,40,1032,1005,1032,217,1008,1039,9,1032,1006,1032,165,1008,1040,9,1032,1006,1032,165,1101,2,0,1044,1106,0,224,2,1041,1043,1032,1006,1032,179,1102,1,1,1044,1105,1,224,1,1041,1043,1032,1006,1032,217,1,1042,1043,1032,1001,1032,-1,1032,1002,1032,39,1032,1,1032,1039,1032,101,-1,1032,1032,101,252,1032,211,1007,0,73,1044,1105,1,224,1101,0,0,1044,1106,0,224,1006,1044,247,101,0,1039,1034,102,1,1040,1035,102,1,1041,1036,1002,1043,1,1038,1001,1042,0,1037,4,1044,1106,0,0,57,60,59,78,42,22,33,69,5,77,92,10,55,22,99,62,27,32,75,13,82,48,40,83,95,38,62,65,70,77,79,61,2,47,27,84,46,48,16,15,87,87,23,95,97,16,93,79,27,7,98,97,76,44,6,75,85,51,8,91,94,99,35,28,84,67,83,82,1,80,40,99,81,92,41,97,87,28,81,52,93,37,27,85,76,4,18,80,96,61,83,16,90,86,77,8,76,55,51,61,72,90,90,5,96,75,13,56,40,82,11,97,21,55,95,17,93,97,16,91,30,77,28,32,77,96,49,13,97,30,14,26,93,61,18,32,85,95,81,65,98,49,65,84,46,19,81,45,76,22,88,79,63,84,60,24,37,4,34,80,98,61,95,46,88,99,76,3,92,75,12,95,9,98,94,57,41,77,52,17,80,83,17,83,59,87,85,2,95,88,41,32,98,72,95,23,91,83,65,82,47,90,17,81,67,81,6,90,61,44,85,40,85,19,82,26,86,1,74,62,6,75,98,93,12,84,11,38,57,88,91,76,66,43,46,84,38,48,99,57,20,97,34,72,75,47,94,33,83,82,55,76,34,90,9,74,34,87,18,84,57,38,76,3,31,87,77,2,86,15,23,92,37,20,75,94,27,88,90,87,28,84,31,92,15,75,14,10,82,48,99,49,79,4,96,91,30,16,78,7,75,67,98,1,75,49,70,87,95,89,30,54,31,92,71,57,39,79,19,55,85,25,92,4,23,77,74,77,89,40,39,77,87,39,44,81,98,80,94,29,40,88,69,74,92,54,2,1,83,30,87,85,15,80,97,94,15,93,89,71,69,86,81,75,56,92,58,95,51,93,29,82,79,24,82,9,46,5,88,53,75,90,31,6,91,69,88,8,10,23,67,5,85,78,71,36,98,19,98,53,66,43,17,91,62,28,86,44,79,33,91,79,66,71,86,98,86,60,83,44,94,96,29,85,89,50,85,98,14,8,43,5,75,62,89,90,33,30,86,48,75,99,88,82,45,99,38,60,76,69,45,89,27,93,71,84,96,15,52,85,58,97,99,20,5,79,75,33,80,84,63,87,1,77,31,44,49,99,79,3,83,16,91,42,7,99,66,82,93,76,31,55,77,30,45,88,3,81,53,30,95,24,90,96,26,33,93,32,40,25,76,21,75,17,89,31,81,18,29,97,72,60,90,14,99,93,9,55,92,26,86,28,99,93,1,98,86,60,26,68,95,7,17,86,94,48,68,95,94,88,86,67,82,77,55,76,68,79,76,65,76,21,23,78,28,82,75,23,84,74,67,69,83,70,93,38,75,81,61,87,23,86,92,43,13,23,81,65,65,76,41,34,95,64,89,90,72,79,16,47,84,47,28,83,74,1,41,59,87,84,34,32,94,4,78,37,17,99,86,91,58,97,69,26,93,41,81,36,78,80,18,79,96,51,37,74,45,94,54,98,90,74,27,8,89,79,81,71,60,88,40,86,33,95,98,21,13,9,20,94,48,82,42,69,15,84,45,89,42,98,99,72,72,84,86,54,80,74,68,74,5,85,80,75,34,29,98,16,85,16,86,13,25,74,95,51,86,90,28,81,90,20,70,7,89,37,74,28,86,29,81,95,66,1,64,83,85,27,74,4,69,54,79,66,50,96,43,94,95,45,52,83,17,37,88,85,55,35,66,78,66,86,4,92,2,99,35,89,13,76,71,81,92,96,18,85,68,95,61,97,76,82,66,85,99,82,6,93,31,81,76,80,27,95,38,94,85,98,41,91,0,0,21,21,1,10,1,0,0,0,0,0,0";
            var robot = _repairRobotFactory.Create(inputString);
            SingleMovement(robot);
            SaveMapToFile();
        }

        private void Part2()
        {
            Map = ReadMapFromFile();
            DiffuseUntilComplete();
        }

        private void DiffuseUntilComplete()
        {
            var i = 0;
            while (!Map.All(ml =>
                       ml.Contains == LocationContains.Oxygen || ml.Contains == LocationContains.Wall))
            {
                DiffuseOxygen();
                i++;
                Console.WriteLine(i);
            }
        }

        private void DiffuseOxygen()
        {
            var newSites = Map
                .Where(ml => ml.Contains == LocationContains.Oxygen)
                .SelectMany(GetNewOxygenSites)
                .ToList();
            ApplyNewSitesToMap(newSites);
        }

        private void ApplyNewSitesToMap(List<MapLocation> oxygenSites)
        {
            foreach (var site in oxygenSites)
            {
                ApplySiteToMap(site);
            }
        }

        private void ApplySiteToMap(MapLocation site)
        {
            var locationIndex = Map.FindIndex(ml => ml.Location == site.Location);
            if (locationIndex > -1 && Map[locationIndex].Contains == LocationContains.Empty)
            {
                Map[locationIndex] = site;
            }
        }

        private IEnumerable<MapLocation> GetNewOxygenSites(MapLocation oxygenSite)
        {
            return Enum.GetValues(typeof(Direction))
                .Cast<Direction>()
                .Select(oxygenSite.GetNewLocation)
                .Select(nl => new MapLocation
                {
                    Location = nl,
                    Contains = LocationContains.Oxygen
                });
        }

        private void SingleMovement(RepairRobot robot)
        {
            var directions = robot.AvailableDirections();
            directions.ForEach(direction =>
            {
                var newRobot = _repairRobotFactory.CloneRobot(robot);
                var response = newRobot.PerformMovement(direction);
                ProcessResponse(response, newRobot);
            });
        }

        private void ProcessResponse(RepairDroidResponse response, RepairRobot robot)
        {
            Map.Add(new MapLocation
            {
                Contains = GetLocationContents(response.Status),
                Location = response.Location,
            });
            
            switch (response.Status)
            {
                case RepairDroidStatus.HitWall:
                    return;
                case RepairDroidStatus.Found:
                    Console.WriteLine($"Found - {robot.Steps}");
                    break;
                case RepairDroidStatus.Moved:
                    SingleMovement(robot);
                    break;
            }
            
        }

        private LocationContains GetLocationContents(RepairDroidStatus status)
        {
            switch (status)
            {
                case RepairDroidStatus.HitWall:
                    return LocationContains.Wall;
                case RepairDroidStatus.Found:
                    return LocationContains.Oxygen;
                case RepairDroidStatus.Moved:
                    return LocationContains.Empty;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        private void SaveMapToFile()
        {
            string path = @"C:\\Work\Training\AoC2019-Day15-Output.txt";
            using (FileStream fs = File.Create(path))
            {
                using (var sr = new StreamWriter(fs))
                {
                    Map.ForEach(mapLocation => sr.WriteLine(mapMapLocationToString(mapLocation)));
                }
            }
        }

        private List<MapLocation> ReadMapFromFile()
        {
            string path = @"C:\\Work\Training\AoC2019-Day15-Output.txt";
            var lines = File.ReadLines(path);
            return lines.Select(mapLocationFromString).ToList();
        }

        private string mapMapLocationToString(MapLocation mapLocation)
        {
            return $"{mapLocation.Location.X},{mapLocation.Location.Y},{(int)mapLocation.Contains}";
        }

        private MapLocation mapLocationFromString(string line)
        {
            var lineParts = line.Split(',');
            return new MapLocation
            {
                Location = new Point(int.Parse(lineParts[0]), int.Parse(lineParts[1])),
                Contains = (LocationContains) int.Parse(lineParts[2])
            };
        }
    }
}