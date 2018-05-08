using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRiscuit.Interface;

namespace VRiscuit
{
    public class ManagerObject : MonoBehaviour
    {
        private RuleManager _manager;

        [SerializeField]
        private GameObject _objectsRoot;

        [SerializeField]
        private GameObject _rulesRoot;

        #region Unity Event
        /// <summary>
        /// 初期化。シングルトンと必要なテーブルの用意
        /// </summary>
        private void Start()
        {
            IVRiscuitObjectSet currentObjectSet = new VRiscuitObjectSet();
            if (_objectsRoot != null)
            {
                foreach (Transform obj in _objectsRoot.transform)
                {
                    var vobj = obj.GetComponent<IVRiscuitObject>();
                    if (vobj == null) continue;
                    currentObjectSet.Add(vobj);
                }
            }
            List<IRule> rules = new List<IRule>();
            if (_rulesRoot != null)
            {
                foreach (Transform obj in _rulesRoot.transform)
                {
                    var rule = obj.GetComponent<IRuleSet>();
                    if (rule == null) continue;
                    rules.AddRange(rule.Rules);
                }
            }
            _manager = new RuleManager(currentObjectSet, rules);
        }

        /// <summary>
        /// 毎フレームルールの適用 
        /// </summary>
        private void Update()
        {
            _manager.ApplyRule();
        }
        #endregion
    }
}
