using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class will eventually be set up accordingly to correspond
// to the rules of each game type.
public static class GameRules
{
    private static bool canSkip = true;

    public static void setSkipAllowed(bool value){
        canSkip = value;
    }
    public static bool skipAllowed(){
        return canSkip;
    }
}
