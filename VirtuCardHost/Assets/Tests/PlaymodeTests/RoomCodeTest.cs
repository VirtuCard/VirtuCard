using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RoomCodeTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void RoomCodeTestSimplePasses()
    {
        // Use the Assert class to test conditions
        
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator RoomCodeTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }

    // Helper function to help test Room Code
    public bool LetterCheck(string RoomCode)
    {
        for (int i = 0; i < RoomCode.Length; i++)
        {
            if (!char.IsLetter(RoomCode[i])) 
            {
                return false;
            }
        }
        return true;
    }

}
