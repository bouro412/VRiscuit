﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRiscuit;
using VRiscuit.Interface;
using UnityEngine;

namespace VRiscuit.Rule {
    public class Candidate {
        private IRule _rule;
        private IVRiscuitObjectSet _table;
        public float Score { get; private set; }
        public float NotNormalizedScore { get; private set; }
        
        public Candidate (IRule rule, IVRiscuitObjectSet table, float[] scores){
            _rule = rule;
            _table = table;
            // オブジェクト個数で優遇する
            Score = RuleManager.NormalizeScore(scores) * _table.Size;
            NotNormalizedScore = scores[0];
        }

        public Candidate(IRule rule, IVRiscuitObjectSet table, float score)
        {
            _rule = rule;
            _table = table;
            Score = score;
            NotNormalizedScore = score;
        }

        public void RuleApply(IVRiscuitObjectSet globalTable) {
            _rule.Apply(_table, globalTable);
        }

        public bool HasSameObject(Candidate another) {
            var objs1 = _table.ObjectArray;
            var objs2 = another._table.ObjectArray;
            return objs1.Intersect(objs2).Count() != 0;
        }
    }
}
