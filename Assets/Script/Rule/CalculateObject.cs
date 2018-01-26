using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRiscuit.Interface;

namespace VRiscuit.Rule {
    /// <summary>
    /// 数値計算用オブジェクト。必要な値のみ保持しUnityオブジェクトには影響を与えない
    /// </summary>
    class CalculateObject : IVRiscuitObject {
        Vector3 _position;
        Vector3 IVRiscuitObject.Position
        {
            get
            {
                return _position;
            }

            set
            {
                _position = value;
            }
        }

        Quaternion _rotation;
        Quaternion IVRiscuitObject.Rotation
        {
            get
            {
                return _rotation;
            }

            set
            {
                _rotation = value;
            }
        }

        string _type;
        string IVRiscuitObject.Type
        {
            get
            {
                return _type;
            }
        }
        public CalculateObject(Vector3 position, Quaternion rotate, string type) {
            _position = position;
            _rotation = rotate;
            _type = type;
        }
        public CalculateObject(IVRiscuitObject obj) {
            _position = obj.Position;
            _rotation = obj.Rotation;
            _type = obj.Type;
        }
    }
}
