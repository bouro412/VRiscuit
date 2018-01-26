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
    class VRiscuitRule : IRule {
        public IBeforePattern BeforePattern { get; private set; }
        public IAfterPattern AfterPattern { get; private set; }
        
        /// <summary>
        /// オブジェクトのテーブルを返す
        /// </summary>
        IVRiscuitObjectSet IRule.BeforeObjectSet
        {
            get
            {
                return _beforeObjectSet;
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
        /// ルール適用で移動したオブジェクトのリスト
        /// beforeとafterそれぞれのオブジェクトセット中のインデックスで表す
        /// 同じオブジェクトは一つのpairの中にある
        /// </summary>
        private List<IndexPair> MovedObjectPairs;

        /// <summary>
        /// 新たに作られたオブジェクトのリスト
        /// after object setのインデックスで表されている
        /// </summary>
        private List<int> CreatedObjects;

        /// <summary>
        /// </summary>
        /// <param name="before"></param>
        /// <param name="after"></param>
        public VRiscuitRule(IBeforePattern before, IAfterPattern after) {
            BeforePattern = before;
            AfterPattern = after;
            _beforeObjectSet = BeforePattern.VRiscuitObjects;
            MakeCorrespondList();
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
            var beforec = new VRiscuitObjectSet(beforeRuleTable);
            var currentc = new VRiscuitObjectSet(currentTable);
            var f = 0.05; // scoreの変動がこの値以下になったら終わり
            for(int i = 0; i < limit; i++) {
                var score = RuleManager.Instance.CalcAppliedFieldScore(currentc, beforec, beforeRuleTable, afterRuleTable);
                if(beforeScore - score <= f) {
                    // Implement
                }
                beforeScore = score;
                // Implement
            }
        }

        /// <summary>
        /// beforeとafterで同じオブジェクト同士を対応づけるリストの作成
        /// MovedObjectPairsとCreatedObjectsの初期化
        /// </summary>
        private void MakeCorrespondList() {
            // Implement
            MovedObjectPairs = new List<IndexPair>();
            
        }

    }
}
