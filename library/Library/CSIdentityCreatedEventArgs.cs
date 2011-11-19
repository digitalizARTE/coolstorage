using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vici.CoolStorage
{
    /// <summary>
    /// Raised when a new object with a server generated identity is created.
    /// </summary>
    public class CSIdentityCreatedEventArgs : EventArgs
    {
        public object IdentityValue { get; private set; }
        private CSIdentityCreatedEventArgs()
        {
        }
        public CSIdentityCreatedEventArgs(object value)
        {
            IdentityValue = value;
        }
    }
}
