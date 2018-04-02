using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRiscuit.Interface;
using UnityEngine;

namespace VRiscuit.Rule {
    /// <summary>
    /// VRiscuit中の一般的なルールを表す
    /// </summary>
    public class VRiscuitRule : IRule {
        public IBeforePattern BeforePattern { get; private set; }
        public IAfterPattern AfterPattern { get; private set; }
        
        /// <summary>
        /// オブジェクトのテーブルを返す
        /// </summary>
        IVRiscuitObjectSet IRule.BeforeObjectSet
        {
            get
            {
                return BeforePattern.VRiscuitObjects;
            }
        }
        /// <summary>
        /// テーブルの実体
        /// </summary>
        private IVRiscuitObjectSet _beforeObjectSet;

        private IVRiscuitObjectSet _afterObjectSet;

        private class IndexPair {
            public int Before;
            public int After;
            public IndexPair(int before, int after) {
                Before = before;
                After = after;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="before"></param>
        /// <param name="after"></param>
        public VRiscuitRule(IBeforePattern before, IAfterPattern after) {
            BeforePattern = before;
            AfterPattern = after;
            _beforeObjectSet = BeforePattern.VRiscuitObjects;
            _afterObjectSet = AfterPattern.ResultObjectSet;
        }

        public VRiscuitRule() {
            
        }

        /// <summary>
        /// ルールの適用
        /// </summary>
        /// <param name="objectsTable"></param>
        void IRule.Apply(IVRiscuitObjectSet objectsTable) {
            var beforeTable = new VRiscuitObjectSet(objectsTable);
            DescentMethod(objectsTable,beforeTable, _beforeObjectSet, _afterObjectSet);
        }

        /// <summary>
        /// 最急降下法で最適な位置を計算する
        /// </summary>
        /// <param name="currentTable"></param>
        /// <param name="beforeRuleTable"></param>
        /// <param name="afterRuleTable"></param>
        /// <param name="changes"></param>
        private void DescentMethod(IVRiscuitObjectSet currentTable, IVRiscuitObjectSet beforeTable, IVRiscuitObjectSet afterRuleTable, IVRiscuitObjectSet beforeRuleTable) {
            var limit = 100;
            var beforeScore = 0.0f;
            var alpha = 1.0f;
            var beforec = new VRiscuitObjectSet(beforeRuleTable);
            var currentc = new VRiscuitObjectSet(currentTable);
            float[] parameters = currentc.ToParameters();
            Func<float[], float> func = delegate (float[] param) {
                (currentc as IVRiscuitObjectSet).SetParameter(param);
                return RuleManager.CalcAppliedFieldScore(currentc, beforec, afterRuleTable, beforeRuleTable);
            };
            var f = 0.05; // scoreの変動がこの値以下になったら終わり
            for(int i = 0; i < limit; i++) {
                // 
                var score = func(parameters);
                if(Mathf.Abs(beforeScore - score) <= f || i != 0) {
                    // 終了
                    currentTable.SetParameter(parameters);
                    return;
                }
                beforeScore = score;
                var delta = Differential(func, parameters);
                for(int j = 0; j < parameters.Length; j++) {
                    parameters[j] += delta[j] * alpha;
                }
            }
        }

        /// <summary>
        /// 配列の各変数で偏微分した配列を返す
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private float[] Differential(Func<float[], float> func, float[] parameters) {
            var ret = new float[parameters.Length];
            var h = 0.1f;
            var current = func(parameters);
            for(int i = 0; i < parameters.Length; i++) {
                parameters[i] += h;
                ret[i] = (func(parameters) - current) / h;
                parameters[i] -= h;
            }
            return ret;
        }

    }
}
