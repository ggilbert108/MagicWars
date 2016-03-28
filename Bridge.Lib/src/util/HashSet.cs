using System.Collections;
using System.Collections.Generic;
using Bridge.Html5;

namespace Bridge.Lib
{
    public class HashSet<T> : IEnumerable<T> where T : IHashable
    {
        private Dictionary<int, T> dictionary; 

        public HashSet()
        {
            dictionary = new Dictionary<int, T>();
        }

        public void Add(T value)
        {
            dictionary[value.GetHash()] = value;
        }

        public void Remove(T value)
        {
            dictionary.Remove(value.GetHash());
        }

        public bool Contains(T value)
        {
            return dictionary.ContainsKey(value.GetHash());
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return dictionary.Values.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return dictionary.Values.GetEnumerator();
        }
    }

    public class HashSet<T, F> : IEnumerable<T> 
    {
        private Dictionary<T, int> dictionary;

        public HashSet()
        {
            dictionary = new Dictionary<T, int>();
        }

        public void Add(T value)
        {
            dictionary[value] = 0;
        }

        public void Remove(T value)
        {
            dictionary.Remove(value);
        }

        public bool Contains(T value)
        {
            return dictionary.ContainsKey(value);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return dictionary.Keys.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return dictionary.Keys.GetEnumerator();
        }
    }
}