using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using FirebaseScripts;

public class MessageTest
{
    public string message = "junieseojunieseojunieseojunieseojunieseojunieseo";

    [UnityTest]
    public IEnumerator MessageTestWithEnumeratorPasses()
    {
        // checks if the message puts ... after 44 characters
        ChatPanelController.SetText(message);
        string edited = "junieseojunieseojunieseojunieseojunieseoj...";
        bool check = false;
        if (String.Compare(ChatPanelController.GetText(), edited) == 0) {
            check = true;
        }
        yield return new WaitForSeconds(2);
        Assert.IsTrue(check);

        // checks if the message has been sent
        ChatPanelController.CreateNewMessage("Testing123", "UsernameTesting");
        yield return new WaitForSeconds(2);
        Assert.IsTrue(ChatPanelController.getMessageCount() == 1);


        yield return new WaitForSeconds(2);;
    }
}
