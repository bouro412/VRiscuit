using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRiscuit.Interface;
using UnityEngine;

namespace VRiscuit.Rule.Change {
    class Move : IChange , IDescentable{
        private int _index;
        private Vector3 _moveVector;

        void IChange.UpdateObject(IVRiscuitObjectSet objects, OffSet offset) {
            var target = objects.ObjectArray[_index];
        }
    }
}
