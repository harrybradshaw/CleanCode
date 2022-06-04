using System.Collections.Generic;
using System.Linq;

namespace CleanCode
{
    public class GameElementFactory
    {
        public GameElement Create(IEnumerable<long> tileProperties)
        {
            var listProperties = tileProperties.ToList();
            return new GameElement
            {
                X = (int) listProperties[0],
                Y = (int) listProperties[1],
                Value = (int) listProperties[2]
            };
        }
        
    }
}