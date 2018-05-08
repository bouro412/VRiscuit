using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRiscuit;
using UnityEditor;
using VRiscuit.Rule;
using VRiscuit.Interface;
using NUnit.Framework;

namespace VRiscuit.Test
{
    class RuleManagerTest : RuleManager
    {
        public RuleManagerTest(IVRiscuitObjectSet objset, IEnumerable<IRule> rules) :base(objset, rules)
        {
        }

        public RuleManagerTest() : base(new VRiscuitObjectSet(), new VRiscuitRule[0])
        {

        }

        [Test]
        public void CalcTest()
        {
            var zero = new CalculateObject(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), "zero");
            var pointOne = new CalculateObject(new Vector3(0, 0, 0.1f), Quaternion.Euler(0, 0, 0), "pointOne");
            var one = new CalculateObject(new Vector3(0, 0, 1), Quaternion.Euler(0, 0, 0), "one");
            var pointOne2 = new CalculateObject(new Vector3(0, 0.1f, 0), Quaternion.Euler(0, 0, 0), "pointOne2");
            var yone = new CalculateObject(new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 0), "yone");
            Debug.Log((zero as IVRiscuitObject).Rotation);
            Debug.Log(Quaternion.FromToRotation((zero as IVRiscuitObject).Position, (yone as IVRiscuitObject).Position));
            Debug.Log(Quaternion.FromToRotation(new Vector3(1, 0, 0), new Vector3(1, 2, 1)));
            Assert.AreNotEqual(Rdir(zero, one), Rdir(zero, yone));

        }

        [Test]
        public void ScoreTest()
        {
            var zero = new CalculateObject(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), "zero");
            var front = new CalculateObject(new Vector3(0, 0, 0.5f), Quaternion.Euler(0, 0, 0), "pointOne");
            var one = new CalculateObject(new Vector3(0, 0, 1), Quaternion.Euler(0, 0, 0), "one");
            var up = new CalculateObject(new Vector3(0, 0.5f, 0), Quaternion.Euler(0, 0, 0), "pointOne2");
            var frontUp = new CalculateObject(new Vector3(0, 0.5f, 0.5f), Quaternion.Euler(0,0,0), "a");
            var frontUpLeft = new CalculateObject(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.Euler(0,0,0), "a");
            var front4 = new CalculateObject(new Vector3(0, 0, 0.4f), Quaternion.Euler(0, 0, 0), "pointOne");
            var front6 = new CalculateObject(new Vector3(0, 0, 0.6f), Quaternion.Euler(0, 0, 0), "pointOne");
            var zeroScore = CalcTwoObjectSimilarity(zero, one, zero, zero);
            var frontScore = CalcTwoObjectSimilarity(zero, one, zero, front);
            var upScore = CalcTwoObjectSimilarity(zero, one, zero, up);
            var frontUpScore = CalcTwoObjectSimilarity(zero, one, zero, frontUp);
            var frontUpLeftScore = CalcTwoObjectSimilarity(zero, one, zero, frontUpLeft);
            var front4Score = CalcTwoObjectSimilarity(zero, one, zero, front4);
            var front6Score = CalcTwoObjectSimilarity(zero, one, zero, front6);
            Debug.Log("zeroScore = " + zeroScore);
            Debug.Log("frontScore = " + frontScore);
            LogArray(CalcTwoObjectSimilarityparameters(zero, one, zero, front));
            Debug.Log("front4Score = " + front4Score);
            LogArray(CalcTwoObjectSimilarityparameters(zero, one, zero, front4));
            Debug.Log("front6Score = " + front6Score);
            LogArray(CalcTwoObjectSimilarityparameters(zero, one, zero, front6));
            Debug.Log("upScore = " + upScore);
            Debug.Log("frontUpScore = " + frontUpScore);
            Debug.Log("frontUpLeftScore = " + frontUpLeftScore);
            Assert.That(upScore, Is.GreaterThan(zeroScore).And.LessThan(frontScore), "up < zero < front");
            Assert.That(frontUpScore, Is.GreaterThan(frontUpLeftScore).And.LessThan(frontScore), "frontUpLeft < frontUp < front");
            Assert.That(frontScore, Is.GreaterThan(front4Score).And.LessThan(front6Score), "front4 < front < front6");
        }

        private void LogArray<T>(IEnumerable<T> array)
        {
            var message = array.Aggregate("", (t, acc) => t.ToString() + ", " + acc.ToString());
            Debug.Log("[" + message.Substring(2) + "]");
        }

        [Test]
        public void OnlyLog()
        {
            var q1 = Quaternion.AngleAxis(30, Vector3.right);
            var q2 = Quaternion.AngleAxis(-30, Vector3.right);
            IVRiscuitObject zero = new CalculateObject(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), "zero");
            IVRiscuitObject one = new CalculateObject(new Vector3(0, 0, 1), Quaternion.Euler(0, 0, 0), "one");
            IVRiscuitObject yone = new CalculateObject(new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 0), "yone");
            Debug.Log(yone.Position - zero.Position);
            Debug.Log(zero.Rotation * Vector3.forward);
            Debug.Log(RuleManager.Rdir(zero, one));
            Debug.Log(Delta(0, 100, 10000));
            Debug.Log(Vector3.Angle(Vector3.forward, new Vector3(0, 0, 0)));
            Debug.Log(Vector3.Angle(Vector3.forward, Vector3.back));
            Debug.Log(EditorUserBuildSettings.activeScriptCompilationDefines.Length); //.Aggregate((s1, s2) => s1 + "\n" + s2));
        }

    }
}
