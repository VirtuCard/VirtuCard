using System.Collections;
using System.Collections.Generic;
using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Photon.Chat;
using Photon.Pun;
using PhotonScripts;
using GameScreen.ChatPanel;

public class MessageTest
{
    [UnityTest]
    public IEnumerator MessageTestWithEnumeratorPasses()
    {
        // checks if the message puts ... after 44 characters
        string message = "junieseojunieseojunieseojunieseojunieseojunieseo";
        ChatPanelController p = new ChatPanelController();
        string edited = p.setAndGetTextTest(message);
        string expected = "junieseojunieseojunieseojunieseojunieseoj...";
        bool check = false;
        if (String.Compare(expected, edited) == 0) {
            check = true;
        }
        yield return new WaitForSeconds(2);
        Assert.IsTrue(check);

        // checks if the message has been sent
        p.CreateNewMessage("Testing123", "UsernameTesting");
        yield return new WaitForSeconds(2);
        Assert.IsTrue(p.getMessageCount() == 1);

        yield return new WaitForSeconds(2);;
    }
}
