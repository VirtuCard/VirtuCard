using System.Collections;
using System.Collections.Generic;
using FirebaseScripts;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Threading.Tasks;

public class FirebaseTests
{

    /// <summary>
    /// This method tests if a new user can register for firebase.
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator FirebaseRegistrationTest()
    {
        if (!FirebaseInit.IsInitialized())
        {
            FirebaseInit.InitializeFirebase(null);
        }
        yield return new WaitForSeconds(3);

        string randomEmail = "TESTING_";
        for (int x = 0; x < 20; x++)
        {
            randomEmail += (int)Random.Range(0, 100000);
        }

        bool registrationDidSucceed = false;
        AuthUser.RegisterTestingAccount(randomEmail + "@gmail.com", "password123", status => { registrationDidSucceed = status; });
        yield return new WaitForSeconds(4);
        Assert.IsTrue(registrationDidSucceed);
    }
}
