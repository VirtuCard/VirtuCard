using System.Collections;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using ExitGames.Client.Photon;

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
                RoomCodeNum[i] = Random.Range(65, 90);
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
            options.MaxPlayers = 10;

            // This will join the room depending on the roomcode
            // If room doesn't exist, it creates the room
            HostData.setJoinCode(RoomCodeString);
            PhotonNetwork.CreateRoom(RoomCodeString, options, null);

             //sendData(HostData.CanHostJoinGame(), HostData.GetSelectedGame(), HostData.GetMaxNumPlayers());
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
        }
        public void DoSomething()
        {
            string gameMode = HostData.GetSelectedGame();
            bool hostToggle = HostData.CanHostJoinGame();
            int maxPlayers = HostData.GetMaxNumPlayers();
            object[] content = new object[] {gameMode, hostToggle, maxPlayers};
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(1, content, raiseEventOptions, SendOptions.SendReliable);
        }
    }
}