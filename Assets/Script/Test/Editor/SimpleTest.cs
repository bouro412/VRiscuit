using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using VRiscuit;
using VRiscuit.Interface;
using VRiscuit.Rule;

namespace VRiscuit.Test {
    public class SimpleTest {

        private RuleManager manager;

        private IRule SimpleRule = new VRiscuitRule(new BeforePattern
                                                     (new VRiscuitObjectSet
                                                        (new IVRiscuitObject[] {
                                                            new CalculateObject(new Vector3(0,0,0), Quaternion.identity, "Cube")
                                                        })),
                                                    new AfterPattern
                                                      (new VRiscuitObjectSet
                                                        (new IVRiscuitObject[] {
                                                            new CalculateObject(new Vector3(0,0,1), Quaternion.identity, "Cube")
                                                          })));

        [Test]
        public void StraightSimpleTest() {
            var rules = new IRule[] {
                SimpleRule
            };
            var objs = new VRiscuitObjectSet(new IVRiscuitObject[] { new CalculateObject(new Vector3(0, 0, 0), Quaternion.identity, "Cube") });
            var manager = new RuleManager(objs, rules);
            ApplyInSec(() => manager.ApplyRule());
            var obj = manager.CurrentObjectSet;
            Assert.AreEqual(obj.Size, 1);
            Assert.AreEqual(obj.First().Type, "Cube");
            Assert.AreEqual(obj.First(), objs.First());
            var pos = obj.First().Position;
            var rot = obj.First().Rotation;
            Debug.Log(pos);
            Debug.Log(pos.x);
            Debug.Log(pos.y);
            Assert.That(pos.x, Is.InRange(-0.1f, 0.1f));
            Assert.That(pos.y, Is.InRange(-0.1f, 0.1f));
            Assert.That(pos.z, Is.GreaterThan(1.0f));
            Assert.That(rot.x, Is.InRange(-0.1f, 0.1f));
            Assert.That(rot.y, Is.InRange(-0.1f, 0.1f));
            Assert.That(rot.z, Is.InRange(-0.1f, 0.1f));
        }

        [Test]
        public void RotateTest()
        {
            var rules = new IRule[]
            {
                new RuleMaker()
                {
                    Before = new IVRiscuitObject[]
                    {
                        new CalculateObject(new Vector3(0, 0, 0), Quaternion.identity, "rotate")
                    },
                    After = new IVRiscuitObject[]
                    {
                        new CalculateObject(new Vector3(0, 0, 0), Quaternion.Euler(0, 30, 0), "rotate")
                    }
                }.Convert()
            };
            var objs = new VRiscuitObjectSet(new IVRiscuitObject[]
            {
                new CalculateObject(new Vector3(0, 0, 0), Quaternion.identity, "rotate")
            });
            var manager = new RuleManager(objs, rules);
            int i = 1;
            ApplyInSec(() => {
                manager.ApplyRule();
                Debug.Log(string.Format("{0}: pos = {1}, rot = {2}", i++, objs.First().Position, objs.First().Rotation));
            });
            var obj = objs.First();
        }

        [Test]
        public void CurveTest()
        {
            var rules = new IRule[]
            {
                new RuleMaker(){
                    Before = new IVRiscuitObject[]
                    {
                        new CalculateObject(new Vector3(0,0,0), Quaternion.identity, "Spear")
                    },
                    After = new IVRiscuitObject[]
                    {
                        new CalculateObject(new Vector3(0,0,1), Quaternion.Euler(0, 30, 0), "Spear")
                    }
                }.Convert()
            };
            var objs = new VRiscuitObjectSet(new IVRiscuitObject[] {
                new CalculateObject(new Vector3(0, 0, 0), Quaternion.identity, "Spear")
            });
            var manager = new RuleManager(objs, rules);
            int i = 0;
            
            ApplyInSec(() => {
                manager.ApplyRule();
                Debug.Log(string.Format("{0}: pos = {1}, rot = {2}", i++, objs.First().Position, objs.First().Rotation));
            });
            Assert.AreEqual(1, 1);
        }

        private void ApplyInSec(Action func, float sec = 1.0f)
        {
            var sum = 0.0f;
            while(sum < sec)
            {
                func.Invoke();
                sum += Time.deltaTime;   
            }
        }

        private class RuleMaker
        {
            public IVRiscuitObject[] Before;
            public IVRiscuitObject[] After;

            public VRiscuitRule Convert()
            {
                var beforePattern = new BeforePattern(new VRiscuitObjectSet(Before));
                var afterPattern = new AfterPattern(new VRiscuitObjectSet(Before));
                return new VRiscuitRule(beforePattern, afterPattern);
            }
        }

        [Test]
        public void ScoreTest()
        {
            var beforeObjs = new VRiscuitObjectSet(new IVRiscuitObject[]{ new CalculateObject(new Vector3(0, 0, 0), Quaternion.identity, "Cube") });
            var afterObjs1 = new VRiscuitObjectSet(new IVRiscuitObject[]{ new CalculateObject(new Vector3(0f, 0f, 2.0f), Quaternion.identity, "Cube") });
            var correctObjs = new VRiscuitObjectSet(new IVRiscuitObject[]{ new CalculateObject(new Vector3(0, 0, 1.0f), Quaternion.identity, "Cube") });
            var zeroScore = RuleManager.CalcAppliedFieldScore(beforeObjs, beforeObjs, SimpleRule.AfterObjectSet, SimpleRule.BeforeObjectSet);
            var correctScore = RuleManager.CalcAppliedFieldScore(correctObjs, beforeObjs, SimpleRule.AfterObjectSet, SimpleRule.BeforeObjectSet);
            var currentScore = RuleManager.CalcAppliedFieldScore(afterObjs1, beforeObjs, SimpleRule.AfterObjectSet, SimpleRule.BeforeObjectSet);
            Debug.Log(zeroScore);
            Debug.Log(currentScore);
            Debug.Log(correctScore);
        }

    }
}