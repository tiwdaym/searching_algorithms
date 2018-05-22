using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms
{
    class NPuzzle : IGenerative<NPuzzle>
    {
        public byte[,] byteState;
        public byte spacePosition;
        public byte rows;
        public byte columns;

        public enum operations
        {
            left,
            right,
            up,
            down
        }

        #region Constructors
        /// <summary>
        /// Dafult static constructor to make default puzzle filled with 1-N ending with 0
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static NPuzzle CreateDefault(byte rows = 3, byte columns = 3)
        {
            byte[,] state = new byte[rows, columns];

            for (byte iRows = 0; iRows < rows; iRows++)
            {
                for (byte iColumns = 0; iColumns < columns; iColumns++)
                {
                    // +1 is offset so number are 1...n not 0...n-1
                    state[iRows, iColumns] = (byte)(iColumns * iRows + iColumns + 1);
                }
            }
            state[rows - 1, columns - 1] = 0; // space have 0
            return new NPuzzle(state, (byte)(rows * columns - 1));
        }

        /// <summary>
        /// Function will return full copy of actual state
        /// </summary>
        /// <returns></returns>
        public NPuzzle GetCopy()
        {
            NPuzzle newPuzzle = new NPuzzle(this.byteState, this.spacePosition);
            return newPuzzle;
        }

        /// <summary>
        /// Constructor creates default puzzle state. Default rows and columns are 3
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        public NPuzzle(byte rows = 3, byte columns = 3)
        {
            NPuzzle defaultState = CreateDefault(rows, columns);
            this.byteState = defaultState.byteState;
            this.spacePosition = defaultState.spacePosition;
            this.rows = defaultState.rows;
            this.columns = defaultState.columns;
        }

        /// <summary>
        /// Constructor will create new state based on input.
        /// You have to check if numbers and space are correct, otherwise, it won't function correctly.
        /// </summary>
        /// <param name="byteState"></param>
        /// <param name="spacePosition"></param>
        public NPuzzle(byte[,] byteState, byte spacePosition)
        {
            if (byteState == null) throw new Exception("Cannot use null state for puzzle.");
            this.byteState = (byte[,])byteState.Clone();
            this.spacePosition = spacePosition;
            this.rows = (byte)byteState.GetLength(0);
            this.columns = (byte)byteState.GetLength(1);
        }

        #endregion

        /// <summary>
        /// Function will generate new state from actual state with given operation to perform.
        /// If operation is valid, but no different state can be generated, returns null.
        /// If operation is invalid, thriws and ArgumentException.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public NPuzzle GenerateNewState(int operation)
        {
            NPuzzle newState = this.GetCopy();

            byte row_space = (byte)(spacePosition / columns);
            byte col_space = (byte)(spacePosition / newState.rows);

            switch (operation)
            {
                case (int)operations.up:
                    if (row_space > newState.rows || newState.rows == 1) return null;

                    newState.byteState[col_space, row_space] = newState.byteState[col_space, row_space + 1];
                    newState.byteState[col_space, row_space + 1] = 0;
                    spacePosition -= columns;
                    return newState;
                case (int)operations.down:
                    if (row_space <= 0 || newState.rows == 1) return null;

                    newState.byteState[col_space, row_space] = newState.byteState[col_space, row_space - 1];
                    newState.byteState[col_space, row_space - 1] = 0;
                    spacePosition += columns;
                    return newState;
                case (int)operations.right:
                    if (col_space <= 0 || newState.columns == 1) return null;

                    newState.byteState[col_space, row_space] = newState.byteState[col_space - 1, row_space];
                    newState.byteState[col_space - 1, row_space] = 0;
                    spacePosition--;
                    return newState;
                case (int)operations.left:
                    if (col_space > newState.columns || newState.columns == 1) return null;

                    newState.byteState[col_space, row_space] = newState.byteState[col_space + 1, row_space];
                    newState.byteState[col_space + 1, row_space] = 0;
                    spacePosition++;
                    return newState;
            }

            throw new ArgumentException("operation is not Implemented");
        }

        /// <summary>
        /// Function will return list of all possible operations over puzzle.
        /// </summary>
        /// <returns></returns>
        public int[] OperationsList()
        {
            return (int[])Enum.GetValues(typeof(operations));
        }
    }
}
