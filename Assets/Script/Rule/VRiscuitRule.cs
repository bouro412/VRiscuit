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

        IVRiscuitObjectSet IRule.AfterObjectSet
        {
            get
            {
                return AfterPattern.ResultObjectSet;
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
            DescentMethod(objectsTable, _afterObjectSet, _beforeObjectSet);
        }

        /// <summary>
        /// 最急降下法で最適な位置を計算する
        /// </summary>
        /// <param name="currentTable"></param>
        /// <param name="beforeRuleTable"></param>
        /// <param name="afterRuleTable"></param>
        /// <param name="changes"></param>
        private void DescentMethod(IVRiscuitObjectSet currentTable, IVRiscuitObjectSet afterRuleTable, IVRiscuitObjectSet beforeRuleTable) {
            var limit = 100;
            var beforeScore = 0.0f;
            var beforec = new VRiscuitObjectSet(currentTable);
            var currentc = new VRiscuitObjectSet(currentTable);
            float[] parameters = currentc.ToParameters();
            Func<float[], float> func = delegate (float[] param) {
                (currentc as IVRiscuitObjectSet).SetParameter(param);
                return RuleManager.CalcAppliedFieldScore(currentc, beforec, afterRuleTable, beforeRuleTable);
            };
            var a = new VRiscuitObjectSet(afterRuleTable).ToParameters();
            var b = new VRiscuitObjectSet(beforeRuleTable).ToParameters();
            var ParamLength = TwoArrayDistance(a, b);
            var alpha = ParamLength;
            var f = 0.00001; // scoreの変動がこの値以下になったら終わり
            for(int i = 0; i < limit; i++) {
                var score = func(parameters);
                var message = String.Format("{0}: {1}, {2}, {3} => {4} points", i, parameters[0], parameters[1], parameters[2], score);
                Debug.Log(message);
                if (Mathf.Abs(beforeScore - score) <= f && i != 0) {
                    // 終了
                    break;
                }
                beforeScore = score;
                var delta = Differential(func, parameters);
                // 正規化
                delta = NormalizeArray(delta);

                // Debug.Log("delta = " + delta.Skip(1).Aggregate(delta[0].ToString(), (acc, next) => acc + ", " + next.ToString()));
                for(int j = 0; j < parameters.Length; j++) {
                    parameters[j] += delta[j] * alpha;
                }
                // Debug.Log("parameter = " + parameters.Skip(1).Aggregate(parameters[0].ToString(), (acc, next) => acc + ", " + next.ToString()));
                alpha *= 0.5f;
            }
            var beforeParam = beforec.ToParameters();
            var d = new float[beforeParam.Length];
            for(int i = 0; i < beforeParam.Length; i++)
            {
                var di = parameters[i] - beforeParam[i];
                // 細かい部分を四捨五入, 秒間スピードに変更
                di = ((float)Math.Round(di, 1)) * Time.deltaTime;
                parameters[i] = beforeParam[i] + di;
            }
            currentTable.SetParameter(parameters);
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

        /// <summary>
        /// 微分したパラメーターのベクトルの正規化用
        /// </summary>
        /// <param name="arry"></param>
        /// <returns></returns>
        private float[] NormalizeArray(float[] arry)
        {
            var sum = Mathf.Sqrt(arry.Select(f => f * f).Sum());
            return arry.Select(f => f / sum).ToArray();
        }

        private float TwoArrayDistance(float[] a, float[] b)
        {
            if(a.Length != b.Length)
            {
                Debug.LogError("パラメーターの数が異なっています");
                return 0.0f;
            }
            var sum = 0.0f;
            for(int i = 0;i < a.Length; i++)
            {
                sum += (float)Mathf.Pow((a[i] - b[i]), 2);
            }
            return Mathf.Sqrt(sum);
        }
    }
}
