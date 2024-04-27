using System.Collections.Generic;
using UnityEngine;

namespace RotaryHeart.Lib.SerializableDictionaryPro
{
    /// <summary>
    /// Base class that most be used for any bi directional dictionary that wants to be implemented
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    [System.Serializable]
    public class SerializableBiDictionary<TKey, TValue> : DrawableDictionary, IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        public SerializableBiDictionary(bool canAdd = true, bool canRemove = true, bool canReorder = true, bool readOnly = false)
            : base(canAdd, canRemove, canReorder, readOnly) { }
        
        Dictionary<TKey, TValue> m_dict;
        Dictionary<TValue, TKey> m_biDict;

        Dictionary<TKey, TValue> Dict
        {
            get
            {
                if (m_dict == null)
                    m_dict = new Dictionary<TKey, TValue>();

                return m_dict;
            }
        }
        Dictionary<TValue, TKey> BiDict
        {
            get
            {
                if (m_biDict == null)
                    m_biDict = new Dictionary<TValue, TKey>();

                return m_biDict;
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
                    Dict[data.Key] = data.Value;
                    BiDict[data.Value] = data.Key;
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
            foreach (KeyValuePair<TKey, TValue> data in Dict)
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
        /// Copies the data to a dictionary. If an entry with the same key is found it replaces the value
        /// </summary>
        /// <param name="dest">Dictionary to copy the data to</param>
        public void CopyTo(IDictionary<TValue, TKey> dest)
        {
            foreach (KeyValuePair<TValue,TKey> data in BiDict)
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
            else
            {
                IDictionary<TValue, TKey> biDictionary = dest as IDictionary<TValue, TKey>;
                if (biDictionary != null)
                {
                    CopyTo(biDictionary);
                }
            }
        }
        
        /// <summary>
        /// Returns a copy of the dictionary.
        /// </summary>
        public Dictionary<TKey, TValue> Clone()
        {
            Dictionary<TKey, TValue> dest = new Dictionary<TKey, TValue>(Count);

            foreach (KeyValuePair<TKey, TValue> data in Dict)
            {
                dest.Add(data.Key, data.Value);
            }

            return dest;
        }
        
        /// <summary>
        /// Returns a copy of the dictionary.
        /// </summary>
        public Dictionary<TValue, TKey> CloneInverted()
        {
            Dictionary<TValue, TKey> dest = new Dictionary<TValue, TKey>(Count);

            foreach (KeyValuePair<TValue, TKey> data in BiDict)
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
            return BiDict.ContainsKey(value);
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
 
                if (m_keys == null)
                    m_keys = new List<TKey>();
                if (m_values == null)
                    m_values = new List<TValue>();
 
                int index = m_keys.IndexOf(key);
                if (index < 0)
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
        
        public TKey this[TValue key]
        {
            get
            {
                return BiDict[key];
            }
            set
            {
                BiDict[key] = value;
 
                if (m_keys == null)
                    m_keys = new List<TKey>();
                if (m_values == null)
                    m_values = new List<TValue>();
 
                int index = m_values.IndexOf(key);
                if (index < 0)
                {
                    m_values.Add(key);
                    m_keys.Add(value);
                }
                else
                {
                    m_keys[m_values.IndexOf(key)] = value;
                }
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
        
        public TKey this[TValue key]
        {
            get
            {
                return BiDict[key];
            }
            set
            {
                BiDict[key] = value;
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
                return BiDict.Keys;
            }
        }
        public int Count
        {
            get
            {
                return Dict.Count;
            }
        }
        public bool IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }
        
        #endregion Properties
        
#if UNITY_EDITOR
        public void Add(TKey key, TValue value)
        {
            Dict.Add(key, value);
            BiDict.Add(value, key);
            
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
            BiDict.Clear();

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

            TValue value;
            
            if (Dict.TryGetValue(key, out value))
            {
                BiDict.Remove(value);
                Dict.Remove(key);
                return true;
            }

            return false;
        }
#else
        public void Add(TKey key, TValue value)
        {
            Dict.Add(key, value);
            BiDict.Add(value, key);
        }

        public void Clear()
        {
            Dict.Clear();
            BiDict.Clear();
        }

        public bool Remove(TKey key)
        {
            TValue value;
            
            if (Dict.TryGetValue(key, out value))
            {
                BiDict.Remove(value);
                Dict.Remove(key);
                return true;
            }
            
            return false;
        }
#endif

        public bool ContainsKey(TKey key)
        {
            return Dict.ContainsKey(key);
        }
        
        public bool ContainsKey(TValue key)
        {
            return BiDict.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Dict.TryGetValue(key, out value);
        }

        public bool TryGetValue(TValue key, out TKey value)
        {
            return BiDict.TryGetValue(key, out value);
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

        public Dictionary<TKey, TValue>.Enumerator GetDictEnumerator()
        {
            return Dict.GetEnumerator();
        }
        
        public Dictionary<TValue, TKey>.Enumerator GetInvertedEnumerator()
        {
            return BiDict.GetEnumerator();
        }
        
        public System.Collections.IEnumerator GetEnumerator()
        {
            return GetDictEnumerator();
        }
        
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return GetDictEnumerator();
        }
        
        #endregion IDictionary Interfacce
        
        #region ISerializationCallbackReceiver

        [SerializeField]
        List<TKey> m_keys;
        [SerializeField]
        List<TValue> m_values;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (m_keys == null || m_values == null)
                return;

            //Need to clear the dictionary
            Dict.Clear();
            BiDict.Clear();

            for (int i = 0; i < m_keys.Count; i++)
            {
                //Key and value cannot be null
                if (m_keys[i] == null)
                    continue;
                
                //Add the data to the dictionaries
                if (i < m_values.Count)
                    Dict[m_keys[i]] = m_values[i];
                else
                    Dict[m_keys[i]] = default(TValue);
            }
            
            //Outside of editor we cleat the arrays to free up memory
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

                using (Dictionary<TKey, TValue>.Enumerator e = Dict.GetEnumerator())
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