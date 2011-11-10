using System;
using System.Data;
namespace Vici.CoolStorage
{
    public interface ICSDataProvider
    {
        /// <summary>
        /// 
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isolationLevel"></param>
        void BeginTransaction(System.Data.IsolationLevel isolationLevel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="joinList"></param>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        string BuildDeleteSQL(string tableName, string joinList, string whereClause);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnList"></param>
        /// <param name="valueList"></param>
        /// <param name="primaryKeys"></param>
        /// <param name="identityField"></param>
        /// <returns></returns>
        string BuildGetKeys(string tableName, string[] columnList, string[] valueList, string[] primaryKeys, string identityField);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnList"></param>
        /// <param name="valueList"></param>
        /// <param name="primaryKeys"></param>
        /// <param name="sequences"></param>
        /// <param name="identityField"></param>
        /// <returns></returns>
        string BuildInsertSQL(string tableName, string[] columnList, string[] valueList, string[] primaryKeys, string[] sequences, string identityField);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tableAlias"></param>
        /// <param name="columnList"></param>
        /// <param name="columnAliasList"></param>
        /// <param name="joinList"></param>
        /// <param name="whereClause"></param>
        /// <param name="orderBy"></param>
        /// <param name="startRow"></param>
        /// <param name="maxRows"></param>
        /// <param name="quoteColumns"></param>
        /// <param name="unOrdered"></param>
        /// <returns></returns>
        string BuildSelectSQL(string tableName, string tableAlias, string[] columnList, string[] columnAliasList, string[] joinList, string whereClause, string orderBy, int startRow, int maxRows, bool quoteColumns, bool unOrdered);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnList"></param>
        /// <param name="valueList"></param>
        /// <param name="whereClasuse"></param>
        /// <returns></returns>
        string BuildUpdateSQL(string tableName, string[] columnList, string[] valueList, string whereClasuse);
        
        /// <summary>
        /// 
        /// </summary>
        void ClearConnectionPool();
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ICSDataProvider Clone();
        
        /// <summary>
        /// 
        /// </summary>
        void CloseConnection();
        
        /// <summary>
        /// 
        /// </summary>
        void Commit();
        
        /// <summary>
        /// 
        /// </summary>
        IDbConnection Connection { get; }
        
        /// <summary>
        /// 
        /// </summary>
        string ConnectionString { get; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IDbCommand CreateCommand(string sqlQuery, CSParameterCollection parameters);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IDbCommand CreateCommandInternal(string sqlQuery, CSParameterCollection parameters);
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDbConnection CreateConnection();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IDataReader CreateReader(string sqlQuery, CSParameterCollection parameters);
        
        /// <summary>
        /// 
        /// </summary>
        void Dispose();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnList"></param>
        /// <param name="valueList"></param>
        /// <param name="primaryKeys"></param>
        /// <param name="sequences"></param>
        /// <param name="identityField"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IDataReader ExecuteInsert(string tableName, string[] columnList, string[] valueList, string[] primaryKeys, string[] sequences, string identityField, CSParameterCollection parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnList"></param>
        /// <param name="valueList"></param>
        /// <param name="primaryKeys"></param>
        /// <param name="sequences"></param>
        /// <param name="identityField"></param>
        /// <param name="parameters"></param>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        bool ExecuteUpdate(string tableName, string[] columnList, string[] valueList, CSParameterCollection parameters, CSFilter whereClause);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int ExecuteNonQuery(string sqlQuery, CSParameterCollection parameters);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        object GetScalar(string sqlQuery, CSParameterCollection parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        DataTable GetSchemaTable(string tableName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        string NativeFunction(string functionName, ref string[] parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        string QuoteField(string fieldName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        string QuoteTable(string tableName);

        /// <summary>
        /// 
        /// </summary>
        bool RequiresSeperateIdentityGet { get; }

        /// <summary>
        /// 
        /// </summary>
        void Rollback();

        /// <summary>
        /// 
        /// </summary>
        bool SupportsMultipleStatements { get; }

        /// <summary>
        /// 
        /// </summary>
        bool SupportsNestedTransactions { get; }

        /// <summary>
        /// 
        /// </summary>
        bool SupportsSequences { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbCommand"></param>
        void DeriveParameters(IDbCommand dbCommand);
    }
}
