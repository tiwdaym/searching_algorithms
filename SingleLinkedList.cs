using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms
{
    class SingleLinkedList<TValue> : ISimpleCollection<TValue>
        where TValue : IEquatable<TValue>
    {
        private SingleLinkedNode<TValue> root;

        public int Count
        {
            get
            {
                if (Root == null) return 0;
                int count = 1;
                SingleLinkedNode<TValue> node = Root;
                while (node.Next != null)
                {
                    count++;
                    node = node.Next;
                }
                return count;
            }
        }
        
        public SingleLinkedNode<TValue> Root { get => root; }

        public SingleLinkedList(TValue root)
        {
            this.root = new SingleLinkedNode<TValue>(root);
        }


        public void Add(TValue item)
        {
            //Just append new value to an end of list
            if (root == null)
            {
                root = new SingleLinkedNode<TValue>(item);
                if (root == null) throw new OutOfMemoryException("Cannot create new Element");
                return;
            }
            SingleLinkedNode<TValue> node = root;
            while (node.Next != null) node = node.Next;
            node.Next = new SingleLinkedNode<TValue>(item);
            if (node.Next == null) throw new OutOfMemoryException("Cannot create new Element");
        }

        public bool Remove(TValue item)
        {
            if (root == null) return false;
            if (root.Value.Equals(item))
            {
                root.Value = default(TValue);
                root = root.Next;
                return true;
            }

            SingleLinkedNode<TValue> node = root.Next;
            SingleLinkedNode<TValue> prevNode = root;
            while (node != null)
            {
                if (node.Value.Equals(item))
                {
                    prevNode.Next = node.Next;
                    node.Value = default(TValue);
                    node.Next = null;
                    return true;
                }
                prevNode = node;
                node = node.Next;
            }
            return false;
        }

        public void Clear()
        {
            SingleLinkedNode<TValue> nodeToDelete = Root;
            SingleLinkedNode<TValue> nextToDelete = Root;

            while (nodeToDelete.Next != null)
            {
                nextToDelete = nodeToDelete.Next;
                nodeToDelete.Value = default(TValue);
                nodeToDelete = nextToDelete;
            }
            nodeToDelete.Value = default(TValue);

            root = null;
        }

        public bool Contains(TValue item)
        {
            if (Root == null) return false;
            if (Root.Value.Equals(item)) return true;
            SingleLinkedNode<TValue> node = Root;
            while (node.Next != null)
            {
                if (node.Value.Equals(item)) return true;
                node = node.Next;
            }
            return false;
        }

        public TValue[] ToList(uint arrayIndex = 0)
        {
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException("Index cannot be less than 0");
            int count = Count;
            if ((count - arrayIndex) <= 0) return null;
            TValue[] list = new TValue[count - arrayIndex];

            SingleLinkedNode<TValue> node = root;
            int i = 0;
            while(node!=null)
            {
                list[i++] = node.Value;
                node = node.Next;
            }
            return list;
        }
    }
}
