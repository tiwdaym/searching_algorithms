using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms
{
    class NPuzzle : Generative<NPuzzle>, Heuristical<NPuzzle>
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
        public static NPuzzle CreateDefault(byte rows = 3, byte columns = 3)
        {
            byte[,] state = new byte[rows, columns];

            for (byte iRows = 0; iRows < rows; iRows++)
            {
                for (byte iColumns = 0; iColumns < columns; iColumns++)
                {
                    state[iRows, iColumns] = (byte)(iColumns * iRows + iColumns);
                }
            }
            return new NPuzzle(state);
        }

        public NPuzzle(byte rows = 3, byte columns = 3)
        {
            NPuzzle defaultState = CreateDefault(rows, columns);
            this.byteState = defaultState.byteState;
            this.spacePosition = defaultState.spacePosition;
            this.rows = defaultState.rows;
            this.columns = defaultState.columns;
        }

        public NPuzzle(byte[,] byteState)
        {
            if (byteState == null) throw new Exception("Cannot use null state for puzzle.");
            this.byteState = byteState;
            this.rows = (byte)byteState.GetLength(0);
            this.columns = (byte)byteState.GetLength(1);
        }

        #endregion

        public NPuzzle generate(int operation)
        {
            throw new NotImplementedException();
        }

        public uint getHash()
        {
            throw new NotImplementedException();
        }

        public int[] getOperations()
        {
            return (int[])Enum.GetNames(typeof(operations)).Select(t => t.GetHashCode());
        }

        public bool isEqual(NPuzzle state)
        {
            throw new NotImplementedException();
        }

        public int getHeuristicDistance(NPuzzle state)
        {
            throw new NotImplementedException();
        }
    }
}
