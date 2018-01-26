using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRiscuit.Interface;

namespace VRiscuit.Rule.Change {
    class Delete : IChange {
        public int Index { get; private set; }
        void IChange.UpdateObject(IVRiscuitObjectSet objects, OffSet offset) {
            
        }
    }
}
