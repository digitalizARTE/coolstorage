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
using System.Reflection;

using Vici.Core;

namespace Vici.CoolStorage
{
	/// <summary>
	/// Represents a raw data base record.
	/// </summary>
	/// <remarks>
	/// ColumnName<->Value pairs.
	/// 
	/// <code>
	///    CSGenericRecordList list = new CSGenericRecordList();
	///    using (IDataReader reader = DB.CreateReader(sql, parameters))
	///    {
	///     while (reader.Read())
	///         {
	///             CSGenericRecord record = new CSGenericRecord();
	///
	///             for (int i = 0; i < reader.FieldCount; i++)
	///             {
	///                 record[reader.GetName(i)] = (reader[i] is DBNull) ? null : reader[i];
	///             }
	///
	///             list.Add(record);
	///          }
	///    }
	/// </code>
	/// 
	/// </remarks>
    public class CSGenericRecord : Dictionary<string, object> { }
	/// <summary>
	/// Represents a generic query result.
	/// </summary>
	/// <remarks>
	/// <see cref="CSGenericRecord"/>
	/// </remarks>
    public class CSGenericRecordList : List<CSGenericRecord> { }

    /// <summary>
    /// Represents a "Data Base" instance defined by a Context Name string.
    /// </summary>
    public class CSDatabaseInstance
    {
        private readonly string _contextName;

        internal CSDatabaseInstance(string contextName)
        {
            _contextName = contextName;
        }

        private CSDataProvider DB
        {
            get { return CSConfig.GetDB(_contextName); }
        }

        /// <summary>
        /// Executes a SQL non query.
        /// </summary>
        /// <param name="sql">The SQL command.</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(sql, CSParameterCollection.Empty);
        }

        /// <summary>Executes a SQL non query given a parameter.
        /// </summary>
        /// <param name="sql">The SQ  commandL.</param>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, string paramName, object paramValue)
        {
            return ExecuteNonQuery(sql, new CSParameterCollection(paramName, paramValue));
        }

        /// <summary>Executes the SQL non-query given two parameters.
        /// </summary>
        /// <param name="sql">The SQL Command.</param>
        /// <param name="paramName1">The param name1.</param>
        /// <param name="paramValue1">The param value1.</param>
        /// <param name="paramName2">The param name2.</param>
        /// <param name="paramValue2">The param value2.</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2)
        {
            return ExecuteNonQuery(sql, new CSParameterCollection(paramName1, paramValue1, paramName2, paramValue2));
        }

        /// <summary>Executes the SQL non-query given three parameters.
        /// </summary>
        /// <param name="sql">The SQL Command.</param>
        /// <param name="paramName1">The param name1.</param>
        /// <param name="paramValue1">The param value1.</param>
        /// <param name="paramName2">The param name2.</param>
        /// <param name="paramValue2">The param value2.</param>
        /// <param name="paramName3">The param name3.</param>
        /// <param name="paramValue3">The param value3.</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2, string paramName3, object paramValue3)
        {
            return ExecuteNonQuery(sql, new CSParameterCollection(paramName1, paramValue1, paramName2, paramValue2, paramName3, paramValue3));
        }

        /// <summary> Executes a SQL non query command given a CSParameter collection.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, CSParameterCollection parameters)
        {
            using (new CSTransaction(DB))
                return DB.ExecuteNonQuery(sql, parameters);
        }

        public int ExecuteNonQuery(string sql, object parameters)
        {
            return ExecuteNonQuery(sql, new CSParameterCollection(parameters));
        }

		 /// <summary>
        /// <summary> Executes a SQL non query command given a CSParameter array.
        /// </summary>
        /// <param name="sql">The SQL command.</param>
        /// <param name="parameters">The parameter array.</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, params CSParameter[] parameters)
        {
            return ExecuteNonQuery(sql, new CSParameterCollection(parameters));
        }

        /// <summary> Returns a SQL scalar.
        /// </summary>
        /// <param name="sql">The SQL statement.</param>
        /// <returns></returns>
        public object GetScalar(string sql)
        {
            return GetScalar(sql, CSParameterCollection.Empty);
        }

        /// <summary> Returns a SQL scalar given a parameter.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public object GetScalar(string sql, string paramName, object paramValue)
        {
            return GetScalar(sql, new CSParameterCollection(paramName, paramValue));
        }

        public object GetScalar(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2)
        {
            return GetScalar(sql, new CSParameterCollection(paramName1, paramValue1, paramName2, paramValue2));
        }

        /// <summary>Returns a SQL scalar given three parameters.
        /// </summary>
        /// <param name="sql">The SQL statement.</param>
        /// <param name="paramName1">The param name1.</param>
        /// <param name="paramValue1">The param value1.</param>
        /// <param name="paramName2">The param name2.</param>
        /// <param name="paramValue2">The param value2.</param>
        /// <param name="paramName3">The param name3.</param>
        /// <param name="paramValue3">The param value3.</param>
        /// <returns></returns>
        public object GetScalar(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2, string paramName3, object paramValue3)
        {
            return GetScalar(sql, new CSParameterCollection(paramName1, paramValue1, paramName2, paramValue2, paramName3, paramValue3));
        }

        public object GetScalar(string sql, object parameters)
        {
            return GetScalar(sql, new CSParameterCollection(parameters));
        }

		/// <summary>Returns a SQL scalar given an array of parameters.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public object GetScalar(string sql, params CSParameter[] parameters)
        {
            return GetScalar(sql, new CSParameterCollection(parameters));
        }

        /// <summary>Returns a SQL scalar given an collection of parameters.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public object GetScalar(string sql, CSParameterCollection parameters)
        {
            using (new CSTransaction(DB))
                return DB.GetScalar(sql, parameters);
        }

        /// <summary>
        /// Returns an array of scalars
        /// </summary>
        /// <typeparam name="TScalar">The type of the scalar.</typeparam>
        /// <param name="sql">the SQL String.</param>
        /// <returns>An array of scalars</returns>
        public T[] GetScalarList<T>(string sql)
        {
            return GetScalarList<T>(sql, CSParameterCollection.Empty);
        }

        /// <summary>
        /// Returns an array of scalars
        /// </summary>
        /// <typeparam name="TScalar">The type of the scalar.</typeparam>
        /// <param name="sql">the SQL String.</param>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns>An array of scalars</returns>
        public T[] GetScalarList<T>(string sql, string paramName, object paramValue)
        {
            return GetScalarList<T>(sql, new CSParameterCollection(paramName, paramValue));
        }

        /// <summary>
        /// Returns an array of scalars
        /// </summary>
        /// <typeparam name="TScalar">The type of the scalar.</typeparam>
        /// <param name="sql">the SQL String.</param>
        /// <param name="paramName1">The param name1.</param>
        /// <param name="paramValue1">The param value1.</param>
        /// <param name="paramName2">The param name2.</param>
        /// <param name="paramValue2">The param value2.</param>
        /// <returns>An array of scalars</returns>
        public T[] GetScalarList<T>(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2)
        {
            return GetScalarList<T>(sql, new CSParameterCollection(paramName1, paramValue1, paramName2, paramValue2));
        }

        /// <summary>
        /// Returns an array of scalars
        /// </summary>
        /// <typeparam name="TScalar">The type of the scalar.</typeparam>
        /// <param name="sql">the SQL String.</param>
        /// <param name="paramName1">The param name1.</param>
        /// <param name="paramValue1">The param value1.</param>
        /// <param name="paramName2">The param name2.</param>
        /// <param name="paramValue2">The param value2.</param>
        /// <param name="paramName3">The param name3.</param>
        /// <param name="paramValue3">The param value3.</param>
        /// <returns>An array of scalars</returns>
        public T[] GetScalarList<T>(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2, string paramName3, object paramValue3)
        {
            return GetScalarList<T>(sql, new CSParameterCollection(paramName1, paramValue1, paramName2, paramValue2, paramName3, paramValue3));
        }

        /// <summary>
        /// Returns an array of scalars
        /// </summary>
        /// <typeparam name="TScalar">The type of the scalar.</typeparam>
        /// <param name="sql">the SQL String.</param>
        /// <param name="parameters">An Array of CSParameters.</param>
        /// <returns>
        /// An array of TScalar
        /// </returns>
        public T[] GetScalarList<T>(string sql, object parameters)
        {
            return GetScalarList<T>(sql, new CSParameterCollection(parameters));
        }

        public T[] GetScalarList<T>(string sql, params CSParameter[] parameters)
        {
            return GetScalarList<T>(sql, new CSParameterCollection(parameters));
        }
        
		/// <summary>
        /// Returns an array of scalars
        /// </summary>
        /// <typeparam name="TScalar">The type of the scalar.</typeparam>
        /// <param name="sql">the SQL String.</param>
        /// <param name="parameters">A Collection of CSParameters.</param>
        /// <returns>
        /// An array of scalars
        /// </returns>
        public T[] GetScalarList<T>(string sql, CSParameterCollection parameters)
        {
            List<T> list = new List<T>();

            using (new CSTransaction(DB))
            {
                using (ICSDbReader reader = DB.CreateReader(sql, parameters))
                {
                    int recs = 0;
                    bool ok = true;
                    while (recs == 0 && ok)
                    {
                        while (reader.Read())
                        {
                            list.Add(reader[0].Convert<T>());
                            recs++;
                        }
                        ok = reader.NextResult();
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns the scalar
        /// </summary>
        /// <typeparam name="TScalar">The type of the scalar.</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <returns></returns>
        public T GetScalar<T>(string sql)
        {
            return GetScalar<T>(sql, CSParameterCollection.Empty);
        }

        /// <summary>
        /// Returns the scalar
        /// </summary>
        /// <typeparam name="TScalar">The type of the scalar.</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public T GetScalar<T>(string sql, string paramName, object paramValue)
        {
            return GetScalar<T>(sql, new CSParameterCollection(paramName, paramValue));
        }

        /// <summary>
        /// Returns the scalar
        /// </summary>
        /// <typeparam name="TScalar">The type of the scalar.</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="paramName1">The param name1.</param>
        /// <param name="paramValue1">The param value1.</param>
        /// <param name="paramName2">The param name2.</param>
        /// <param name="paramValue2">The param value2.</param>
        /// <returns></returns>
        public T GetScalar<T>(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2)
        {
            return GetScalar<T>(sql, new CSParameterCollection(paramName1, paramValue1, paramName2, paramValue2));
        }

        /// <summary>
        /// Returns the scalar
        /// </summary>
        /// <typeparam name="TScalar">The type of the scalar.</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="paramName1">The param name1.</param>
        /// <param name="paramValue1">The param value1.</param>
        /// <param name="paramName2">The param name2.</param>
        /// <param name="paramValue2">The param value2.</param>
        /// <param name="paramName3">The param name3.</param>
        /// <param name="paramValue3">The param value3.</param>
        /// <returns></returns>
        public T GetScalar<T>(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2, string paramName3, object paramValue3)
        {
            return GetScalar<T>(sql, new CSParameterCollection(paramName1, paramValue1, paramName2, paramValue2, paramName3, paramValue3));
        }

        /// <summary>
        /// Returns the scalar
        /// </summary>
        /// <typeparam name="TScalar">The type of the scalar.</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public T GetScalar<T>(string sql, params CSParameter[] parameters)
        {
            return GetScalar<T>(sql, new CSParameterCollection(parameters));
        }

        public T GetScalar<T>(string sql, object parameters)
        {
            return GetScalar<T>(sql, new CSParameterCollection(parameters));
        }

		/// <summary>
        /// Returns the scalar
        /// </summary>
        /// <typeparam name="TScalar">The type of the scalar.</typeparam>
        /// <param name="sql">The SQL string.</param>
        /// <param name="parameters">A CSParameterCollection</param>
        /// <returns>An instance of TScalar</returns>
        public T GetScalar<T>(string sql, CSParameterCollection parameters)
        {
            return GetScalar(sql, parameters).Convert<T>();
        }

        /// <summary>
        /// Runs the query.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <returns></returns>
        public CSGenericRecordList RunQuery(string sql)
        {
            return RunQuery(sql, CSParameterCollection.Empty);
        }

        /// <summary>
        /// Runs the query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public CSGenericRecordList RunQuery(string sql, string paramName, object paramValue)
        {
            return RunQuery(sql, new CSParameterCollection(paramName, paramValue));
        }

        /// <summary>
        /// Runs the query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="paramName1">The param name1.</param>
        /// <param name="paramValue1">The param value1.</param>
        /// <param name="paramName2">The param name2.</param>
        /// <param name="paramValue2">The param value2.</param>
        /// <returns></returns>
        public CSGenericRecordList RunQuery(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2)
        {
            return RunQuery(sql, new CSParameterCollection(paramName1, paramValue1, paramName2, paramValue2));
        }

        /// <summary>
        /// Runs the query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="paramName1">The param name1.</param>
        /// <param name="paramValue1">The param value1.</param>
        /// <param name="paramName2">The param name2.</param>
        /// <param name="paramValue2">The param value2.</param>
        /// <param name="paramName3">The param name3.</param>
        /// <param name="paramValue3">The param value3.</param>
        /// <returns></returns>
        public CSGenericRecordList RunQuery(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2, string paramName3, object paramValue3)
        {
            return RunQuery(sql, new CSParameterCollection(paramName1, paramValue1, paramName2, paramValue2, paramName3, paramValue3));
        }

        /// <summary>
        /// Runs the query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public CSGenericRecordList RunQuery(string sql, params CSParameter[] parameters)
        {
            return RunQuery(sql, new CSParameterCollection(parameters));
        }

        public CSGenericRecordList RunQuery(string sql, object parameters)
        {
            return RunQuery(sql, new CSParameterCollection(parameters));
        }

		/// <summary>
        /// Runs the query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public CSGenericRecordList RunQuery(string sql, CSParameterCollection parameters)
        {
            CSGenericRecordList list = new CSGenericRecordList();

            using (new CSTransaction(DB))
            {
                using (ICSDbReader reader = DB.CreateReader(sql, parameters))
                {
                    int recs = 0;
                    bool ok = true;
                    while (recs == 0 && ok)
                {
                    while (reader.Read())
                    {
                        CSGenericRecord record = new CSGenericRecord();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            record[reader.GetName(i)] = (reader[i] is DBNull) ? null : reader[i];
                        }

                            list.Add(record);
                            recs++;
                        }
                        ok = reader.NextResult();
                    }
                }
            }

            return list;
        }

		/// <summary>
        /// Runs the single query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns></returns>
        public CSGenericRecord RunSingleQuery(string sql)
        {
            return RunSingleQuery(sql, CSParameterCollection.Empty);
        }

        /// <summary>
        /// Runs the single query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public CSGenericRecord RunSingleQuery(string sql, string paramName, object paramValue)
        {
            return RunSingleQuery(sql, new CSParameterCollection(paramName, paramValue));
        }

        /// <summary>
        /// Runs the single query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="paramName1">The param name1.</param>
        /// <param name="paramValue1">The param value1.</param>
        /// <param name="paramName2">The param name2.</param>
        /// <param name="paramValue2">The param value2.</param>
        /// <returns></returns>
        public CSGenericRecord RunSingleQuery(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2)
        {
            return RunSingleQuery(sql, new CSParameterCollection(paramName1, paramValue1, paramName2, paramValue2));
        }

        /// <summary>
        /// Runs the single query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="paramName1">The param name1.</param>
        /// <param name="paramValue1">The param value1.</param>
        /// <param name="paramName2">The param name2.</param>
        /// <param name="paramValue2">The param value2.</param>
        /// <param name="paramName3">The param name3.</param>
        /// <param name="paramValue3">The param value3.</param>
        /// <returns></returns>
        public CSGenericRecord RunSingleQuery(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2, string paramName3, object paramValue3)
        {
            return RunSingleQuery(sql, new CSParameterCollection(paramName1, paramValue1, paramName2, paramValue2, paramName3, paramValue3));
        }

        /// <summary>
        /// Runs the single query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public CSGenericRecord RunSingleQuery(string sql, params CSParameter[] parameters)
        {
            return RunSingleQuery(sql, new CSParameterCollection(parameters));
        }

        public CSGenericRecord RunSingleQuery(string sql,object parameters)
        {
            return RunSingleQuery(sql, new CSParameterCollection(parameters));
        }

/// <summary>
        /// Runs the single query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public CSGenericRecord RunSingleQuery(string sql, CSParameterCollection parameters)
        {
            int recs = 0;
            bool ok = true;
            CSGenericRecord rec = new CSGenericRecord();

            using (new CSTransaction(DB))
            {
                using (ICSDbReader reader = DB.CreateReader(sql, parameters))
                {
                    while (recs == 0 && ok)
                    {
                        if (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                rec[reader.GetName(i)] = (reader[i] is DBNull) ? null : reader[i];
                            }
                            //return rec;
                            recs++;
                        }
                        ok = reader.NextResult();
                    }
                }
            }
            return (recs == 0) ? null : rec;
        }

        /// <summary>
        /// Runs the query.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <returns></returns>
        public T[] RunQuery<T>(string sql) where T : new()
        {
            return RunQuery<T>(sql, null, 0);
        }

        /// <summary>
        /// Runs the query.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public T[] RunQuery<T>(string sql, CSParameterCollection parameters) where T : new()
        {
            return RunQuery<T>(sql, parameters, 0);
        }

        /// <summary>
        /// Runs the query.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public T[] RunQuery<T>(string sql, object parameters) where T : new()
        {
            return RunQuery<T>(sql, new CSParameterCollection(parameters), 0);
        }

        /// <summary>
        /// Runs the query.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="paramName1">The param name1.</param>
        /// <param name="paramValue1">The param value1.</param>
        /// <param name="paramName2">The param name2.</param>
        /// <param name="paramValue2">The param value2.</param>
        /// <returns></returns>
        public T[] RunQuery<T>(string sql, string paramName, object paramValue) where T : new()
        {
            return RunQuery<T>(sql, new CSParameterCollection(paramName, paramValue), 0);
        }

        /// <summary>
        /// Runs the query.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="paramName1">The param name1.</param>
        /// <param name="paramValue1">The param value1.</param>
        /// <param name="paramName2">The param name2.</param>
        /// <param name="paramValue2">The param value2.</param>
        /// <param name="paramName3">The param name3.</param>
        /// <param name="paramValue3">The param value3.</param>
        /// <returns></returns>
        public T[] RunQuery<T>(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2) where T : new()
        {
            return RunQuery<T>(sql, new CSParameterCollection(paramName1, paramValue1, paramName2, paramValue2), 0);
        }

        public T[] RunQuery<T>(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2, string paramName3, object paramValue3) where T : new()
        {
            return RunQuery<T>(sql, new CSParameterCollection(paramName1, paramValue1, paramName2, paramValue2, paramName3, paramValue3), 0);
        }
   		 /// <summary>
        /// Runs the query.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <returns></returns>
		public T[] RunQuery<T>() where T : new()
		{
			return RunQuery<T>(CSHelper.GetQueryExpression<T>(), null, 0);
		}
	
        /// <summary>
        /// Runs the query.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public T[] RunQuery<T>(object parameters) where T : new()
        {
            return RunQuery<T>(null, new CSParameterCollection(parameters), 0);
        }

        /// <summary>
        /// Runs the query.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="paramName1">The param name1.</param>
        /// <param name="paramValue1">The param value1.</param>
        /// <param name="paramName2">The param name2.</param>
        /// <param name="paramValue2">The param value2.</param>
        /// <returns></returns>
        public T[] RunQuery<T>(CSParameterCollection parameters) where T : new()
        {
            return RunQuery<T>(null, parameters, 0);
        }

        /// <summary>
        /// Runs the query.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="paramName1">The param name1.</param>
        /// <param name="paramValue1">The param value1.</param>
        /// <param name="paramName2">The param name2.</param>
        /// <param name="paramValue2">The param value2.</param>
        /// <param name="paramName3">The param name3.</param>
        /// <param name="paramValue3">The param value3.</param>
        /// <returns></returns>
		public T[] RunQuery<T>(string paramName1, object paramValue1, string paramName2, object paramValue2, string paramName3, object paramValue3) where T : new()
		{
            return RunQuery<T>(CSHelper.GetQueryExpression<T>(), new CSParameterCollection(paramName1, paramValue1, paramName2, paramValue2, paramName3, paramValue3), 0);
		}
		
        public T RunSingleQuery<T>() where T : class, new()
        {
            return RunSingleQuery<T>((string)null);
        }

        public T RunSingleQuery<T>(CSParameterCollection parameters) where T : class, new()
        {
            return RunSingleQuery<T>(null, parameters);
        }

        public T RunSingleQuery<T>(object parameters) where T : class, new()
        {
            return RunSingleQuery<T>(null, parameters);
        }

        public T RunSingleQuery<T>(string sql) where T : class, new()
        {
            return RunSingleQuery<T>(sql, null);
        }

        public T RunSingleQuery<T>(string sql, object parameters) where T : class, new()
        {
            return RunSingleQuery<T>(sql, new CSParameterCollection(parameters));
        }

        public T RunSingleQuery<T>(string sql, CSParameterCollection parameters) where T : class, new()
        {
            T[] objects = RunQuery<T>(sql, parameters, 1);

            return (objects.Length > 0) ? objects[0] : null;
        }

        public T RunSingleQuery<T>(string sql, string paramName, object paramValue) where T : class, new()
        {
            return RunSingleQuery<T>(sql, new CSParameterCollection(paramName, paramValue));
        }

        public T RunSingleQuery<T>(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2) where T : class, new()
        {
            return RunSingleQuery<T>(sql, new CSParameterCollection(paramName1, paramValue1, paramName2, paramValue2));
        }

        public T RunSingleQuery<T>(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2, string paramName3, object paramValue3) where T : class, new()
        {
            return RunSingleQuery<T>(sql, new CSParameterCollection(paramName1, paramValue1, paramName2, paramValue2, paramName3, paramValue3));
        }

        private T[] RunQuery<T>(string sql, CSParameterCollection parameters, int maxRows) where T : new()
        {
            Type objectType = typeof(T);

            List<T> list = new List<T>();

            if (maxRows == 0)
                maxRows = int.MaxValue;

            using (new CSTransaction(DB))
            {
                using (ICSDbReader reader = DB.CreateReader(sql ?? CSHelper.GetQueryExpression<T>(), parameters))
                {
                    int rowNum = 0;
                    //GetHashTable de T
                    Dictionary<string, MemberInfo> map = CSTypedQueryMapper.GetMap(typeof(T));
                    int recs = 0;
                    bool ok = true;
                    while (recs == 0 && ok)
                    {
                        while (rowNum < maxRows && reader.Read())
                        {
                            rowNum++;

                        T obj = new T();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string columnName = reader.GetName(i);

                            PropertyInfo propertyInfo = objectType.GetProperty(columnName);

                            object columnValue = reader[i];

                            if (columnValue is DBNull)
                                columnValue = null;

                                //PropertyInfo propertyInfo;
                                FieldInfo fieldInfo;

                                if (map.ContainsKey(columnName))
                                {
                                    //if (map[columnName].GetType().IsInstanceOfType() == typeof(PropertyInfo))
                                    if (typeof(PropertyInfo).IsInstanceOfType(map[columnName]))
                                    {
                                        propertyInfo = (PropertyInfo)map[columnName];
                                        propertyInfo.SetValue(obj, columnValue.Convert(propertyInfo.PropertyType), null);
                                    }
                                    //else if (map[columnName].GetType().MemberType == typeof(FieldInfo))
                                    else if (typeof(FieldInfo).IsInstanceOfType(map[columnName]))
                                    {
                                        fieldInfo = (FieldInfo)map[columnName];
                                        fieldInfo.SetValue(obj, columnValue.Convert(fieldInfo.FieldType));
                                    }
                                }
#if false
		                            if (propertyInfo != null)
		                            {
		                                propertyInfo.SetValue(obj, columnValue.Convert(propertyInfo.PropertyType), null);
		                            }
		                            else
		                            {
		                                FieldInfo fieldInfo = objectType.GetField(columnName);

		                                if (fieldInfo != null)
		                                {
		                                    fieldInfo.SetValue(obj, columnValue.Convert(fieldInfo.FieldType));
		                                }
		                            }
		                            
#endif

                            }
                            list.Add(obj);
                            recs++;
                        }
                        ok = reader.NextResult();
                    }
                }
            }

            return list.ToArray();
        }    
		
        public CSParameterCollection GetSpParams(string StoredProcedureName, bool WithReturn)
		{
			return DB.GetSpParams(StoredProcedureName, WithReturn);
        }

        public CSParameterCollection GetSpParams(string StoredProcedureName)
        {
            return DB.GetSpParams(StoredProcedureName, false);
        }
	}
	
	public static class CSDatabase
	{
        private static readonly CSDatabaseContext _dbContext = new CSDatabaseContext();
        
        public class CSDatabaseContext
        {
            public CSDatabaseInstance Default
            {
                get { return this[CSConfig.DEFAULT_CONTEXTNAME]; }
            }

            public CSDatabaseInstance this[string contextName]
            {
                get { return new CSDatabaseInstance(contextName); }
            }
        }

        public static CSDatabaseContext Context
        {
            get { return _dbContext; }
        }

        public static int ExecuteNonQuery(string sql)
        {
			return Context.Default.ExecuteNonQuery(sql);
        }

        public static int ExecuteNonQuery(string sql, string paramName, object paramValue)
        {
            return Context.Default.ExecuteNonQuery(sql, paramName, paramValue);
        }

        public static int ExecuteNonQuery(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2)
        {
            return Context.Default.ExecuteNonQuery(sql, paramName1, paramValue1, paramName2, paramValue2);
        }

        public static int ExecuteNonQuery(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2, string paramName3, object paramValue3)
        {
            return Context.Default.ExecuteNonQuery(sql, paramName1, paramValue1, paramName2, paramValue2, paramName3, paramValue3);
        }

		public static int ExecuteNonQuery(string sql, CSParameterCollection parameters)
		{
            return Context.Default.ExecuteNonQuery(sql, parameters);
		}

		public static int ExecuteNonQuery(string sql, params CSParameter[] parameters)
		{
			return Context.Default.ExecuteNonQuery(sql, parameters);
		}

        public static int ExecuteNonQuery(string sql, object parameters)
        {
            return Context.Default.ExecuteNonQuery(sql, parameters);
        }

        public static object GetScalar(string sql)
        {
            return Context.Default.GetScalar(sql);
        }

		public static object GetScalar(string sql, string paramName, object paramValue)
		{
            return Context.Default.GetScalar(sql, paramName, paramValue);
		}

        public static object GetScalar(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2)
        {
            return Context.Default.GetScalar(sql, paramName1, paramValue1, paramName2, paramValue2);
        }

        public static object GetScalar(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2, string paramName3, object paramValue3)
        {
            return Context.Default.GetScalar(sql, paramName1, paramValue1, paramName2, paramValue2, paramName3, paramValue3);
        }

		public static object GetScalar(string sql, params CSParameter[] parameters)
		{
            return Context.Default.GetScalar(sql, parameters);
		}

        public static object GetScalar(string sql, object parameters)
        {
            return Context.Default.GetScalar(sql, parameters);
        }

		public static object GetScalar(string sql, CSParameterCollection parameters)
		{
            return Context.Default.GetScalar(sql, parameters);
		}

        public static T GetScalar<T>(string sql)
        {
            return Context.Default.GetScalar<T>(sql);
        }

        public static T GetScalar<T>(string sql, string paramName, object paramValue)
        {
            return Context.Default.GetScalar<T>(sql, paramName, paramValue);
        }

        public static T GetScalar<T>(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2)
        {
            return Context.Default.GetScalar<T>(sql, paramName1, paramValue1, paramName2, paramValue2);
        }

        public static T GetScalar<T>(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2, string paramName3, object paramValue3)
        {
            return Context.Default.GetScalar<T>(sql, paramName1, paramValue1, paramName2, paramValue2, paramName3, paramValue3);
        }

		public static T GetScalar<T>(string sql, params CSParameter[] parameters)
		{
            return Context.Default.GetScalar<T>(sql, parameters);
		}

        public static T GetScalar<T>(string sql, object parameters)
        {
            return Context.Default.GetScalar<T>(sql, parameters);
        }

		public static T GetScalar<T>(string sql, CSParameterCollection parameters)
		{
            return Context.Default.GetScalar<T>(sql, parameters);
		}

        public static T[] GetScalarList<T>(string sql)
        {
            return Context.Default.GetScalarList<T>(sql);
        }

        public static T[] GetScalarList<T>(string sql, string paramName, object paramValue)
        {
            return Context.Default.GetScalarList<T>(sql, paramName, paramValue);
        }

        public static T[] GetScalarList<T>(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2)
        {
            return Context.Default.GetScalarList<T>(sql, paramName1, paramValue1, paramName2, paramValue2);
        }

        public static T[] GetScalarList<T>(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2, string paramName3, object paramValue3)
        {
            return Context.Default.GetScalarList<T>(sql, paramName1, paramValue1, paramName2, paramValue2, paramName3, paramValue3);
        }

        public static T[] GetScalarList<T>(string sql, params CSParameter[] parameters)
        {
            return Context.Default.GetScalarList<T>(sql, parameters);
        }

        public static T[] GetScalarList<T>(string sql, object parameters)
        {
            return Context.Default.GetScalarList<T>(sql, parameters);
        }

        public static T[] GetScalarList<T>(string sql, CSParameterCollection parameters)
        {
            return Context.Default.GetScalarList<T>(sql, parameters);
        }

        public static CSGenericRecordList RunQuery(string sql)
		{
            return Context.Default.RunQuery(sql);
		}

        public static CSGenericRecordList RunQuery(string sql, string paramName, object paramValue)
		{
            return Context.Default.RunQuery(sql, paramName, paramValue);
		}

        public static CSGenericRecordList RunQuery(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2)
		{
            return Context.Default.RunQuery(sql, paramName1, paramValue1, paramName2, paramValue2);
		}

        public static CSGenericRecordList RunQuery(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2, string paramName3, object paramValue3)
		{
            return Context.Default.RunQuery(sql, paramName1, paramValue1, paramName2, paramValue2, paramName3, paramValue3);
		}

		public static CSGenericRecordList RunQuery(string sql, CSParameterCollection parameters)
		{
		    return Context.Default.RunQuery(sql, parameters);
		}

        public static CSGenericRecordList RunQuery(string sql, params CSParameter[] parameters)
        {
            return Context.Default.RunQuery(sql, parameters);
        }

        public static CSGenericRecordList RunQuery(string sql, object parameters)
        {
            return Context.Default.RunQuery(sql, parameters);
        }

		public static T[] RunQuery<T>(string sql) where T : new()
		{
            return Context.Default.RunQuery<T>(sql);
		}

		public static T[] RunQuery<T>(string sql, CSParameterCollection parameters) where T : new()
		{
            return Context.Default.RunQuery<T>(sql, parameters);
		}

        public static T[] RunQuery<T>(string sql, object parameters) where T : new()
        {
            return Context.Default.RunQuery<T>(sql, parameters);
        }

		public static T[] RunQuery<T>(string sql, string paramName, object paramValue) where T : new()
		{
            return Context.Default.RunQuery<T>(sql, paramName, paramValue);
		}

		public static T[] RunQuery<T>(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2) where T : new()
		{
		    return Context.Default.RunQuery<T>(sql, paramName1, paramValue1, paramName2, paramValue2);
		}

		public static T[] RunQuery<T>(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2, string paramName3, object paramValue3) where T : new()
		{
		    return Context.Default.RunQuery<T>(sql, paramName1, paramValue1, paramName2, paramValue2, paramName3, paramValue3);
		}

		public static T[] RunQuery<T>() where T:new()
		{
            return Context.Default.RunQuery<T>();
		}

		public static T[] RunQuery<T>(CSParameterCollection parameters) where T : new()
		{
            return Context.Default.RunQuery<T>(parameters);
		}

        public static T[] RunQuery<T>(object parameters) where T : new()
        {
            return Context.Default.RunQuery<T>(parameters);
        }

		public static T RunSingleQuery<T>() where T : class, new()
		{
            return Context.Default.RunSingleQuery<T>();
		}

		public static T RunSingleQuery<T>(CSParameterCollection parameters) where T : class, new()
		{
		    return Context.Default.RunSingleQuery<T>(parameters);
		}

        public static T RunSingleQuery<T>(object parameters) where T : class, new()
        {
            return Context.Default.RunSingleQuery<T>(parameters);
        }

		public static T RunSingleQuery<T>(string sql) where T : class, new()
		{
            return Context.Default.RunSingleQuery<T>(sql);
		}

		public static T RunSingleQuery<T>(string sql,CSParameterCollection parameters) where T : class, new()
		{
		    return Context.Default.RunSingleQuery<T>(sql, parameters);
		}

		public static T RunSingleQuery<T>(string sql, string paramName, object paramValue) where T : class, new()
		{
            return Context.Default.RunSingleQuery<T>(sql,paramName, paramValue);
		}

		public static T RunSingleQuery<T>(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2) where T : class, new()
		{
            return Context.Default.RunSingleQuery<T>(sql, paramName1, paramValue1, paramName2, paramValue2);
		}

		public static T RunSingleQuery<T>(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2, string paramName3, object paramValue3) where T : class, new()
		{
            return Context.Default.RunSingleQuery<T>(sql, paramName1, paramValue1, paramName2, paramValue2, paramName3, paramValue3);
		}

        public static CSGenericRecord RunSingleQuery(string sql)
        {
            return Context.Default.RunSingleQuery(sql);
        }

        public static CSGenericRecord RunSingleQuery(string sql, string paramName, object paramValue)
        {
            return Context.Default.RunSingleQuery(sql, paramName, paramValue);
        }

        public static CSGenericRecord RunSingleQuery(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2)
        {
            return Context.Default.RunSingleQuery(sql, paramName1, paramValue1, paramName2, paramValue2);
        }

        public static CSGenericRecord RunSingleQuery(string sql, string paramName1, object paramValue1, string paramName2, object paramValue2, string paramName3, object paramValue3)
        {
            return Context.Default.RunSingleQuery(sql, paramName1, paramValue1, paramName2, paramValue2, paramName3, paramValue3);
        }

        public static CSGenericRecord RunSingleQuery(string sql, CSParameterCollection parameters)
        {
            return Context.Default.RunSingleQuery(sql, parameters);
        }

        public static CSGenericRecord RunSingleQuery(string sql, params CSParameter[] parameters)
        {
            return Context.Default.RunSingleQuery(sql, parameters);
        }

	}
}
