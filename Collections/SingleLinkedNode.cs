using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms
{
    class SingleLinkedNode<TValue>
    {
        private TValue value;
        private SingleLinkedNode<TValue> next;

        public SingleLinkedNode(TValue value)
        {
            this.Value = value;
            this.Next = null;
        }

        public TValue Value { get => value; set => this.value = value; }
        internal SingleLinkedNode<TValue> Next { get => next; set => next = value; }
    }
}
