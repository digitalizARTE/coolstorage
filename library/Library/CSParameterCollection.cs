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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Data;

namespace Vici.CoolStorage
{
	[Serializable]
	public class CSParameterCollection : IEnumerable<CSParameter>
	{
		public static CSParameterCollection Empty = new CSParameterCollection();

		private readonly List<CSParameter> _parameterList = new List<CSParameter>();

		[NonSerialized]
		private Dictionary<string, CSParameter> _parameterMap = new Dictionary<string, CSParameter>();

		[OnDeserialized]
		private void AfterDeserialization(StreamingContext context)
		{
			_parameterMap = new Dictionary<string, CSParameter>();

			foreach (CSParameter parameter in _parameterList)
				//_parameterMap.Add(parameter.Name, parameter);
				_parameterMap.Add(CSConfig.MappingStrategy.ParamToInternal(parameter.Name), parameter);
		}

        #region Constructor
		public CSParameterCollection()
		{
		}

        public CSParameterCollection(object o)
        {
            var members = o.GetType().GetMembers(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance);

            foreach (var member in members)
            {
                object value;

                if (member is FieldInfo)
                    value = ((FieldInfo)member).GetValue(o);
                else if (member is PropertyInfo)
                    value = ((PropertyInfo)member).GetValue(o, null);
                else
                    continue;

                Add('@' + member.Name, value);
            }
        }

		public CSParameterCollection(string paramName,object paramValue)
		{
			Add(paramName, paramValue);
		}

        public CSParameterCollection(string paramName1, object paramValue1,string paramName2,object paramValue2)
        {
            Add(paramName1, paramValue1);
            Add(paramName2, paramValue2);
        }

        public CSParameterCollection(string paramName1, object paramValue1, string paramName2, object paramValue2, string paramName3, object paramValue3)
        {
            Add(paramName1, paramValue1);
            Add(paramName2, paramValue2);
            Add(paramName3, paramValue3);
        }

        public CSParameterCollection(string paramName1, object paramValue1, string paramName2, object paramValue2, string paramName3, object paramValue3 ,params object[] otherParameters)
        {
            Add(paramName1, paramValue1);
            Add(paramName2, paramValue2);
            Add(paramName3, paramValue3);

            if ((otherParameters.Length % 2) != 0)
                throw new CSException("Bad parameter list !");

            for(int i=0; i < otherParameters.Length ; i+=2)
            {
                if (otherParameters[i] == null)
                    throw new CSException("Parameter name cannot be null");

                if (!(otherParameters[i] is string))
                    throw new CSException("Parameter name should be a string");

                Add((string) otherParameters[i], otherParameters[i+1]);
            }
        }

		public CSParameterCollection(CSParameterCollection sourceCollection)
		{
		    Add(sourceCollection);
        }

        public CSParameterCollection(params CSParameter[] parameters)
        {
			foreach (CSParameter param in parameters)
			{
				_parameterList.Add(param);
				//_parameterMap.Add(param.Name, param);
				_parameterMap.Add(CSConfig.MappingStrategy.ParamToInternal(param.Name), param);
			}
        }
        #endregion Constructor

		public CSParameter Add()
		{
		    return Add('@' + CSNameGenerator.NextParameterName);
		}

        public CSParameter Add(string parameterName)
        {
            CSParameter param = new CSParameter(parameterName);

        	//_parameterMap.Add(param.Name, param);
			_parameterMap.Add(CSConfig.MappingStrategy.ParamToInternal(param.Name), param);
			_parameterList.Add(param);

            return param;
        }

        public void Add(string paramName,object paramValue)
        {
            CSParameter param = new CSParameter(paramName,paramValue);

            //_parameterMap.Add(param.Name, param);
			_parameterMap.Add(CSConfig.MappingStrategy.ParamToInternal(param.Name), param);
            _parameterList.Add(param);
        }

        public void Add(CSParameter parameter)
        {
            Add(parameter.Name, parameter.Value);
            CSParameter newParam = this[parameter.Name];
            newParam.Direction = parameter.Direction;
            newParam.Precision = parameter.Precision;
            newParam.Scale = parameter.Scale;
            newParam.Size = parameter.Size;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public void Add(CSParameterCollection parameters)
        {
            CSParameter newParam;
            if (parameters != null)
            {
                foreach (CSParameter parameter in parameters)
                {
                    Add(parameter.Name, parameter.Value);
                    newParam = this[parameter.Name];
                    newParam.Direction = parameter.Direction;
                    newParam.Precision = parameter.Precision;
                    newParam.Scale = parameter.Scale;
                    newParam.Size = parameter.Size;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public CSParameter AddOutPut(string parameterName)
        {
            CSParameter param = new CSParameter(parameterName)
            {
                Direction = ParameterDirection.Output
            };

            //_parameterMap.Add(param.Name, param);
            _parameterMap.Add(CSConfig.MappingStrategy.ParamToInternal(param.Name), param);
            _parameterList.Add(param);

            return param;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        public void AddOutPut(string paramName, object paramValue)
        {
            CSParameter param = new CSParameter(paramName, paramValue)
            {
                Direction = ParameterDirection.Output
            };
            //_parameterMap.Add(param.Name, param);
            _parameterMap.Add(CSConfig.MappingStrategy.ParamToInternal(param.Name), param);
            _parameterList.Add(param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        public void AddOutPut(CSParameter parameter)
        {
            AddOutPut(parameter.Name, parameter.Value);
            CSParameter newParam = this[parameter.Name];
            newParam.Direction = parameter.Direction;
            newParam.Precision = parameter.Precision;
            newParam.Scale = parameter.Scale;
            newParam.Size = parameter.Size;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public void AddOutPut(CSParameterCollection parameters)
        {
            CSParameter newParam;
            if (parameters != null)
            {
                foreach (CSParameter parameter in parameters)
                {
                    AddOutPut(parameter.Name, parameter.Value);
                    newParam = this[parameter.Name];
                    newParam.Direction = parameter.Direction;
                    newParam.Precision = parameter.Precision;
                    newParam.Scale = parameter.Scale;
                    newParam.Size = parameter.Size;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CSParameter this[string name]
        {
            get
            {
                CSParameter parameter;
                //_parameterMap.TryGetValue(name, out parameter);
                _parameterMap.TryGetValue(CSConfig.MappingStrategy.ParamToInternal(name), out parameter);
                return parameter;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                return _parameterList.Count;
            }       
	    }

		public bool IsEmpty
		{
			get
			{
				return (_parameterList.Count == 0);
			}
		}

		IEnumerator<CSParameter> IEnumerable<CSParameter>.GetEnumerator()
		{
			return _parameterList.GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			return _parameterList.GetEnumerator();
		}
	}
}
