using System;
using System.Collections;
using System.Collections.Generic;
using FirebaseScripts;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ExistingUserTest
{
    private readonly string username = "UserLoginTest";
    private readonly string password = "TheCakeIsALie";
    private readonly string email = "testing@unitTesters.com";

    // This test is designed to see if a user that is already in the
    // the database can login to their account using the login page.
    // this is mostly based on Kade's unit test for registering
    // a new account into firebase.
    [UnityTest]
    public IEnumerator ExistingUserTestWithEnumeratorPasses()
    {
        // start of kade's registration test
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
        // end of kade's registration test

        // start my part of the test
        bool loginSuccessful = false;
        AuthUser.Login(username, password, loginResult => loginSuccessful = loginResult);
        yield return new WaitForSeconds(2);

        Assert.IsTrue(loginSuccessful);

        // end my part of the test

        // back to kade's code
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
