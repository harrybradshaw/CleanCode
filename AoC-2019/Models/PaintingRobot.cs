using System;
using System.Collections.Generic;

namespace CleanCode
{
    public class PaintingRobot
    {
        public IntcodeComputer Computer { get; set; }
        public int XCoOrd { get; set; } = 100;
        public int YCoOrd { get; set; } = 100;
        public int Direction { get; set; } = 90;
        public HashSet<string> PaintedPanels { get; set; } = new HashSet<string>();

        public void PaintAndTurn(int[] computerOutput, HullBody body)
        {
            body.Body[XCoOrd, YCoOrd] = computerOutput[0];
            PaintedPanels.Add($"{XCoOrd},{YCoOrd}");
            Turn(computerOutput[1]);
            MoveAlongDirection();
        }
        
        public int DetectColor(HullBody body)
        {
            return body.Body[XCoOrd, YCoOrd];
        }
        private void Turn(int directionOutput)
        {
            if (directionOutput == 0)
            {
                Direction += 90;
                Direction %= 360;
                return;
            } 
            Direction -= 90;
            if (Direction < 0)
            {
                Direction += 360;
            }
        }

        private void MoveAlongDirection()
        {
            XCoOrd += (int) Math.Cos(DegreesToRadians(Direction));
            YCoOrd += (int) Math.Sin(DegreesToRadians(Direction));
        }

        private static double DegreesToRadians(int degrees)
        {
            return Math.PI * degrees / 180.0;
        }
    }
}