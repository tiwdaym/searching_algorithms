using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SearchingAlgorithms.Collections;

namespace SearchingAlgorithms
{
    class Evolution<TChromosome, TGene>
        where TChromosome : IEvaluable<TGene>
        where TGene : IEquatable<TGene>, IHashable, IRandomizable<TGene>
    {
        #region Global data and settings
        TChromosome evaluator;
        TGene randomizer;
        bool isProcessingChangesDisabled = false;
        static Random rnd = new Random();
        #endregion


        #region DNA chromosome data and settings
        public const uint FITNESS_OFFSET = 1; //We want Fitness to be always at least 1 for evaluating.
        uint dnaLength;
        public uint DnaLength
        {
            get => dnaLength;
            set
            {
                if (isProcessingChangesDisabled) throw new InvalidOperationException("Cannot change DNA length while processing.");
                if (value == 0) throw new ArgumentNullException("DNA length cannot be 0");
                dnaLength = value;
            }
        }
        #endregion


        #region Population data and settings
        DNA<TGene>[] population;
        public DNA<TGene>[] Population
        {
            get
            {
                DNA<TGene>[] populationCopy = new DNA<TGene>[PopulationSize];
                population.CopyTo(populationCopy, 0);
                return populationCopy;
            }
        }

        /// <summary>
        /// Total size or count of population to work with in one generation
        /// </summary>
        public uint PopulationSize
        {
            get => populationSize;
            set
            {
                if (isProcessingChangesDisabled) throw new InvalidOperationException("Cannot change populationCount while processing.");
                populationSize = value;
            }
        }
        uint populationSize;

        ulong totalPopulationFitness;
        public ulong TotalPopulationFitness { get => totalPopulationFitness; }
        #endregion


        #region Generations data and settings
        uint currentGeneration = 0;
        public uint CurrentGeneration { get => currentGeneration; }

        uint maxGenerationsCount;
        public uint MaxGenerationsCount
        {
            get => maxGenerationsCount;
            set
            {
                if (isProcessingChangesDisabled) throw new InvalidOperationException("Cannot change Max Generations Count while processing.");
                maxGenerationsCount = value;
            }
        }

        uint offspringToKeep;
        public uint OffspringToKeep
        {
            get => offspringToKeep;
            set
            {
                if (isProcessingChangesDisabled) throw new InvalidOperationException("Cannot change Offspring To Keep while processing.");
                offspringToKeep = value;
            }
        }
        public uint RealOffspringToKeep
        {
            get
            {
                return (populationSize / (elitistToKeep + offspringToKeep + randomNewIndividualsCount)) * offspringToKeep;
            }
        }

        uint elitistToKeep;
        public uint ElitistToKeep
        {
            get => elitistToKeep;
            set
            {
                if (isProcessingChangesDisabled) throw new InvalidOperationException("Cannot change Elitist To Keep count while processing.");
                elitistToKeep = value;
            }
        }
        public uint RealElitistToKeep
        {
            get
            {
                return (populationSize / (elitistToKeep + offspringToKeep + randomNewIndividualsCount)) * elitistToKeep;
            }
        }

        uint randomNewIndividualsCount;
        public uint RandomNewIndividualsCount
        {
            get => randomNewIndividualsCount;
            set
            {
                if (isProcessingChangesDisabled) throw new InvalidOperationException("Cannot change randomNewIndividualsCount while processing.");
                randomNewIndividualsCount = value;
            }
        }
        public uint RealRandomNewIndividualsCount
        {
            get
            {
                return populationSize - (RealOffspringToKeep + RealElitistToKeep);
            }
        }
        #endregion



        #region Childrens Crossover/Breeding data and settings
        uint childrenBreedingCrossoverPointsCount;
        public uint ChildrenBreedingCrossoverPointsCount
        {
            get => childrenBreedingCrossoverPointsCount;
            set
            {
                if (isProcessingChangesDisabled) throw new InvalidOperationException("Cannot change childrenBreedingCrossoverPointsCount while processing.");
                if (value < 1) throw new ArgumentOutOfRangeException("There must be at least 1 crossover point for breeding");
                childrenBreedingCrossoverPointsCount = value;
            }
        }

        double mutationChance;
        public double MutationChance
        {
            get => mutationChance;
            set
            {
                if (isProcessingChangesDisabled) throw new InvalidOperationException("Cannot change mutationChance while processing.");
                mutationChance = value;
            }
        }
        #endregion



        public Evolution(TChromosome evaluator, TGene randomizer, uint populationSize = 500, uint dnaLength = 100, uint maxGenerationsCount = 1000,
            uint offspringToKeep = 300, uint elitistToKeep = 100, uint randomNewIndividualsCount = 100,
            uint childrenBreedingCrossoverPointsCount = 2, double mutationChance = 0.05)
        {
            this.evaluator = evaluator;
            this.randomizer = randomizer;
            this.populationSize = populationSize;
            this.dnaLength = dnaLength;
            this.maxGenerationsCount = maxGenerationsCount;
            this.offspringToKeep = offspringToKeep;
            this.elitistToKeep = elitistToKeep;
            this.randomNewIndividualsCount = randomNewIndividualsCount;
            this.childrenBreedingCrossoverPointsCount = childrenBreedingCrossoverPointsCount;
            this.mutationChance = mutationChance;
        }

        /// <summary>
        /// Function will run whole Evolution Process
        /// 1. Initialize Random Population
        /// 2. Evaluate Current Population Fitness
        /// 3. while current generation is lower than maximum count of generations do:
        ///     3.1. Generate New Generation
        ///     3.2. Evaluate Current Population Fitness
        ///     3.3. increment current generation
        /// </summary>
        public void RunEvolutionProcess()
        {
            isProcessingChangesDisabled = true;
            currentGeneration = 0;
            InitializeRandomPopulation();
            EvaluateCurrentPopulationFitness();
            while (currentGeneration < maxGenerationsCount)
            {
                GenerateNewGeneration();
                EvaluateCurrentPopulationFitness();
            }
            isProcessingChangesDisabled = false;
        }

        /// <summary>
        /// This function initializes Random Population for start
        /// </summary>
        public void InitializeRandomPopulation()
        {
            isProcessingChangesDisabled = true;
            this.population = new DNA<TGene>[populationSize];
            this.population = CreateRandomPopulation(populationSize);
        }


        /// <summary>
        /// Function will generate Random Population
        /// </summary>
        /// <param name="randomPopulationCount">Count of Random Individuals</param>
        /// <returns></returns>
        public DNA<TGene>[] CreateRandomPopulation(uint randomPopulationCount)
        {
            DNA<TGene>[] randomPopulation = new DNA<TGene>[randomPopulationCount];
            for (int i = 0; i < randomPopulationCount; i++) randomPopulation[i] = DNA<TGene>.Random(randomizer, this.dnaLength);
            return randomPopulation;
        }


        /// <summary>
        /// Function will evaluate and sort current population based on Fitness
        /// </summary>
        public void EvaluateCurrentPopulationFitness()
        {
            HeapMaxList<DNA<TGene>> sorter = new HeapMaxList<DNA<TGene>>(PopulationSize);

            DNA<TGene> currentDna;
            totalPopulationFitness = 0;

            for (int i = 0; i < PopulationSize; i++)
            {
                currentDna = population[i];
                currentDna.Fitness = Evaluate(currentDna);
                totalPopulationFitness += currentDna.Fitness;
                sorter.Add(currentDna);
            }

            for (int i = 0; i < PopulationSize; i++) population[i] = sorter.RemoveMax();
        }


        /// <summary>
        /// Function will generate new generation
        /// </summary>
        public void GenerateNewGeneration()
        {
            DNA<TGene>[] newPopulation = new DNA<TGene>[populationSize];
            //1.Choose elitist
            Array.Copy(population, 0, newPopulation, 0, this.RealElitistToKeep);
            //2.New population from osspring
            DNA<TGene>[] offspring = StochasticUniversalSampling();
            DNA<TGene>[] newChildrens = BreedChildrensFromParents(offspring);
            MutatePopulation(newChildrens, this.mutationChance);
            Array.Copy(newChildrens, 0, newPopulation, this.RealElitistToKeep, this.RealOffspringToKeep);
            //3. New random individuals
            DNA<TGene>[] randomIndividuals = CreateRandomPopulation(this.RealRandomNewIndividualsCount);
            Array.Copy(randomIndividuals, 0, newPopulation, this.RealElitistToKeep + this.RealOffspringToKeep, this.RealRandomNewIndividualsCount);
            //4. Assign new population
            population = newPopulation;
            currentGeneration++;
        }


        /// <summary>
        /// Function will select offspring to breed - elements in result are sorted from highest fitness to lowest.
        /// Higher fitness individuals have more occurences in array.
        /// </summary>
        /// <returns></returns>
        public DNA<TGene>[] StochasticUniversalSampling()
        {
            DNA<TGene>[] offspringSelection = new DNA<TGene>[this.offspringToKeep];
            ulong distanceBetweenPointers = this.totalPopulationFitness / this.offspringToKeep;
            uint randomStart = (uint)rnd.Next((int)distanceBetweenPointers);

            uint populationPointer = 0;
            ulong populationFitnessPointer = this.population[populationPointer].Fitness;
            for (uint i = 0; i < this.offspringToKeep; i++)
            {
                while (randomStart + i * distanceBetweenPointers >= populationFitnessPointer)
                {
                    populationPointer++;
                    populationFitnessPointer += this.population[populationPointer].Fitness;
                }
                offspringSelection[i] = this.population[populationPointer];
            }

            return offspringSelection;
        }


        /// <summary>
        /// Function will create 2 new childrens from 2 parents
        /// </summary>
        /// <param name="parent1">First parent to breed</param>
        /// <param name="parent2">Second parent to breed</param>
        /// <param name="children1">First breeded children</param>
        /// <param name="children2">Second breeded children</param>
        public void BreedNewChildrens(DNA<TGene> parent1, DNA<TGene> parent2, out DNA<TGene> children1, out DNA<TGene> children2)
        {
            int chromosomeLength = Math.Max(parent1.chromosome.Length, parent2.chromosome.Length);
            TGene[] children1Chromosome = new TGene[chromosomeLength];
            TGene[] children2Chromosome = new TGene[chromosomeLength];
            TGene[] tmpChildren; //to switch childrens
            int randomNumber = 0;

            HeapMinList<int> crossoverPoints = new HeapMinList<int>(childrenBreedingCrossoverPointsCount);

            while (crossoverPoints.Count < childrenBreedingCrossoverPointsCount)
            {
                randomNumber = rnd.Next(1, chromosomeLength); //min 1 for guaranted crossover points count
                if (!crossoverPoints.Contains(randomNumber)) crossoverPoints.Add(randomNumber);
            }

            int indexStart = 0;
            for (int i = 0; i < childrenBreedingCrossoverPointsCount; i++)
            {
                randomNumber = crossoverPoints.RemoveMin();
                Array.Copy(parent1.chromosome, indexStart, children1Chromosome, indexStart, randomNumber - indexStart);
                Array.Copy(parent2.chromosome, indexStart, children2Chromosome, indexStart, randomNumber - indexStart);
                indexStart = randomNumber;

                tmpChildren = children1Chromosome;
                children1Chromosome = children2Chromosome;
                children2Chromosome = tmpChildren;
            }

            //finisher
            Array.Copy(parent1.chromosome, indexStart, children1Chromosome, indexStart, chromosomeLength - indexStart);
            Array.Copy(parent2.chromosome, indexStart, children2Chromosome, indexStart, chromosomeLength - indexStart);

            children1 = new DNA<TGene>(children1Chromosome);
            children2 = new DNA<TGene>(children2Chromosome);
        }


        /// <summary>
        /// Function will create childrens from provided parents
        /// </summary>
        /// <param name="parents"></param>
        /// <returns></returns>
        public DNA<TGene>[] BreedChildrensFromParents(DNA<TGene>[] parents)
        {
            DNA<TGene>[] childrens = new DNA<TGene>[parents.Length];
            parents = parents.OrderBy(x => rnd.Next()).ToArray();

            int parentsLength = parents.Length;

            if (parentsLength % 2 == 1) parentsLength--;
            for (int i = 0; i < parentsLength; i += 2) BreedNewChildrens(parents[i], parents[i + 1], out childrens[i], out childrens[i + 1]);

            if (parentsLength % 2 == 1) BreedNewChildrens(parents[rnd.Next(parents.Length)], parents[rnd.Next(parents.Length)], out childrens[parents.Length - 1], out childrens[parents.Length - 1]);

            return childrens;
        }

        /// <summary>
        /// Function will just mutate Individual
        /// </summary>
        /// <param name="individual"></param>
        public void MutateIndividual(DNA<TGene> individual)
        {
            individual.chromosome[rnd.Next(0, (int)dnaLength)] = randomizer.NextRandom();
        }

        /// <summary>
        /// Function will mutate provided population with mutation chance
        /// </summary>
        /// <param name="population"></param>
        public void MutatePopulation(DNA<TGene>[] population, double mutationChance)
        {
            for (int i = 0; i < population.Length; i++)
            {
                if (rnd.NextDouble() < mutationChance) MutateIndividual(population[i]);
            }
        }

        /// <summary>
        /// Function will Evaluate fitness of one DNA chromosome.
        /// For special data
        /// </summary>
        /// <param name="IO_DATA"></param>
        /// <returns></returns>
        public uint Evaluate(DNA<TGene> dnaEvaluating)
        {
            #region checks
            if (dnaEvaluating == null) throw new ArgumentNullException("DNA cannot be null.");
            if (dnaEvaluating.chromosome == null) throw new ArgumentNullException("DNA chromosome cannot be null.");
            #endregion

            return evaluator.Fitness(dnaEvaluating.chromosome) + FITNESS_OFFSET;
        }


        /// <summary>
        /// This should be called to clean all data to default values
        /// </summary>
        public void ResetProcessing()
        {
            currentGeneration = 0;
            isProcessingChangesDisabled = false;
        }

    }
}
