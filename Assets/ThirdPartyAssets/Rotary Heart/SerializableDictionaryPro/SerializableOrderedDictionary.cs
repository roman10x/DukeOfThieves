using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace RotaryHeart.Lib.SerializableDictionaryPro
{
    /// <summary>
    /// Used to create a fully serializable ordered dictionary
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    [System.Serializable]
    public class SerializableOrderedDictionary<TKey, TValue> : DrawableDictionary, ISerializationCallbackReceiver
    {
        public SerializableOrderedDictionary(bool canAdd = true, bool canRemove = true, bool canReorder = true, bool readOnly = false)
            : base(canAdd, canRemove, canReorder, readOnly) { }

        Dictionary<TKey, TValue> m_dict;
        List<KeyValuePair<TKey, TValue>> m_list;

        Dictionary<TKey, TValue> Dict
        {
            get
            {
                if (m_dict == null)
                    m_dict = new Dictionary<TKey, TValue>();

                return m_dict;
            }
        }
        List<KeyValuePair<TKey, TValue>> List
        {
            get
            {
                if (m_list == null)
                    m_list = new List<KeyValuePair<TKey, TValue>>();

                return m_list;
            }
        }

        /// <summary>
        /// Copies the data from a dictionary. If an entry with the same key is found it replaces the value
        /// </summary>
        /// <param name="src">Dictionary to copy the data from</param>
        public void CopyFrom(Dictionary<TKey, TValue> src)
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
        /// Note that it will only copy the values that are
        /// of the same <typeparamref name="TKey"/> and <typeparamref name="TValue"/> type
        /// </summary>
        /// <param name="src">OrderedDictionary to copy the data from</param>
        public void CopyFrom(OrderedDictionary src)
        {
            IDictionaryEnumerator data = src.GetEnumerator();
            while (data.MoveNext())
            {
                if (data.Key.GetType() != typeof(TKey) || data.Value.GetType() != typeof(TValue))
                    continue;

                if (ContainsKey((TKey)data.Key))
                {
                    this[(TKey)data.Key] = (TValue)data.Value;
                }
                else
                {
                    Add((TKey)data.Key, (TValue)data.Value);
                }
            }
        }

        /// <summary>
        /// Copies the data from a dictionary or ordered dictionary. If an entry with the same key is found it replaces the value.
        /// Note that if <paramref name="src"/> is not a dictionary or ordered dictionary of the same type it will not be copied
        /// </summary>
        /// <param name="src">Dictionary or ordered dictionary to copy the data from</param>
        public void CopyFrom(object src)
        {
            Dictionary<TKey, TValue> dictionary = src as Dictionary<TKey, TValue>;
            if (dictionary != null)
            {
                CopyFrom(dictionary);
            }
            else
            {
                OrderedDictionary dict = src as OrderedDictionary;
                if (dict != null)
                {
                    CopyFrom(dict);
                }
            }
        }

        /// <summary>
        /// Copies the data to a dictionary. If an entry with the same key is found it replaces the value
        /// </summary>
        /// <param name="dest">Dictionary to copy the data to</param>
        public void CopyTo(Dictionary<TKey, TValue> dest)
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
        /// Copies the data to a ordered dictionary. If an entry with the same key is found it replaces the value.
        /// </summary>
        /// <param name="dest">OrderedDictionary to copy the data to</param>
        public void CopyTo(OrderedDictionary dest)
        {
            foreach (KeyValuePair<TKey, TValue> data in this)
            {
                if (dest.Contains(data.Key))
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
        /// Copies the data to a dictionary or ordered dictionary. If an entry with the same key is found it replaces the value.
        /// Note that if <paramref name="dest"/> is not a dictionary or ordered dictionary of the same type it will not be copied
        /// </summary>
        /// <param name="dest">Dictionary or ordered dictionary to copy the data to</param>
        public void CopyTo(object dest)
        {
            Dictionary<TKey, TValue> dictionary = dest as Dictionary<TKey, TValue>;
            if (dictionary != null)
            {
                CopyTo(dictionary);
            }
            else
            {
                OrderedDictionary dict = dest as OrderedDictionary;
                if (dict != null)
                {
                    CopyTo(dict);
                }
            }
        }

        /// <summary>
        /// Returns a copy of the ordered dictionary.
        /// </summary>
        public OrderedDictionary CloneOrdered()
        {
            OrderedDictionary dest = new OrderedDictionary(Count);

            foreach (KeyValuePair<TKey, TValue> data in this)
            {
                dest.Add(data.Key, data.Value);
            }

            return dest;
        }

        /// <summary>
        /// Returns a copy of the dictionary.
        /// </summary>
        public Dictionary<TKey, TValue> CloneDictionary()
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

        #if UNITY_EDITOR
        
        public TValue this[int index]
        {
            get
            {
                return Dict[List[index].Key];
            }
            set
            {
                List[index] = new KeyValuePair<TKey, TValue>(List[index].Key, value);
                Dict[List[index].Key] = value;
                
                if (m_keys == null)
                    m_keys = new List<TKey>();
                if (m_values == null)
                    m_values = new List<TValue>();
 
                int tempIndex = m_keys.IndexOf(List[index].Key);
                if (tempIndex < 0)
                {
                    m_keys.Add(List[index].Key);
                    m_values.Add(value);
                }
                else
                {
                    m_values[m_keys.IndexOf(List[index].Key)] = value;
                }
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                return Dict[key];
            }
            set
            {
                int index = List.IndexOf(new KeyValuePair<TKey, TValue>(key, Dict[key]));

                List[index] = new KeyValuePair<TKey, TValue>(key, value);
                Dict[key] = value;
                
                if (m_keys == null)
                    m_keys = new List<TKey>();
                if (m_values == null)
                    m_values = new List<TValue>();
 
                int tempIndex = m_keys.IndexOf(key);
                if (tempIndex < 0)
                {
                    m_keys.Add(key);
                    m_values.Add(value);
                }
                else
                {
                    m_values[m_keys.IndexOf(key)] = value;
                }
            }
        }
        
        #else
        
        public TValue this[int index]
        {
            get
            {
                return Dict[List[index].Key];
            }
            set
            {
                List[index] = new KeyValuePair<TKey, TValue>(List[index].Key, value);
                Dict[List[index].Key] = value;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                return Dict[key];
            }
            set
            {
                int index = List.IndexOf(new KeyValuePair<TKey, TValue>(key, Dict[key]));

                List[index] = new KeyValuePair<TKey, TValue>(key, value);
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

        public bool ContainsKey(TKey key)
        {
            return Dict.ContainsKey(key);
        }

#if UNITY_EDITOR
        public void Add(TKey key, TValue value)
        {
            Dict.Add(key, value); 
            List.Add(new KeyValuePair<TKey, TValue>(key, value));

            if (m_keys == null)
                m_keys = new List<TKey>();
            if (m_values == null)
                m_values = new List<TValue>();

            m_keys.Add(key);
            m_values.Add(value);
        }

        public void Clear()
        {
            Dict.Clear();
            List.Clear();

            if (m_keys != null)
                m_keys.Clear();
            if (m_values != null)
                m_values.Clear();
        }

        public bool Remove(TKey key)
        {
            TValue value = Dict[key];

            int index = List.IndexOf(new KeyValuePair<TKey, TValue>(key, value));

            if (index != -1)
            {
                if (m_keys != null)
                {
                    m_keys.RemoveAt(index);

                    if (m_values != null)
                        m_values.RemoveAt(index);
                }

                List.RemoveAt(index);
            }
            return Dict.Remove(key);
        }

        public bool Remove(int index)
        {
            if (index < 0 || index >= Count)
                return false;

            if (m_keys != null)
            {
                m_keys.RemoveAt(index);

                if (m_values != null)
                    m_values.RemoveAt(index);
            }

            TKey key = List[index].Key;
            List.RemoveAt(index);
            return Dict.Remove(key);
        }
#else
        public void Add(TKey key, TValue value)
        {
            Dict.Add(key, value);
            List.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public void Clear()
        {
            Dict.Clear();
            List.Clear();
        }

        public bool Remove(TKey key)
        {
            TValue value = Dict[key];

            int index = List.IndexOf(new KeyValuePair<TKey, TValue>(key, value));

            if (index != -1)
            {
                List.RemoveAt(index);
            }

            return Dict.Remove(key);
        }

        public bool Remove(int index)
        {
            if (index < 0 || index >= Count)
                return false;

            TKey key = List[index].Key;
            List.RemoveAt(index);
            return Dict.Remove(key);
        }
#endif

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Dict.TryGetValue(key, out value);
        }

        public List<KeyValuePair<TKey, TValue>>.Enumerator GetEnumerator()
        {
            return List.GetEnumerator();
        }

        #endregion IDictionary Interface

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
                List.Clear();

                for (int i = 0; i < m_keys.Count; i++)
                {
                    //Key cannot be null, skipping entry
                    if (m_keys[i] == null)
                    {
                        continue;
                    }

                    //Add the data to the dictionary. Value can be null so no special step is required
                    if (i < m_values.Count)
                    {
                        Dict[m_keys[i]] = m_values[i];
                        List.Add(new KeyValuePair<TKey, TValue>(m_keys[i], m_values[i]));
                    }
                    else
                    {
                        Dict[m_keys[i]] = default(TValue);
                        List.Add(new KeyValuePair<TKey, TValue>(m_keys[i], default(TValue)));
                    }
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

            if (List.Count == 0)
            {
                //Dictionary is empty, erase data
                m_keys = null;
                m_values = null;
            }
            else
            {
                //Initialize arrays
                int cnt = List.Count;
                m_keys = new List<TKey>(cnt);
                m_values = new List<TValue>(cnt);

                using (List<KeyValuePair<TKey, TValue>>.Enumerator e = List.GetEnumerator())
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
        #endregion ISerializationCallbackReceiver
    }
}
