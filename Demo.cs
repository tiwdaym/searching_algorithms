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

    static void Main(string[] args)
    {
        Demo main = new Demo();
        main.Run();
    }

    public Demo()
    {
        defaultPuzzle = new NPuzzle(3,3);
        myPuzzle = defaultPuzzle.GenerateRandomState(5);
        bfs = new BFS<NPuzzle>(myPuzzle, defaultPuzzle);
        greedy = new GreedySearch<NPuzzle>(myPuzzle, defaultPuzzle);
        astar = new AStar<NPuzzle>(myPuzzle, defaultPuzzle);
        dfs = new DFS<NPuzzle>(myPuzzle, defaultPuzzle);
        idastar = new IDAStar<NPuzzle>(myPuzzle, defaultPuzzle);
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
    }

    void printNicePathStatistics(GeneratedPath<NPuzzle> path)
    {
        Console.WriteLine("Path Length: " + (path.pathLength - 1) + " | Searched Nodes: " + path.searchedNodes + " | Hash used: " + path.maximumUsedHashMemory + " | Heap used: " + path.maximumUsedHeapMemory + " | Generated Nodes: " + path.generatedNodes);
        for (int i = 0; i < path.pathLength && i<=8; i++) Console.Write(i + ": " + path.pathOperations[i] + " ");
        Console.WriteLine();
    }

}

