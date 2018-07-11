using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms.Collections
{
    class DNA<T> : IEquatable<DNA<T>>, IHashable, IComparable<DNA<T>>
        where T: IEquatable<T>, IHashable, IRandomizable<T>
    {
        public T[] chromosome;

        private uint fitness;
        public uint Fitness { get => fitness; set => fitness = value; }

        static Random rnd = new Random();

        public DNA(T[] chromosome, int fitness = 0)
        {
            if (chromosome == null) throw new ArgumentNullException("Chromosome cannot be null.");
            this.chromosome = chromosome;
            this.fitness = 0;
        }

        public bool Equals(DNA<T> other)
        {
            int chromosomeLength = chromosome.Length;
            if (chromosomeLength != other.chromosome.Length) return false;
            for (int i = 0; i < chromosomeLength; i++) if (!chromosome[i].Equals(other.chromosome[i])) return false;
            return true;
        }

        public uint GetHash()
        {
            uint hash = 0;
            uint seed = 101;
            for (int i = chromosome.Length - 1; i >= 0; i--) hash = hash * seed + chromosome[i].GetHash();
            return hash;
        }

        public static DNA<T> Random(T randomizer, uint chromosomeLength)
        {
            T[] chromosome = new T[chromosomeLength];

            for (uint i = 0; i < chromosomeLength; i++) chromosome[i] = randomizer.NextRandom();
            
            return new DNA<T>(chromosome);
        }

        public int CompareTo(DNA<T> other)
        {
            return (int)this.fitness - (int)other.fitness;
        }
    }
}

