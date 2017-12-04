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
        private ScoreCoefficient _scoreCoefficient;
        public float Score { get; private set; }

        public Candidate (IRule rule, IVRiscuitObjectSet table){
            _rule = rule;
            _table = table;
            _scoreCoefficient = new ScoreCoefficient();
            Score = CalcScore(_scoreCoefficient);
        }

        #region scoreの計算
        private float CalcScore(ScoreCoefficient ef) {
            var score = 0.0f;
            var fieldObjectList = _table.ObjectArray;
            var ruleObjectList = _rule.ObjectSet.ObjectArray;
            var len = fieldObjectList.Length;
            for(int i = 0; i < len - 1; i++) {
                for(int j = i + 1; j < len; j++) {
                    score += CalcTwoObjectSimilarity(ruleObjectList[i], ruleObjectList[j],
                        fieldObjectList[i], fieldObjectList[j], ef);
                }
            }
            return score;
        }

        private class ScoreCoefficient {
            public float c0;
            public float c1;
            public float c2;
            public float c3;
            public float c4;
            public float c5;
            public float c6;
            public float w0;
            public float w1;
            public float w2;
            public float w3;
            public float w4;
        }

        private float CalcTwoObjectSimilarity(IVRiscuitObject a, IVRiscuitObject b, IVRiscuitObject x, IVRiscuitObject y, ScoreCoefficient ef) {
            var score = 0.0f;
            score += ef.c0 * delta(norm(a, b), norm(x, y), ef.w0);
            score += ef.c1 * eps(a, b, x, y, ef) * delta(rdir(a, b), rdir(x, y), ef.w1);
            score += ef.c2 * eps(a, b, x, y, ef) * delta(rdir(b, a), rdir(y, x), ef.w2);
            score += ef.c3 * delta(angle(a, b), angle(x, y), ef.w3);
            return score;
        }

        /// <summary>
        /// e^(-(x-y)^2 / w)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        private float delta(float x, float y, float weight) {
            return (float)Math.Exp(-(Math.Pow(x - y, 2) / weight));
        }

        private float norm(IVRiscuitObject x, IVRiscuitObject y) {
            return (x.Position - y.Position).magnitude;
        }

        private float rdir(IVRiscuitObject x, IVRiscuitObject y) {
            return Quaternion.Angle(x.Rotation, Quaternion.FromToRotation(x.Position, y.Position));
        }

        private float angle(IVRiscuitObject x, IVRiscuitObject y) {
            return Quaternion.Angle(x.Rotation, y.Rotation);
        }

        private float eps(IVRiscuitObject a, IVRiscuitObject b, IVRiscuitObject x, IVRiscuitObject y, ScoreCoefficient ef) {
            return (float)(1 - Math.Exp(-ef.c5 / Math.Pow(norm(a, b) + norm(x, y) + ef.c6, 2)));
        }

        #endregion

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
