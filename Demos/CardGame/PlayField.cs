using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms.Demos.CardGame
{
    public class PlayField
    {
        public CardDeck MyHand { get; set; }
        public CardDeck MyPackage { get; set; }
        public CardDeck MyGraveyard { get; set; }
        public CardDeck MyField { get; set; }

        public CardDeck EnemyHand { get; set; }
        public CardDeck EnemyPackage { get; set; }
        public CardDeck EnemyGraveyard { get; set; }
        public CardDeck EnemyField { get; set; }
        

    }
}
