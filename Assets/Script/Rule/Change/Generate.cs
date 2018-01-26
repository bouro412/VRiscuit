using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRiscuit.Interface;

namespace VRiscuit.Rule.Change {
    class Generate : IChange , IDescentable{
        private string _type;
        private Vector3 _position;
        private Quaternion _angle;

        public Generate(string type, Vector3 position, Quaternion angle) {
            _type = type;
            _position = position;
            _angle = angle;
        }

        void IChange.UpdateObject(IVRiscuitObjectSet objects, OffSet offset) {
            RuleManager.Instance.GenerateObject(_type, offset.correctPosition(_position), 
                offset.correctAngle(_angle));
        }    
        
    }
}
