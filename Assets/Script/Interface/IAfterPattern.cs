using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRiscuit.Interface {
    interface IAfterPattern {
        IChange[] Changes { get; }
    }
}
