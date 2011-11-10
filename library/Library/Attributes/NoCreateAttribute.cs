using System;

namespace Vici.CoolStorage
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class NoCreateAttribute : Attribute
    {
    }
}