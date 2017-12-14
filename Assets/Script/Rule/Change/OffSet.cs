using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VRiscuit.Rule.Change {
    class OffSet {
        private Vector3 _position;
        private Quaternion _angle;

        public OffSet(Vector3 positionOffset, Quaternion angleOffset) {
            _position = positionOffset;
            _angle = angleOffset;
        }

        public  Vector3 correctPosition(Vector3 position) {
            return position + _position;
        }

        public Quaternion correctAngle(Quaternion angle) {
            return Quaternion.Euler(angle.eulerAngles + _angle.eulerAngles);
        }
        public static OffSet Zero = new OffSet(Vector3.zero, Quaternion.identity);
    }
}
