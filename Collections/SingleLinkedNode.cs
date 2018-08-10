using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms
{
    public class SingleLinkedNode<T>
    {
        private T value;
        private SingleLinkedNode<T> next;

        public SingleLinkedNode(T value)
        {
            this.Value = value;
            this.Next = null;
        }

        public T Value { get => value; set => this.value = value; }
        internal SingleLinkedNode<T> Next { get => next; set => next = value; }
    }
}
