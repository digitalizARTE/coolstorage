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
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;

namespace Vici.CoolStorage
{
    public static partial class CSConfig
    {
        internal const string DEFAULT_CONTEXTNAME = "_DEFAULT_";

        private static bool? _useTransactionScope;
        private static int? _commandTimeout;
        private static bool _doLogging;
        private static string _logFileName;
        private static string _mappingStrategyName = "Vici.CoolStorage.CSMappingStrategyBase";
        private static ICSMappingStrategy _mappingStrategy;

        /// <summary>
        /// 
        /// </summary>
        public static List<string> Decorators { get; private set; }

        static CSConfig()
        {
            ReadConfig();
        }

        private static void ReadConfig()
        {
#if !MONOTOUCH && !WINDOWS_PHONE && !SILVERLIGHT
            NameValueCollection configurationSection = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection("CoolStorage");

            if (configurationSection == null)
                return;

            if (configurationSection["UseTransactionScope"] != null)
                _useTransactionScope = (configurationSection["UseTransactionScope"].ToUpper() == "TRUE");

            int commandTimeout;

            if (configurationSection["CommandTimeout"] != null && int.TryParse(configurationSection["CommandTimeout"], out commandTimeout))
                _commandTimeout = commandTimeout;

            if (configurationSection["Logging"] != null)
                _doLogging = (configurationSection["Logging"].ToUpper() == "TRUE");

            if (configurationSection["LogFile"] != null)
            
				_logFileName = configurationSection["LogFile"];
            

            if (configurationSection["mappingStrategyName"] != null)
            {
                _mappingStrategyName = configurationSection["mappingStrategyName"];
            }
            else
            {
                _mappingStrategyName = "Vici.CoolStorage.MappingStrategyBase";
            }

            int bufferLogLenght;
            if (configurationSection["BufferLogLenght"] != null && int.TryParse(configurationSection["BufferLogLenght"], out bufferLogLenght))
                BufferLogLenght = bufferLogLenght;
            else
                BufferLogLenght = 50;

            int i = 1;
            string value;
            if (Decorators == null)
                Decorators = new List<string>();
            while ((value = configurationSection[String.Format("Decorator_{0}", i)]) != null)
            {
                //Type type = Type.GetType(value, true, true);
                //[ber - 20101110] - Permito cargar desde otro assembly con sintaxis typename@assembly
                CSTypeLoader.LoadType(value);
                Decorators.Add(value);
                i++;
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public static ICSMappingStrategy MappingStrategy
        {
            get
            {
                if (_mappingStrategy == null)
                {
                    Type type = Type.GetType(_mappingStrategyName, false, true);
                    _mappingStrategy = (ICSMappingStrategy)Activator.CreateInstance(type);
                }
                return _mappingStrategy;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool UseTransactionScope
        {
            get
            {
                return _useTransactionScope ?? false;
            }
            set
            {
                _useTransactionScope = value;
            }
        }

        public static int? CommandTimeout
        {
            get
            {
                return _commandTimeout;
            }
            set
            {
                _commandTimeout = value;
            }
        }

        public static bool Logging
        {
            get { return _doLogging; }
            set { _doLogging = value; }
        }
		/// <summary>
        /// 
        /// </summary>
        public static int BufferLogLenght
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public static string LogFileName
        {
            get
            {
                string logFileName = _logFileName;
                if (logFileName.Contains("|"))
                {
                    logFileName = logFileName.Replace("|CD|", Environment.CurrentDirectory)
                                                .Replace("|FREQY|", "{Y}")
                                                    .Replace("|FREQM|", "{Y}{M}")
                                                        .Replace("|FREQD|", "{Y}{M}{D}")
                                                            .Replace("|FREQH|", "{Y}{M}{D}{H}")
                                                            .Replace("{Y}", String.Format("{0:yyyy}", DateTime.Now))
                                                        .Replace("{M}", String.Format("{0:MM}", DateTime.Now))
                                                    .Replace("{D}", String.Format("{0:dd}", DateTime.Now))
                                                .Replace("{H}", String.Format("{0:HH}", DateTime.Now));
                }
                return logFileName;
            }
            set
            {
                if (_logFileName == value)
                    return;
                _logFileName = value;
            }
        }

        internal static readonly Dictionary<string, string> ColumnMappingOverrideMap = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

        private static readonly Dictionary<string, CSDataProvider> _globalDbMap = new Dictionary<string, CSDataProvider>(StringComparer.CurrentCultureIgnoreCase);
        private static readonly Dictionary<string, bool> _globalDbMapChanged = new Dictionary<string, bool>(StringComparer.CurrentCultureIgnoreCase);

        [ThreadStatic]
        private static ThreadData _threadData;


        /// <summary>
        /// Determines whether a database connection has been specified.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance has DB; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasDB()
        {
            return HasDB(DEFAULT_CONTEXTNAME);
        }

        /// <summary>
        /// Determines whether the database connection for the given context has been specified.
        /// </summary>
        /// <param name="contextName">Name of the context.</param>
        /// <returns>
        /// 	<c>true</c> if the specified context name has DB; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasDB(string contextName)
        {
            lock (_globalDbMap)
            {
                return _globalDbMap.ContainsKey(contextName);
            }
        }

        internal static CSDataProvider GetDB(string strContext)
        {
            if (_threadData == null)
                _threadData = new ThreadData(Decorators);

            return _threadData.GetDB(strContext);
        }

        public static void SetDB(CSDataProvider db)
        {
            SetDB(db, DEFAULT_CONTEXTNAME);
        }

        public static void SetDB(CSDataProvider db, string contextName)
        {
            lock (_globalDbMap)
            {
                _globalDbMap[contextName] = db;
                _globalDbMapChanged[contextName] = true;
            }
        }
		
        public static void ChangeTableMapping(Type type, string tableName, string contextName)
        {
            CSSchema.ChangeMapTo(type, tableName, contextName);
        }

        public static void ChangeColumnMapping(Type type, string propertyName, string columnName)
        {
            lock (ColumnMappingOverrideMap)
            {
                PropertyInfo propInfo = type.GetProperty(propertyName);

                if (propInfo == null)
                    throw new CSException("ChangeColumnMapping() : Property [" + propertyName + "] undefined");

                ColumnMappingOverrideMap[propInfo.DeclaringType.Name + ":" + propInfo.Name] = columnName;
            }
        }


        private class ThreadData
        {
            private readonly Thread _callingThread;
            private readonly Dictionary<string, CSDataProvider> _threadDbMap = new Dictionary<string, CSDataProvider>(StringComparer.CurrentCultureIgnoreCase);

            private List<string> Decorators { get; set; }

            #region Constructor
            /// <summary>
            /// 
            /// </summary>
            internal ThreadData()
            {
                _callingThread = Thread.CurrentThread;

                Thread cleanupThread = new Thread(CleanupBehind);

                cleanupThread.IsBackground = true;

                cleanupThread.Start();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="decorators"></param>
            internal ThreadData(List<string> decorators)
                : this()
            {
                Decorators = decorators;
            }
            #endregion Constructor

            /// <summary>
            /// 
            /// </summary>
            /// <param name="provider"></param>
            /// <returns></returns>
            internal CSDataProvider ApplyDecorators(CSDataProvider provider)
            {
                CSDataProvider prov = provider;
                if (Decorators != null)
                {
                    Decorators.ForEach(decorator =>
                    {
                        // [ber] 20101110 - Permito cargar desde cualquier assembly
                        //Type type = Type.GetType(decorator, false, true);
                        Type type = CSTypeLoader.LoadType(decorator);
                        prov = (CSDataProvider)Activator.CreateInstance(type, prov);
                    });
                }
                return prov;
            }

            internal CSDataProvider GetDB(string contextName)
            //internal ICSDataProvider GetDB(string contextName)
            {
                lock (_globalDbMap)
                {
                    if (_globalDbMapChanged.ContainsKey(contextName) && _globalDbMapChanged[contextName])
                    {
                        _globalDbMapChanged[contextName] = false;

                        if (_threadDbMap.ContainsKey(contextName))
                        {
                            _threadDbMap[contextName].Dispose();
                            _threadDbMap.Remove(contextName);
                        }
                    }
                }

                if (_threadDbMap.ContainsKey(contextName))
                    return _threadDbMap[contextName];

                lock (_globalDbMap)
                {
                    if (!_globalDbMap.ContainsKey(contextName))
                    {
#if !MONOTOUCH && !WINDOWS_PHONE && !SILVERLIGHT
                        NameValueCollection configurationSection = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection("CoolStorage");

                        if (configurationSection != null)
                        {
                            string key = (contextName == DEFAULT_CONTEXTNAME) ? "Connection" : ("Connection." + contextName);

                            string value = configurationSection[key];
							if (value == null)
							{
								throw new InvalidOperationException("CSConfig: Context has no entry in ConfigurationSection");
							}

                            if (value.IndexOf('/') > 0)
                            {
                                string dbType = value.Substring(0, value.IndexOf('/')).Trim();
                                string connString = value.Substring(value.IndexOf('/') + 1).Trim();

                                string typeName = "Vici.CoolStorage." + dbType;

                                Type type = Type.GetType(typeName);

                                if (type == null)
                                    throw new CSException("Unable to load type <" + typeName + ">");

                                _globalDbMap[contextName] = (CSDataProvider)Activator.CreateInstance(type, new object[] { connString });
                            }
                        }
#endif

                        if (!_globalDbMap.ContainsKey(contextName))
                            throw new CSException("GetDB(): context [" + contextName + "] not found");
                    }
                }

                CSDataProvider db = _globalDbMap[contextName];

                //db = db.Clone();
                db = ApplyDecorators(db.Clone());

                _threadDbMap[contextName] = db;

                return db;
            }

            private void CleanupBehind()
            {
                _callingThread.Join();

                foreach (CSDataProvider db in _threadDbMap.Values)
                    db.Dispose();
            }
        }

    }
}
