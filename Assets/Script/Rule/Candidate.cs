using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRiscuit;
using VRiscuit.Interface;
using UnityEngine;

namespace VRiscuit.Rule {
    class Candidate {
        private IRule _rule;
        private IVRiscuitObjectSet _table;
        private RuleManager.ScoreCoefficient _scoreCoefficient;
        public float Score { get; private set; }

        public Candidate (IRule rule, IVRiscuitObjectSet table){
            _rule = rule;
            _table = table;
            _scoreCoefficient = new RuleManager.ScoreCoefficient();
            Score = RuleManager.Instance.CalcScore(_rule.BeforeObjectSet, _table, _scoreCoefficient);
        }

        

        public void RuleApply() {
            _rule.Apply(_table);
        }

        public bool HasSameObject(Candidate another) {
            var objs1 = _table.ObjectArray;
            var objs2 = another._table.ObjectArray;
            return objs1.Intersect(objs2).Count() != 0;
        }
    }
}
