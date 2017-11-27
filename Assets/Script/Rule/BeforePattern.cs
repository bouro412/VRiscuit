using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRiscuit.Interface;

namespace VRiscuit.Script.Rule {
    class BeforePattern : IBeforePattern {
        IVRiscuitObject[] IBeforePattern.VRiscuitObjects
        {
            get
            {
                return _vriscuitObjects;
            }
        }
        private IVRiscuitObject[] _vriscuitObjects;

        public BeforePattern(IVRiscuitObject[] objects) {
            _vriscuitObjects = objects;
        }
    }
}
