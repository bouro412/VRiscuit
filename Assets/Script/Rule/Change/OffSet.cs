using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VRiscuit.Rule.Change {
    class OffSet {
        private Vector3 _position;
        private Vector3 _angle;

        public OffSet(Vector3 positionOffset, Vector3 angleOffset) {
            _position = positionOffset;
            _angle = angleOffset;
        }

        public  Vector3 correctPosition(Vector3 position) {
            return position + _position;
        }

        public Vector3 correctAngle(Vector3 angle) {
            return angle + _angle;
        }
    }
}
