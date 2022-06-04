using System.Linq;

namespace CleanCode
{
    public class PaintingService
    {
        public void RunPaintingProgram(PaintingRobot robot, HullBody body)
        {
            while (robot.Computer.State != IntCodeStates.Halted)
            {
                robot.Computer.IntcodeIoHandler.InputList.Add(robot.DetectColor(body));
                robot.Computer.IntcodeIoHandler.OutputList.Clear();
                robot.Computer.RunUntilTwoOutputs();
                if (robot.Computer.State == IntCodeStates.Halted) continue;
                robot.Computer.IntcodeIoHandler.InputList.Clear();
                robot.PaintAndTurn(
                    robot.Computer.IntcodeIoHandler.OutputList
                        .Select(output => (int) output)
                        .ToArray()
                    , body);
            }
        }
    }
}