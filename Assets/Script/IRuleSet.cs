using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRiscuit.Interface;

namespace VRiscuit
{
    interface IRuleSet
    {
        IRule[] Rules { get; }
    }
}
