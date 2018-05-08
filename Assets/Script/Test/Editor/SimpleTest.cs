using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Linq;
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
            manager.ApplyRule();
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
            Assert.That(pos.z, Is.InRange(0.9f, 1.1f));
            Assert.That(rot.x, Is.InRange(-0.1f, 0.1f));
            Assert.That(rot.y, Is.InRange(-0.1f, 0.1f));
            Assert.That(rot.z, Is.InRange(-0.1f, 0.1f));
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