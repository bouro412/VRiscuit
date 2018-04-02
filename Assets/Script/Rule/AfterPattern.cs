using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRiscuit.Interface;

namespace VRiscuit.Rule {
    public class AfterPattern : IAfterPattern {
        private IVRiscuitObjectSet _objects;
        IVRiscuitObjectSet IAfterPattern.ResultObjectSet { get { return _objects; } }

        public AfterPattern(IVRiscuitObjectSet objects) {
            _objects = objects;
        }
    }
}
