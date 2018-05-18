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
            return new CalculateObject(new Vector3(x, y, z), Quaternion.Euler(rx, ry, rz), type);
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
            var ten = new CalculateObject(new Vector3(0, 0, 0), Quaternion.Euler(0, 1, 0), "ten");
            var xten = new CalculateObject(new Vector3(0, 0, 0), Quaternion.Euler(10, 0, 0), "xten");
            var zmove = new CalculateObject(new Vector3(0, 0, 1), Quaternion.Euler(0, 0, 0), "zmove");
            var xmove = new CalculateObject(new Vector3(1, 0, 0), Quaternion.Euler(0, 0, 0), "xmove");
            Func<CalculateObject, float> ScoreFunc = obj => CalcTwoObjectSimilarity(zero, a, zero, obj);
            var zeroScore = ScoreFunc(zero);
            var aScore = ScoreFunc(a);
            var tenScore = ScoreFunc(ten);
            var zmoveScore = ScoreFunc(zmove);
            var xmoveScore = ScoreFunc(xmove);
            var xtenScore = ScoreFunc(xten);
            Func<CalculateObject, float[]> ParamFunc = obj => CalcTwoObjectSimilarityparameters(zero, a, zero, obj);
            Debug.Log("zero param:" + ArrayToString(ParamFunc(zero)));
            Debug.Log("a param" + ArrayToString(ParamFunc(a)));
            Debug.Log("ten param" + ArrayToString(ParamFunc(ten)));
            Debug.Log("xten param" + ArrayToString(ParamFunc(xten)));
            Debug.Log("zmove param" + ArrayToString(ParamFunc(zmove)));
            Debug.Log("xmove param" + ArrayToString(ParamFunc(xmove)));
            Debug.Log(Angle(zero, a));
            Debug.Log(Angle(zero, ten));
            Debug.Log(Angle(zero, xten));
            Debug.Log(Quaternion.Angle((a as IVRiscuitObject).Rotation, (ten as IVRiscuitObject).Rotation));
            Debug.Log(Quaternion.Angle((a as IVRiscuitObject).Rotation, (xten as IVRiscuitObject).Rotation));
            Debug.Log(Quaternion.Angle((a as IVRiscuitObject).Rotation, Quaternion.Euler(10,0,0)));
            Assert.That(tenScore, Is.GreaterThanOrEqualTo(xtenScore).And.LessThan(aScore));
            var test = GenCalObj(-7.777673f, -7.777673f, -7.777673f, -2.319252f, 26.60387f, -2.319252f, "test");
            Debug.Log("Test: " + ScoreFunc(test));
            Debug.Log("param test" + ArrayToString(ParamFunc(test)));
            var test2 = GenCalObj(-7.777673f, -7.777673f, -7.777673f, -2.319252f, 36.60387f, -2.319252f, "test");
            Debug.Log("Test2: " + ScoreFunc(test2));
            Debug.Log("param test2" + ArrayToString(ParamFunc(test2)));
            Func<IVRiscuitObject, float[]> diffFunc = obj =>
            {
                var objset = new VRiscuitObjectSet(new IVRiscuitObject[] { obj });
                var ps = objset.ToParameters();
                return Differential(prms => {
                    var objs = new VRiscuitObjectSet(objset);
                    (objs as IVRiscuitObjectSet).SetParameter(prms);
                    return ScoreFunc(objs.First() as CalculateObject);
                }, ps);
            };
            var diff = diffFunc(test);
            var diffstr = ArrayToString(diff);
            Debug.Log("diff test" + ArrayToString(diffFunc(test)));
            Debug.Log("diff test2" + ArrayToString(diffFunc(test2)));

        }

        private float[] Differential(Func<float[], float> func, float[] parameters)
        {
            var ret = new float[parameters.Length];
            var h = 1f;
            var current = func(parameters);
            for (int i = 0; i < parameters.Length; i++)
            {
                if (i % 6 > 2)
                {
                    h = 10;
                }
                parameters[i] += h;
                var newscore = func(parameters);
                ret[i] = (newscore - current) / h;
                parameters[i] -= h;
            }
            return ret;
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
            Debug.Log(Quaternion.Angle(Quaternion.Euler(0, 30, 0), Quaternion.Euler(0, 10, 0)));
            Debug.Log(Quaternion.Angle(Quaternion.Euler(0, 30, 0), Quaternion.Euler(10, 0, 0)));
            for(int i = 0;i < 18; i++)
            {
                Debug.Log(Quaternion.Euler(0, 10 * i, 0));
            }
            var beforeR = Quaternion.Euler(10, 10, 10);
            var afterR = Quaternion.Euler(20, 20, 20);
            var d1 =  Quaternion.Inverse(afterR) * beforeR;
            var d2 =  Quaternion.Inverse(beforeR) * afterR;
            var d3 = beforeR * Quaternion.Inverse(afterR);
            var d4 = afterR * Quaternion.Inverse(beforeR);
            var current = Quaternion.Euler(0, 0, 0);
            Debug.Log((current * d1).eulerAngles);
            Debug.Log((d1 * current).eulerAngles);
            Debug.Log((current * d2).eulerAngles);
            Debug.Log((d2 * current).eulerAngles);
            Debug.Log((current * d3).eulerAngles);
            Debug.Log((d3 * current).eulerAngles);
            Debug.Log((current * d4).eulerAngles);
            Debug.Log((d4 * current).eulerAngles);
        }

        public string ArrayToString<T>(T[] fs)
        {
            var message = fs.Aggregate("", (t, acc) => t.ToString() + ", " + acc.ToString());
            return "[" + message.Substring(2) + "]";
        }
    }
}
