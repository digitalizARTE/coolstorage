// Added to Vici.CoolStorage by Bruce Pearson - 7/2010
#region License
//=============================================================================
// Vici CoolStorage - .NET Object Relational Mapping Library 
//
// Copyright (c) 2004-2009 Philippe Leybaert
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//=============================================================================
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Vici.Core;
using System.Reflection;

namespace Vici.CoolStorage
{
    public class CSPrimaryKey : IEquatable<CSPrimaryKey>
    {
        public static CSPrimaryKey New<T>()
        {
            return new CSPrimaryKey(typeof(T));
        }

        public static CSPrimaryKey PrimaryKey(CSObject entity)
        {
            CSPrimaryKey pk = new CSPrimaryKey(entity.GetType().BaseType);
            CSSchemaColumnCollection keyColumns = entity.Schema.KeyColumns;
            if (keyColumns.Count == 1)
            {
                CSSchemaField mappedField = keyColumns[0].MappedField;
                pk.Identity = mappedField.MappedColumn.Identity;
                CSFieldValue keyValue = entity.FieldData[mappedField.Name];
                pk.Add(mappedField.Name, keyValue);
            }
            else
            {
                foreach (var item in keyColumns)
                {
                    CSSchemaField mappedField = item.MappedField;
                    CSFieldValue keyValue = entity.FieldData[mappedField.Name];
                    pk.Add(mappedField.Name, keyValue);
                }
            }
            return pk;
        }

        public CSPrimaryKey(Type entityType)
        {
            _keyDictionary = new Dictionary<string, object>();
            _EntityType = entityType;
        }

        public CSPrimaryKey(Type entityType, Dictionary<string, object> keyDictionary)
        {
            _keyDictionary = keyDictionary;
            _EntityType = entityType;
        }

        Type _EntityType;
        public Type EntityType
        {
            get { return _EntityType; }
        }

        Dictionary<string, object> _keyDictionary;
        bool _Identity = false;

        public bool Identity
        {
            get { return _Identity; }
            set { _Identity = value; }
        }

        public string ConcatenatedKey
        {
            get
            {
                StringBuilder key = new StringBuilder();
                foreach (var item in _keyDictionary.Select(rec => rec.Value))
                {
                    key.Append(item.ToString());
                }
                return key.ToString();
            }
        }
        public object[] Values
        {
            get
            {
                return _keyDictionary.Values.ToArray();
            }
        }
        public void SetTempKey(object tempKeyValue)
        {
            KeyValuePair<string, object> oldEntry = _keyDictionary.ElementAt(0);
            _keyDictionary.Remove(oldEntry.Key);
            _keyDictionary.Add(oldEntry.Key, tempKeyValue);
        }

        #region IEquatable<CSPrimaryKey> Members

        public bool Equals(CSPrimaryKey other)
        {
            return (this.EntityType == other.EntityType && this.ConcatenatedKey == other.ConcatenatedKey);
        }
        public override bool Equals(Object obj)
        {
            if (obj == null) return base.Equals(obj);

            if (!(obj is CSPrimaryKey))
                throw new InvalidCastException("The 'obj' argument is not a CSPrimaryKey object.");
            else
                return Equals(obj as CSPrimaryKey);
        }

        public override int GetHashCode()
        {
            string stringToHash = String.Format("{0}{1}", ConcatenatedKey, _EntityType.FullName);
            return stringToHash.GetHashCode();
        }

        public static bool operator ==(CSPrimaryKey key1, CSPrimaryKey key2)
        {
            return key1.Equals(key2);
        }

        public static bool operator !=(CSPrimaryKey key1, CSPrimaryKey key2)
        {
            return (!key1.Equals(key2));
        }
        #endregion

        public CSPrimaryKey Add(string key, object value)
        {
            _keyDictionary.Add(key, value);
            return this;
        }

    }
    //public class CSPrimaryKey<TEntityType> : CSPrimaryKey
    //    where TEntityType : CSObject<TEntityType>
    //{
    //}
}
