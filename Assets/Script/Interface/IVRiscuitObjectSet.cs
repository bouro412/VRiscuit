﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRiscuit.Interface {
    public interface IVRiscuitObjectSet : IEnumerable<IVRiscuitObject>{
        IVRiscuitObject[] ObjectArray { get; }
        Dictionary<string, List<IVRiscuitObject>> TypeTable { get; }
        int Add(IVRiscuitObject newObject);
        void Delete(int index);
        void Delete(IVRiscuitObject obj);
        List<IVRiscuitObject> GetByType(string type);
        void SetParameter(float[] parameter);
        int Size { get; }
        IVRiscuitObject this[int i] { get; set; }
        Dictionary<string, int> DistributionTable { get; }
    }
}
