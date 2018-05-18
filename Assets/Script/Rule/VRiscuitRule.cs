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

        private Dictionary<String, int> _genDelTable;

        private bool isGenerateOrDeleteObject;

        private class IndexPair {
            public int Before;
            public int After;
            public IndexPair(int before, int after) {
                Before = before;
                After = after;
            }
        }

        private bool _isDebug = false;

        /// <summary>
        /// </summary>
        /// <param name="before"></param>
        /// <param name="after"></param>
        public VRiscuitRule(IBeforePattern before, IAfterPattern after) {
            BeforePattern = before;
            AfterPattern = after;
            _beforeObjectSet = BeforePattern.VRiscuitObjects;
            _afterObjectSet = AfterPattern.ResultObjectSet;
            _genDelTable = new Dictionary<string, int>(_afterObjectSet.DistributionTable);
            foreach (var kvp in _beforeObjectSet.DistributionTable)
            {
                if (_genDelTable.ContainsKey(kvp.Key))
                {
                    _genDelTable[kvp.Key] -= kvp.Value;
                }
                else {
                    _genDelTable.Add(kvp.Key, -kvp.Value);
                }
                if (_genDelTable[kvp.Key] != 0)
                {
                    isGenerateOrDeleteObject = true;
                }
            }
        }

        public VRiscuitRule() {
            
        }

        /// <summary>
        /// ルールの適用
        /// </summary>
        /// <param name="objectsTable"></param>
        void IRule.Apply(IVRiscuitObjectSet objectsTable) {
            GenerateOrDeleteObject(objectsTable);
            DescentMethod(objectsTable, _afterObjectSet, _beforeObjectSet);
        }

        void GenerateOrDeleteObject(IVRiscuitObjectSet objset)
        {
            if(isGenerateOrDeleteObject == false)
            {
                return;
            }
            foreach(var kvp in _genDelTable)
            {
                var val = kvp.Value;
                if(val < 0)
                {
                    var objs = objset.TypeTable[kvp.Key];
                    if (objs.Count < -val)
                    {
                        Debug.LogError("Error: 必要な数のオブジェクトがありません。");
                        objs.RemoveAll((a) => true);
                    }
                    objs.RemoveRange(0, - val);
                }
                else if(val > 0)
                {
                    var first = objset.First();
                    for(int i = 0; i < val; i++)
                    {
                        var pos = new Vector3(first.Position.x + 1, first.Position.y, first.Position.z);
                        var rot = Quaternion.Euler(first.Rotation.eulerAngles.x, first.Rotation.eulerAngles.y, first.Rotation.eulerAngles.z);
                        var x = _isDebug ? (IVRiscuitObject)new CalculateObject(pos, rot, kvp.Key) : new VRiscuitObject(pos, rot, kvp.Key);
                        objset.Add(x);
                    }
                }
            }
        }

        /// <summary>
        /// 最急降下法で最適な位置を計算する
        /// </summary>
        /// <param name="currentTable"></param>
        /// <param name="beforeRuleTable"></param>
        /// <param name="afterRuleTable"></param>
        /// <param name="changes"></param>
        private void DescentMethod(IVRiscuitObjectSet currentTable, IVRiscuitObjectSet afterRuleTable, IVRiscuitObjectSet beforeRuleTable) {
            var limit = 300;
            var beforeScore = 0.0f;
            var beforec = new VRiscuitObjectSet(currentTable);
            var currentc = new VRiscuitObjectSet(currentTable);
            var parameters = currentc.ToParameters();
            var beforeDelta = new float[parameters.Length]; 

            Func<float[], float> func = delegate (float[] param) {
                (currentc as IVRiscuitObjectSet).SetParameter(param);
                return RuleManager.CalcAppliedFieldScore(currentc, beforec, afterRuleTable, beforeRuleTable);
            };

            // alphaの値をルールの動き量で決める
            var a = new VRiscuitObjectSet(afterRuleTable).ToParameters();
            var b = new VRiscuitObjectSet(beforeRuleTable).ToParameters();
            var ParamLength = TwoArrayDistance(a, b);
            var alpha = ParamLength;

            var f = 0.001; // scoreの変動がこの値以下になったら終わり
            for(int i = 0; i < limit; i++) {
                var score = func(parameters);
                var message = String.Format("{0}: {1}, {2}, {3}, {5}, {6}, {7} => {4} points", i, parameters[0], parameters[1], parameters[2], score, parameters[3], parameters[4], parameters[5]);
                // Debug.Log(message);
                // Debug.Log("alpha = " + alpha);
                if (Mathf.Abs(beforeScore - score) <= f && i != 0) {
                    // 終了
                    break;
                }
                beforeScore = score;
                var delta = Differential(func, parameters);
                delta = NormalizeArray(delta);

                // deltaの中に0の要素があったら、beforeDeltaをもとに前回の変更を半分もとに戻す
                // その場合次のbeforeDeltaは
                // 同時にbeforeDeltaの更新
                for (int di = 0; di < delta.Length; di++)
                {
                    if(delta[di] == 0)
                    {
                        delta[di] = -(beforeDelta[di] / 2);
                        beforeDelta[di] /= 2;
                    }
                    else
                    {
                        beforeDelta[di] = delta[di];
                    }
                }


                //Debug.Log("delta = " + delta.Skip(1).Aggregate(delta[0].ToString(), (acc, next) => acc + ", " + next.ToString()));
                for(int j = 0; j < parameters.Length; j++) {
                    parameters[j] += delta[j] * alpha;
                }
                // Debug.Log("parameter = " + parameters.Skip(1).Aggregate(parameters[0].ToString(), (acc, next) => acc + ", " + next.ToString()));
                alpha *= Mathf.Max((float)Math.Exp(-alpha), 0.1f);
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
            var h = 0.01f;
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
            if (a.Length != b.Length)
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
