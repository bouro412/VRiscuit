using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRiscuit.Interface;

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
        IVRiscuitObjectSet IRule.ObjectSet
        {
            get
            {
                return _objectTypeTable;
            }
        }
        /// <summary>
        /// テーブルの実体
        /// </summary>
        private IVRiscuitObjectSet _objectTypeTable;

        /// <summary>
        /// </summary>
        /// <param name="before"></param>
        /// <param name="after"></param>
        public VRiscuitRule(IBeforePattern before, IAfterPattern after) {
            BeforePattern = before;
            AfterPattern = after;
            _objectTypeTable = BeforePattern.VRiscuitObjects;
        }

        /// <summary>
        /// ルールの適用
        /// </summary>
        /// <param name="objectsTable"></param>
        void IRule.Apply(IVRiscuitObjectSet objectsTable) {
            throw new NotImplementedException();
        }

        private Dictionary<string, IVRiscuitObject[]> MakeTable(IBeforePattern before) {
            var lis = before.VRiscuitObjects;
            var typeAndList = new Dictionary<string, List<IVRiscuitObject>>();
            foreach (var obj in lis) {
                if (typeAndList.ContainsKey(obj.Type)) {
                    typeAndList[obj.Type].Add(obj);
                } else {
                    var l = new List<IVRiscuitObject>();
                    l.Add(obj);
                    typeAndList.Add(obj.Type, l);
                }
            }
            var ret = new Dictionary<string, IVRiscuitObject[]>();
            foreach(var tandl in typeAndList) {
                ret.Add(tandl.Key, tandl.Value.ToArray());
            }
            return ret;
        }
    }
}
