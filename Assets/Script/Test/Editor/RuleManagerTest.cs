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
        public void PositionScoreTest()
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
            var oneScore = CalcTwoObjectSimilarity(zero, one, zero, one);
            var upScore = CalcTwoObjectSimilarity(zero, one, zero, up);
            var frontUpScore = CalcTwoObjectSimilarity(zero, one, zero, frontUp);
            var frontUpLeftScore = CalcTwoObjectSimilarity(zero, one, zero, frontUpLeft);
            var front4Score = CalcTwoObjectSimilarity(zero, one, zero, front4);
            var front6Score = CalcTwoObjectSimilarity(zero, one, zero, front6);
            Func<CalculateObject, float[]> paramFunc = obj => CalcTwoObjectSimilarityparameters(zero, one, zero, obj);
            Debug.Log("zeroScore = " + zeroScore);
            Debug.Log("frontScore = " + frontScore);
            LogArray(paramFunc(front));
            Debug.Log("front4Score = " + front4Score);
            LogArray(paramFunc(front4));
            Debug.Log("front6Score = " + front6Score);
            LogArray(paramFunc(front6));
            Debug.Log("upScore = " + upScore);
            LogArray(paramFunc(up));
            Debug.Log("frontUpScore = " + frontUpScore);
            LogArray(paramFunc(frontUp));
            Debug.Log("frontUpLeftScore = " + frontUpLeftScore);
            LogArray(paramFunc(frontUpLeft));
            Debug.Log("oneScore = " + oneScore);
            LogArray(paramFunc(one));
            Assert.That(frontScore, Is.GreaterThan(upScore).And.LessThan(oneScore), "up < front < one");
            Assert.That(frontUpScore, Is.GreaterThan(frontUpLeftScore).And.LessThan(frontScore), "frontUpLeft < frontUp < front");
            Assert.That(frontScore, Is.GreaterThan(front4Score).And.LessThan(front6Score), "front4 < front < front6");
        }

        /// <summary>
        /// 想定した目標地点が最高スコアを取れることを確認
        /// </summary>
        [Test]
        public void PositionScoreRandomTest()
        {
            var zero = GenCalObj(0, 0, 0, "zero");
            var one = GenCalObj(0, 0, 1, "one");
            var f = ScoreFun(zero, one);
            var oneScore = f(one);
            var range = 1.0;
            var limit = 100;
            var seed = new System.Random();
            Func<float> genRandomVal = () => (float)(((2 * seed.NextDouble()) - 1) * range);
            Debug.Log("oneScore = " + oneScore);
            for(int i = 1; i <= limit; i++)
            {
                var ps = new float[] {genRandomVal(), genRandomVal(), genRandomVal() + 1,
                                      genRandomVal(), genRandomVal(), genRandomVal()};
                var obj = GenCalObj(ps[0], ps[1], ps[2], /* ps[3], ps[4], ps[5],*/ "random");
                var score = f(obj);
                Assert.That(score, Is.LessThan(oneScore),
                             String.Format("({0}, {1}, {2}, {3}, {4}, {5}) => {6} > {7}",
                                           ps[0], ps[1], ps[2], ps[3], ps[4], ps[5], score, oneScore));
                Debug.Log(String.Format("{7}: ({0}, {1}, {2}, {3}, {4}, {5}) => {6}",
                                           ps[0], ps[1], ps[2], ps[3], ps[4], ps[5], score, i));

            }
        }

        private CalculateObject GenCalObj(float x, float y, float z, float rx, float ry, float rz, string type)
        {
            return new CalculateObject(new Vector3(x, y, z), Quaternion.Euler(0, 0, 0), type);
        }

        private CalculateObject GenCalObj(float x, float y, float z, string type)
        {
            return GenCalObj(x, y, z, 0, 0, 0, type);
        }

        private Func<CalculateObject, float> ScoreFun(CalculateObject before, CalculateObject after, CalculateObject start)
        {
            return obj => CalcTwoObjectSimilarity(before, after, start, obj);
        }

        private Func<CalculateObject, float> ScoreFun(CalculateObject before, CalculateObject after)
        {
            return ScoreFun(before, after, before);
        }

        [Test]
        public void RotateScoreTest()
        {
            var zero = new CalculateObject(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), "zero");
            var a = new CalculateObject(new Vector3(0, 0, 0), Quaternion.Euler(0, 30, 0), "a");
            var ten = new CalculateObject(new Vector3(0, 0, 0), Quaternion.Euler(0, 10, 0), "ten");
            var zmove = new CalculateObject(new Vector3(0, 0, 1), Quaternion.Euler(0, 0, 0), "zmove");
            var xmove = new CalculateObject(new Vector3(1, 0, 0), Quaternion.Euler(0, 0, 0), "xmove");
            Func<CalculateObject, float> ScoreFunc = obj => CalcTwoObjectSimilarity(zero, a, zero, obj);
            var zeroScore = ScoreFunc.Invoke(zero);
            var aScore = ScoreFunc.Invoke(a);
            var tenScore = ScoreFunc.Invoke(ten);
            var zmoveScore = ScoreFunc(zmove);
            var xmoveScore = ScoreFunc(xmove);
            Func<CalculateObject, float[]> ParamFunc = obj => CalcTwoObjectSimilarityparameters(zero, a, zero, obj);
            LogArray(ParamFunc(zero));
            LogArray(ParamFunc(a));
            LogArray(ParamFunc(ten));
            LogArray(ParamFunc(zmove));
            var c3 = 10;
            var w3 = 10000;
            Debug.Log(zeroScore);
            Debug.Log(zmoveScore);
            Debug.Log(xmoveScore);
            Debug.Log(tenScore);
            Debug.Log(aScore);
            Debug.Log(c3 * Delta(0, 0, w3));
            Debug.Log(c3 * Delta(0, 30, w3));
            Debug.Log(c3 * Delta(0, 60, w3));
            Debug.Log(c3 * Delta(0, 90, w3));
            Debug.Log(c3 * Delta(0, 120, w3));
            Debug.Log(c3 * Delta(0, 150, w3));
            Debug.Log(c3 * Delta(0, 180, w3));
            Assert.That(tenScore, Is.GreaterThan(zeroScore).And.LessThan(aScore));
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
            var a = GenCalObj(-0.1470235f, -0.1470365f, 0.9444349f, "sample");
            var f = ScoreFun(zero as CalculateObject, one as CalculateObject);
            Debug.Log("Best Score: " + f(one as CalculateObject));
            Debug.Log(String.Format("a Score: {0} => {1}", f(a), ArrayToString(CalcTwoObjectSimilarityparameters(zero, one, zero, a))));
            var b = GenCalObj(-0.1470235f, -0.1470365f, 0.9544349f, "sample");
            Debug.Log(String.Format("a Score: {0} => {1}", f(b), ArrayToString(CalcTwoObjectSimilarityparameters(zero, one, zero, b))));
            Debug.Log(yone.Position - zero.Position);
            Debug.Log(zero.Rotation * Vector3.forward);
            Debug.Log(RuleManager.Rdir(zero, one));
            Debug.Log(Delta(0, 100, 10000));
            Debug.Log(Vector3.Angle(Vector3.forward, new Vector3(0, 0, 0)));
            Debug.Log(Vector3.Angle(Vector3.forward, Vector3.back));
            Debug.Log(EditorUserBuildSettings.activeScriptCompilationDefines.Length); //.Aggregate((s1, s2) => s1 + "\n" + s2));
        }

        public string ArrayToString<T>(T[] fs)
        {
            var message = fs.Aggregate("", (t, acc) => t.ToString() + ", " + acc.ToString());
            return "[" + message.Substring(2) + "]";
        }
    }
}
