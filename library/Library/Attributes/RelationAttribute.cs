using System;
using System.Collections.Generic;

namespace Vici.CoolStorage
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class RelationAttribute : Attribute
    {
        private string[] sep = new string[] { "," };

        //TODO DAE 2010-09-30 para las claves multiples siguinestes atributos pueden ser listas. 
		
		public bool Optional { get; set; }
		

        /// <summary>
        /// 
        /// </summary>
        public string LocalKey { get; set; }

        private List<string> _localKeys;
        
        /// <summary>
        /// 
        /// </summary>
        public List<string> LocalKeys
        {
            get
            {
                if (_localKeys == null)
                {
                    Array.ForEach<string>(LocalKey.Split(sep, StringSplitOptions.RemoveEmptyEntries), (_localKeys = new List<string>()).Add);
                }
                return _localKeys;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ForeignKey { get; set; }

        private List<string> _foreignKeys;

        /// <summary>
        /// 
        /// </summary>
        public List<string> ForeignKeys
        {
            get
            {
                if (_foreignKeys == null)
                {
                    Array.ForEach<string>(ForeignKey.Split(sep, StringSplitOptions.RemoveEmptyEntries), (_foreignKeys = new List<string>()).Add);
                }
                return _foreignKeys;
            }
        }

        /// <summary>
        /// Indica si la relación tiene una clave compuesta. 
        /// </summary>
        public bool HasCompositeKey
        {
            get
            {
                int cntLocalKeys = LocalKeys.Count;
                int cntForeignKeys = ForeignKeys.Count;
                return cntLocalKeys == cntForeignKeys && cntLocalKeys > 1;
            }
        }
    }
}