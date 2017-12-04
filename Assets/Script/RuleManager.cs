using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRiscuit.Interface;
using VRiscuit.Rule;

namespace VRiscuit {
    class RuleManager : MonoBehaviour {
        // シングルトン
        public static RuleManager Instance { get; private set; }


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
        public IVRiscuitObject GenerateObject(string objectName, Vector3 position, Vector3 angle) {
            var obj = Instantiate(VRiscuitPrefavTable[objectName],
                                                   position,
                                                   Quaternion.Euler(angle),
                                                   transform);
            if (obj == null) {
                Debug.LogError(objectName + "の生成に失敗");
                return null;
            }
            var vobj = obj.GetComponent<IVRiscuitObject>();
            if (vobj == null) {
                Debug.LogError(objectName + "はVRiscuitObjectではありません");
                return null;
            }
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

        #region Unity Event
        /// <summary>
        /// 初期化。シングルトンと必要なテーブルの用意
        /// </summary>
        private void Awake() {
            Instance = this;
            VRiscuitPrefavTable = new Dictionary<string, GameObject>();
            CurrentObjectSet = new VRiscuitObjectSet();
        }

        /// <summary>
        /// 毎フレームルールの適用 
        /// </summary>
        private void Update() {
            ApplyRule();
        }
        #endregion

        #region ルール適用周り

        /// <summary>
        /// 定義されたルールをゲーム世界に適用、変更する
        /// </summary>
        private void ApplyRule() {
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
                var objs = rule.ObjectSet.TypeTable;
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
                var lis = Choice(typesAndObjs.Select(i => i.Value).ToList()).Select(flatten);
                foreach(var objlist in lis) {
                    var objset = new VRiscuitObjectSet(objlist);
                    var cand = new Candidate(rule, objset);
                    result.Add(cand);
                }
            }
            return result.ToArray();
        }

        private List<T> flatten<T>(List<List<T>> lislis) {
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
                    var ret = new List<T>(restChoice);
                    ret.Add(t);
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
                        var p = new List<T>(perm);
                        p.Add(item);
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

    }
}
