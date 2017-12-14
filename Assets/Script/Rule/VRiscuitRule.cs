using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRiscuit.Interface;
using VRiscuit.Rule.Change;
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

        /// <summary>
        /// このルールによる変化の一覧
        /// </summary>
        private IChange[] _changes;

        /// <summary>
        /// </summary>
        /// <param name="before"></param>
        /// <param name="after"></param>
        public VRiscuitRule(IBeforePattern before, IAfterPattern after) {
            BeforePattern = before;
            AfterPattern = after;
            _beforeObjectSet = BeforePattern.VRiscuitObjects;
            _changes = after.Changes;
        }

        /// <summary>
        /// ルールの適用
        /// </summary>
        /// <param name="objectsTable"></param>
        void IRule.Apply(IVRiscuitObjectSet objectsTable) {
            foreach(var change in _changes) {
                change.UpdateObject(objectsTable, OffSet.Zero);
            }
            DescentMethod(objectsTable, _beforeObjectSet, _afterObjectSet, _changes.Where(c => c is IDescentable).Select(c => c as IDescentable).ToArray());
        }

        /// <summary>
        /// 最急降下法で最適な位置を計算する
        /// </summary>
        /// <param name="current"></param>
        /// <param name="before"></param>
        /// <param name="after"></param>
        /// <param name="changes"></param>
        private void DescentMethod(IVRiscuitObjectSet current, IVRiscuitObjectSet before, IVRiscuitObjectSet after, IDescentable[] changes) {
            
        }
    }
}
