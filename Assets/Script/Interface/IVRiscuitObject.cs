using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VRiscuit.Interface {
    interface IVRiscuitObject {
        string Type { get; }
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
    }
}
