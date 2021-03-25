using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using GameScreen;

public class ShuffleLogicTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void ShuffleLogicTestSimplePasses()
    {
        // Use the Assert class to test conditions
        TestGame testGame = new TestGame();

        //Creating two decks to test the shuffle logic
        CardDeck deckAfterShuffle = testGame.CreateStandard52Deck();
        CardDeck deckBeforeShuffle = testGame.CreateStandard52Deck();
        
        //Copying contents of deckBeforeShuffle to deckAfterShuffle for
        // comparing after the Shuffle() call.
        deckAfterShuffle.DuplicateDeck(deckBeforeShuffle);

        deckAfterShuffle.Shuffle();

        bool LogicTest = CardDeck.IsEqual(deckBeforeShuffle, deckAfterShuffle);
        Assert.IsFalse(LogicTest);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator ShuffleLogicTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
