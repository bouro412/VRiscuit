using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRiscuit.Interface {
    interface IRule {
        /// <summary>
        /// マッチ条件となるオブジェクトのテーブル
        /// </summary>
        IVRiscuitObjectSet BeforeObjectSet { get;  }
        /// <summary>
        /// ルールを適用するメソッド
        /// </summary>
        /// <param name="objectsTable"></param>
        void Apply(IVRiscuitObjectSet objectsTable);
    }
}
