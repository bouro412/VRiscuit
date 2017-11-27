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


        private IRule[] _rules;

        /// <summary>
        /// 各オブジェクトのprefavを取り出すテーブル
        /// </summary>
        public Dictionary<string, GameObject> VRiscuitObjectTable;

        /// <summary>
        /// 現在のオブジェクトのテーブル
        /// </summary>
        private Dictionary<string, List<IVRiscuitObject>> CurrentObjectTable;

        #region 公開している汎用関数

        /// <summary>
        /// オブジェクトを生成する汎用関数
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="position"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public IVRiscuitObject GenerateObject(string objectName, Vector3 position, Vector3 angle) {
            var obj = Instantiate(VRiscuitObjectTable[objectName],
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
            CurrentObjectTable[objectName].Add(vobj);
            return vobj;
        }

        /// <summary>
        /// 新規オブジェクトタイプの登録
        /// </summary>
        /// <param name="name"></param>
        /// <param name="prefab"></param>
        public void RegisterObject(string name, GameObject prefab) {
            VRiscuitObjectTable.Add(name, prefab);
            CurrentObjectTable.Add(name, new List<IVRiscuitObject>());
        }

        #endregion

        /// <summary>
        /// 初期化。シングルトンと必要なテーブルの用意
        /// </summary>
        private void Awake() {
            Instance = this;
            VRiscuitObjectTable = new Dictionary<string, GameObject>();
            CurrentObjectTable = new Dictionary<string, List<IVRiscuitObject>>();
        }

        /// <summary>
        /// 毎フレームルールの適用 
        /// </summary>
        private void Update() {
            ApplyRule();
        }

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
            return _rules.Select(rule =>
                rule.ObjectTypeTable.Select(nameAndObjs => {
                    var len = nameAndObjs.Value.Length;
                    var ch = choice(nameAndObjs.Value, len);
                    return new KeyValuePair<string, List<List<IVRiscuitObject>>>(nameAndObjs.Key, ch);
                }).Aggregate(new List<Dictionary<string, IVRiscuitObject[]>>(),
                             ((result, nameAndObjs) => {
                                 if (result.Count == 0) {
                                     foreach (var objs in nameAndObjs.Value) {
                                         var dict = new Dictionary<string, IVRiscuitObject[]>();
                                         dict.Add(nameAndObjs.Key, objs.ToArray());
                                         result.Add(dict);
                                     }
                                 } else {
                                     foreach (var dict in result) {
                                         var newdict = new Dictionary<string, IVRiscuitObject[]>(dict);
                                         foreach (var objs in nameAndObjs.Value) {
                                             newdict.Add(nameAndObjs.Key, objs.ToArray());
                                             result.Add(newdict);
                                         }
                                     }
                                 }
                                 return result;
                             })).Select(dict =>  new Candidate(rule, dict)))
                             .SelectMany(i => i).ToArray();
        }

        /// <summary>
        /// objlistからlength個の組合せを列挙する
        /// GetApplyCandidatesの補助関数
        /// </summary>
        /// <param name="objlist"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private List<List<IVRiscuitObject>> choice(IVRiscuitObject[] objlist, int length) {
            if (length == 0) {
                var ret = new List<List<IVRiscuitObject>>();
                ret.Add(new List<IVRiscuitObject>());
                return ret;
            }
            if (objlist.Length == 0) {
                return null;
            }
            var notcontains = choice(objlist.Skip(1).ToArray(), length);
            var contains = choice(objlist.Skip(1).ToArray(), length - 1);
            if (contains == null && notcontains == null)
                return null;
            if(contains != null) {
                foreach(var lis in contains) {
                    lis.Add(objlist[0]);
                }
                if (notcontains != null)
                    contains.AddRange(notcontains);
                return contains;
            }
            return notcontains;
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
