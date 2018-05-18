using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRiscuit.Interface;
using VRiscuit.Rule;

namespace VRiscuit {
    /// <summary>
    /// VRiscuit Objectの集合を表す
    /// </summary>
    public class VRiscuitObjectSet : IVRiscuitObjectSet {
        #region IVRiscuitObjectSet Interface
        /// <summary>
        /// オブジェクトのタイプごとにまとめたテーブル。
        /// 基本的にこっちで保持
        /// </summary>
        Dictionary<string, List<IVRiscuitObject>> IVRiscuitObjectSet.TypeTable { get { return _table; } }
        private Dictionary<string, List<IVRiscuitObject>> _table;

        /// <summary>
        /// 配列にして返す
        /// 型のアルファベット順で並べる。同じ型ならTable時の順番を維持
        /// </summary>
        IVRiscuitObject[] IVRiscuitObjectSet.ObjectArray
        {
            get
            {
                var lis = _table.ToList();
                lis.Sort(delegate (KeyValuePair<string, List<IVRiscuitObject>> kvp1, KeyValuePair<string, List<IVRiscuitObject>> kvp2) {
                    return String.Compare(kvp1.Key, kvp2.Key);
                });
                return lis.Select(i => i.Value).SelectMany(i => i).ToArray();
            }
        }
        /// <summary>
        /// オブジェクトの追加
        /// indexを返り値で返す
        /// </summary>
        /// <param name="newObject"></param>
        int IVRiscuitObjectSet.Add(IVRiscuitObject newObject) {
            var type = newObject.Type;
            if (_table.ContainsKey(type)) {
                _table[type].Add(newObject);
            } else {
                var lis = new List<IVRiscuitObject>
                {
                    newObject
                };
                _table.Add(type, lis);
            }
            var ary = (this as IVRiscuitObjectSet).ObjectArray;
            for(int i = 0;i < ary.Length; i++) {
                if(ary[i] == newObject) {
                    return i;
                }
            }
            return -1;
        }
        List<IVRiscuitObject> IVRiscuitObjectSet.GetByType(string type) {
            if(_table.ContainsKey(type))
                return _table[type];
            return null;
        }

        /// <summary>
        /// Array中のIndexで指定されたオブジェクトを削除
        /// </summary>
        /// <param name="index"></param>
        void IVRiscuitObjectSet.Delete(int index) {
            var target = (this as IVRiscuitObjectSet).ObjectArray[index];
            if(target == null) {
                Debug.LogError("delete target is null");
                return;
            }
            var type = target.Type;
            _table[type].Remove(target);
        }

        void IVRiscuitObjectSet.Delete(IVRiscuitObject obj)
        {
            if(_table.ContainsKey(obj.Type) && _table[obj.Type].Contains(obj))
            {
                _table[obj.Type].Remove(obj);
            }
            else
            {
                Debug.LogError("ERROR: 削除予定のオブジェクトが見つかりませんでした: " + obj.Type);
            }
        }

        /// <summary>
        /// 与えられたパラメーターをもとにobjectの位置、回転を設定する
        /// </summary>
        void IVRiscuitObjectSet.SetParameter(float[] parameter) {
            var array = (this as IVRiscuitObjectSet).ObjectArray;
            if (array.Length * 6 != parameter.Length) {
                Debug.LogError("パラメーターの数が合いません");
                return;
            }
            for (int i = 0; i < array.Length; i++) {
                var obj = array[i];
                obj.Position = new Vector3(parameter[i * 6], parameter[i * 6 + 1], parameter[i * 6 + 2]);
                obj.Rotation = Quaternion.Euler(euler(parameter[i * 6 + 3]), 
                                                euler(parameter[i * 6 + 4]), 
                                                euler(parameter[i * 6 + 5]));
            }
        }

        private float euler(float a)
        {
            a %= 360;
            a = a < 0 ? a + 360 : a;
            return a;
        }

        /// <summary>
        /// 入っているオブジェクトの数
        /// </summary>
        int IVRiscuitObjectSet.Size { get { return (this as IVRiscuitObjectSet).ObjectArray.Length; } }

        Dictionary<string, int> IVRiscuitObjectSet.DistributionTable
        {
            get
            {
                var result = new Dictionary<string, int>();
                foreach(var kvp in _table)
                {
                    result.Add(kvp.Key, kvp.Value.Count());
                }
                return result;
            }
        }

        IVRiscuitObject IVRiscuitObjectSet.this[int i] { get { return (this as IVRiscuitObjectSet).ObjectArray[i]; }
                                                         set { (this as IVRiscuitObjectSet).ObjectArray[i] = value; } }

        #endregion
        /// <summary>
        /// オブジェクトの配列から生成
        /// </summary>
        /// <param name="array"></param>
        public VRiscuitObjectSet(IVRiscuitObject[] array) {
            _table = new Dictionary<string, List<IVRiscuitObject>>();
            foreach (var obj in array) {
               if (_table.ContainsKey(obj.Type)) {
                    _table[obj.Type].Add(obj);
                } else {
                    var l = new List<IVRiscuitObject>
                    {
                        obj
                    };
                    _table.Add(obj.Type, l);
                }
            }
        }

        /// <summary>
        /// リストから。上と同様
        /// </summary>
        /// <param name="list"></param>
        public VRiscuitObjectSet(List<IVRiscuitObject> list) {
            _table = new Dictionary<string, List<IVRiscuitObject>>();
            foreach (var obj in list) {
                if (_table.ContainsKey(obj.Type)) {
                    _table[obj.Type].Add(obj);
                } else {
                    var l = new List<IVRiscuitObject>
                    {
                        obj
                    };
                    _table.Add(obj.Type, l);
                }
            }
        }

        /// <summary>
        /// テーブルから生成
        /// </summary>
        /// <param name="table"></param>
        public VRiscuitObjectSet(Dictionary<string, List<IVRiscuitObject>> table) {
            _table = table ?? new Dictionary<string, List<IVRiscuitObject>>();
        }

        /// <summary>
        /// 引数なしの場合からのテーブルを定義
        /// </summary>
        public VRiscuitObjectSet() {
            _table = new Dictionary<string, List<IVRiscuitObject>>();
        }

        /// <summary>
        /// オブジェクト集合のコピー
        /// 実体のないcalculateObjectで構成する
        /// </summary>
        /// <param name="source"></param>
        public VRiscuitObjectSet(IVRiscuitObjectSet source) {
            _table = new Dictionary<string, List<IVRiscuitObject>>();
            foreach(var kvp in source.TypeTable) {
                _table.Add(kvp.Key, kvp.Value.Select(obj => new CalculateObject(obj) as IVRiscuitObject).ToList());
            }
        }


        IEnumerator IEnumerable.GetEnumerator() {
            foreach (var v in ((IVRiscuitObjectSet)this).ObjectArray) {
                yield return v;
            }
        }

        IEnumerator<IVRiscuitObject> IEnumerable<IVRiscuitObject>.GetEnumerator() {
            foreach(var v in ((IVRiscuitObjectSet)this).ObjectArray) {
                yield return v;
            }
        }

        /// <summary>
        /// objectの集合の基礎パラメーターをもとにfloat[]に変換する
        /// </summary>
        /// <returns></returns>
        public float[] ToParameters() {
            var array = (this as IVRiscuitObjectSet).ObjectArray;
            float[] ret = array.Select(obj => new float[] { obj.Position.x,
                                                            obj.Position.y,
                                                            obj.Position.z,
                                                            obj.Rotation.eulerAngles.x,
                                                            obj.Rotation.eulerAngles.y,
                                                            obj.Rotation.eulerAngles.z}
                                       ).SelectMany(i => i).ToArray();
            return ret;
        }

        
    }
}
