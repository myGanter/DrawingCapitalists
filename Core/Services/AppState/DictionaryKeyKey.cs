using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Services.AppState
{
    public class DictionaryKeyKey<K1, K2> : IEnumerable<KeyValuePair<K1, K2>>
    {
        protected readonly Dictionary<K1, K2> K1K2;

        protected readonly Dictionary<K2, K1> K2K1;

        public DictionaryKeyKey()
        {
            K1K2 = new Dictionary<K1, K2>();
            K2K1 = new Dictionary<K2, K1>();
        }

        public K2 this[K1 key1]
        {
            get
            {
                K1K2.TryGetValue(key1, out K2 res);
                return res;
            }
        }

        public K1 this[K2 key2]
        {
            get
            {
                K2K1.TryGetValue(key2, out K1 res);
                return res;
            }
        }

        public void Add(K1 key1, K2 key2)
        {
            K1K2.Add(key1, key2);
            K2K1.Add(key2, key1);
        }

        public bool ContainsKey(K1 key1)
        {
            return K1K2.ContainsKey(key1);
        }

        public bool ContainsKey(K2 key2)
        {
            return K2K1.ContainsKey(key2);
        }

        public void Remove(K1 key1)
        {
            var val = K1K2[key1];
            K1K2.Remove(key1);
            K2K1.Remove(val);
        }

        public void Remove(K2 key2)
        {
            var val = K2K1[key2];
            K2K1.Remove(key2);
            K1K2.Remove(val);
        }

        public IEnumerator<KeyValuePair<K1, K2>> GetEnumerator()
        {
            return K1K2.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
