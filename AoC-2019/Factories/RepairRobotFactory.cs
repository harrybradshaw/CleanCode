using System.Collections.Generic;
using System.Drawing;

namespace CleanCode
{
    public class RepairRobotFactory
    {
        public RepairRobot Create(string inputString)
        {
            return new RepairRobot
            {
                Steps = 0,
                Computer = new IntcodeComputer(inputString)
                {
                    AwaitInput = true,
                },
                VisitedLocations = new HashSet<Point>(),
                Location = new Point(0,0)
            };
        }

        public RepairRobot CloneRobot(RepairRobot robotToClone)
        {
            return new RepairRobot
            {
                Steps = robotToClone.Steps,
                Computer = new IntcodeComputer(robotToClone.Computer.IntcodeProgram)
                {
                    IntcodeIoHandler = new IntcodeIoHandler(robotToClone.Computer.IntcodeIoHandler.InputList),
                    AwaitInput = true,
                },
                VisitedLocations = robotToClone.VisitedLocations,
                Location = robotToClone.Location,
            };
        }
    }
}