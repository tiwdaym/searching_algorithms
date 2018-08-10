using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms
{
    public class SingleLinkedList<T> : ISimpleCollection<T>
        where T : IEquatable<T>
    {
        private SingleLinkedNode<T> root;
        private SingleLinkedNode<T> last;
        private uint count;

        public uint Count { get => count; }

        public SingleLinkedList(T rootInitItem = default(T))
        {
            if (rootInitItem != null)
            {
                root = new SingleLinkedNode<T>(rootInitItem);
                last = root;
                count = 1;
            }
            else count = 0;
        }


        public void Add(T item)
        {
            //Create new node if root is null
            if (root == null)
            {
                root = new SingleLinkedNode<T>(item);
                if (root == null) throw new OutOfMemoryException("Cannot create new Element");
                count++;
                last = root;
                return;
            }

            last.Next = new SingleLinkedNode<T>(item);
            if (last.Next == null) throw new OutOfMemoryException("Cannot create new Element");
            last = last.Next;
            count++;
        }

        public bool Remove(T item)
        {
            if (root == null) return false;
            if (root.Value.Equals(item))
            {
                root.Value = default(T);
                if (last == root) last = root.Next;
                root = root.Next;
                count--;
                return true;
            }

            SingleLinkedNode<T> node = root.Next;
            SingleLinkedNode<T> prevNode = root;
            while (node != null)
            {
                if (node.Value.Equals(item))
                {
                    if (last == node) last = prevNode;
                    prevNode.Next = node.Next;
                    node.Value = default(T);
                    node.Next = null;
                    count--;
                    return true;
                }
                prevNode = node;
                node = node.Next;
            }
            return false;
        }

        public void Clear()
        {
            SingleLinkedNode<T> nodeToDelete = root;
            SingleLinkedNode<T> nextToDelete = root;

            while (nodeToDelete.Next != null)
            {
                nextToDelete = nodeToDelete.Next;
                nodeToDelete.Value = default(T);
                nodeToDelete = nextToDelete;
            }
            nodeToDelete.Value = default(T);

            count = 0;
            root = null;
        }

        public bool Contains(T item)
        {
            if (root == null) return false;
            if (root.Value.Equals(item)) return true;
            SingleLinkedNode<T> node = root;
            while (node.Next != null)
            {
                if (node.Value.Equals(item)) return true;
                node = node.Next;
            }
            if (node.Value.Equals(item)) return true;
            return false;
        }

        public bool TryGetValue(T item, out T foundValue)
        {
            foundValue = default(T);

            if (root == null) return false;
            if (root.Value.Equals(item))
            {
                foundValue = root.Value;
                return true;
            }
            SingleLinkedNode<T> node = root;
            while (node.Next != null)
            {
                if (node.Value.Equals(item))
                {
                    foundValue = node.Value;
                    return true;
                }
                node = node.Next;
            }
            return false;
        }

        /// <summary>
        /// Function will convert list to array
        /// </summary>
        /// <returns></returns>
        public T[] ToList()
        {
            if (count == 0) return null;
            T[] list = new T[count];

            SingleLinkedNode<T> node = root;
            int i = 0;
            while (node != null)
            {
                list[i++] = node.Value;
                node = node.Next;
            }
            return list;
        }
    }
}
