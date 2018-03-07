using System;
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
        private IVRiscuitObjectSet CurrentObjectSet;

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
            cands.ToList().Sort(new CandComp());
            // 先頭から呼び出し & 一度使ったオブジェクトを使うルールは消去
            CallCands(cands.ToList());
        }

        /// <summary>
        /// 現在のゲームフィールドからルールが適用可能な候補を取得する
        /// </summary>
        /// <returns></returns>
        private Candidate[] GetApplyCandidates() {
            var result = new List<Candidate>();
            foreach(var rule in _rules) {
                var objs = rule.BeforeObjectSet.TypeTable;
                // オブジェクト候補を作れるかのチェック
                
                var canMakeCandidate = true;
                var typesAndObjs = objs.Select(kvp => {
                    var currentObjs = CurrentObjectSet.GetByType(kvp.Key);
                    if (currentObjs == null || currentObjs.Count < kvp.Value.Count) {
                        canMakeCandidate = false;
                        return new KeyValuePair<string, List<List<IVRiscuitObject>>>();
                    }
                    var candInThisType = Permutation(currentObjs, kvp.Value.Count());
                    return new KeyValuePair<string, List<List<IVRiscuitObject>>>(kvp.Key, candInThisType.ToList());
                }).ToList();
                if (canMakeCandidate == false) break;
                var lis = Choice(typesAndObjs.Select(i => i.Value).ToList()).Select(Flatten);
                foreach(var objlist in lis) {
                    var objset = new VRiscuitObjectSet(objlist);
                    var score = CalcScore(rule.BeforeObjectSet, objset, new ScoreCoefficient());
                    var cand = new Candidate(rule, objset, score);
                    result.Add(cand);
                }
            }
            return result.ToArray();
        }

        private List<T> Flatten<T>(List<List<T>> lislis) {
            return lislis.SelectMany(i => i).ToList();
        }

        /// <summary>
        /// objlistからlength個の順列を列挙する
        /// GetApplyCandidatesの補助関数
        /// </summary>
        /// <param name="objlist"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private IEnumerable<List<T>> Choice<T>(List<List<T>> objlist) {
            if(objlist.Count() == 0) {
                yield break;
            }
            var head = objlist[0];
            var tail = objlist.Skip(0).ToList();
            foreach(var t in head) {
                foreach(var restChoice in Choice(tail)) {
                    var ret = new List<T>(restChoice)
                    {
                        t
                    };
                    yield return ret;
                }
            }
        }

        private IEnumerable<List<T>> Permutation<T>(IEnumerable<T> lis, int length) {
            if (lis == null || lis.Count() < length) yield break;
            if (length == 0) yield return new List<T>();
            else {
                for (int i = 0; i < lis.Count(); i++) {
                    var item = lis.ElementAt(i);
                    foreach (var perm in Permutation<T>(lis.Skip(i), length - 1)) {
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
            var tail = cands.Skip(1);

            head.RuleApply();
            CallCands(tail.Where(cand => !head.HasSameObject(cand)).ToList());
        }

        #endregion
        #region scoreの計算

        public static float CalcAppliedFieldScore(IVRiscuitObjectSet afterField, IVRiscuitObjectSet beforeField, IVRiscuitObjectSet afterRuleSet, IVRiscuitObjectSet beforeRuleSet) {
            var score = 0.0f;
            for(int a = 0; a < afterField.Size; a++) {
                for (int b = 0; b < beforeField.Size; b++) {
                    score += CalcTwoObjectSimilarity(beforeRuleSet[b], afterRuleSet[a], beforeField[b], afterField[a], new ScoreCoefficient());
                }
            }
            score += CalcScore(afterRuleSet, afterField, new ScoreCoefficient());
            return score;
        }

        /// <summary>
        /// Rule上のオブジェクトとフィールドのオブジェクトがどれだけ近い関係にあるかを計算
        /// </summary>
        /// <param name="ruleObjectSet"></param>
        /// <param name="fieldObjectSet"></param>
        /// <param name="ef"></param>
        /// <returns></returns>
        private static float CalcScore(IVRiscuitObjectSet ruleObjectSet, IVRiscuitObjectSet fieldObjectSet, ScoreCoefficient ef) {
            var score = 0.0f;
            var ruleObjectList = ruleObjectSet.ObjectArray;
            var fieldObjectList = fieldObjectSet.ObjectArray;
            var len = fieldObjectList.Length;
            for (int i = 0; i < len - 1; i++) {
                for (int j = i + 1; j < len; j++) {
                    score += CalcTwoObjectSimilarity(ruleObjectList[i], ruleObjectList[j],
                        fieldObjectList[i], fieldObjectList[j], ef);
                }
            }
            return score;
        }

        public class ScoreCoefficient {
            public float  c0;
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

        private static float CalcTwoObjectSimilarity(IVRiscuitObject a, IVRiscuitObject b, IVRiscuitObject x, IVRiscuitObject y, ScoreCoefficient ef) {
            var score = 0.0f;
            score += ef.c0 * Delta(Norm(a, b), Norm(x, y), ef.w0);
            score += ef.c1 * Eps(a, b, x, y, ef) * Delta(Rdir(a, b), Rdir(x, y), ef.w1);
            score += ef.c2 * Eps(a, b, x, y, ef) * Delta(Rdir(b, a), Rdir(y, x), ef.w2);
            score += ef.c3 * Delta(Angle(a, b), Angle(x, y), ef.w3);
            return score;
        }

        /// <summary>
        /// e^(-(x-y)^2 / w)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        private static float Delta(float x, float y, float weight) {
            return (float)Math.Exp(-(Math.Pow(x - y, 2) / weight));
        }

        private static float Norm(IVRiscuitObject x, IVRiscuitObject y) {
            return (x.Position - y.Position).magnitude;
        }

        private static float Rdir(IVRiscuitObject x, IVRiscuitObject y) {
            return Quaternion.Angle(x.Rotation, Quaternion.FromToRotation(x.Position, y.Position));
        }

        private static float Angle(IVRiscuitObject x, IVRiscuitObject y) {
            return Quaternion.Angle(x.Rotation, y.Rotation);
        }

        private static float Eps(IVRiscuitObject a, IVRiscuitObject b, IVRiscuitObject x, IVRiscuitObject y, ScoreCoefficient ef) {
            return (float)(1 - Math.Exp(-ef.c5 / Math.Pow(Norm(a, b) + Norm(x, y) + ef.c6, 2)));
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
