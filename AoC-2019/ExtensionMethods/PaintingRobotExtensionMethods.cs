using System;

namespace CleanCode
{
    public static class PaintingRobotExtensionMethods
    {
        public static void LogLocation(this PaintingRobot robot)
        {
            Console.WriteLine($"{robot.XCoOrd}, {robot.YCoOrd}, {robot.Direction}");
        }
    }
}