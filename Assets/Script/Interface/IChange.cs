using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRiscuit.Rule.Change;

namespace VRiscuit.Interface {
    interface IChange {
        void UpdateObject(IVRiscuitObjectSet objects, OffSet offset);
    }
}
