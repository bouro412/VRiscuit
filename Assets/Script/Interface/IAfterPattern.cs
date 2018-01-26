using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRiscuit.Interface {
    interface IAfterPattern {
        /// <summary>
        /// ルール適用後のオブジェクト
        /// 原理的には対応するbeforeオブジェクトにchangesを適用したものと同じであって欲しい
        /// </summary>
        IVRiscuitObjectSet ResultObjectSet { get; }
    }
}
