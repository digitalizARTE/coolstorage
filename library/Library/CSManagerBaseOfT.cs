// <copyright file="CSManagerBaseOfT.cs" company="County of Kern">
// Copyright (c) 2010 All Rights Reserved
// </copyright>
// <author>Bruce Pearson</author>
// <email>pearsobr@co.kern.ca.us</email>
// <date>8/17/2010</date>
// <summary>Extension to Vici.CoolStorage</summary>
// <changelog>
//		<logentry>
//		</logentry>
// </changelog>


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Vici.CoolStorage;

namespace Vici.CoolStorage
{
    public abstract class CSManagerBase<TEntityType> : CSManagerBase
        where TEntityType : CSObject<TEntityType>
    {
        MethodInfo getMethod;
        public CSManagerBase()
            : base()
        {
        }

        public CSManagerBase(string csContext)
            : base(csContext)
        {
        }

        public CSManagerBase(string csContext, CSUnitOfWork UofW)
            : base(csContext, UofW)
        {
        }

        public CSManagerBase(CSUnitOfWork UofW)
            : base(UofW)
        {
        }

        public TEntityType GetNew(bool addToUnitOfWork)
        {
            return GetNew<TEntityType>(addToUnitOfWork);
        }

        public TEntityType GetNew()
        {
            return GetNew<TEntityType>();
        }

        public abstract IEnumerable<TEntityType> Find();

        public TEntityType Get(object primaryKey)
        {
            TEntityType _cachedRec;
            Type TType = typeof(TEntityType);
            bool _cached = UnitOfWork.TryGetCached<TEntityType>(primaryKey, out _cachedRec);
            if (_cached)
            {
                return _cachedRec;
            }
            if (getMethod == null)
            {
                MethodInfo readMethod = null;
                while (readMethod == null)
                {
                    readMethod = TType.GetMethod("ReadSafe");
                    if (readMethod == null)
                    {
                        TType = TType.BaseType;
                    }
                }

                getMethod = readMethod;
            }

            TEntityType entity = (TEntityType)getMethod.Invoke(null, new object[] { primaryKey });
            if (entity != null)
            {
                UnitOfWork.AddEntity(entity);
            }
            return entity;
        }

        #region CoolStorage
        public IEnumerable<TEntityType> List()
        {
            CSList<TEntityType> cSObjectList = CSObject<TEntityType>.List();
            UnitOfWork.AddRange<TEntityType>(cSObjectList);
            return cSObjectList;
        }

        public IEnumerable<TEntityType> List(string filter)
        {
            CSList<TEntityType> cSObjectList = CSObject<TEntityType>.List(filter);
            UnitOfWork.AddRange<TEntityType>(cSObjectList);
            return cSObjectList;
        }

        public IEnumerable<TEntityType> List(string filter, CSParameterCollection parameters)
        {
            CSList<TEntityType> cSObjectList = CSObject<TEntityType>.List(filter, parameters);
            UnitOfWork.AddRange<TEntityType>(cSObjectList);
            return cSObjectList;
        }

        public IEnumerable<TEntityType> List(string filter, params CSParameter[] parameters)
        {
            CSList<TEntityType> cSObjectList = CSObject<TEntityType>.List(filter, parameters);
            UnitOfWork.AddRange<TEntityType>(cSObjectList);
            return cSObjectList;
        }

        public IEnumerable<TEntityType> List(string filter, string paramName, object paramValue)
        {
            CSList<TEntityType> cSObjectList = CSObject<TEntityType>.List(filter, paramName, paramValue);
            UnitOfWork.AddRange<TEntityType>(cSObjectList);
            return cSObjectList;
        }

        public IEnumerable<TEntityType> List(string filter, string paramName1, object paramValue1, string paramName2, object paramValue2)
        {
            CSList<TEntityType> cSObjectList = CSObject<TEntityType>.List(filter, paramName1, paramValue1, paramName2, paramValue2);
            UnitOfWork.AddRange<TEntityType>(cSObjectList);
            return cSObjectList;
        }
        #endregion
    }
}
