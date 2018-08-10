using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms.Collections
{
    public class GraphNodeSimple<T> : IComparable<GraphNodeSimple<T>>, IHashable, IEquatable<GraphNodeSimple<T>>
        where T:IEquatable<T>, IHashable
    {
        public readonly T node;
        public GraphNodeSimple<T> graphParent;
        public string lastOperation;
        public int graphDepth;

        public GraphNodeSimple(T node, GraphNodeSimple<T> graphParent, string lastOperation, int graphDepth)
        {
            if (node == null) throw new ArgumentNullException("Cannot create Graph node with empty node");
            this.node = node;
            this.graphParent = graphParent;
            this.graphDepth = graphDepth;
            this.lastOperation = lastOperation;
        }

        public int CompareTo(GraphNodeSimple<T> node)
        {
            return graphDepth - node.graphDepth;
        }

        public bool Equals(GraphNodeSimple<T> other)
        {
            return node.Equals(other.node);
        }

        public uint GetHash()
        {
            return node.GetHash();
        }
    }
}
