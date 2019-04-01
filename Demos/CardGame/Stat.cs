using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms.Demos.CardGame
{
    public class Stat : IEquatable<Stat>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Value { get; set; }

        public static Stat LoadFromString(string json)
        {
            Stat stat = new Stat()
            {
                
            };

            return stat;
        }

        public bool Equals(Stat other)
        {
            return this == other;
        }
    }
}
