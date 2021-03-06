﻿using UnityEngine;
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

        [Test]
        public void StraightSimpleTest() {
            var rules = new IRule[] {
                new RuleMaker(){
                    Before = new IVRiscuitObject[]
                    {
                        new CalculateObject(new Vector3(0, 0, 0), Quaternion.identity, "Cube")
                    },
                    After = new IVRiscuitObject[]
                    {
                        new CalculateObject(new Vector3(0, 0, 1), Quaternion.identity, "Cube")
                    }
                }.Convert()
            };
            var objs = new VRiscuitObjectSet(new IVRiscuitObject[] { new CalculateObject(new Vector3(0, 0, 0), Quaternion.identity, "Cube") });
            var manager = new RuleManager(objs, rules);
            int i = 1;
            ApplyInSec(() => {
                manager.ApplyRule();
                Debug.Log(string.Format("{0}: pos = {1}, rot = {2}", i++, objs.First().Position, objs.First().Rotation));
            });
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
            Assert.That(pos.z, Is.InRange(-0.9f, 1.1f));
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
                Debug.Log(string.Format("{0} times Apply: pos = {1}, rot = {2}", i++, objs.First().Position, objs.First().Rotation.eulerAngles));
            });
            
            var obj = objs.First();
            Assert.That(obj.Position.x, Is.GreaterThan(-0.1f).And.LessThan(0.1f));
            Assert.That(obj.Position.y, Is.GreaterThan(-0.1f).And.LessThan(0.1f));
            Assert.That(obj.Position.z, Is.GreaterThan(-0.1f).And.LessThan(0.1f));
            Assert.That(obj.Rotation.eulerAngles.x, Is.GreaterThan(-0.1f).And.LessThan(0.1f));
            Assert.That(obj.Rotation.eulerAngles.y, Is.GreaterThan(29.0f).And.LessThan(31.0f));
            Assert.That(obj.Rotation.eulerAngles.z, Is.GreaterThan(-0.1f).And.LessThan(0.1f));
        }

        [Test]
        public void GenerateTest()
        {
            var rules = new IRule[]
            {
                new RuleMaker()
                {
                    Before = new IVRiscuitObject[]
                    {
                        new CalculateObject(new Vector3(0, 0, 0), Quaternion.identity, "a")
                    },
                    After = new IVRiscuitObject[]
                    {
                        new CalculateObject(new Vector3(0, 0, 0), Quaternion.identity, "a"),
                        new CalculateObject(new Vector3(1, 1, 1), Quaternion.identity, "b")
                    }
                }.Convert()
            };
            var objs = new VRiscuitObjectSet(new IVRiscuitObject[]
            {
                new CalculateObject(new Vector3(0, 0, 0), Quaternion.identity, "a"),
                new CalculateObject(new Vector3(0, 0, 1), Quaternion.identity, "a")

            });
            var manager = new RuleManager(objs, rules);
            int i = 1;
            manager.ApplyRule();
            Debug.Log(string.Format("{0} times Apply: {1}", i++, manager.CurrentObjectSet.ObjectArray.Aggregate("", (str, obj) => str + obj.Position + " ")));

            Assert.That(manager.CurrentObjectSet.Size, Is.EqualTo(4));
        }

        [Test]
        public void DeleteTest()
        {
            var rules = new IRule[]
            {
                new RuleMaker()
                {
                    Before = new IVRiscuitObject[]
                    {
                        new CalculateObject(new Vector3(0, 0, 0), Quaternion.identity, "a"),
                        new CalculateObject(new Vector3(1, 0, 0), Quaternion.identity, "b")
                    },
                    After = new IVRiscuitObject[]
                    {
                        new CalculateObject(new Vector3(0, 0, 0), Quaternion.identity, "a")
                    }
                }.Convert()
            };
            var objs = new VRiscuitObjectSet(new IVRiscuitObject[]
            {
                new CalculateObject(new Vector3(0, 0, 0), Quaternion.identity, "a"),
                new CalculateObject(new Vector3(1, 0, 0), Quaternion.identity, "b")

            });
            var manager = new RuleManager(objs, rules);
            int i = 1;
            manager.ApplyRule();
            Debug.Log(string.Format("{0} times Apply: {1}", i++, manager.CurrentObjectSet.ObjectArray.Aggregate("", (str, obj) => str + obj.Position + " ")));
            Assert.That(manager.CurrentObjectSet.Size, Is.EqualTo(1));
            Assert.That(manager.CurrentObjectSet.First().Type, Is.EqualTo("a"));
        }

        [Test]
        public void ChangeObjectTest()
        {
            var rules = new IRule[]
            {
                new RuleMaker()
                {
                    Before = new IVRiscuitObject[]
                    {
                        new CalculateObject(new Vector3(0, 0, 0), Quaternion.identity, "a"),
                    },
                    After = new IVRiscuitObject[]
                    {
                        new CalculateObject(new Vector3(0, 0, 0), Quaternion.identity, "b")
                    }
                }.Convert()
            };
            var objs = new VRiscuitObjectSet(new IVRiscuitObject[]
            {
                new CalculateObject(new Vector3(0, 0, 0), Quaternion.identity, "a"),
            });
            var manager = new RuleManager(objs, rules);
            int i = 1;
            manager.ApplyRule();
            Debug.Log(string.Format("{0} times Apply: {1}", i++, manager.CurrentObjectSet.ObjectArray.Aggregate("", (str, obj) => str + obj.Position + " ")));
            Assert.That(manager.CurrentObjectSet.Size, Is.EqualTo(1));
            Assert.That(manager.CurrentObjectSet.First().Type, Is.EqualTo("b"));
            Assert.That(manager.CurrentObjectSet.First().Position.x, Is.GreaterThan(-0.1f).And.LessThan(0.1f));
            Assert.That(manager.CurrentObjectSet.First().Position.y, Is.GreaterThan(-0.1f).And.LessThan(0.1f));
            Assert.That(manager.CurrentObjectSet.First().Position.z, Is.GreaterThan(-0.1f).And.LessThan(0.1f));
        }

        [Test]
        public void MultiObjectTest()
        {
            var rules = new IRule[]
            {
                new RuleMaker()
                {
                    Before = new IVRiscuitObject[]
                    {
                        new CalculateObject(Vector3.zero, Quaternion.identity, "a")
                    },
                    After = new IVRiscuitObject[]
                    {
                        new CalculateObject(new Vector3(0, 0, 1), Quaternion.identity, "a")
                    }
                 }.Convert(),
                new RuleMaker()
                {
                    Before = new IVRiscuitObject[]
                    {
                        new CalculateObject(Vector3.zero, Quaternion.identity, "a"),
                        new CalculateObject(Vector3.zero, Quaternion.Euler(0, 180, 0), "a")
                    },
                    After = new IVRiscuitObject[0]
                }.Convert()
            };
            var objs = new VRiscuitObjectSet(new IVRiscuitObject[]{
                new CalculateObject(new Vector3(0, 0, 0), Quaternion.identity, "a"),
                new CalculateObject(new Vector3(0, 1, 0), Quaternion.identity, "a"),
                new CalculateObject(new Vector3(0, 2, 0), Quaternion.identity, "a"),
                new CalculateObject(new Vector3(0, 0, 0), Quaternion.Euler(0, 180, 0), "a"),
                new CalculateObject(new Vector3(0, 1, 0), Quaternion.Euler(0, 180, 0), "a"),
                new CalculateObject(new Vector3(0, 2, 0), Quaternion.Euler(0, 180, 0), "a"),
            });
            var manager = new RuleManager(objs, rules);
            var cands = manager.GetApplyCandidates();
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
                var afterPattern = new AfterPattern(new VRiscuitObjectSet(After));
                return new VRiscuitRule(beforePattern, afterPattern, true); 
            }
        }
    }
}