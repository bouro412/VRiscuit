﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRiscuit.Interface;
using VRiscuit.Rule;

namespace VRiscuit {
    public class RuleManager {

        private List<IRule> _rules = new List<IRule>();

        /// <summary>
        /// 各オブジェクトのprefavを取り出すテーブル
        /// </summary>
        public Dictionary<string, GameObject> VRiscuitPrefavTable;

        /// <summary>
        /// 現在のオブジェクトのテーブル
        /// </summary>
        public IVRiscuitObjectSet CurrentObjectSet;

        #region 公開している汎用関数

        /// <summary>
        /// オブジェクトを生成する汎用関数
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="position"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public IVRiscuitObject GenerateObject(string objectName, Vector3 position, Quaternion angle) {
            var vobj = new CalculateObject(position, angle, objectName);
            CurrentObjectSet.GetByType(objectName).Add(vobj);
            return vobj;
        }

        /// <summary>
        /// 新規オブジェクトタイプの登録
        /// </summary>
        /// <param name="name"></param>
        /// <param name="prefab"></param>
        public void RegisterObject(string name, GameObject prefab) {
            VRiscuitPrefavTable.Add(name, prefab);
        }

        #endregion

        public RuleManager(IVRiscuitObjectSet objects, IEnumerable<IRule> rules) {
            VRiscuitPrefavTable = new Dictionary<string, GameObject>();
            CurrentObjectSet = objects;
            _rules = rules.ToList();
        }


        #region ルール適用周り

        /// <summary>
        /// 定義されたルールをゲーム世界に適用、変更する
        /// </summary>
        public void ApplyRule() {
            // Candidateの取得
            var cands = GetApplyCandidates();
            // Score順にソート
            var sorted = cands.OrderBy(cand => -cand.Score).ToList();
            // 先頭から呼び出し & 一度使ったオブジェクトを使うルールは消去
            Debug.Log(String.Format("one {0}, two {1}", sorted[0].Score, sorted[sorted.Count - 1].Score));
            CallCands(sorted);
        }

        /// <summary>
        /// 現在のゲームフィールドからルールが適用可能な候補を取得する
        /// </summary>
        /// <returns></returns>
        public Candidate[] GetApplyCandidates() {
            var result = new List<Candidate>();
            foreach(var rule in _rules) {
                // オブジェクト候補を作れるかのチェック
                var typesAndObjs = SameNumberObjectGroupsByType(CurrentObjectSet, rule.BeforeObjectSet);
                if (typesAndObjs == null) break;
                foreach(var objlist in typesAndObjs) {
                    var objset = new VRiscuitObjectSet(objlist);
                    var score = CalcScore(rule.BeforeObjectSet, objset, rule.RuleScoreCoefficient);
                    // scoreで足切り
                    if (NormalizeScore(score) < 0.1)
                    {
                        continue;
                    }
                    var cand = new Candidate(rule, objset, score);
                    result.Add(cand);
                }
            }
            return result.ToArray();
        }

        protected IEnumerable<List<IVRiscuitObject>> SameNumberObjectGroupsByType(IVRiscuitObjectSet objectSet, IVRiscuitObjectSet ruleBeforeSet)
        {
            var typesAndObjs = new List<List<List<IVRiscuitObject>>>();
            foreach(var kvp in ruleBeforeSet.TypeTable)
            {
                var currentObjs = CurrentObjectSet.GetByType(kvp.Key);
                if (currentObjs == null || currentObjs.Count < kvp.Value.Count)
                {
                    return null;
                }
                typesAndObjs.Add(Permutation(currentObjs, kvp.Value.Count()).ToList());
            }
            var choice = Choice(typesAndObjs);
            var c = choice.ToList();
            var result = choice.Select(Flatten);
            var a = result.ToList();
            return result;
        }

        private List<T> Flatten<T>(IEnumerable<List<T>> lislis) {
            var result = new List<T>();
            foreach (var l in lislis)
            {
                result.AddRange(l);
            }
            return result;
        }

        /// <summary>
        /// objlistからlength個の順列を列挙する
        /// GetApplyCandidatesの補助関数
        /// </summary>
        /// <param name="objlist"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private IEnumerable<List<T>> Choice<T>(List<List<T>> objlist) {
            if(!objlist.Any()) {
                yield break;
            }
            if(objlist.Count() == 1)
            {
                foreach (var l in objlist.First())
                {
                    yield return new List<T> { l };
                }
            }
            else { 
                var head = objlist[0];
                var tail = objlist.Skip(1).ToList();
                foreach (var t in head)
                {
                    foreach (var restChoice in Choice(tail))
                    {
                        var ret = new List<T>(restChoice)
                        {
                            t
                        };
                        yield return ret;
                    }
                }
            }
        }

        private IEnumerable<List<T>> Permutation<T>(IEnumerable<T> lis, int length) {
            if (lis == null || lis.Count() < length) yield break;
            if (length == 0) yield return new List<T>();
            else {
                for (int i = 0; i < lis.Count(); i++) {
                    var item = lis.ElementAt(i);
                    var rest = new List<T>(lis);
                    rest.RemoveAt(i);
                    foreach (var perm in Permutation<T>(rest, length - 1)) {
                        var p = new List<T>(perm)
                        {
                            item
                        };
                        yield return p;
                    }
                }
            }
        }

        /// <summary>
        /// Candのリストを先頭から適用する。
        /// ただし一つのオブジェクトは一度しかルール適用されないように適宜Candは削減する
        /// </summary>
        /// <param name="cands"></param>
        private void CallCands(List<Candidate> cands) {
            if(cands.Count == 0) 
                return;
            var head = cands[0];
            var tail = cands.Skip(1).Where(cand => !head.HasSameObject(cand)).ToList();
            head.RuleApply(CurrentObjectSet);
            CallCands(tail);
        }

        public static float NormalizeScore(float[] scoreAndMax)
        {
            return scoreAndMax[0] / scoreAndMax[1];
        }

        #endregion
        #region scoreの計算

        public static float[] CalcAppliedFieldScore(IVRiscuitObjectSet currentField, IVRiscuitObjectSet beforeField, IVRiscuitObjectSet afterRuleSet, IVRiscuitObjectSet beforeRuleSet, ScoreCoefficient ef) {
            var score = 0.0f;
            var max = 0.0f;
            var beforeRuleSetArray = beforeRuleSet.ObjectArray;
            var afterRuleSetArray = afterRuleSet.ObjectArray;
            var beforeFieldArray = beforeField.ObjectArray;
            var currentFieldArray = currentField.ObjectArray;
            var currentSize = currentField.Size;
            var beforeSize = beforeField.Size;
            for(int a = 0; a < currentSize; a++) {
                for (int b = 0; b < beforeSize; b++) {
                    var f = CalcTwoObjectSimilarity(beforeRuleSetArray[b], afterRuleSetArray[a], beforeFieldArray[b], currentFieldArray[a], ef);
                    score += f[0];
                    max += f[1];
                }
            }
            var cc = CalcScore(afterRuleSet, currentField, ef);
            score += cc[0];
            max += cc[1];
            return new float[] { score, max};
        }

        /// <summary>
        /// Rule上のオブジェクトとフィールドのオブジェクトがどれだけ近い関係にあるかを計算
        /// </summary>
        /// <param name="ruleObjectSet"></param>
        /// <param name="fieldObjectSet"></param>
        /// <param name="ef"></param>
        /// <returns></returns>
        public static float[] CalcScore(IVRiscuitObjectSet ruleObjectSet, IVRiscuitObjectSet fieldObjectSet, ScoreCoefficient ef) {
            var score = 0.0f;
            var max = 0.0f;
            var ruleObjectList = ruleObjectSet.ObjectArray;
            var fieldObjectList = fieldObjectSet.ObjectArray;
            var len = fieldObjectList.Length;
            if(len == 1)
            {
                return new float[] { 10, 10 };
            }
            for (int i = 0; i < len - 1; i++) {
                for (int j = i + 1; j < len; j++) {
                    var fs = CalcTwoObjectSimilarity(ruleObjectList[i], ruleObjectList[j],
                        fieldObjectList[i], fieldObjectList[j], ef);
                    score += fs[0];
                    max += fs[1];
                }
            }
            return new float[] { score, max };
        }

        public static readonly float alpha = 0.01f;

        /// <summary>
        /// a,bは必ずルール内のオブジェクトなので、ルール中は不変
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="ef"></param>
        /// <returns></returns>
        public static float[] CalcTwoObjectSimilarity(IVRiscuitObject a, IVRiscuitObject b, IVRiscuitObject x, IVRiscuitObject y, ScoreCoefficient ef) {
            var fs = CalcTwoObjectSimilarityparameters(a, b, x, y, ef);
            var score = fs[0] + fs[1] + fs[2] + fs[3];
            return new float[2] { score, fs[4] };
        }        

        public static float[] CalcTwoObjectSimilarity(IVRiscuitObject a, IVRiscuitObject b, IVRiscuitObject x, IVRiscuitObject y)
        {
            return CalcTwoObjectSimilarity(a, b, x, y, new ScoreCoefficient());
        }

        protected static float[] CalcTwoObjectSimilarityparameters(IVRiscuitObject a, IVRiscuitObject b, IVRiscuitObject x, IVRiscuitObject y, ScoreCoefficient ef)
        {
            var eps = Eps(a, b, x, y, ef);
            var max = ef.NormWeight + eps * (ef.RdirWeight1 + ef.RdirWeight2) + ef.AngleWeight;
            var score = new float[]{
                ef.NormWeight * Delta(Norm(a, b), Norm(x, y), ef.NormLeveling),
                ef.RdirWeight1 * eps * Delta(Rdir(a, b), Rdir(x, y), ef.RdirLeveling1),
                ef.RdirWeight2 * eps * Delta(Rdir(b, a), Rdir(y, x), ef.RdirLeveling2),
                ef.AngleWeight * Delta3(Angle(a, b), Angle(x, y), ef.AngleLeveling),
                max
            };
            return score;
        }

        protected static float[] CalcTwoObjectSimilarityparameters(IVRiscuitObject a, IVRiscuitObject b, IVRiscuitObject x, IVRiscuitObject y)
        {
            return CalcTwoObjectSimilarityparameters(a, b, x, y, new ScoreCoefficient());
        }

        /// <summary>
        /// e^(-(x-y)^2 / w)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        protected static float Delta(float x, float y, float weight) {
            var result = (float)Math.Exp(-(Math.Pow(x - y, 2) / weight));
#if UNITY_EDITOR
            // Debug.Log(String.Format("Delta({0}, {1}) = {2}", x, y, result));
#endif
            return result;
        }

        protected static float Norm(IVRiscuitObject x, IVRiscuitObject y) {
            var result = (x.Position - y.Position).magnitude;
#if UNITY_EDITOR
            //Debug.Log(String.Format("Norm({0}, {1}) = {2}", x.Type, y.Type, result));
#endif
            return result;
        }

        protected static float Rdir(IVRiscuitObject x, IVRiscuitObject y) {
            var xtoy = y.Position - x.Position;
            if(xtoy.magnitude == 0) { }
            var result = Vector3.Angle(x.Rotation * Vector3.forward, xtoy);
            // xからyへのベクトルvとxの回転クオータニオンqとの関係
#if UNITY_EDITOR
            //Debug.Log(String.Format("Rdir({0}, {1}) = {2}", x.Type, y.Type, result));
#endif
            return result;
        }

        protected static Vector3 Angle(IVRiscuitObject x, IVRiscuitObject y) {
            var result = x.Rotation.eulerAngles - y.Rotation.eulerAngles;
#if UNITY_EDITOR
//            Debug.Log(String.Format("Angle({0}, {1}) = {2}", x.Rotation, y.Rotation, result));
#endif
            return result;
        }

        protected static float Delta3(Vector3 xs, Vector3 ys, float weight)
        {
            var result = (float)Math.Exp(- VectorSubtract(xs, ys) / weight);
#if UNITY_EDITOR
            // Debug.Log(String.Format("Delta3({0}, {1}) = {2}", xs, ys, result));
#endif
            return result;
        }

        protected static float VectorSubtract(Vector3 xs, Vector3 ys)
        {
            Func<float, float, float> sub = (f1, f2) =>
            {
                var d = Math.Abs((f1 - f2) % 360);
                return d < 180 ? d : 360 - d;
            };
            var result = new Vector3(sub(xs.x, ys.x), sub(xs.y, ys.y), sub(xs.z, ys.z)).magnitude;
            // Debug.Log("VectorSubtract(" + xs + ", " + ys + ") = " + result);
            return result;
        }

        protected static float Eps(IVRiscuitObject a, IVRiscuitObject b, IVRiscuitObject x, IVRiscuitObject y, ScoreCoefficient ef) {
            var ab = Norm(a, b);
            var xy = Norm(x, y);
            if (ab == 0) return 0;
            var result = (float)(1 - Math.Exp(-ef.EpsWeight1 / Math.Pow(ab /*+ xy*/ + ef.EpsWeight2, 2)));
#if UNITY_EDITOR
            // Debug.Log(String.Format("Eps({0}, {1}, {2}, {3}) = {4}", a.Type, b.Type, x.Type, y.Type, result));
#endif
            return result;
        }

        #endregion
        #region テスト用関数
        protected void AddRule(IVRiscuitObject[] before, IVRiscuitObject[] after)
        {
            var beforeSet = new VRiscuitObjectSet(before);
            Debug.Log(beforeSet != null);
            var rule = new VRiscuitRule(new BeforePattern(new VRiscuitObjectSet(before)),
                                        new AfterPattern(new VRiscuitObjectSet(after)));
            Debug.Log(rule);
            _rules.Add(rule);
        }

        protected IVRiscuitObject TestObject(float positionX, float positionY, float positionZ, 
                                             float angleX, float angleY, float angleZ, 
                                             string type)
        {
            return new CalculateObject(new Vector3(positionX, positionY, positionZ), 
                                       Quaternion.Euler(angleX, angleY, angleZ), 
                                       type);
        }

        #endregion
    }
}
