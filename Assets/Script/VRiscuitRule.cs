using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRiscuit.Interface;

namespace VRiscuit {
    class VRiscuitRule : IRule {
        public IBeforePattern BeforePattern { get; private set; }
        public IAfterPattern AfterPattern { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        Dictionary<string, IVRiscuitObject[]> IRule.ObjectTypeTable
        {
            get
            {
                return _objectTypeTable;
            }
        }
        private Dictionary<string, IVRiscuitObject[]> _objectTypeTable;

        public VRiscuitRule(IBeforePattern before, IAfterPattern after) {
            BeforePattern = before;
            AfterPattern = after;
            _objectTypeTable = MakeTable(BeforePattern);
        }

        void IRule.Apply(Dictionary<string, IVRiscuitObject[]> objectsTable) {
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
