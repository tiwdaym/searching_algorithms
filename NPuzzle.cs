using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms
{
    class NPuzzle : IGenerative<NPuzzle>, IHeuristical<NPuzzle>, IEquatable<NPuzzle>, IHashable
    {
        private byte[,] byteState;
        private byte spacePosition;
        private byte rows;
        private byte columns;

        public byte[,] ByteState { get => byteState; }
        public byte SpacePosition { get => spacePosition; }
        public byte Rows { get => rows; }
        public byte Columns { get => columns; }

        static Random rnd = new Random();

        public enum operations
        {
            left,
            right,
            up,
            down
        }

        public enum heuristics
        {
            empty,
            position,
            distance,
            distanceWithCollisions
        }

        #region constructors

        /// <summary>
        /// Constructor creates default puzzle state. Default rows and columns are 3
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        public NPuzzle(byte rows = 3, byte columns = 3)
        {
            this.byteState = new byte[rows, columns];
            for (byte iRows = 0; iRows < rows; iRows++)
            {
                for (byte iColumns = 1; iColumns <= columns; iColumns++)
                {
                    // +1 is offset so number are 1...n not 0...n-1
                    this.byteState[iRows, iColumns - 1] = (byte)(iRows * columns + iColumns);
                }
            }
            this.byteState[rows - 1, columns - 1] = 0;
            this.spacePosition = (byte)(rows * columns - 1);
            this.rows = rows;
            this.columns = columns;
        }

        /// <summary>
        /// Constructor will create new state based on input.
        /// You have to check if numbers and space are correct, otherwise, it won't function correctly.
        /// </summary>
        /// <param name="byteState"></param>
        /// <param name="spacePosition"></param>
        public NPuzzle(byte[,] byteState, byte spacePosition)
        {
            if (byteState == null) throw new ArgumentNullException("Cannot use null state for puzzle.");
            this.byteState = (byte[,])byteState.Clone();
            this.spacePosition = spacePosition;
            this.rows = (byte)byteState.GetLength(0);
            this.columns = (byte)byteState.GetLength(1);
        }

        #endregion

        /// <summary>
        /// Function will return full copy of actual state
        /// </summary>
        /// <returns></returns>
        public NPuzzle GetCopy()
        {
            NPuzzle newPuzzle = new NPuzzle(this.ByteState, this.SpacePosition);
            return newPuzzle;
        }

        /// <summary>
        /// Function will generate new state from actual state with given operation to perform.
        /// If operation is valid, but no different state can be generated, returns null.
        /// If operation is invalid, thriws and ArgumentException.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public NPuzzle GenerateNewState(string operation)
        {
            NPuzzle newState = this.GetCopy();

            byte row_space = (byte)(newState.SpacePosition / newState.Columns);
            byte col_space = (byte)(newState.SpacePosition % newState.Columns);

            switch (operation)
            {
                case nameof(operations.up):
                    if (row_space >= newState.Rows - 1 || newState.Rows == 1) return null;

                    newState.ByteState[row_space, col_space] = newState.ByteState[row_space + 1, col_space];
                    newState.ByteState[row_space + 1, col_space] = 0;
                    newState.spacePosition += Columns;
                    return newState;
                case nameof(operations.down):
                    if (row_space <= 0 || newState.Rows == 1) return null;

                    newState.ByteState[row_space, col_space] = newState.ByteState[row_space - 1, col_space];
                    newState.ByteState[row_space - 1, col_space] = 0;
                    newState.spacePosition -= Columns;
                    return newState;
                case nameof(operations.right):
                    if (col_space <= 0 || newState.Columns == 1) return null;

                    newState.ByteState[row_space, col_space] = newState.ByteState[row_space, col_space - 1];
                    newState.ByteState[row_space, col_space - 1] = 0;
                    newState.spacePosition--;
                    return newState;
                case nameof(operations.left):
                    if (col_space >= newState.Columns - 1 || newState.Columns == 1) return null;

                    newState.ByteState[row_space, col_space] = newState.ByteState[row_space, col_space + 1];
                    newState.ByteState[row_space, col_space + 1] = 0;
                    newState.spacePosition++;
                    return newState;
            }

            throw new ArgumentException("operation is not Implemented");
        }

        /// <summary>
        /// Function will generate Random state with number of randomMoves
        /// </summary>
        /// <param name="randomMoves"></param>
        /// <returns></returns>
        public NPuzzle GenerateRandomState(uint randomMoves)
        {
            NPuzzle randomState = this, tempState = null;

            uint currentMoves = 0;
            while (currentMoves < randomMoves)
            {
                tempState = randomState.GenerateNewState(OperationsList()[rnd.Next(0, 4)]);
                if (tempState == null) continue;
                randomState = tempState;
                currentMoves++;
            }
            return randomState;
        }

        /// <summary>
        /// Function will return list of all possible operations over puzzle.
        /// </summary>
        /// <returns></returns>
        public string[] OperationsList()
        {
            return Enum.GetNames(typeof(operations));
        }

        /// <summary>
        /// Function will check, if puzzle is solvable
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool isSolvable(NPuzzle state)
        {
            if (state == null) throw new ArgumentNullException("Cannot use null state for puzzle.");
            return false;
        }

        public int HeuristicDistance(NPuzzle state, int param)
        {
            switch(param)
            {
                case (int)heuristics.empty: return 0;
                case (int)heuristics.position: return HeuristicPosition(this, state);
                case (int)heuristics.distance: return HeuristicManhattanDistance(this, state);
                case (int)heuristics.distanceWithCollisions: return HeuristicManhattanCollistionsDistance(this, state);
                default: return 0;
            }
        }
        
        /// <summary>
        /// Heuristic Computation for displacement position will count, how many pieces are not on correct position.
        /// </summary>
        /// <param name="startState"></param>
        /// <param name="finishState"></param>
        /// <returns></returns>
        public static int HeuristicPosition(NPuzzle startState, NPuzzle finishState)
        {
            if (startState == null || finishState == null) throw new ArgumentNullException("States must be initialized");
            if (startState.Columns != finishState.Columns || startState.Rows != finishState.Rows) throw new ArgumentException("States must have same rows and columns count.");

            int displacementCount = 0;

            for (int row = 0; row < startState.Rows; row++)
            {
                for (int col = 0; col < startState.Columns; col++)
                {
                    if (startState.SpacePosition == row * startState.Columns + col) continue;
                    if (startState.ByteState[row, col] != finishState.ByteState[row, col]) displacementCount++;
                }
            }

            return displacementCount;
        }


        /// <summary>
        /// Struct used in computing of manhattan distance
        /// </summary>
        public struct Coordinates
        {
            public int row;
            public int col;
        }

        /// <summary>
        /// Function will return manhattan distance from start to end
        /// </summary>
        /// <param name="startState"></param>
        /// <param name="finishState"></param>
        /// <returns></returns>
        public static int HeuristicManhattanDistance(NPuzzle startState, NPuzzle finishState)
        {
            if (startState == null || finishState == null) throw new ArgumentNullException("States must be initialized");
            if (startState.Columns != finishState.Columns || startState.Rows != finishState.Rows) throw new ArgumentException("States must have same rows and columns count.");

            int manhattanDistance = 0;
            int distanceArraySize = startState.Rows * startState.Columns;

            Coordinates[] coordinates = new Coordinates[distanceArraySize];

            for (int row = 0; row < startState.Rows; row++)
            {
                for (int col = 0; col < startState.Columns; col++)
                {
                    coordinates[startState.ByteState[row, col]].row = row;
                    coordinates[startState.ByteState[row, col]].col = col;
                }
            }

            for (int row = 0; row < finishState.Rows; row++)
            {
                for (int col = 0; col < finishState.Columns; col++)
                {
                    if (finishState.SpacePosition == row * finishState.Columns + col) continue;
                    manhattanDistance += Math.Abs(coordinates[finishState.ByteState[row, col]].row - row);
                    manhattanDistance += Math.Abs(coordinates[finishState.ByteState[row, col]].col - col);
                }
            }

            return manhattanDistance;
        }

        /// <summary>
        /// Function will return manhattan distance from start to end with Linear Collisions
        /// </summary>
        /// <param name="startState"></param>
        /// <param name="finishState"></param>
        /// <returns></returns>
        public static int HeuristicManhattanCollistionsDistance(NPuzzle startState, NPuzzle finishState)
        {
            if (startState == null || finishState == null) throw new ArgumentNullException("States must be initialized");
            if (startState.Columns != finishState.Columns || startState.Rows != finishState.Rows) throw new ArgumentException("States must have same rows and columns count.");

            int manhattanCollisionsDistance = 0;
            int distanceArraySize = startState.Rows * startState.Columns;

            Coordinates[] coordinatesStart = new Coordinates[distanceArraySize];
            Coordinates[] coordinatesFinish = new Coordinates[distanceArraySize];

            for (int row = 0; row < startState.Rows; row++)
            {
                for (int col = 0; col < startState.Columns; col++)
                {
                    coordinatesStart[startState.ByteState[row, col]].row = row;
                    coordinatesStart[startState.ByteState[row, col]].col = col;
                    coordinatesFinish[finishState.ByteState[row, col]].row = row;
                    coordinatesFinish[finishState.ByteState[row, col]].col = col;
                }
            }

            for (int i = 1; i < distanceArraySize; i++) //skip space so i = 1 not 0
            {
                manhattanCollisionsDistance += Math.Abs(coordinatesStart[i].row - coordinatesFinish[i].row);
                manhattanCollisionsDistance += Math.Abs(coordinatesStart[i].col - coordinatesFinish[i].col);


                //Now check if linear collision exists
                if (coordinatesStart[i].row == coordinatesFinish[i].row)
                {
                    for (int colCollision = 0; colCollision < startState.Columns; colCollision++)
                    {
                        if (startState.ByteState[coordinatesStart[i].row, colCollision] == i || startState.ByteState[coordinatesStart[i].row, colCollision] == 0 || coordinatesFinish[startState.ByteState[coordinatesStart[i].row, colCollision]].row != coordinatesStart[i].row) continue;
                        else
                        {
                            if (coordinatesStart[startState.ByteState[coordinatesStart[i].row, colCollision]].col == coordinatesStart[i].col - 1 && coordinatesFinish[startState.ByteState[coordinatesStart[i].row, colCollision]].col == coordinatesFinish[i].col + 1) manhattanCollisionsDistance++;
                            if (coordinatesStart[startState.ByteState[coordinatesStart[i].row, colCollision]].col == coordinatesStart[i].col + 1 && coordinatesFinish[startState.ByteState[coordinatesStart[i].row, colCollision]].col == coordinatesFinish[i].col - 1) manhattanCollisionsDistance++;
                        }
                    }
                }

                if (coordinatesStart[i].col == coordinatesFinish[i].col)
                {
                    for (int rowCollision = 0; rowCollision < startState.Rows; rowCollision++)
                    {
                        if (startState.ByteState[rowCollision, coordinatesStart[i].col] == i || startState.ByteState[rowCollision, coordinatesStart[i].col] == 0 || coordinatesFinish[startState.ByteState[rowCollision, coordinatesStart[i].col]].col != coordinatesStart[i].col) continue;
                        else
                        {
                            if (coordinatesStart[startState.ByteState[rowCollision, coordinatesStart[i].col]].row == coordinatesStart[i].row - 1 && coordinatesFinish[startState.ByteState[rowCollision, coordinatesStart[i].col]].row == coordinatesFinish[i].row + 1) manhattanCollisionsDistance++;
                            if (coordinatesStart[startState.ByteState[rowCollision, coordinatesStart[i].col]].row == coordinatesStart[i].row + 1 && coordinatesFinish[startState.ByteState[rowCollision, coordinatesStart[i].col]].row == coordinatesFinish[i].row - 1) manhattanCollisionsDistance++;
                        }
                    }
                }
            }
            return manhattanCollisionsDistance;
        }


        /// <summary>
        /// Equality checker
        /// </summary>
        /// <param name="npuzzle"></param>
        /// <returns></returns>
        public bool Equals(NPuzzle npuzzle)
        {
            if (spacePosition != npuzzle.spacePosition || rows != npuzzle.rows || columns != npuzzle.columns) return false;
            for (int x = 0; x < rows; x++)
                for (int y = 0; y < columns; y++)
                    if (byteState[x, y] != npuzzle.byteState[x, y]) return false;
            
            return true;
        }


        /// <summary>
        /// Hash maker
        /// </summary>
        /// <returns></returns>
        public uint GetHash()
        {
            uint hash = 0;
            uint seed = 101;
            for (int x = 0; x < rows; x++)
                for (int y = 0; y < columns; y++)
                    hash = hash * seed + byteState[x,y];
            return hash;
        }
    }
}
