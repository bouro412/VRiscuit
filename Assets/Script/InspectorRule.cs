using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRiscuit.Interface;
using VRiscuit.Rule;


namespace VRiscuit {
    public class InspectorRule : MonoBehaviour, IRuleSet {

        [Serializable]
        private class SimpleObject {
            public Vector3 Position;
            public Vector3 Rotation;
            public string Type;
        }

        [Serializable]
        private class SimpleRule
        {
            public SimpleObject[] Before;
            public SimpleObject[] After;
        }

        [SerializeField]
        private SimpleRule[] _rules;

        private VRiscuitRule[] _vRules;

        IRule[] IRuleSet.Rules
        {
            get
            {
                return _vRules;
            }
        }

        // Use this for initialization
        void Start() {
            // _rules -> VRules
            if(_rules == null || _rules.Length == 0)
            {
                _vRules = new VRiscuitRule[0];
                return;
            }
            _vRules = _rules.Select(RuleConverter).ToArray();
        }

        private CalculateObject ObjectConverter(SimpleObject obj)
        {
            return new CalculateObject(obj.Position, Quaternion.Euler(obj.Rotation), obj.Type);
        }

        private VRiscuitRule RuleConverter(SimpleRule rule)
        {
            var before = new BeforePattern(new VRiscuitObjectSet(rule.Before.Select(ObjectConverter).ToArray()));
            var after = new AfterPattern(new VRiscuitObjectSet(rule.After.Select(ObjectConverter).ToArray()));
            return new VRiscuitRule(before, after);
        }

        // Update is called once per frame
        void Update() {

        }
    }
}