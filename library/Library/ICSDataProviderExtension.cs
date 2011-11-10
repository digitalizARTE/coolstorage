using System;
using System.Data;
using System.Collections.Generic;

namespace Vici.CoolStorage
{
    /// <summary>
    /// 
    /// </summary>
    public static class ICSDataProviderExtension
    {
        private readonly static Dictionary<string, CSParameterCollection> _map = new Dictionary<string, CSParameterCollection>();

        /// <summary>
        /// Recupera los nombres y tipos de los parametros de un stored procedure
        /// </summary>
        /// <param name="StoredProcedureName">Nombre del stored procedure</param>
        /// <param name="WithReturn">Si debe incluir en los parametros el tipo de retorno o no</param>
        /// <returns>Un array de SqlParameter con los parametros que espera (y devuelve, si aplica) el stored procedure</returns>
        private static CSParameterCollection InternalGetSpParams(CSDataProvider dp, string StoredProcedureName, bool WithReturn)
        {
            CSParameterCollection _SPParameters;
            using (ICSDbCommand dbCommand = dp.CreateCommandInternal(String.Format("!{0}", StoredProcedureName), null))
            {
                //Asigna el tipo a stored procedure, porque solo se pueden recuperar los parametros de sp's
                try
                {
                    //SqlCommandBuilder.DeriveParameters(cmd);
                    dp.DeriveParameters(dbCommand);
                }
                catch (InvalidOperationException)
                {
                    //throw new GYF_InvalidStoredProcedureException(StoredProcedureName);
                    throw new Exception(StoredProcedureName);
                }
                //Si no debe obtener el param de retorno, lo quita
                if (!WithReturn)
                {
                    for (int i = 0; i < dbCommand.Parameters.Count; i++)
                    {
                        if (((IDbDataParameter)dbCommand.Parameters[i]).Direction == ParameterDirection.ReturnValue)
                        {
                            dbCommand.Parameters.RemoveAt(i);
                            break;
                        }
                    }
                }
                _SPParameters = new CSParameterCollection();
                for (int i = 0; i < dbCommand.Parameters.Count; i++)
                {
                    IDbDataParameter dbparam = (IDbDataParameter)dbCommand.Parameters[i];
                    CSParameter parameter = _SPParameters.Add(dbparam.ParameterName);
                    parameter.Direction = dbparam.Direction;
                    //if (dbparam.Size > 0)
                    parameter.Size = dbparam.Size;
                    parameter.Precision = dbparam.Precision;
                    parameter.Scale = dbparam.Scale;
                }
            }
            return _SPParameters;
        }

        /// <summary>
        /// Recupera los nombres y tipos de los parametros de un stored procedure
        /// </summary>
        /// <param name="StoredProcedureName">Nombre del stored procedure</param>
        /// <param name="WithReturn">Si debe incluir en los parametros el tipo de retorno o no</param>
        /// <returns>Un array de SqlParameter con los parametros que espera (y devuelve, si aplica) el stored procedure</returns>
        public static CSParameterCollection GetSpParams(this CSDataProvider dp, string StoredProcedureName, bool WithReturn)
        {
            //Cache
            CSParameterCollection pc;
            CSParameterCollection pcRet;
            if (_map.ContainsKey(StoredProcedureName))
            {
                pc = _map[StoredProcedureName];
            }
            else
            {
                pc = InternalGetSpParams(dp, StoredProcedureName, WithReturn);
                _map[StoredProcedureName] = pc;
            }
            pcRet = new CSParameterCollection(pc);
            pcRet.ForEach<CSParameter>(param =>
            {
                if (param.Direction.BitOn(ParameterDirection.Output))
                {
                    param.Value = DBNull.Value;
                    if (!param.Size.HasValue || param.Size == 0)
                    {
                        param.Size = 1024;
                    }
                }
            });
            return pcRet;
        }
    }
}
