using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vici.CoolStorage
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute
    {
    }
}
