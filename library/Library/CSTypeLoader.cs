using System;
using System.Reflection;

namespace Vici.CoolStorage
{
    /// <summary>
    /// 
    /// </summary>
    public class CSTypeLoader
    {
        /// <summary>
        /// Levanta un type a partir de un string con forma xxx.yyy.zzz@assembly
        /// </summary>
        /// <param name="pTypeString"></param>
        /// <returns></returns>
        public static Type LoadType(string pTypeString)
        {
            string[] vAux = pTypeString.Split('@');
            if (vAux.Length == 1)
                return Type.GetType(vAux[0], true, true);
            else
                return Assembly.Load(vAux[1]).GetType(vAux[0],true,true);
        }
    }
}
