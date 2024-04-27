using System.Collections.Generic;
using UnityEngine;
// ReSharper disable RedundantAssignment

namespace RotaryHeart.Lib.SerializableDictionaryPro
{
    /// <summary>
    /// Used to create a fully serializable hash set
    /// </summary>
    /// <typeparam name="TValue">Value type</typeparam>
    [System.Serializable]
    public class SerializableHashSet<TValue> : DrawableDictionary, ISerializationCallbackReceiver
    {
        public SerializableHashSet(bool canAdd = true, bool canRemove = true, bool canReorder = true, bool readOnly = false)
            : base(canAdd, canRemove, canReorder, readOnly) { }

        HashSet<TValue> m_data;

        HashSet<TValue> Data
        {
            get
            {
                if (m_data == null)
                    m_data = new HashSet<TValue>();

                return m_data;
            }
        }

        /// <summary>
        /// Copies the data from <paramref name="array"/>. If an entry with the same key is found it won't be added
        /// </summary>
        /// <param name="array">Array to copy the data from</param>
        public void CopyFrom(IEnumerable<TValue> array)
        {
            foreach (TValue data in array)
            {
                Add(data);
            }
        }

        /// <summary>
        /// Copies the data from <paramref name="array"/>, starting at the specified array index. If an entry with the same key is found it won't be added
        /// </summary>
        /// <param name="array">Array to copy the data from</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyFrom(TValue[] array, int arrayIndex)
        {
            for (int i = arrayIndex; i < array.Length; i++)
            {
                Add(array[i]);
            }
        }

        /// <summary>
        /// Copies the specified number of elements from <paramref name="array"/>, starting at the specified array index.  If an entry with the same key is found it won't be added
        /// </summary>
        /// <param name="array">Array to copy the data from</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <param name="count">The number of elements to copy.</param>
        public void CopyFrom(TValue[] array, int arrayIndex, int count)
        {
            int tmpCount = 0;
            
            for (int i = arrayIndex; i < array.Length; i++)
            {
                if (tmpCount > count)
                    break;
                
                tmpCount++;
                Add(array[i]);
            }
        }

        /// <summary>
        /// Copies the data from <paramref name="list"/>, starting at the specified array index.  If an entry with the same key is found it won't be added
        /// </summary>
        /// <param name="list">List to copy the data from</param>
        /// <param name="listIndex">The zero-based index in the list at which copying begins.</param>
        public void CopyFrom(List<TValue> list, int listIndex)
        {
            for (int i = listIndex; i < list.Count; i++)
            {
                Add(list[i]);
            }
        }
        
        /// <summary>
        /// Copies the specified number of elements from <paramref name="list"/>, starting at the specified array index.  If an entry with the same key is found it won't be added
        /// </summary>
        /// <param name="list">List to copy the data from</param>
        /// <param name="listIndex">The zero-based index in the list at which copying begins.</param>
        /// <param name="count">The number of elements to copy.</param>
        public void CopyFrom(List<TValue> list, int listIndex, int count)
        {
            int tmpCount = 0;
            
            for (int i = listIndex; i < list.Count; i++)
            {
                if (tmpCount > count)
                    break;
                
                tmpCount++;
                Add(list[i]);
            }
        }
        
        /// <summary>
        /// Copies the data from an array. If an entry with the same key is found it won't be added.
        /// Note that if the <paramref name="src"/> is not an array of the same type it will not be copied
        /// </summary>
        /// <param name="src">Array to copy the data from</param>
        public void CopyFrom(object src)
        {
            TValue[] array = src as TValue[];
            if (array != null)
            {
                CopyFrom(array);
            }
            else
            {
                List<TValue> list = src as List<TValue>;
                if (list != null)
                {
                    CopyFrom(list);
                }
            }
        }
        
        /// <summary>
        /// Copies the elements to an array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the HashSet object.
        /// The array must have zero-based indexing.</param>
        public void CopyTo(TValue[] array)
        {
            Data.CopyTo(array);
        }

        /// <summary>
        /// Copies the elements to an array, starting at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the HashSet object.
        /// The array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(TValue[] array, int arrayIndex)
        {
            Data.CopyTo(array, arrayIndex, Count);
        }

        /// <summary>
        /// Copies the specified number of elements to an array, starting at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the HashSet object.
        /// The array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <param name="count">The number of elements to copy to the array.</param>
        public void CopyTo(TValue[] array, int arrayIndex, int count)
        {
            Data.CopyTo(array, arrayIndex, count);
        }

        /// <summary>
        /// Copies the elements to a list.
        /// </summary>
        /// <param name="list">The one-dimensional list that is the destination of the elements copied from the HashSet object.
        /// The list must have zero-based indexing.</param>
        public void CopyTo(List<TValue> list)
        {
            list = new List<TValue>(Count);

            foreach (TValue value in this)
            {
                list.Add(value);
            }
        }

        /// <summary>
        /// Copies the elements to a list, starting at the specified list index.
        /// </summary>
        /// <param name="list">The one-dimensional list that is the destination of the elements copied from the HashSet object.
        /// The list must have zero-based indexing.</param>
        /// <param name="listIndex">The zero-based index in the list at which copying begins.</param>
        public void CopyTo(List<TValue> list, int listIndex)
        {
            if (Count < listIndex)
                return;

            list = new List<TValue>(Count - listIndex);

            int index = 0;
            foreach (TValue value in this)
            {
                if (index < listIndex)
                {
                    index++;
                    continue;
                }

                list.Add(value);
            }
        }

        /// <summary>
        /// Copies the specified number of elements to a list, starting at the specified list index.
        /// </summary>
        /// <param name="list">The one-dimensional list that is the destination of the elements copied from the HashSet object.
        /// The list must have zero-based indexing.</param>
        /// <param name="listIndex">The zero-based index in the list at which copying begins.</param>
        /// <param name="count">The number of elements to copy to the list.</param>
        public void CopyTo(List<TValue> list, int listIndex, int count)
        {
            if (Count < listIndex)
                return;

            list = new List<TValue>(count);

            int tmpCount = 0;
            int index = 0;
            foreach (TValue value in this)
            {
                if (index < listIndex)
                {
                    index++;
                    continue;
                }

                if (tmpCount > count)
                    break;

                tmpCount++;
                list.Add(value);
            }
        }
        
        /// <summary>
        /// Copies the data to an array or list.
        /// Note that if <paramref name="dest"/> is not an array or list of the same type it will not be copied
        /// </summary>
        /// <param name="dest">Array or list to copy the data to</param>
        public void CopyTo(object dest)
        {
            TValue[] array = dest as TValue[];
            if (array != null)
            {
                CopyTo(array);
            }
            else
            {
                List<TValue> list = dest as List<TValue>;
                if (list != null)
                {
                    CopyTo(list);
                }
            }
        }

        /// <summary>
        /// Returns a copy of the hashset
        /// </summary>
        public HashSet<TValue> Clone()
        {
            return new HashSet<TValue>(Data);
        }
        
        #region HashSet logic

        public ICollection<TValue> Values
        {
            get
            {
                return Data;
            }
        }

        public int Count
        {
            get
            {
                return Data.Count;
            }
        }

        public bool Add(TValue value)
        {
            return Data.Add(value);
        }

#if UNITY_EDITOR
        public void Clear()
        {
            Data.Clear();

            if (m_keys != null)
                m_keys.Clear();
        }

        public bool Remove(TValue value)
        {
            if (m_keys != null)
            {
                m_keys.Remove(value);
            }

            return Data.Remove(value);
        }
#else
        public void Clear()
        {
            Data.Clear();
        }

        public bool Remove(TValue value)
        {
            return Data.Remove(value);
        }
#endif

        public bool Contains(TValue value)
        {
            return Data.Contains(value);
        }

        public HashSet<TValue>.Enumerator GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            Data.GetObjectData(info, context);
        }

        public void UnionWith(IEnumerable<TValue> other)
        {
            Data.UnionWith(other);
        }

        public void IntersectWith(IEnumerable<TValue> other)
        {
            Data.IntersectWith(other);
        }

        public void ExceptWith(IEnumerable<TValue> other)
        {
            Data.ExceptWith(other);
        }

        public void SymmetricExceptWith(IEnumerable<TValue> other)
        {
            Data.SymmetricExceptWith(other);
        }

        public bool IsSubsetOf(IEnumerable<TValue> other)
        {
            return Data.IsSubsetOf(other);
        }

        public bool IsProperSubsetOf(IEnumerable<TValue> other)
        {
            return Data.IsProperSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<TValue> other)
        {
            return Data.IsSupersetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<TValue> other)
        {
            return Data.IsProperSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<TValue> other)
        {
            return Data.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<TValue> other)
        {
            return Data.SetEquals(other);
        }

        public int RemoveWhere(System.Predicate<TValue> match)
        {
            return Data.RemoveWhere(match);
        }

        public void TrimExcess()
        {
            Data.TrimExcess();
        }

        #endregion HashSet logic

        #region ISerializationCallbackReceiver

        [SerializeField]
        List<TValue> m_keys;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (m_keys != null)
            {
                //Need to clear the dictionary
                Data.Clear();

                for (int i = 0; i < m_keys.Count; i++)
                {
                    //Key cannot be null, skipping entry
                    if (m_keys[i] == null)
                    {
                        continue;
                    }

                    //Add the data to the dictionary. Value can be null so no special step is required

                    Data.Add(m_keys[i]);
                }
            }
            
            //Outside of editor we clear the arrays to free up memory
#if !UNITY_EDITOR
            m_keys = null;
#endif
        }
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (!Application.isPlaying)
                return;

            if (Data.Count == 0)
            {
                //Dictionary is empty, erase data
                m_keys = null;
            }
            else
            {
                //Initialize arrays
                int cnt = Data.Count;
                m_keys = new List<TValue>(cnt);

                using (HashSet<TValue>.Enumerator e = Data.GetEnumerator())
                {
                    while (e.MoveNext())
                    {
                        //Set the respective data from the dictionary
                        m_keys.Add(e.Current);
                    }
                }
            }
        }

        #endregion ISerializationCallbackReceiver
    }
}