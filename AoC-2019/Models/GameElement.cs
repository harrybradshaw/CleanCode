namespace CleanCode
{
    public class GameElement
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Value { get; set; }
        public bool ElementIsScoreboard => this.GetIsElementScoreboard();
    }
}