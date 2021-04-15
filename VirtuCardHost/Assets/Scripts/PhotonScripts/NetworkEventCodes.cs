using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NetworkEventCodes
{
    HostSendInfoToConnectedClient = 1,
    ClientPlayedCard = 2,
    VerifyClientCard = 4,
    HostSendingCardVerification = 5,
    StartGame = 6,
    ClientDrawCard = 7,
    HostSendingCardsToPlayer = 8,
    UpdatePlayerTurnIndex = 9,
    ClientSkipTurn = 10,
    HostEnablingTimer = 11,
    HostRemovingCardsFromPlayer = 13,
    WinnerSelected = 20,
    ExitGame = 21,
    SongVerification = 27,
    ClientWarFlipCard = 34,
    PlayAgain = 35,
    PlayerKicked = 36,
    BoilerUpEmoji = 69,
    IUSucksEmoji = 70,
    HostSendingUnoCards = 24
}