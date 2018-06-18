using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms.Collections
{
    class DNA : IEquatable<DNA>, IHashable
    {
        public uint[] chromosome;

        static Random rnd = null;

        public DNA(uint[] chromosome)
        {
            if (chromosome == null) throw new ArgumentNullException("Chromosome cannot be null.");
            this.chromosome = chromosome;
            if (rnd == null) rnd = new Random();
        }

        public bool Equals(DNA other)
        {
            int chromosomeLength = chromosome.Length;
            if (chromosomeLength != other.chromosome.Length) return false;
            for (int i = 0; i < chromosomeLength; i++) if (chromosome[i] != other.chromosome[i]) return false;
            return true;
        }

        public uint GetHash()
        {
            uint hash = 0;
            uint seed = 101;
            for (int i = chromosome.Length - 1; i >= 0; i--) hash = hash * seed + chromosome[i];
            return hash;
        }

        public static DNA Random(uint length, int operationsCount)
        {
            uint[] chromosome = new uint[length];

            for (uint i = 0; i < length; i++)
            {
                chromosome[i] = (uint)rnd.Next(0, operationsCount);
            }

            return new DNA(chromosome);
        }

    }
}

