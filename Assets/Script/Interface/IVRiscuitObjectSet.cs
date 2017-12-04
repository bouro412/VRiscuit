﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRiscuit.Interface {
    interface IVRiscuitObjectSet : IEnumerable<IVRiscuitObject>{
        IVRiscuitObject[] ObjectArray { get; }
        Dictionary<string, List<IVRiscuitObject>> TypeTable { get; }
        void Add(IVRiscuitObject newObject);
        List<IVRiscuitObject> GetByType(string type);
    }
}
