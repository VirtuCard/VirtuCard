using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using ExitGames.Client.Photon;
using System;

namespace PhotonScripts
{
    public class NetworkController : MonoBehaviourPunCallbacks, IConnectionCallbacks, IMatchmakingCallbacks, IInRoomCallbacks, ILobbyCallbacks, IErrorInfoCallback
    {
        private int RoomCodeLength = 6;
        private GameObject eventTest;

        //Field for Host's RoomCode.
        //Format: ABCDEF
        private string RoomCode = "";

        //Field to store the name of the file
        // that holds the RoomCode for testing purposes
        private string RoomCodeFileName = "RoomCode.txt";

        /* Start is called before the first frame update
    public static void OnCreateButtonClick()
    {
        PhotonNetwork.ConnectUsingSettings(); //Connects to Photon Master Servers
    
    } */

        void Start()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("The Host is now connected to the " + PhotonNetwork.CloudRegion + " server.");
            Debug.Log("Generated Room Code is " + RoomCode);
        }

        void OnServerConnect()
        {
            DoSomething();
        }
        /// generateCode()
        /// 
        /// Method to generate Host's RoomCode.
        /// Format: ABCDEF
        /// 
        /// <returns> RoomCode String </returns>
        public static string generateCode()
        {
            //Integer Array used to create RoomCode

            int RoomCodeLength = 6;
            int[] RoomCodeNum = new int[RoomCodeLength];

            //Placing Numbers to act as 'pseudo' letters. Each number
            // corresponds to a letter. For example, 0 is A.
            for (int i = 0; i < RoomCodeLength; i++)
            {
                RoomCodeNum[i] = UnityEngine.Random.Range(65, 90);
            }

            //Coverting Numbers into ASCII and then appending them to
            // roomCodeBuffer
            string roomCodeBuffer = "";
            for (int i = 0; i < RoomCodeLength; i++)
            {
                roomCodeBuffer += (char) (RoomCodeNum[i]);
            }

            //Writing Room Code to file
            WriteRoomCodeToFile(roomCodeBuffer);

            //Setting Host's RoomCode field to generated roomCodeBuffer
            //RoomCode = roomCodeBuffer;
            return roomCodeBuffer;
        }

        // Function to Create and Join a Room with associated Room Code
        public static void CreateAndJoinRoom(string RoomCodeString)
        {
            // Sets the max players that can join to 10
            // Number will change depending on the game
            RoomOptions options = new RoomOptions();
            options.CustomRoomProperties = HostData.ToHashtable();
            options.MaxPlayers = 10;

            // This will join the room depending on the roomcode
            // If room doesn't exist, it creates the room
            HostData.setJoinCode(RoomCodeString);
            PhotonNetwork.CreateRoom(RoomCodeString, options, null);
            //sendData(HostData.CanHostJoinGame(), HostData.GetSelectedGame(), HostData.GetMaxNumPlayers());
        }

        public override void OnCreatedRoom()
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(HostData.ToHashtable());
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log("-----PLAYER ENTERED-----");
            Debug.Log(newPlayer.ToString());
            if(HostData.GetGame().AddPlayer(newPlayer))
            {
                Debug.Log("Added new player to game");
            }
            else
            {
                Debug.Log("Failed to add new player to game");
            }
            DoSomething();
        }

        public override void OnPlayerLeftRoom(Player playerToDisconnect)
        {
            Debug.Log("-----PLAYER LEFT (" + playerToDisconnect.NickName + ")-----");
            HostData.GetGame().DisconnectPlayerFromGame(playerToDisconnect);
        }

        /// WriteRoomCodeToFile()
        /// 
        /// Function used to write the Room Code to a File
        static void WriteRoomCodeToFile(string RoomCode)
        {
            File.WriteAllText("RoomCode.txt", RoomCode);
        }

        // Function that returns a list of all the players in a room
        public static ArrayList ListAllPlayers()
        {
            Debug.Log("Getting list of players from Photon Server (every 5 seconds)");

            ArrayList playerList = new ArrayList();
            foreach(var player in PhotonNetwork.PlayerList)
            {
                playerList.Add(player.NickName);
            }
            Debug.Log(playerList.ToString());
            return playerList;
        }

        public static void SetUsername(string user1Username)
        {
            PhotonNetwork.NickName = user1Username;
        }

        // This code acts as a way to send information to the client
        // it passes through an array of objects and the client
        // listens for the host call which is currently called
        // by pressing the settings button lol
        private void OnEnable()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnSignalSent;
        }

        private void OnDisable()
        {
             PhotonNetwork.NetworkingClient.EventReceived -= OnSignalSent;
        }

        private void OnSignalSent(EventData photonEvent)
        {
            if (photonEvent.Code == 1){
                object[] data = (object[])photonEvent.CustomData;
                string s = (string)data[0];
                bool test = (bool)data[1];
                int players = (int)data[2];
                Debug.Log(s);
                Debug.Log(test);
                Debug.Log(players);
            }
            // playing card event
            else if (photonEvent.Code == 2)
            {
                object[] data = (object[])photonEvent.CustomData;
                string username = (string)data[0];
                string cardType = (string)data[1];
                StandardCardRank rank = (StandardCardRank)data[2];
                StandardCardSuit suit = (StandardCardSuit)data[3];
                StandardCard card = new StandardCard(rank, suit);

                Debug.Log("Receiving a Played Card from " + username + ": " + card.ToString());
                int userIndex = HostData.GetGame().GetPlayerIndex(username);
                HostData.GetGame().DoMove(card, userIndex);
            }
            // verifying card event
            else if (photonEvent.Code == 4)
            {
                object[] data = (object[])photonEvent.CustomData;
                string cardType = (string)data[0];
                StandardCardRank rank = (StandardCardRank)data[1];
                StandardCardSuit suit = (StandardCardSuit)data[2];
                StandardCard card = new StandardCard(rank, suit);

                Debug.Log("Verifying a Card: " + card.ToString());
                string username = (string)data[3];
                bool isValid = HostData.GetGame().VerifyMove(card);
                SendThatCardIsValid(username, isValid);
            }
            // draw card event
            else if (photonEvent.Code == 7)
            {
                object[] data = (object[])photonEvent.CustomData;
                string username = (string)data[0];
                int numOfCards = (int)data[1];
                List<Card> cards = HostData.GetGame().DrawCardsFromDeck(numOfCards, DeckChoices.UNDEALT);
                SendCardsToPlayer(username, cards);
            }
            // skip turn event
            else if (photonEvent.Code == 10)
            {
                object[] data = (object[])photonEvent.CustomData;
                // just get the username in case we need it in the future
                string username = (string)data[0];
                HostData.GetGame().AdvanceTurn(true);
            }
        }

        /// <summary>
        /// Sends a list of cards to the player one by one
        /// </summary>
        /// <param name="username">PhotonNetwork.NickName of the player to send them to</param>
        /// <param name="cards">Cards to send</param>
        public void SendCardsToPlayer(string username, List<Card> cards)
        {
            if (cards[0].GetType().Name == "StandardCard")
            {
                foreach (Card card in cards)
                {
                    StandardCard cardToSend = (StandardCard)card;
                    object[] content = new object[] { username, cards[0].GetType().Name, cardToSend.GetRank(), cardToSend.GetSuit() };
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    PhotonNetwork.RaiseEvent(8, content, raiseEventOptions, SendOptions.SendUnreliable);
                }
            }
        }

        /// <summary>
        /// Sends whether or not the card was valid to the specific username
        /// </summary>
        /// <param name="username"></param>
        /// <param name="isValid"></param>
        public void SendThatCardIsValid(string username, bool isValid)
        {
            object[] content = new object[] { username, isValid };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(5, content, raiseEventOptions, SendOptions.SendUnreliable);
        }

        public void DoSomething()
        {
            string gameMode = HostData.GetSelectedGame();
            bool hostToggle = HostData.CanHostJoinGame();
            int maxPlayers = HostData.GetMaxNumPlayers();
            string hostName = PhotonNetwork.NickName;
            object[] content = new object[] {gameMode, hostToggle, maxPlayers, hostName};
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(1, content, raiseEventOptions, SendOptions.SendReliable);
        }
    }
}