using System;

namespace Vici.CoolStorage
{
    /// <summary>
    /// Estrategia para resolver diferencias en el mapeo de entidades y base de datos
    /// </summary>
    public class CSMappingStrategyBase : ICSMappingStrategy
    {
        /// <summary>
        /// Convierte el nombre de una columna en el nombre de una propiedad
        /// </summary>
        /// <param name="name">Nombre de la columna</param>
        /// <returns></returns>
        public virtual string ColumnToInternal(string name)
        {
            return name;
        }

        /// <summary>
        /// Convierte el nombre de una propiedad en el nombre de una columna  
        /// </summary>
        /// <param name="name">Nombre de la propiedad</param>
        /// <returns></returns>
        public virtual string FieldToInternal(string name)
        {
            return name;
        }

        public virtual string NameTo(string name)
        {
            return name;
        }

        #region Stored Procedures
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual string ParamToInternal(string name)
        {
            return name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual string ParamToField(string name)
        {
            return name;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual string FieldToParam(string name)
        {
            return name;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual string NameToSPCreateName(string name)
        {
            return name;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual string NameToSPReadName(string name)
        {
            return name;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual string NameToSPUpdateName(string name)
        {
            return name;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual string NameToSPDeleteName(string name)
        {
            return name;
        }
        #endregion
    }
}