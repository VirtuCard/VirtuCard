using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using FirebaseScripts;

public class UserRegistrationTests
{
    private readonly string username = "UserRegistrationUnitTest";
    private readonly string password = "openSesame123";
    private readonly string email = "unitTest@unitTesters.com";

    // This tests to make sure a user can be registered for an account on firebase
    [UnityTest]
    public IEnumerator UserRegistrationTest()
    {
        // initialize firebase and create a new user account
        if (!FirebaseInit.IsInitialized())
        {
            FirebaseInit.InitializeFirebase(null);
        }
        yield return new WaitForSeconds(2);
        bool didSuccessfullyCreate = false;
        RegistrationPageManager.CreateUserAccount(username, email, password, result => didSuccessfullyCreate = result);
        yield return new WaitForSeconds(2);

        // verify that it was created successfully
        Assert.IsTrue(didSuccessfullyCreate);

        // clean up the user that was created
        bool didSuccessfullyRemove = false;
        DatabaseUtils.RemoveUserWithName(username, result => didSuccessfullyRemove = result);
        yield return new WaitForSeconds(2);

        // assert that it was deleted successfully
        Assert.IsTrue(didSuccessfullyRemove);
        AuthUser.UnregisterAccount(email, password, null);

        yield return new WaitForSeconds(2);
    }
}
