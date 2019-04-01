using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms.Collections
{
    public class Json : IHashable, IEquatable<Json>
    {

        HashList<Json> hashList = null;
        SingleLinkedList<Json> linkedList = null;
        string key = null;
        public string value = null;

        static int i;

        public Json this[string key]
        {
            get
            {
                Json result;
                if (hashList.TryGetValue(new Json()
                {
                    key = key
                }, out result))
                {
                    return result;
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    if (hashList == null) return;
                    hashList.Remove(value);
                    if (hashList.Count == 0) hashList = null;
                }
                else
                {
                    if (hashList == null) hashList = new HashList<Json>();
                    hashList.Add(value);
                }
            }
        }

        public Json this[int key]
        {
            get
            {
                if (linkedList == null || linkedList.Count <= key) return null;
                return linkedList[key];
            }
            set
            {
                if (value == null)
                {
                    if (linkedList == null) return;
                    linkedList.Remove(value);
                    if (linkedList.Count == 0) linkedList = null;
                }
                else
                {
                    if (linkedList == null) linkedList = new SingleLinkedList<Json>();
                    if (linkedList.Count <= key) throw new IndexOutOfRangeException();
                    linkedList[key] = value;
                }
            }
        }

        public void Push(Json value)
        {
            if (linkedList == null) linkedList = new SingleLinkedList<Json>();
            linkedList.Add(value);
        }


        public static Json DecodeJsonFromString(string json)
        {
            Json jsonObject = new Json();
            int jsonLength = json.Length;
            i = 0;
            while (i < jsonLength)
            {
                EatWhiteSpaces(ref json);
                switch(json[i])
                {
                    case '{':
                        break;
                    case '"':
                        break;
                    case '[':
                        break;
                    default:
                        break;
                }
            }

            return jsonObject;
        }

        static Json AddArray(ref string json)
        {
            Json result = new Json();

            return result;
        }

        static Json AddObject(ref string json)
        {
            Json result = new Json();

            return result;
        }

        static Json AddString(ref string json)
        {
            Json result = new Json();

            return result;
        }

        static void EatWhiteSpaces(ref string json)
        {

        }

        public uint GetHash()
        {
            int length = key.Length;
            if (key.Length <= 0) return default(uint);
            uint hash = key[0];
            int i = 0;
            while (i < length) hash *= 101 + (uint)key[i++];
            return hash;
        }

        public bool Equals(Json other)
        {
            return value.Equals(other.value);
        }
    }
}
