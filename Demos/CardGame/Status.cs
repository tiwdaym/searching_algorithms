using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms.Demos.CardGame
{
    public class Status : IEquatable<Status>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int Intensity { get; set; }

        public void DoStatus(Card card)
        {

        }

        public static Status LoadFromJson(string json)
        {
            Status status = new Status();

            return status;
        }

        public bool Equals(Status other)
        {
            return this == other;
        }
    }
}
