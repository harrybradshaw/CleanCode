namespace CleanCode
{
    public class PaintingRobotFactory
    {
        public PaintingRobot Create(string inputString)
        {
            var computer = new IntcodeComputer(inputString)
            {
                PauseOnOutput = false,
                PauseOnInput = true,
                IntcodeIoHandler =
                {
                    IncrementOnReadingInput = false
                }
            };

            return new PaintingRobot
            {
                Computer = computer,
            };
        }
    }
}