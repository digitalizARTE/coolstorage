using System;

namespace Vici.CoolStorage
{
    /// <summary>
    /// Estrategia para resolver diferencias en el mapeo de entidades y base de datos
    /// </summary>
    public class CSMappingStrategyToLower : CSMappingStrategyBase
    {
        /// <summary>
        /// Convierte el nombre de una columna en el nombre de una propiedad
        /// </summary>
        /// <param name="name">Nombre de la columna</param>
        /// <returns></returns>
        public override string ColumnToInternal(string name)
        {
            return name.ToLower();
        }

        /// <summary>
        /// Convierte el nombre de una propiedad en el nombre de una columna  
        /// </summary>
        /// <param name="name">Nombre de la propiedad</param>
        /// <returns></returns>
        public override string FieldToInternal(string name)
        {
            return name.ToLower();
        }

        public override string NameTo(string name)
        {
            return name.ToLower();
        }

        #region Stored Procedures

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string ParamToInternal(string name)
        {
            return name.ToLower();
        }

        public override string ParamToField(string name)
        {
            return name.ToLower();
        }

        public override string FieldToParam(string name)
        {
            return name.ToLower();
        }

        public override string NameToSPCreateName(string name)
        {
            return name.ToLower();
        }

        public override string NameToSPReadName(string name)
        {
            return name.ToLower();
        }

        public override string NameToSPUpdateName(string name)
        {
            return name.ToLower();
        }

        public override string NameToSPDeleteName(string name)
        {
            return name.ToLower();
        }
        #endregion
    }
}
