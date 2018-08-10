using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SearchingAlgorithms;
using SearchingAlgorithms.Collections;

public class Demo
{
    static void Main(string[] args)
    {
        Demo main = new Demo();
        main.Run();
    }

    public Demo() { }



    public void Run()
    {
        Console.WriteLine("Enter demo command to run that demo. Help for list of commands.");
        string input;
        while ((input = Console.ReadLine()) != "exit")
        {
            switch (input)
            {

                case "reader":
                    ReaderSubstractor();
                    break;
                case "evo-heur":
                    EvolutionHeuristics();
                    break;
                case "generator":
                    GeneratePuzzleStatesDemo();
                    break;
                case "solver":
                    RunPuzzleSolvingDemo();
                    break;
                case "makedb":
                    NPuzzle.InitDatabaseForWholeHeuristic(3, 3);
                    break;
                case "help":
                default:
                    Console.WriteLine("Current commands available:\n" +
                        "help - show this help\n" +
                        "exit - exits program\n" +
                        "reader - run reader\n" +
                        "evo-heur - evolution heuristics search\n" +
                        "generator - generator of states\n" +
                        "solver - solver of puzzle");
                    break;
            }
            Console.WriteLine("Enter demo command to run that demo. Help for list of commands.");
        }
    }


    void EvolutionHeuristics()
    {

    }


    void ReaderSubstractor()
    {
        Console.WriteLine("Enter all possible numbers, then enter exit for end.");
        string input;
        LinkedList<long> list = new LinkedList<long>();
        while ((input = Console.ReadLine()) != "exit")
        {
            list.AddLast(long.Parse(input));
        }

        long data;
        while (list.Count > 1)
        {
            data = list.First.Value;
            list.RemoveFirst();
            data = list.First.Value - data;
            Console.WriteLine(Math.Abs(data));
        }
        Console.WriteLine("Difference of all written down.");
    }


    void GeneratePuzzleStatesDemo()
    {
        byte puzzleRows = 3;
        byte puzzleColumns = 3;
        NPuzzle defaultPuzzle = new NPuzzle(puzzleRows, puzzleColumns);

        StatesGenerator<NPuzzle> statesGenerator = new StatesGenerator<NPuzzle>(defaultPuzzle, 0, 0, 4194304, 4194304);
        GraphNodeSimple<NPuzzle>[] generatedStates = null;

        PrintPuzzleState(defaultPuzzle);
        uint generatingDepth = 0;
        bool onlyChosenDepth = false;
        

        Console.WriteLine("Type Commands. Or help for list of them. Exit to exit.");

        string input = null;
        while ((input = Console.ReadLine().ToLower()) != "exit")
        {
            switch (input)
            {
                case "lookup-make":
                    generatedStates = statesGenerator.GenerateStates();
                    NPuzzle.InitDatabaseForWholeHeuristic(defaultPuzzle.Rows, defaultPuzzle.Columns);
                    Console.WriteLine("Generated States Count: " + generatedStates.Length);
                    NPuzzle.InsertMultiRecordsIntoLookupDb(generatedStates);
                    Console.WriteLine("Insertion Coompleted");
                    break;
                case "toggle-depth":
                    onlyChosenDepth = onlyChosenDepth ? false : true;
                    Console.WriteLine("Only chosen depth is now: " + onlyChosenDepth);
                    break;
                case "generate":
                    if (onlyChosenDepth) generatedStates = statesGenerator.GenerateStates(generatingDepth);
                    else generatedStates = statesGenerator.GenerateStates();
                    Console.WriteLine("Generated States Count: " + generatedStates.Length);
                    break;
                case "loop":
                    uint totalCount = 0;
                    uint i = 0;
                    generatedStates = statesGenerator.GenerateStates(i);
                    while (generatedStates != null)
                    {
                        totalCount += (uint)generatedStates.Length;
                        Console.WriteLine("Generated States Count: " + generatedStates.Length);
                        generatedStates = statesGenerator.GenerateStates(++i);
                    }
                    Console.WriteLine("Total Generated States Count: " + totalCount);
                    break;
                case "set-depth":
                    Console.WriteLine("Enter new searching depth:");
                    input = Console.ReadLine();
                    if (uint.TryParse(input, out generatingDepth)) statesGenerator.MaxSearchingDepth = generatingDepth;
                    Console.WriteLine("New Max Searching Depth is: " + generatingDepth);
                    break;
                case "rows":
                    Console.WriteLine("Enter new rows count:");
                    input = Console.ReadLine();
                    if (byte.TryParse(input, out puzzleRows)) defaultPuzzle = new NPuzzle(puzzleRows, puzzleColumns);
                    Console.WriteLine("New rows count is: " + puzzleRows);
                    statesGenerator = new StatesGenerator<NPuzzle>(defaultPuzzle, 0, 0, 4194304, 4194304);
                    PrintPuzzleState(defaultPuzzle);
                    break;
                case "columns":
                    Console.WriteLine("Enter new columns count:");
                    input = Console.ReadLine();
                    if (byte.TryParse(input, out puzzleColumns)) defaultPuzzle = new NPuzzle(puzzleRows, puzzleColumns);
                    Console.WriteLine("New columns count is: " + puzzleColumns);
                    statesGenerator = new StatesGenerator<NPuzzle>(defaultPuzzle, 0, 0, 4194304, 4194304);
                    PrintPuzzleState(defaultPuzzle);
                    break;
                case "heur":
                    Console.WriteLine("Not yet implemented.");
                    break;
                case "help":
                default:
                    Console.WriteLine("Current commands available:\n" +
                        "help - show this help\n" +
                        "exit - exits program\n" +
                        "toggle-depth - toggle depth finding\n" +
                        "generate - This will start generating states\n" +
                        "loop - This will loop thorugh depth and generate states in each depth\n" +
                        "rows - This will set new rows count for puzzle\n" +
                        "columns - This will set new columns count for puzzle\n" +
                        "lookup-make - This will create lookup database\n" +
                        "heur - This will generate random puzzle state and print heuristic result\n" +
                        "set-depth - This will set depth for searching");
                    break;
            }
        }
    }


    void RunPuzzleSolvingDemo()
    {
        NPuzzle defaultPuzzle = new NPuzzle(3, 3);
        NPuzzle myPuzzle = defaultPuzzle.GenerateRandomState(30);
        NPuzzle newPuzzle = null;

        Console.WriteLine("Start!");
        PrintPuzzleStateAndSearch(myPuzzle, defaultPuzzle);

        ConsoleKeyInfo input = Console.ReadKey();
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
            PrintPuzzleStateAndSearch(myPuzzle, defaultPuzzle);
            input = Console.ReadKey();
        }
    }


    void PrintPuzzleState(NPuzzle state)
    {
        for (int rows = 0; rows < state.Rows; rows++)
        {
            for (int cols = 0; cols < state.Columns; cols++)
            {
                if (state.ByteState[rows, cols] == 0) Console.Write("  ");
                else Console.Write("{0,2:##}", state.ByteState[rows, cols]);
                if (cols < state.Columns - 1) Console.Write(" ");
            }
            Console.WriteLine();
        }
    }


    void PrintPuzzleStateAndSearch(NPuzzle startState, NPuzzle finishState)
    {
        Console.WriteLine("\n--------------");
        Console.WriteLine("\nDisplacement: " + NPuzzle.HeuristicPosition(startState, finishState) + " | Manhattan: " + NPuzzle.HeuristicManhattanDistance(startState, finishState) + " | M.Collisions: " + NPuzzle.HeuristicManhattanCollistionsDistance(startState, finishState));

        PrintPuzzleState(startState);

        //BFSSearch(startState, finishState);
        GreedySearch(startState, finishState);
        AStarSearch(startState, finishState);
        //DFSSearch(startState, finishState);
        IDAStarSearch(startState, finishState);
        //EvolutionSearch(startState, finishState);

    }


    void BFSSearch(NPuzzle startState, NPuzzle finishState)
    {
        //BFS search
        BFS<NPuzzle> bfs = new BFS<NPuzzle>(startState, finishState);

        bfs.StartState = startState;
        bfs.FinishState = finishState;

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
    }


    void GreedySearch(NPuzzle startState, NPuzzle finishState)
    {
        //Greedy search
        GreedySearch<NPuzzle> greedy = new GreedySearch<NPuzzle>(startState, finishState);
        greedy.MaxSearchingTime = 400;

        greedy.StartState = startState;
        greedy.FinishState = finishState;
        greedy.HeuristicParam = (int)NPuzzle.heuristics.lookup;
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
    }


    void AStarSearch(NPuzzle startState, NPuzzle finishState)
    {
        //AStar search
        AStar<NPuzzle> astar = new AStar<NPuzzle>(startState, finishState);
        //astar.HashSize = 2097152;
        //astar.MaxGeneratedElementsCount = 2097152;
        astar.MaxSearchingTime = 400;

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
    }


    void DFSSearch(NPuzzle startState, NPuzzle finishState)
    {
        DFS<NPuzzle> dfs = new DFS<NPuzzle>(startState, finishState);
        dfs.MaxSearchingTime = 40;

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
    }


    void IDAStarSearch(NPuzzle startState, NPuzzle finishState)
    {
        IDAStar<NPuzzle> idastar = new IDAStar<NPuzzle>(startState, finishState);
        //idastar.HashSize = 2097152;
        //idastar.MaxStackSize = 2097152;
        idastar.MaxSearchingTime = 400;

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
    }


    void EvolutionSearch(NPuzzle startState, NPuzzle finishState)
    {
        Evolution<NPuzzle, NPuzzle.Operation> evolution = new Evolution<NPuzzle, NPuzzle.Operation>(finishState, new NPuzzle.Operation());
        NPuzzle.FitnessStartState = startState;
        NPuzzle.FitnessFinishState = finishState; //For Evolution

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

