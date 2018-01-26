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
            set
            {
                transform.position = value;
            }
        }

        Quaternion IVRiscuitObject.Rotation
        {
            get
            {
                return transform.rotation;
            }
            set
            {
                transform.rotation = value;
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
        void Move(Vector3 vector) {
            transform.Translate(vector);
        }
    }
}
