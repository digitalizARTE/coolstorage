using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vici.CoolStorage
{
    internal partial class CSFactory
    {
        /// <summary>
        /// Reads the specified primary key.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="primaryKey">The primary key.</param>
        /// <returns></returns>
        internal static TEntity Read<TEntity>(CSPrimaryKey primaryKey) where TEntity : CSObject<TEntity>
        {
            var keyArray = primaryKey.Values;
            return CSFactory.Read<TEntity>(keyArray);
        }

        /// <summary>
        /// Reads the specified primary key.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="primaryKey">The primary key.</param>
        /// <param name="UofW">The u of W.</param>
        /// <returns></returns>
        internal static TEntity Read<TEntity>(CSPrimaryKey primaryKey, CSUnitOfWork UofW) where TEntity : CSObject<TEntity>
        {
            if (UofW.Contains(primaryKey))
            {
                object UofWrec = UofW.Get(primaryKey);
                return (TEntity)UofWrec;
            }

            TEntity rec = CSFactory.Read<TEntity>(primaryKey);
            UofW.AddEntity(rec);
            return rec;
        }
    }
}
