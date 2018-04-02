using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Linq;
using VRiscuit;
using VRiscuit.Interface;
using VRiscuit.Rule;

public class SimpleTest {

    private RuleManager manager;

	[Test]
	public void SimpleTestSimplePasses() {
        // Use the Assert class to test conditions.
        Assert.AreEqual(2, 1 + 1);
        var objs = new IVRiscuitObject[]
        {
            new CalculateObject(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), "Cube")
        };
        var objset = new VRiscuitObjectSet(objs);
        var before = new VRiscuitObjectSet(new IVRiscuitObject[] {
            new CalculateObject(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), "Cube")
        });
        var after = new VRiscuitObjectSet(new IVRiscuitObject[]
        {
            new CalculateObject(new Vector3(1, 0, 0), Quaternion.Euler(0, 0, 0), "Cube")
        });
        var rules = new IRule[] {
            new VRiscuitRule(new BeforePattern(before), new AfterPattern(after))
        };
        var manager = new RuleManager(objset, rules);
        manager.ApplyRule();
        var obj = manager.CurrentObjectSet;
        Assert.AreEqual(obj.Size, 1);
        Assert.AreEqual(obj.First().Type, "Cube");
        Assert.AreSame(obj.First(), objs.First());
	}

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator SimpleTestWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}

}
