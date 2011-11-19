using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vici.CoolStorage
{
    internal class columnListEntry
    {
        string _tableAlias;
        public string TableAlias
        {
            get
            {
                return _tableAlias;
            }
        }
        string _columnName;
        public string ColumnName
        {
            get
            {
                return _columnName;
            }
        }
        string _fieldAlias;
        public string FieldAlias
        {
            get
            {
                return _fieldAlias;
            }
        }

        public columnListEntry(string tableAlias, string columnName, string fieldAlias)
        {
            _tableAlias = tableAlias;
            _columnName = columnName;
            _fieldAlias = fieldAlias;
        }

        public override string ToString()
        {
            return string.Join(".", new string[] { _tableAlias, _columnName });
        }
    }

    internal class columnList : List<columnListEntry>
    {
        public Dictionary<string, string> AliasMap
        {
            get
            {
                Dictionary<string, string> aliasMap = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                foreach (columnListEntry item in this)
                {
                    aliasMap.Add(item.FieldAlias, item.ColumnName);
                }
                return aliasMap;
            }
        }
        public List<string> asStringList()
        {
            List<string> aList = new List<string>();
            foreach (var item in this)
            {
                aList.Add(item.ToString());
            }
            //var x = aList.ToArray();
            return aList;
        }

        public string[] asArray() { return this.asStringList().ToArray(); }

        public List<string> AliasList
        {
            get
            {
                List<string> aList = new List<string>();
                foreach (columnListEntry item in this)
                {
                    aList.Add(item.FieldAlias);
                }
                return aList;
            }
        }


    }




}
