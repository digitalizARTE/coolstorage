// Created by Bruce Pearson, Kern County DHS, 7/2010
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
    /// <summary>
    /// Represents all entities affected by a single business transaction.
    /// </summary>
    /// <remarks>
    /// Quoting Fowler: <see cref="//martinfowler.com/eaaCatalog/unitOfWork.html"/>
    /// <i>Maintains a list of objects affected by a business transaction and coordinates the writing out of changes and the resolution of concurrency problems.</i>
    /// 
    /// The Unit of Work should be created by the aggregate root manager at the start of a transaction.
    /// If the transaction aggregate root manager requires enlistment of other aggregate root managers, it
    /// instanciates them passing its Unit of Work. Thus, all DB changes are collected in a single Unit of Work.
    /// </remarks>
    public class CSUnitOfWork
    {
        public CSUnitOfWork()
        {

        }
        private string _contextName;
        private IsolationLevel _IsolationLevel;
        public string ContextName
        {
            get
            {
                return _contextName;
            }
        }

        public CSUnitOfWork(string contextName)
        {
            _contextName = contextName;
        }


        /// <summary>
        /// Dictionary of all entities affected by a transaction.
        /// </summary>
        private Dictionary<CSPrimaryKey, CSObject> _EntityDictionary = new Dictionary<CSPrimaryKey, CSObject>();

        int tempKey = 0;
        /// <summary>Add an entity to those to be managed by the Unit of Work.
        /// </summary>
        /// <param name="rec">
        /// The entity object.
        /// </param>
        public void AddEntity(CSObject rec)
        {
            CSPrimaryKey pK = rec.PrimaryKey();
            if (pK.Identity && rec.IsNew)
            {
                pK.SetTempKey((--tempKey).ToString());
            }
            try
            {
                _EntityDictionary.Add(pK, rec);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("CSUnitOfWork- AddEntity", ex);
            }
        }

        /// <summary>Adds a list of CSObjects to the UnitOfWork;
        /// </summary>
        /// <param name="list">IEnumerable of CSObject</param>
        public void AddRange<TEntityType>(IEnumerable<TEntityType> list)
            where TEntityType : CSObject
        {
            foreach (var item in list)
            {
                AddEntity(item);
            }
        }

        /// <summary> Commit all DB changes.
        /// </summary>
        /// <param name="contextName">Context name.</param>
        /// <returns>
        /// <c>True</c> - Commited sucessfully.
        /// <c>False</c> - Commit failed.
        /// </returns>
        public bool Commit(string contextName)
        {
            return Commit();
        }

        /// <summary>
        /// Commits the cached db changes as an atomic transaction.
        /// </summary>
        /// <returns></returns>
        public bool Commit()
        {
            bool saveOk = true;
            _IsolationLevel = IsolationLevel.ReadCommitted;
            using (CSTransaction transaction = _contextName != null ?
                new CSTransaction(_IsolationLevel, _contextName) :
                new CSTransaction(_IsolationLevel))
            {
                foreach (var item in _EntityDictionary)
                {
                    if (!(saveOk = item.Value.Save()))
                    {
                        break;
                    };
                }
                if (saveOk)
                {
                    transaction.Commit();
                }
            }
            return saveOk;
        }


        public bool Contains(CSPrimaryKey pKey)
        {
            return _EntityDictionary.ContainsKey(pKey);
        }

        internal object Get(CSPrimaryKey pKey)
        {
            return _EntityDictionary.SingleOrDefault(rec => rec.Key == pKey).Value;
        }

        public bool TryGetCached<TEntityType>(object primaryKey, out TEntityType _cachedRec)
            where TEntityType : CSObject
        {
            foreach (var item in _EntityDictionary)
            {
                CSPrimaryKey itemKey = item.Key;
                bool sameType = itemKey.EntityType == typeof(TEntityType);
                var keyType = primaryKey.GetType();
                object itemKeyValuesConvert = itemKey.Values[0].Convert(keyType);
                //todo Implement test fo concatenated key
                bool sameKey = itemKeyValuesConvert.ToString() == primaryKey.ToString();
                if (sameType && sameKey)
                {
                    _cachedRec = (TEntityType)item.Value;
                    return true;
                }
            }
            _cachedRec = null;
            return false;
        }

        #region CoolStorage
        public IEnumerable<TEntity> List<TEntity>()
            where TEntity : CSObject<TEntity>
        {
            CSList<TEntity> cSObjectList = CSObject<TEntity>.List();
            AddRange<TEntity>(cSObjectList);
            return cSObjectList;
        }

        public IEnumerable<TEntity> List<TEntity>(string filter)
            where TEntity : CSObject<TEntity>
        {
            CSList<TEntity> cSObjectList = CSObject<TEntity>.List(filter);
            AddRange<TEntity>(cSObjectList);
            return cSObjectList;
        }

        public IEnumerable<TEntity> List<TEntity>(string filter, CSParameterCollection parameters)
            where TEntity : CSObject<TEntity>
        {
            CSList<TEntity> cSObjectList = CSObject<TEntity>.List(filter, parameters);
            AddRange<TEntity>(cSObjectList);
            return cSObjectList;
        }

        public IEnumerable<TEntity> List<TEntity>(string filter, params CSParameter[] parameters)
            where TEntity : CSObject<TEntity>
        {
            CSList<TEntity> cSObjectList = CSObject<TEntity>.List(filter, parameters);
            AddRange<TEntity>(cSObjectList);
            return cSObjectList;
        }

        public IEnumerable<TEntity> List<TEntity>(string filter, string paramName, object paramValue)
             where TEntity : CSObject<TEntity>
       {
           CSList<TEntity> cSObjectList = CSObject<TEntity>.List(filter, paramName, paramValue);
           AddRange<TEntity>(cSObjectList);
           return cSObjectList;
       }

        public IEnumerable<TEntity> List<TEntity>(string filter, string paramName1, object paramValue1, string paramName2, object paramValue2)
             where TEntity : CSObject<TEntity>
        {
           CSList<TEntity> cSObjectList = CSObject<TEntity>.List(filter, paramName1, paramValue1,paramName2, paramValue2);
           AddRange<TEntity>(cSObjectList);
           return cSObjectList;
        }

        #endregion
    }
}
