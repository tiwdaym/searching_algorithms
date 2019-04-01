using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms.Demos.CardGame
{
    public class Ability : IEquatable<Ability>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public void DoAbility(Card card)
        {

        }

        public static Ability LoadFromString(string json)
        {
            Ability ability = new Ability();

            return ability;
        }

        public bool Equals(Ability other)
        {
            return this == other;
        }
    }
}
