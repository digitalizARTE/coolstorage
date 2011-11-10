using System;
using System.Collections.Generic;
using System.Reflection;


namespace Vici.CoolStorage
{
    /// <summary>
    /// Obtiene y guarda las asociaciones de los Fields y Properties de una clase cualquiera contra nombres de columna de tablas.
    /// Los nombres de columna se obtienen examinando los correspondientes atributos MapTo.
    /// </summary>
    /// <remarks>
    /// Los miembros sin atributo MapTo se asocian a una columna con el mismo nombre que el atributo
    /// </remarks>
    public class CSTypedQueryMapper
    {
        private static Dictionary<Type, Dictionary<string, MemberInfo>> _map = new Dictionary<Type, Dictionary<string, MemberInfo>>();

        /// <summary>
        /// Crea el cache del mapeo para una clase
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static Dictionary<string, MemberInfo> createMap(Type t)
        {
            Dictionary<string, MemberInfo> ret = new Dictionary<string, MemberInfo>();
            MapToAttribute mapattr;
            // Copia los mapings de los fields
            string key;
            Array.ForEach(t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy), fi =>
            {
                key = fi.Name;
                if (Attribute.GetCustomAttribute(fi, typeof(MapToAttribute)) != null)
                {
                    mapattr = (MapToAttribute)Attribute.GetCustomAttribute(fi, typeof(MapToAttribute));
                    key = mapattr.Name;
                }
                ret[key] = fi;
            });

            // Copia los mapings de las properties
            Array.ForEach(t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy), pi =>
            {
                key = pi.Name;
                if (Attribute.GetCustomAttribute(pi, typeof(MapToAttribute)) != null && pi.CanWrite)
                {
                    mapattr = (MapToAttribute)Attribute.GetCustomAttribute(pi, typeof(MapToAttribute));
                    key = mapattr.Name;
                }
                ret[key] = pi;
            });
            return ret;
        }

        /// <summary>
        /// Obtiene la tabla de mapeo Columna-Miembro para una clase dada
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Dictionary<string, MemberInfo> GetMap(Type type)
        {
            if (!_map.ContainsKey(type))
            {
                lock (_map)
                {
                    _map[type] = createMap(type);
                }
            }
            return _map[type];
        }
    }
}
