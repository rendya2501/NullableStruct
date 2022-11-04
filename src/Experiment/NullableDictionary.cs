using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NullableDictionary.Experiment
{
    public class NullableDict2<K, V> : IDictionary
    {
        readonly Dictionary<K, V> dict = new Dictionary<K, V>();
        V nullValue = default;
        bool hasNull = false;

        public object this[object key]
        {
            get
            {
                if ((K)key == null)
                    if (hasNull)
                        return nullValue;
                    else
                        throw new KeyNotFoundException();
                else
                    return dict[(K)key];
            }
            set
            {
                if (key == null)
                {
                    nullValue = (V)value;
                    hasNull = true;
                }
                else
                    dict[(K)key] = (V)value;
            }
        }

        public bool IsFixedSize => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public ICollection Keys => throw new NotImplementedException();

        public ICollection Values => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public void Add(object key, object value)
        {
            if (key == null)
            {
                if (hasNull) throw new ArgumentException("Duplicate key");

                nullValue = (V)value;
                hasNull = true;
            }
            else
            {
                dict.Add((K)key, (V)value);
            }
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object key)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public void Remove(object key)
        {
            throw new NotImplementedException();
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        //public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        //{
        //    if (!hasNull)
        //        return dict.GetEnumerator();
        //    else
        //        return GetEnumeratorWithNull();
        //}

        //private IEnumerator<KeyValuePair<K, V>> GetEnumeratorWithNull()
        //{
        //    yield return new KeyValuePair<K, V>(default, nullValue);
        //    foreach (var kv in dict)
        //        yield return kv;
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return GetEnumerator();
        //}
    }

    public class NullableDict<K, V> : IDictionary<K, V>
    {
        readonly Dictionary<K, V> dict = new Dictionary<K, V>();
        V nullValue = default;
        bool hasNull = false;

        public NullableDict()
        {
        }

        public void Add(K key, V value)
        {
            if (key == null)
            {
                if (hasNull) throw new ArgumentException("Duplicate key");

                nullValue = value;
                hasNull = true;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        public bool ContainsKey(K key)
        {
            if (key == null)
                return hasNull;
            return dict.ContainsKey(key);
        }

        public ICollection<K> Keys
        {
            get
            {
                if (!hasNull)
                    return dict.Keys;

                List<K> keys = dict.Keys.ToList();
                keys.Add(default);
                return new ReadOnlyCollection<K>(keys);
            }
        }

        public bool Remove(K key)
        {
            if (key != null)
                return dict.Remove(key);

            bool oldHasNull = hasNull;
            hasNull = false;
            return oldHasNull;
        }

        public bool TryGetValue(K key, out V value)
        {
            if (key != null)
                return dict.TryGetValue(key, out value);

            value = hasNull ? nullValue : default;
            return hasNull;
        }

        public ICollection<V> Values
        {
            get
            {
                if (!hasNull)
                    return dict.Values;

                List<V> values = dict.Values.ToList();
                values.Add(nullValue);
                return new ReadOnlyCollection<V>(values);
            }
        }

        public V this[K key]
        {
            get
            {
                if (key == null)
                    if (hasNull)
                        return nullValue;
                    else
                        throw new KeyNotFoundException();
                else
                    return dict[key];
            }
            set
            {
                if (key == null)
                {
                    nullValue = value;
                    hasNull = true;
                }
                else
                    dict[key] = value;
            }
        }

        public void Add(KeyValuePair<K, V> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            hasNull = false;
            dict.Clear();
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            if (item.Key != null)
                return ((ICollection<KeyValuePair<K, V>>)dict).Contains(item);
            if (hasNull)
                return EqualityComparer<V>.Default.Equals(nullValue, item.Value);
            return false;
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<K, V>>)dict).CopyTo(array, arrayIndex);
            if (hasNull)
                array[arrayIndex + dict.Count] = new KeyValuePair<K, V>(default, nullValue);
        }

        public int Count
        {
            get { return dict.Count + (hasNull ? 1 : 0); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            if (TryGetValue(item.Key, out V value) && EqualityComparer<V>.Default.Equals(item.Value, value))
                return Remove(item.Key);
            return false;
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            if (!hasNull)
                return dict.GetEnumerator();
            else
                return GetEnumeratorWithNull();
        }

        private IEnumerator<KeyValuePair<K, V>> GetEnumeratorWithNull()
        {
            yield return new KeyValuePair<K, V>(default, nullValue);
            foreach (var kv in dict)
                yield return kv;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    //public class NullableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    //{
    //    private TValue _nullValue;
    //    private bool _gotNullValue;
    //    private readonly Dictionary<TKey, TValue> _dict;

    //    public NullableDictionary()
    //    {
    //        _dict = new Dictionary<TKey, TValue>();
    //    }

    //    public NullableDictionary(IEqualityComparer<TKey> comparer)
    //    {
    //        _dict = new Dictionary<TKey, TValue>(comparer);
    //    }

    //    public bool ContainsKey(TKey key)
    //    {
    //        if (key == null)
    //        {
    //            return _gotNullValue;
    //        }
    //        else
    //        {
    //            return _dict.ContainsKey(key);
    //        }
    //    }

    //    public void Add(TKey key, TValue value)
    //    {
    //        if (key == null)
    //        {
    //            _nullValue = value;
    //            _gotNullValue = true;
    //        }
    //        else
    //        {
    //            _dict[key] = value;
    //        }
    //    }

    //    public bool Remove(TKey key)
    //    {
    //        if (key == null)
    //        {
    //            if (_gotNullValue)
    //            {
    //                _nullValue = default(TValue);
    //                _gotNullValue = false;
    //                return true;
    //            }
    //            else
    //            {
    //                return false;
    //            }
    //        }
    //        else
    //        {
    //            return _dict.Remove(key);
    //        }
    //    }

    //    public bool TryGetValue(TKey key, out TValue value)
    //    {
    //        if (key == null)
    //        {
    //            if (_gotNullValue)
    //            {
    //                value = _nullValue;
    //                return true;
    //            }
    //            else
    //            {
    //                value = default(TValue);
    //                return false;
    //            }
    //        }
    //        else
    //        {
    //            return _dict.TryGetValue(key, out value);
    //        }
    //    }

    //    public TValue this[TKey key]
    //    {
    //        get
    //        {
    //            if (key == null)
    //            {
    //                return _nullValue;
    //            }
    //            else
    //            {
    //                TValue ret;

    //                _dict.TryGetValue(key, out ret);

    //                return ret;
    //            }
    //        }
    //        set
    //        {
    //            if (key == null)
    //            {
    //                _nullValue = value;
    //                _gotNullValue = true;
    //            }
    //            else
    //            {
    //                _dict[key] = value;
    //            }
    //        }
    //    }

    //    public ICollection<TKey> Keys
    //    {
    //        get
    //        {
    //            if (_gotNullValue)
    //            {
    //                List<TKey> keys = new List<TKey>(_dict.Keys);
    //                keys.Add(null);
    //                return keys;
    //            }
    //            else
    //            {
    //                return _dict.Keys;
    //            }
    //        }
    //    }

    //    public ICollection<TValue> Values
    //    {
    //        get
    //        {
    //            if (_gotNullValue)
    //            {
    //                List<TValue> values = new List<TValue>(_dict.Values);
    //                values.Add(_nullValue);
    //                return values;
    //            }
    //            else
    //            {
    //                return _dict.Values;
    //            }
    //        }
    //    }

    //    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    //    {
    //        foreach (KeyValuePair<TKey, TValue> kvp in _dict)
    //        {
    //            yield return kvp;
    //        }

    //        if (_gotNullValue)
    //        {
    //            yield return new KeyValuePair<TKey, TValue>(null, _nullValue);
    //        }
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return GetEnumerator();
    //    }

    //    public void Add(KeyValuePair<TKey, TValue> item)
    //    {
    //        if (item.Key == null)
    //        {
    //            _nullValue = item.Value;
    //            _gotNullValue = true;
    //        }
    //        else
    //        {
    //            _dict.Add(item.Key, item.Value);
    //        }
    //    }

    //    public void Clear()
    //    {
    //        _dict.Clear();
    //        _nullValue = default;
    //        _gotNullValue = false;
    //    }

    //    public bool Contains(KeyValuePair<TKey, TValue> item)
    //    {
    //        TValue val;

    //        if (TryGetValue(item.Key, out val))
    //        {
    //            return Equals(item.Value, val);
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }

    //    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public bool Remove(KeyValuePair<TKey, TValue> item)
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public int Count
    //    {
    //        get
    //        {
    //            if (_gotNullValue)
    //            {
    //                return _dict.Count + 1;
    //            }
    //            else
    //            {
    //                return _dict.Count;
    //            }
    //        }
    //    }

    //    public bool IsReadOnly
    //    {
    //        get { return false; }
    //    }
    //}
}
