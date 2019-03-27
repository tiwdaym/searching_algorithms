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

        public Stat[] Stats { get; set; }

        public Ability[] Abilities { get; set; }

        public Status[] Statuses { get; set; }

        public bool Equals(Card other)
        {
            return this == other;
        }

        public static Card LoadFromJson(string json)
        {
            Card card = new Card();

            return card;
        }
    }
}
