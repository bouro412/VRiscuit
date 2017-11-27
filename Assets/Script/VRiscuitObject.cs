using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRiscuit.Interface;


namespace VRiscuit {
    class VRiscuitObject : MonoBehaviour, IVRiscuitObject {
        Vector3 IVRiscuitObject.Position
        {
            get
            {
                return transform.position;
            }
        }

        Quaternion IVRiscuitObject.Rotation
        {
            get
            {
                return transform.rotation;
            }
        }

        string IVRiscuitObject.Type
        {
            get
            {
                return _objectType;
            }
        }
        [SerializeField]
        private string _objectType;
    }
}
