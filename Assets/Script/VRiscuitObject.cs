using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRiscuit.Interface;


namespace VRiscuit {
    class VRiscuitObject : MonoBehaviour, IVRiscuitObject {
        private bool _isDeleted = false;

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

        void IVRiscuitObject.Delete() {
            _isDeleted = true;
        }


        [SerializeField]
        private string _objectType;
        void Move(Vector3 vector) {
            transform.Translate(vector);
        }

        public VRiscuitObject(Vector3 position, Quaternion angle, string type) {
            transform.position = position;
            transform.rotation = angle;
            _objectType = type;
        }

        public void Update()
        {
            if(_isDeleted == true)
            {
                DestroyImmediate(gameObject);
            }
        }
    }
}
