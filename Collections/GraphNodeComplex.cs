using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms.Collections
{
    public class GraphNodeComplex<T> : IComparable<GraphNodeComplex<T>>, IHashable, IEquatable<GraphNodeComplex<T>>
        where T:IEquatable<T>, IHashable
    {
        public readonly T node;
        public GraphNodeComplex<T> graphParent;
        public string lastOperation;
        public int comparingParam;
        public int realGraphDepth;


        public GraphNodeComplex(T node, GraphNodeComplex<T> graphParent, string lastOperation, int realGraphDepth, int comparingParam )
        {
            if (node == null) throw new ArgumentNullException("Cannot create Graph node with empty node");
            this.node = node;
            this.graphParent = graphParent;
            this.lastOperation = lastOperation;
            this.realGraphDepth = realGraphDepth;
            this.comparingParam = comparingParam;
        }

        public int CompareTo(GraphNodeComplex<T> node)
        {
            return comparingParam - node.comparingParam;
        }

        public bool Equals(GraphNodeComplex<T> other)
        {
            return node.Equals(other.node);
        }

        public uint GetHash()
        {
            return node.GetHash();
        }
    }
}
