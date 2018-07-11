using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SearchingAlgorithms;
using SearchingAlgorithms.Collections;

public class Demo
{
    ConsoleKeyInfo input;
    NPuzzle defaultPuzzle;
    NPuzzle myPuzzle;
    NPuzzle newPuzzle = null;
    BFS<NPuzzle> bfs;
    GreedySearch<NPuzzle> greedy;
    AStar<NPuzzle> astar;
    DFS<NPuzzle> dfs;
    IDAStar<NPuzzle> idastar;
    Evolution<NPuzzle, NPuzzle.Operation> evolution;

    static void Main(string[] args)
    {
        Demo main = new Demo();
        main.Run();
    }

    public Demo()
    {
        defaultPuzzle = new NPuzzle(4, 4);
        myPuzzle = defaultPuzzle.GenerateRandomState(30);
        bfs = new BFS<NPuzzle>(myPuzzle, defaultPuzzle);
        bfs.MaxSearchingTime = 40;
        greedy = new GreedySearch<NPuzzle>(myPuzzle, defaultPuzzle);
        greedy.MaxSearchingTime = 40;
        astar = new AStar<NPuzzle>(myPuzzle, defaultPuzzle);
        //astar.HashSize = 2097152;
        //astar.MaxGeneratedElementsCount = 2097152;
        astar.MaxSearchingTime = 40;
        dfs = new DFS<NPuzzle>(myPuzzle, defaultPuzzle);
        dfs.MaxSearchingTime = 40;
        idastar = new IDAStar<NPuzzle>(myPuzzle, defaultPuzzle);
        //idastar.HashSize = 2097152;
        //idastar.MaxStackSize = 2097152;
        idastar.MaxSearchingTime = 40;

        evolution = new Evolution<NPuzzle, NPuzzle.Operation>(defaultPuzzle, new NPuzzle.Operation());
        NPuzzle.FitnessStartState = myPuzzle;
        NPuzzle.FitnessFinishState = defaultPuzzle; //For Evolution
    }

    public void Run()
    {
        Console.WriteLine("Start!");
        printPuzzleState(myPuzzle, defaultPuzzle);


        input = Console.ReadKey();
        while (input.Key != ConsoleKey.Q)
        {
            switch (input.Key)
            {
                case ConsoleKey.UpArrow:
                    newPuzzle = myPuzzle.GenerateNewState(NPuzzle.operations.up.ToString());
                    break;
                case ConsoleKey.DownArrow:
                    newPuzzle = myPuzzle.GenerateNewState(NPuzzle.operations.down.ToString());
                    break;
                case ConsoleKey.RightArrow:
                    newPuzzle = myPuzzle.GenerateNewState(NPuzzle.operations.right.ToString());
                    break;
                case ConsoleKey.LeftArrow:
                    newPuzzle = myPuzzle.GenerateNewState(NPuzzle.operations.left.ToString());
                    break;
                default:
                    Console.WriteLine("\nPress Up, Down, Right or Left arrow. Or q to exit.");
                    break;
            }
            if (newPuzzle != null) myPuzzle = newPuzzle;
            printPuzzleState(myPuzzle, defaultPuzzle);
            input = Console.ReadKey();
        }
    }


    void printPuzzleState(NPuzzle startState, NPuzzle finishState)
    {
        Console.WriteLine("\n--------------");
        for (int rows = 0; rows < startState.Rows; rows++)
        {
            for (int cols = 0; cols < startState.Columns; cols++)
            {
                if (startState.ByteState[rows, cols] == 0) Console.Write("  ");
                else Console.Write("{0,2:##}", startState.ByteState[rows, cols]);
                if (cols < startState.Columns - 1) Console.Write(" ");
            }
            Console.WriteLine();
        }
        //BFS search
        bfs.StartState = startState;
        bfs.FinishState = finishState;

        Console.WriteLine("\nDisplacement: " + NPuzzle.HeuristicPosition(startState, finishState) + " | Manhattan: " + NPuzzle.HeuristicManhattanDistance(startState, finishState) + " | M.Collisions: " + NPuzzle.HeuristicManhattanCollistionsDistance(startState, finishState));
        try
        {
            bfs.UseBidirectionalSearch = false;
            bfs.findPath();
            Console.WriteLine("BFS Search:");
            printNicePathStatistics(bfs.PathResult);
        }
        catch (OutOfMemoryException e)
        {
            Console.WriteLine("BFS Search is Out of memory: " + e.Message);
            bfs.ResetProcessing();
        }

        try
        {
            bfs.UseBidirectionalSearch = true;
            bfs.findPath();
            Console.WriteLine("BFS Bi-Search:");
            printNicePathStatistics(bfs.PathResult);
        }
        catch (OutOfMemoryException e)
        {
            Console.WriteLine("BFS Bi-Search is Out of memory: " + e.Message);
            bfs.ResetProcessing();
        }

        //Greedy search
        greedy.StartState = startState;
        greedy.FinishState = finishState;
        greedy.HeuristicParam = (int)NPuzzle.heuristics.distanceWithCollisions;
        try
        {
            greedy.findPath();
            Console.WriteLine("Greedy Search:");
            printNicePathStatistics(greedy.PathResult);
        }
        catch (OutOfMemoryException e)
        {
            Console.WriteLine("Greedy Search is Out of memory: " + e.Message);
            greedy.ResetProcessing();
        }

        //AStar search
        astar.StartState = startState;
        astar.FinishState = finishState;
        astar.HeuristicParam = (int)NPuzzle.heuristics.distanceWithCollisions;
        try
        {
            astar.findPath();
            Console.WriteLine("A* Search:");
            printNicePathStatistics(astar.PathResult);
        }
        catch (OutOfMemoryException e)
        {
            Console.WriteLine("A* Search is Out of memory: " + e.Message);
            astar.ResetProcessing();
        }

        //DFS search
        dfs.StartState = startState;
        dfs.FinishState = finishState;
        dfs.HeuristicParam = (int)NPuzzle.heuristics.distanceWithCollisions;
        dfs.UseBetterPath = true;
        try
        {
            dfs.findPath();
            Console.WriteLine("DFS Search:");
            printNicePathStatistics(dfs.PathResult);
        }
        catch (OutOfMemoryException e)
        {
            Console.WriteLine("DFS is Out of memory: " + e.Message);
            dfs.ResetProcessing();
        }

        //IDA* search
        idastar.StartState = startState;
        idastar.FinishState = finishState;
        idastar.HeuristicParam = (int)NPuzzle.heuristics.distanceWithCollisions;
        try
        {
            idastar.findPath();
            Console.WriteLine("IDA* Search:");
            printNicePathStatistics(idastar.PathResult);
        }
        catch (OutOfMemoryException e)
        {
            Console.WriteLine("IDA* is Out of memory: " + e.Message);
            idastar.ResetProcessing();
        }
        Console.WriteLine("--------------");

        //Evolution search
        try
        {
            Console.WriteLine("Evolution Search:");
            evolution.ResetProcessing();
            evolution.MaxGenerationsCount = 100;
            evolution.MutationChance = 0.05;
            evolution.InitializeRandomPopulation();
            evolution.EvaluateCurrentPopulationFitness();
            
            for (int i = 0; i < 10000; i++)
            {
                printEvolutionDataSearch(startState, evolution);
                evolution.EvaluateCurrentPopulationFitness();
                evolution.GenerateNewGeneration();
            }
            //evolution.RunEvolutionProcess();
        }
        catch (OutOfMemoryException e)
        {
            Console.WriteLine("Evolution Search is Out of memory: " + e.Message);
            evolution.ResetProcessing();
        }
        Console.WriteLine("--------------");
    }

    void printEvolutionDataSearch(NPuzzle startState, Evolution<NPuzzle, NPuzzle.Operation> evolution)
    {
        DNA<NPuzzle.Operation> individual = evolution.Population[0];
        Console.WriteLine("Best Individual Fitness: " + evolution.Evaluate(individual) + " | Average Fitness: " + evolution.TotalPopulationFitness / evolution.PopulationSize + " | Generation: " + evolution.CurrentGeneration);
        NPuzzle tmpState = startState.GetCopy();
        string currentOperation = "";
        int i = 0;
        while (i < individual.chromosome.Length)
        {
            if (i <= 8) Console.Write(i + ": " + currentOperation + " ");
            if (tmpState.Equals(NPuzzle.FitnessFinishState)) break;
            currentOperation = Enum.GetName(typeof(NPuzzle.operations), individual.chromosome[i].bit);
            tmpState = tmpState.GenerateNewState(currentOperation) ?? tmpState;
            i++;
        }
        Console.WriteLine("Path Length: " + i);
        Console.WriteLine();
    }

    void printNicePathStatistics(GeneratedPath<NPuzzle> path)
    {
        Console.WriteLine("Path Length: " + (path.pathLength - 1) + " | Searched Nodes: " + path.searchedNodes + " | Hash used: " + path.maximumUsedHashMemory + " | Heap used: " + path.maximumUsedHeapMemory + " | Generated Nodes: " + path.generatedNodes);
        for (int i = 0; i < path.pathLength && i <= 8; i++) Console.Write(i + ": " + path.pathOperations[i] + " ");
        Console.WriteLine();
    }

}

