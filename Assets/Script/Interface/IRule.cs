using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRiscuit.Interface {
    public interface IRule {
        /// <summary>
        /// マッチ条件となるオブジェクトのテーブル
        /// </summary>
        IVRiscuitObjectSet BeforeObjectSet { get;  }

        /// <summary>
        /// ルール適用後のオブジェクトテーブル
        /// </summary>
        IVRiscuitObjectSet AfterObjectSet { get; }

        /// <summary>
        /// ルールを適用するメソッド
        /// </summary>
        /// <param name="objectsTable"></param>
        void Apply(IVRiscuitObjectSet objectsTable);
    }
}
