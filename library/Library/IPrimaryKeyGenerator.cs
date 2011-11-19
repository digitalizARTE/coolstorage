using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vici.CoolStorage
{
    public interface IPrimaryKeyGenerator
    {
        object NextPrimaryKey();
    }
}
