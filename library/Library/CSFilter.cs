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
using System.Linq;
using System.Collections.Generic;

namespace Vici.CoolStorage
{
	[Serializable]
	public sealed class CSFilter
	{
		private static readonly CSFilter _staticFilterNone = new CSFilter();

		private readonly string _expression;
		private readonly CSParameterCollection _parameters;

		public CSFilter()
		{
			_expression = "";
			_parameters = new CSParameterCollection();
		}

		public CSFilter(CSFilter sourceFilter)
		{
			_expression = sourceFilter._expression;
			_parameters = new CSParameterCollection(sourceFilter._parameters);
		}

		public CSFilter(string expression)
		{
			_expression = expression;
			_parameters = new CSParameterCollection();
		}

        public CSFilter(string expression, CSParameterCollection parameters)
        {
            _expression = expression;
            _parameters = new CSParameterCollection(parameters);
        }
        public CSFilter(string expression, object parameters)
        {
            _expression = expression;
            _parameters = new CSParameterCollection(parameters);
        }
        /// <summary>
        /// TODO Completar documentación
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="parameters"></param>
        public CSFilter(string expression, params CSParameter[] parameters)
        {
            _expression = expression;
            _parameters = new CSParameterCollection(parameters);
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="CSFilter"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        public CSFilter(string expression, IEnumerable<CSParameter> parameters)
        {
            _expression = expression;
            _parameters = new CSParameterCollection(parameters.ToArray());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSFilter"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="paramValue">The param value.</param>
        public CSFilter(string expression, string paramName, object paramValue)
        {
            _expression = expression;
            _parameters = new CSParameterCollection(paramName, paramValue);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSFilter"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="paramName1">The param name1.</param>
        /// <param name="paramValue1">The param value1.</param>
        /// <param name="paramName2">The param name2.</param>
        /// <param name="paramValue2">The param value2.</param>
		public CSFilter(string expression, string paramName1, object paramValue1, string paramName2, object paramValue2)
		{
			_expression = expression;
			_parameters = new CSParameterCollection(paramName1, paramValue1, paramName2, paramValue2);
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="CSFilter"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="paramName1">The param name1.</param>
        /// <param name="paramValue1">The param value1.</param>
        /// <param name="paramName2">The param name2.</param>
        /// <param name="paramValue2">The param value2.</param>
        /// <param name="paramName3">The param name3.</param>
        /// <param name="paramValue3">The param value3.</param>
		public CSFilter(string expression, string paramName1, object paramValue1, string paramName2, object paramValue2, string paramName3, object paramValue3)
		{
			_expression = expression;
			_parameters = new CSParameterCollection(paramName1, paramValue1, paramName2, paramValue2, paramName3, paramValue3);
		}

        public enum AndOr
        {
            And,
            Or
        }
		
		public CSFilter(CSFilter filter1, string andOr, CSFilter filter2)
		{
            if (filter1.IsBlank && filter2.IsBlank)
            {
                _expression = "";
                _parameters = new CSParameterCollection();
            }
            else if (filter1.IsBlank)
            {
                _expression = "(" + filter2.Expression + ")";
                _parameters = new CSParameterCollection(filter2.Parameters);
                return;
            }
            else if (filter2.IsBlank)
            {
                _expression = "(" + filter1.Expression + ")";
                _parameters = new CSParameterCollection(filter1.Parameters);
            }
            else
            {
                _expression = "(" + filter1._expression + ") " + andOr + " (" + filter2.Expression + ")";

                _parameters = new CSParameterCollection(filter1.Parameters);
                _parameters.Add(filter2.Parameters);
            }
		}

		public CSFilter(CSFilter filter1, AndOr andOr, CSFilter filter2)
		{
            if (filter1.IsBlank && filter2.IsBlank)
            {
                _expression = "";
                _parameters = new CSParameterCollection();
            }
            else if (filter1.IsBlank)
            {
                _expression = "(" + filter2.Expression + ")";
                _parameters = new CSParameterCollection(filter2.Parameters);
                return;
            }
            else if (filter2.IsBlank)
            {
                _expression = "(" + filter1.Expression + ")";
                _parameters = new CSParameterCollection(filter1.Parameters);
            }
            else
            {
                _expression = "(" + filter1._expression + ") " + andOr.ToString() + " (" + filter2.Expression + ")";

                _parameters = new CSParameterCollection(filter1.Parameters);
                _parameters.Add(filter2.Parameters);
            }
		}
        /// <summary>
        /// Gets the none.
        /// </summary>
		public static CSFilter None
		{
			get
			{
				return _staticFilterNone;
			}
		}

		internal CSParameterCollection Parameters
		{
			get
			{
				return _parameters;
			}
		}

		internal string Expression
		{
			get
			{
				return _expression;
			}
		}

        /// <summary>
        /// Gets a value indicating whether this instance is blank.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is blank; otherwise, <c>false</c>.
        /// </value>
		public bool IsBlank
		{
			get
			{
				return _expression.Trim().Length < 1;
			}
		}

        /// <summary>
        /// Or's the specified filter.
        /// </summary>
        /// <param name="filterOr">The filter to be or'ed with this filter.</param>
        /// <returns>A new filter.</returns>
		public CSFilter Or(CSFilter filterOr)
		{
			return new CSFilter(this, "OR", filterOr);
		}

        /// <summary>
        /// Ors the specified filter expression with this.
        /// </summary>
        /// <param name="filterExpression">The filter expression.</param>
        /// <returns>A new CSFilter.</returns>
        public CSFilter Or(string expression)
        {
            return new CSFilter(this, "OR", new CSFilter(expression));
        }

        /// <summary>
        /// Creates a new CSFilter using the filter expession and parm and or's it with this CSFilter.
        /// </summary>
        /// <param name="filterExpression">The filter expression.</param>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns>A new CSFilter.</returns>
		public CSFilter Or(string expression, string paramName , object paramValue)
		{
			return new CSFilter(this, "OR", new CSFilter(expression, paramName, paramValue));
		}

        /// <summary>
        /// Creates a new CSFilter using the filter expession and parms and or's it with this CSFilter.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters collection.</param>
        /// <returns>A new CSFilter.</returns>
		public CSFilter Or(string expression, CSParameterCollection parameters)
		{
			return new CSFilter(this, "OR", new CSFilter(expression, parameters));
		}

        public CSFilter Or(string expression, object parameters)
        {
            return new CSFilter(this, "OR", new CSFilter(expression, parameters));
        }
		 /// <summary>
        /// Ands the specified filter with this CSFilter
        /// </summary>
        /// <param name="filterOr">The CSfilter to be And'ed</param>
        /// <returns>A new CSFilter</returns>
		public CSFilter And(CSFilter filterOr)
		{
			return new CSFilter(this, "AND", filterOr);
		}

        /// <summary>
        /// Ands this CSFilter with a new CSFilter created from a filter expression.
        /// </summary>
        /// <param name="expression">The filter expression.</param>
        /// <returns>A new CSFilter</returns>
        public CSFilter And(string expression)
        {
            return new CSFilter(this, "AND", new CSFilter(expression));
        }

        /// <summary>
        /// Ands this CSFilter with a new CSFilter created from the filter expression and parms.
        /// </summary>
        /// <param name="expression">The filter expression.</param>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns>A new CSFilter</returns>
		public CSFilter And(string expression, string paramName, object paramValue)
		{
			return new CSFilter(this, "AND", new CSFilter(expression, paramName, paramValue));
		}

        /// <summary>
        /// Ands this CSFilter with a new CSFilter created from the filter expression and parms.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameter collection.</param>
        /// <returns>A new CSFilter</returns>
		public CSFilter And(string expression, CSParameterCollection parameters)
		{
			return new CSFilter(this, "AND", new CSFilter(expression, parameters));
		}

        public CSFilter And(string expression, object parameters)
        {
            return new CSFilter(this, "AND", new CSFilter(expression, parameters));
        }

/// <summary>
        /// TODO Completar documentación
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramValueOrig"></param>
        /// <param name="paramValueDest"></param>
        /// <returns></returns>
        public CSFilter AndBetWeen(string paramName, object paramValueOrig, object paramValueDest)
        {
            return AndBetWeen(paramName, paramValueOrig, paramValueDest, false);
        }

        /// <summary>
        /// TODO Completar documentación
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramValueOrig"></param>
        /// <param name="paramValueDest"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        public CSFilter AndBetWeen(string paramName, object paramValueOrig, object paramValueDest, bool strict)
        {
            string expOrig = String.Format("{0}>{1}@{0}", paramName, (strict) ? "" : "=");
            string expDest = String.Format("{0}<{1}@{0}", paramName, (strict) ? "" : "=");
            CSFilter filter = new CSFilter(expOrig, String.Format("@{0}", paramName), paramValueOrig);
            filter = filter.And(expDest, String.Format("@{0}", paramName), paramValueDest);
            return new CSFilter(this, "AND", filter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramValueOrig"></param>
        /// <param name="paramValueDest"></param>
        /// <returns></returns>
        public CSFilter OrBetWeen(string paramName, object paramValueOrig, object paramValueDest)
        {
            return OrBetWeen(paramName, paramValueOrig, paramValueDest, false);
        }

        /// <summary>
        /// TODO Completar documentación
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramValueOrig"></param>
        /// <param name="paramValueDest"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        public CSFilter OrBetWeen(string paramName, object paramValueOrig, object paramValueDest, bool strict)
        {
            string expOrig = String.Format("{0}>{1}@{0}", paramName, (strict) ? "" : "=");
            string expDest = String.Format("{0}<{1}@{0}", paramName, (strict) ? "" : "=");
            CSFilter filter = new CSFilter(expOrig, String.Format("@{0}", paramName), paramValueOrig);
            filter = filter.And(expDest, String.Format("@{0}", paramName), paramValueDest);
            return new CSFilter(this, "OR", filter);
        }

        /// <summary>
        /// TODO Completar documentación
        /// </summary>
        /// <param name="filter1"></param>
        /// <param name="filter2"></param>
        /// <returns></returns>
        public static CSFilter operator |(CSFilter filter1, CSFilter filter2)
        {
            return new CSFilter(filter1, "OR", filter2);
        }

        /// <summary>
        /// Implements the (And) operator &amp;.
        /// </summary>
        /// <param name="filter1">The filter1.</param>
        /// <param name="filter2">The filter2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
		public static CSFilter operator&(CSFilter filter1, CSFilter filter2)
		{
			return new CSFilter(filter1, "AND", filter2);
		}

        /// <summary>
        /// Implements the (And) operator &amp;.
        /// </summary>
        /// <param name="filter1">The filter1.</param>
        /// <param name="filter2">The filter2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
		public static CSFilter operator &(CSFilter filter1, string filter2)
		{
			return new CSFilter(filter1, "AND", new CSFilter(filter2));
		}

        /// <summary>
        /// Implements the (or) operator |.
        /// </summary>
        /// <param name="filter1">The filter1.</param>
        /// <param name="filter2">The filter2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
		public static CSFilter operator |(CSFilter filter1, string filter2)
		{
			return new CSFilter(filter1, "OR", new CSFilter(filter2));
		}
	}
}
