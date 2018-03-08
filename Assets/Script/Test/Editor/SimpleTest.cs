using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using VRiscuit;
using VRiscuit.Interface;
using VRiscuit.Rule;

public class SimpleTest {

    private RuleManager manager;

	[Test]
	public void SimpleTestSimplePasses() {
        // Use the Assert class to test conditions.
        Assert.AreEqual(1, 1 + 1);
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
