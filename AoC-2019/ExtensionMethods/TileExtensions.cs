using System.Collections.Generic;
using System.Linq;

namespace CleanCode
{
    public static class TileExtensions
    {
        public static IEnumerable<IEnumerable<T>> SplitIntoChunks<T>
            (this IEnumerable<T> source, int itemsPerSet) 
        {
            var sourceList = source as List<T> ?? source.ToList();
            for (var index = 0; index < sourceList.Count; index += itemsPerSet)
            {
                yield return sourceList.Skip(index).Take(itemsPerSet);
            }
        }
        
        public static bool GetIsElementScoreboard(this GameElement tile)
        {
            return tile.X == -1 && tile.Y == 0;
        }
    }
}