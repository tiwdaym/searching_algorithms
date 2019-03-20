using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SearchingAlgorithms.Collections;

namespace SearchingAlgorithms.Demos.CardGame
{
    public class CardDeck
    {
        public int MaxCardCount { get; set; }
        public string Tag { get; set; }
        public SingleLinkedList<Card> Deck { get; set; }
    }
}
