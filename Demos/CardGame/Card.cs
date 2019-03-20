using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms.Demos.CardGame
{
    public class Card : IEquatable<Card>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int StartCost { get; set; }
        public int StartHealth { get; set; }
        public int StartAttack { get; set; }

        public int CurrentCost { get; set; }
        public int CurrentHealth { get; set; }
        public int CurrentAttack { get; set; }

        public Ability[] Abilities { get; set; }

        public Ability[] ActiveAbilities { get; set; }

        public bool Equals(Card other)
        {
            return this.Id == other.Id;
        }
    }
}
