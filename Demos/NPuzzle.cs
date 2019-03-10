using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using SearchingAlgorithms.Collections;

namespace SearchingAlgorithms
{
    public class NPuzzle : IGenerative<NPuzzle>, IHeuristical<NPuzzle>, IEquatable<NPuzzle>, IHashable, IEvaluable<NPuzzle.Operation>
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
        static NPuzzle fitnessStartState = null;
        public static NPuzzle FitnessStartState { get => fitnessStartState; set => fitnessStartState = value; }
        static NPuzzle fitnessFinishState = null;
        public static NPuzzle FitnessFinishState { get => fitnessFinishState; set => fitnessFinishState = value; }

        public enum operations
        {
            left,
            right,
            up,
            down
        }

        /// <summary>
        /// STruct defined for use in Evolution algorithm
        /// </summary>
        public struct Operation : IRandomizable<Operation>, IEquatable<Operation>, IHashable
        {
            public operations bit;
            static Random rnd = new Random();

            public Operation(operations bit)
            {
                this.bit = bit;
            }

            public bool Equals(Operation other)
            {
                return bit == other.bit;
            }

            public uint GetHash()
            {
                return (uint)bit;
            }

            public Operation NextRandom()
            {
                return new Operation((operations)rnd.Next(Enum.GetValues(typeof(operations)).Length));
            }
        }


        public enum heuristics
        {
            empty,
            position,
            distance,
            distanceWithCollisions,
            lookup
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
            switch (param)
            {
                case (int)heuristics.empty: return 0;
                case (int)heuristics.position: return HeuristicPosition(this, state);
                case (int)heuristics.distance: return HeuristicManhattanDistance(this, state);
                case (int)heuristics.distanceWithCollisions: return HeuristicManhattanCollistionsDistance(this, state);
                case (int)heuristics.lookup: return HeuristicLookUpTable(this, state);
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
                    hash = hash * seed + byteState[x, y];
            return hash;
        }

        /// <summary>
        /// Function will return fitness for evolution search
        /// </summary>
        /// <param name="IO_DATA"></param>
        /// <returns></returns>
        public uint Fitness(Operation[] chromosome)
        {
            if (fitnessFinishState == null || fitnessStartState == null) throw new ArgumentNullException("To use fitness function, you must first initialize static Fitness States");

            NPuzzle tmpState;
            NPuzzle evaluatedState = fitnessStartState.GetCopy();

            uint i = 0;
            while (i < chromosome.Length)
            {
                tmpState = evaluatedState.GenerateNewState(Enum.GetName(typeof(operations), chromosome[i].bit));
                if (tmpState != null) evaluatedState = tmpState;
                if (evaluatedState.Equals(fitnessFinishState)) break;
                i++;
            }

            return (uint)(2 * chromosome.Length - i - HeuristicManhattanCollistionsDistance(evaluatedState, fitnessFinishState));
        }


        /// <summary>
        /// Function will create SQLLite database
        /// </summary>
        /// <param name="dbName"></param>
        public static void InitDatabaseForWholeHeuristic(int rows, int columns)
        {
            string dbname = "Lookup" + rows + "-" + columns + ".sqlite";
            SQLiteConnection.CreateFile(dbname);
            SQLiteConnection dbConn = new SQLiteConnection("Data Source=" + dbname + ";Version=3");
            dbConn.Open();

            SQLiteCommand command = new SQLiteCommand(dbConn);
            command.CommandText = "CREATE TABLE lookup (depth INT, state BLOB)";
            command.ExecuteNonQuery();
            command.CommandText = "CREATE INDEX indexer ON lookup(state)";
            command.ExecuteNonQuery();
            dbConn.Close();
        }

        /// <summary>
        /// Function will encode NPuzzle to byte[] array
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static byte[] EncodePuzzleState(NPuzzle state)
        {
            byte[] result = new byte[state.Rows * state.Columns];
            int i = 0;
            for (int x = 0; x < state.Rows; x++)
                for (int y = 0; y < state.Columns; y++) result[i++] = state.ByteState[x, y];
            return result;
        }

        /// <summary>
        /// Function will decode byte array to NPuzzle
        /// </summary>
        /// <param name="state"></param>
        /// <param name="rows">Number of rows of puzzle</param>
        /// <param name="columns">Number of columns of puzzle</param>
        /// <returns></returns>
        public static NPuzzle DecodePuzzleState(byte[] state, byte rows, byte columns)
        {
            byte[,] byteState = new byte[rows, columns];
            byte i = 0;
            byte spacePosition = 0;
            for (int x = 0; x < rows; x++)
                for (int y = 0; y < columns; y++)
                {
                    if (byteState[x, y] == 0) spacePosition = i;
                    byteState[x, y] = state[i++];
                }
            return new NPuzzle(byteState, spacePosition);
        }


        public static void InsertRecordIntoLookupDb(GraphNodeSimple<NPuzzle> nodeData)
        {
            string dbname = "Lookup" + nodeData.node.Rows + "-" + nodeData.node.Columns + ".sqlite";
            SQLiteConnection dbConn = new SQLiteConnection("Data Source=" + dbname + ";Version=3");
            dbConn.Open();

            byte[] encodedPuzzleState = EncodePuzzleState(nodeData.node);

            SQLiteCommand command = new SQLiteCommand(dbConn);
            command.CommandText = "INSERT INTO lookup (depth, state) VALUES (" + nodeData.graphDepth + ", @state)";
            command.Parameters.Add("@state", System.Data.DbType.Binary, encodedPuzzleState.Length).Value = encodedPuzzleState;
            command.ExecuteNonQuery();

            dbConn.Close();
        }

        public static void InsertMultiRecordsIntoLookupDb(GraphNodeSimple<NPuzzle>[] nodeData)
        {
            string dbname = "Lookup" + nodeData[0].node.Rows + "-" + nodeData[0].node.Columns + ".sqlite";
            SQLiteConnection dbConn = new SQLiteConnection("Data Source=" + dbname + ";Version=3");
            dbConn.Open();

            byte[] encodedPuzzleState = null;
            SQLiteCommand command;

            for (int i = 0; i < nodeData.Length; i++)
            {
                if (i % 1000 == 0) Console.WriteLine("Currently at record: " + i);
                encodedPuzzleState = EncodePuzzleState(nodeData[i].node);

                command = new SQLiteCommand(dbConn);
                command.CommandText = "INSERT INTO lookup (depth, state) VALUES (" + nodeData[i].graphDepth + ", @state)";
                command.Parameters.Add("@state", System.Data.DbType.Binary, encodedPuzzleState.Length).Value = encodedPuzzleState;
                command.ExecuteNonQuery();
            }

            dbConn.Close();
        }

        public static int HeuristicLookUpTable(NPuzzle startState, NPuzzle finishState)
        {
            string dbname = "Lookup" + startState.Rows + "-" + startState.Columns + ".sqlite";
            SQLiteConnection dbConn = new SQLiteConnection("Data Source=" + dbname + ";Version=3");
            dbConn.Open();

            byte[] encodedStartPuzzleState = EncodePuzzleState(startState);
            byte[] encodedFinishPuzzleState = EncodePuzzleState(finishState);
            int startDepth;
            int finishDepth;

            SQLiteCommand command = new SQLiteCommand(dbConn);
            command.CommandText = "SELECT depth FROM lookup WHERE state=@state";
            command.Parameters.Add("@state", System.Data.DbType.Binary, encodedStartPuzzleState.Length).Value = encodedStartPuzzleState;
            SQLiteDataReader reader = command.ExecuteReader();
            reader.Read();
            startDepth = (int)reader["depth"];

            command = new SQLiteCommand(dbConn);
            command.CommandText = "SELECT depth FROM lookup WHERE state=@state";
            command.Parameters.Add("@state", System.Data.DbType.Binary, encodedFinishPuzzleState.Length).Value = encodedFinishPuzzleState;
            reader = command.ExecuteReader();
            reader.Read();
            finishDepth = (int)reader["depth"];

            dbConn.Close();

            return Math.Abs(startDepth - finishDepth);
        }

    }
}
