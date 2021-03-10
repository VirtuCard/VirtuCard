using System.Collections;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using ExitGames.Client.Photon;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Realtime;

namespace PhotonScripts
{
    public class NetworkController : MonoBehaviourPunCallbacks
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

        public override void OnConnectedToMaster()
        {
            Debug.Log("The Host is now connected to the " + PhotonNetwork.CloudRegion + " server.");
            Debug.Log("Generated Room Code is " + RoomCode);
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

       
        /// WriteRoomCodeToFile()
        /// 
        /// Function used to write the Room Code to a File
        static void WriteRoomCodeToFile(string RoomCode)
        {
            File.WriteAllText("RoomCode.txt", RoomCode);
        }

        // Function that returns a list of all the players in a room
        public ArrayList ListAllPlayers()
        {
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
                string updated = (string) photonEvent[0];
                Debug.Log(updated);
            }
        }
        public void DoSomething()
        {
            Debug.Log("settings was clicked");
            string s = "Hello Darkness";
            //int test = 69;
            object[] content = new object[] {s};
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(1, content, raiseEventOptions, SendOptions.SendReliable);
        }
    }
}