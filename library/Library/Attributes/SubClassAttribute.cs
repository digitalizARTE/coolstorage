using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vici.CoolStorage
{
    /// <summary>
    /// Declares that this class is a subclass of the class specified.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SubClassAttribute : Attribute
    {
        private Type _superClassType;
        /// <summary>
        /// Gets the type of the super class.
        /// </summary>
        /// <value>The type of the super class.</value>
        public Type SuperClassType
        {
            get
            {
                return _superClassType;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubClassAttribute"/> class.
        /// </summary>
        /// <param name="superclassType">Type of the superclass.</param>
        public SubClassAttribute(Type superclassType)
        {
            _superClassType = superclassType;
        }
    }
}
