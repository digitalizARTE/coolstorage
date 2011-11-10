using System;

namespace Vici.CoolStorage
{
    // BER: Agregué AttributeTargets.Field para poder usar el mismo atributo
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field) ]
    public sealed class MapToAttribute : Attribute
    {
        private readonly string _name;
        private readonly string _context;

        public MapToAttribute(string name)
        {
            _name = name;
        }

        public MapToAttribute(string name , string context)
            : this(name)
        {
            _context = context;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Context
        {
            get { return _context; }
        }
    }
}