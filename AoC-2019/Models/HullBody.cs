using System.IO;

namespace CleanCode
{
    public class HullBody
    {
        public int [,] Body = new int[200, 200];
        public int HullX => Body.GetLength(0);
        public int HullY => Body.GetLength(1);

        public void PrintBody()
        {
            string path = @"C:\\Work\Training\AoC2019-Day11-Output.txt";
            using (FileStream fs = File.Create(path))
            {
                using (var sr = new StreamWriter(fs))
                {
                    for (var y = Body.GetLength(1) - 1; y >= 0; y--)
                    {
                        var line = "";
                        for (var x = 0; x < Body.GetLength(0); x++)
                        {
                            line += Body[x, y];
                        }
                        line = line.Replace('1', (char)9608);
                        line = line.Replace('0',' ');
                        sr.WriteLine(line);
                    }
                }
            }
        }
    }
}