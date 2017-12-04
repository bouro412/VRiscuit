using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRiscuit.Interface;

namespace VRiscuit.Script.Rule {
    class BeforePattern : IBeforePattern {
        IVRiscuitObjectSet IBeforePattern.VRiscuitObjects
        {
            get
            {
                return _vriscuitObjects;
            }
        }
        private IVRiscuitObjectSet _vriscuitObjects;

        public BeforePattern(IVRiscuitObjectSet objects) {
            _vriscuitObjects = objects;
        }
    }
}
