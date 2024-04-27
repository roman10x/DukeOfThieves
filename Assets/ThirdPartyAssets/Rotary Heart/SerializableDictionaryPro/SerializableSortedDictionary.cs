using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RotaryHeart.Lib.SerializableDictionaryPro
{
    /// <summary>
    /// Used to create a fully serializable sorted dictionary
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    [System.Serializable]
    public class SerializableSortedDictionary<TKey, TValue> : DrawableDictionary, IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        public SerializableSortedDictionary(bool canAdd = true, bool canRemove = true, bool canReorder = true, bool readOnly = false)
            : base(canAdd, canRemove, canReorder, readOnly) { }

        SortedDictionary<TKey, TValue> m_dict;

        SortedDictionary<TKey, TValue> Dict
        {
            get
            {
                if (m_dict == null)
                    m_dict = new SortedDictionary<TKey, TValue>();
                
                return m_dict;
            }
        }

        /// <summary>
        /// Copies the data from a dictionary. If an entry with the same key is found it replaces the value
        /// </summary>
        /// <param name="src">Dictionary to copy the data from</param>
        public void CopyFrom(IDictionary<TKey, TValue> src)
        {
            foreach (KeyValuePair<TKey, TValue> data in src)
            {
                if (ContainsKey(data.Key))
                {
                    this[data.Key] = data.Value;
                }
                else
                {
                    Add(data.Key, data.Value);
                }
            }
        }

        /// <summary>
        /// Copies the data from a dictionary. If an entry with the same key is found it replaces the value.
        /// Note that if the <paramref name="src"/> is not a dictionary of the same type it will not be copied
        /// </summary>
        /// <param name="src">Dictionary to copy the data from</param>
        public void CopyFrom(object src)
        {
            IDictionary<TKey, TValue> dictionary = src as IDictionary<TKey, TValue>;
            if (dictionary != null)
            {
                CopyFrom(dictionary);
            }
        }

        /// <summary>
        /// Copies the data to a dictionary. If an entry with the same key is found it replaces the value
        /// </summary>
        /// <param name="dest">Dictionary to copy the data to</param>
        public void CopyTo(IDictionary<TKey, TValue> dest)
        {
            foreach (KeyValuePair<TKey, TValue> data in this)
            {
                if (dest.ContainsKey(data.Key))
                {
                    dest[data.Key] = data.Value;
                }
                else
                {
                    dest.Add(data.Key, data.Value);
                }
            }
        }

        /// <summary>
        /// Copies the data to a dictionary. If an entry with the same key is found it replaces the value.
        /// Note that if <paramref name="dest"/> is not a dictionary of the same type it will not be copied
        /// </summary>
        /// <param name="dest">Dictionary to copy the data to</param>
        public void CopyTo(object dest)
        {
            IDictionary<TKey, TValue> dictionary = dest as IDictionary<TKey, TValue>;
            if (dictionary != null)
            {
                CopyTo(dictionary);
            }
        }

        /// <summary>
        /// Returns a copy of the sorted dictionary.
        /// </summary>
        public SortedDictionary<TKey, TValue> CloneSorted()
        {
            SortedDictionary<TKey, TValue> dest = new SortedDictionary<TKey, TValue>();

            foreach (KeyValuePair<TKey, TValue> data in this)
            {
                dest.Add(data.Key, data.Value);
            }

            return dest;
        }

        /// <summary>
        /// Returns a copy of the dictionary
        /// </summary>
        public Dictionary<TKey, TValue> CloeDictionary()
        {
            Dictionary<TKey, TValue> dest = new Dictionary<TKey, TValue>(Count);

            foreach (KeyValuePair<TKey, TValue> data in this)
            {
                dest.Add(data.Key, data.Value);
            }

            return dest;
        }
        
        /// <summary>
        /// Returns true if the value exists; otherwise, false
        /// </summary>
        /// <param name="value">Value to check</param>
        public bool ContainsValue(TValue value)
        {
            return Dict.ContainsValue(value);
        }

        #region IDictionary Interface

        #region Properties
        
        #if UNITY_EDITOR
        
        public TValue this[TKey key]
        {
            get
            {
                return Dict[key];
            }
            set
            {
                Dict[key] = value;
                
                m_keys = Dict.Keys.ToList();
                m_values = Dict.Values.ToList();
            }
        }
        
        #else

        public TValue this[TKey key]
        {
            get
            {
                return Dict[key];
            }
            set
            {
                Dict[key] = value;
            }
        }
        
        #endif

        public ICollection<TKey> Keys
        {
            get
            {
                return Dict.Keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                return Dict.Values;
            }
        }

        public int Count
        {
            get
            {
                return Dict.Count;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        #endregion Properties

        public bool ContainsKey(TKey key)
        {
            return Dict.ContainsKey(key);
        }

#if UNITY_EDITOR
        public void Add(TKey key, TValue value)
        {
            Dict.Add(key, value);

            m_keys = Dict.Keys.ToList();
            m_values = Dict.Values.ToList();
        }

        public void Clear()
        {
            Dict.Clear();

            if (m_keys != null)
                m_keys.Clear();
            if (m_values != null)
                m_values.Clear();
        }

        public bool Remove(TKey key)
        {
            if (m_keys != null)
            {
                int index = m_keys.IndexOf(key);

                if (index != -1)
                {
                    m_keys.RemoveAt(index);

                    if (m_values != null)
                        m_values.RemoveAt(index);
                }
            }

            return Dict.Remove(key);
        }
#else
        
        public void Add(TKey key, TValue value)
        {
            Dict.Add(key, value);
        }

        public void Clear()
        {
            Dict.Clear();
        }

        public bool Remove(TKey key)
        {
            return Dict.Remove(key);
        }
#endif

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Dict.TryGetValue(key, out value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            (Dict as ICollection<KeyValuePair<TKey, TValue>>).Add(item);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return (Dict as ICollection<KeyValuePair<TKey, TValue>>).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            (Dict as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return (Dict as ICollection<KeyValuePair<TKey, TValue>>).Remove(item);
        }

        public SortedDictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            return Dict.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region ISerializationCallbackReceiver

        [SerializeField]
        List<TKey> m_keys;
        [SerializeField]
        List<TValue> m_values;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (m_keys != null && m_values != null)
            {
                //Need to clear the dictionary
                Dict.Clear();

                for (int i = 0; i < m_keys.Count; i++)
                {
                    //Key cannot be null, skipping entry
                    if (m_keys[i] == null)
                    {
                        continue;
                    }

                    //Add the data to the dictionary. Value can be null so no special step is required
                    if (i < m_values.Count)
                        Dict[m_keys[i]] = m_values[i];
                    else
                        Dict[m_keys[i]] = default(TValue);
                }
            }
            
            //Outside of editor we clear the arrays to free up memory
#if !UNITY_EDITOR
            m_keys = null;
            m_values = null;
#endif
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (!Application.isPlaying)
                return;

            if (Dict.Count == 0)
            {
                //Dictionary is empty, erase data
                m_keys = null;
                m_values = null;
            }
            else
            {
                //Initialize arrays
                int cnt = Dict.Count;
                m_keys = new List<TKey>(cnt);
                m_values = new List<TValue>(cnt);

                using (SortedDictionary<TKey, TValue>.Enumerator e = Dict.GetEnumerator())
                {
                    while (e.MoveNext())
                    {
                        //Set the respective data from the dictionary
                        m_keys.Add(e.Current.Key);
                        m_values.Add(e.Current.Value);
                    }
                }
            }
        }

        #endregion

    }
}
