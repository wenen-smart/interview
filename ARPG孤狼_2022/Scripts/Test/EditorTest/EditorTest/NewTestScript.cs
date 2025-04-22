using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NewTestScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void NewTestScriptSimplePasses()
    {
        SimpleEnum simpleEnum = (SimpleEnum)Enum.Parse(typeof(SimpleEnum), "3");
        Debug.Log(simpleEnum.ToString());
    }
     [Test]
    public void TestGravity()
    {
        Debug.Log(Physics.gravity);
    }
    public enum SimpleEnum
    {
        Simple=1,
        Strong=2
    }
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
