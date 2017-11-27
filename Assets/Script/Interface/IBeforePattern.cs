using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRiscuit.Interface {
    interface IBeforePattern {
        IVRiscuitObject[] VRiscuitObjects { get; }
    }
}