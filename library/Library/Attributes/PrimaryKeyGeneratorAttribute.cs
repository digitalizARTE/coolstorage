using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vici.CoolStorage 
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyGeneratorAttribute : Attribute
    {
        Type _GeneratorType;
        public Type GeneratorType
        {
            get
            {
                return _GeneratorType;
            }
        }

        public PrimaryKeyGeneratorAttribute(Type generatorType)
        {
            _GeneratorType = generatorType;
        }
    }
}
