// <copyright file="CSManagerBase.cs" company="County of Kern">
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
using System.Data;
using System.Data.Odbc;
using Vici.CoolStorage;
using Vici.Core;
using System.Reflection;

namespace Vici.CoolStorage
{
    /// <summary>
    /// Abstract Manager Base for CSObject aggregate roots.
    /// </summary>
    /// <remarks>
    /// Managers maintain a CSUnitOfWork: either they create a new CSUnitOfWork or take an existing CSUnitOfWork as a contructor parameter.
    /// </remarks>
    public abstract class CSManagerBase
    {
        private static MethodInfo _CreateNewHierarchyMethod;
        private static MethodInfo CreateNewHierarchyMethod
        {
            get
            {
                if (_CreateNewHierarchyMethod == null)
                {
                    _CreateNewHierarchyMethod = typeof(CSManagerBase).GetMethod("CreateNewHierarchy", BindingFlags.NonPublic | BindingFlags.Instance);
                }
                return _CreateNewHierarchyMethod;
            }
        }


        /// <summary>
        /// MethodInfo cache; used initially to cache the CSObject "New()" methods of entity classes.
        /// </summary>
        static Dictionary<Type, MethodInfo> _newMethodCache = new Dictionary<Type, MethodInfo>();
        //static Dictionary<Type, MethodInfo> _typeInitializeMethodCache = new Dictionary<Type, MethodInfo>();
        //static Dictionary<Type, MethodInfo> _initializePrimaryKeyMethodCache = new Dictionary<Type, MethodInfo>();

        /// <summary>
        /// 
        /// </summary>
        protected string _csContext;

        /// <summary>
        /// Represents <see cref="CSUnitOfWork"/> CSUnitOfWork object for this manager.
        /// </summary>
        protected CSUnitOfWork UnitOfWork {get; private set;}

        /// <summary>
        /// Create a new manager and a default CSUnitOfWork and default Context
        /// </summary>
        /// <overloads>
        /// Overloads exist for creating managers with:
        ///     Default CSUnitOfWork
        ///     Passed CSUnitOfWork
        ///     Default Context Name
        ///     Passed ContextName
        /// </overloads>
        public CSManagerBase()
        {
            UnitOfWork = new CSUnitOfWork();
        }

        /// <summary>
        /// Create a new manager.
        /// </summary>
        /// <param name="csContext">Context Name</param>
        public CSManagerBase(string csContext)
        {
            UnitOfWork = new CSUnitOfWork();
            _csContext = csContext;
        }

        /// <summary>
        /// Create a new manager using a passed Context Name and Unit of Work
        /// 
        /// </summary>
        /// <param name="csContext"></param>
        /// <param name="UofW"></param>
        public CSManagerBase(string csContext, CSUnitOfWork UofW)
        {
            UnitOfWork = UofW;
            _csContext = csContext;
        }

        public CSManagerBase(CSUnitOfWork UofW)
        {
            UnitOfWork = UofW;
        }

        /// <summary>
        /// Represents a new object of type TEntityType.
        /// </summary>
        /// <remarks>
        /// The object is fully initialized including the primary key if a PrimaryKeyGenerator is specified.
        /// </remarks>
        /// <typeparam name="TEntityType">
        /// The type of the object to create
        /// </typeparam>
        /// <returns>A new object of type TEntityType.</returns>
        public TEntityType GetNew<TEntityType>()
            where TEntityType : CSObject<TEntityType>
        {
            TEntityType entity = CreateNewHierarchy<TEntityType>();
            UnitOfWork.AddEntity(entity);
            return entity;
        }

        /// <summary>
        /// Creates new instance of a CSObject<TEntityType>
        /// </summary>
        /// <typeparam name="TEntityType">The type of the entity type.</typeparam>
        /// <param name="addToUnitOfWork">if set to <c>true</c> [add to unit of work].</param>
        /// <returns></returns>
        public TEntityType GetNew<TEntityType>(bool addToUnitOfWork)
            where TEntityType : CSObject<TEntityType>
        {
            TEntityType entity = CreateNewHierarchy<TEntityType>();
            if (addToUnitOfWork)
            {
                UnitOfWork.AddEntity(entity);
            }
            return entity;
        }



        /// <summary>
        /// Create each of the table entries required to define type.
        /// </summary>
        /// <remarks>
        ///  The CoolStorage DAL implements Table per Class inheritance. Consequenty, creating a type
        ///  which is a subclass requires the creation of all superclass table entries.
        ///  
        ///  The superclass property accessor is recusively populated with an instance of the superclass.
        ///  
        ///  The MethodInfo for the "New()" method on the TEntityType is cached.
        ///  
        ///  Implemented by BP 7/2010
        /// </remarks>
        /// <typeparam name="TEntityType">The type of the object to return.</typeparam>
        /// <returns>An object of TEntityType.</returns>
        protected TEntityType CreateNewHierarchy<TEntityType>()
            where TEntityType : CSObject<TEntityType>
        {
            TEntityType entity = New<TEntityType>();

            Type entityType = typeof(TEntityType);

            //ApplyPrimaryKeyGenerator(entity);

            // check if this is a sub-class table;
            SubClassAttribute subClassAttr = entityType.GetAttribute<SubClassAttribute>(true);

            // get superclass property
            if (subClassAttr != null)
            {
                // Get the property on the new object which return an instance of the superclass.
                PropertyInfo superClassPropertyInfo = entityType.GetProperty(
                    subClassAttr.SuperClassType.Name,
                    subClassAttr.SuperClassType);

                // Construct the CreateNewHierarchy<TEntityType> method where TEntityType is the type of the supper type.
                MethodInfo superClassGetNewMethod = CreateNewHierarchyMethod.MakeGenericMethod(new Type[] { subClassAttr.SuperClassType });

                // Create the supertype object and set the super type accessor property;
                CSObject superClassObject = (CSObject)superClassGetNewMethod.Invoke(this, null);
                superClassPropertyInfo.SetValue(entity, superClassObject, null);

                // Find the primary key column in the superclass object and set it to the primary key value of the new entity.
                foreach (var item in superClassObject.FieldData)
                {
                    if (item.SchemaField.MappedColumn.IsKey)
                    {
                        item.Value = entity.PrimaryKeyValue;
                        break;
                    }
                }
            }

            return entity;
        }

        /// <summary>
        /// Represents a new instance of TEntityType.
        /// </summary>
        /// <remarks>
        /// The new object is create via reflection using cached MethodInfo for the "New" method.
        /// </remarks>
        /// <typeparam name="TEntityType">The type of entity to create.</typeparam>
        /// <returns>A fully initialized object of type TEntityType</returns>
        private TEntityType New<TEntityType>()
            where TEntityType : CSObject<TEntityType>
        {
            Type csEntityType = typeof(CSObject<TEntityType>);
            MethodInfo newEntityMethodInfo;
            bool cached = _newMethodCache.TryGetValue(csEntityType, out newEntityMethodInfo);
            if (!cached)
            {
                newEntityMethodInfo = csEntityType.GetMethod("New");
                _newMethodCache.Add(csEntityType, newEntityMethodInfo);
            }
            TEntityType entity = (TEntityType)newEntityMethodInfo.Invoke(null, null);
            entity.InitializeNew();
            return (TEntityType)ApplyPrimaryKeyGenerator(entity);
        }

        /// <summary>
        /// Applies the primary key generator.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public CSObject ApplyPrimaryKeyGenerator(CSObject entity)
        {
            // check if type has a primary key generator
            Type entityType = entity.GetType().BaseType;
            PropertyInfo[] props = entityType.GetProperties();
            PropertyInfo keyProp = null;
            PrimaryKeyGeneratorAttribute genAttr = null;
            foreach (var item in props)
            {
                genAttr = item.GetAttribute<PrimaryKeyGeneratorAttribute>(true);
                if (genAttr != null)
                {
                    keyProp = item;
                    break;
                }
            }

            // Prop with generator found
            if (keyProp != null)
            {
                keyProp.SetValue(entity, PrimaryKeyGenerator.NextPrimaryKey(), null);
            }
            return entity;
        }


        protected abstract IPrimaryKeyGenerator PrimaryKeyGenerator { get; }

        public virtual void Save()
        {
            UnitOfWork.Commit(_csContext);
        }
    }
}
