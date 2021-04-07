using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Video;
using Button = UnityEngine.UI.Button;

namespace Music
{
    public class PlaylistController : MonoBehaviour
    {
        public VideoPlayer player;
        public GameObject musicPanel;
        public Button playButton;
        public Button pauseButton;
        public Text currentSongName;

        public GameObject songsListPanel;
        public GameObject songTemplate;

        //Songs list. Should match name of song in MusicFiles/<Name>
        public List<string> songNames;

        //Downlaoder
        private MusicDownloader downloader;

        private void Start()
        {
            musicPanel.SetActive(false);
            songNames = new List<string>();
            downloader = gameObject.AddComponent<MusicDownloader>();
            // Since object is MonoBehavior, instantiating occurs this way. Perhaps make non Monobehavior? 
        }

        private void Update()
        {
            // Playback songs update
            /*
            if (!player.isPlaying)
            {
                if (songNames.Count > 0)
                {
                    //TODO: Not complete, songs will be played from here. Also forgot to take pause case
                    //Enabling this effectively empties the list. 
                    player.source = VideoSource.Url;
                    player.url = MusicDownloader.MUSIC_FOLDER + songNames[0];

                    player.Play();
                    songNames.RemoveAt(0);
                }
            }*/

            // UI (scroll panel) update
            // Remove all old children
            while (songsListPanel.transform.childCount > 0)
            {
                DestroyImmediate(songsListPanel.transform.GetChild(0).gameObject);
            }

            // Add songs to song panel
            foreach (string songName in songNames)
            {
                GameObject songView = Instantiate(songTemplate, songsListPanel.transform);
                songView.transform.Find("SongName").gameObject.GetComponent<Text>().text = songName;
                songView.transform.Find("DeleteSong").gameObject.GetComponent<Button>().onClick
                    .AddListener(delegate { onRemoveSongButtonClick(songName); });
                songView.SetActive(true);
            }
        }

        public void onMusicButtonClick()
        {
            musicPanel.SetActive(!musicPanel.activeSelf);
        }

        public void onPlayButtonClick()
        {
            player.Play();
            playButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(true);
        }

        public void onPauseButtonClick()
        {
            player.Pause();
            pauseButton.gameObject.SetActive(false);
            playButton.gameObject.SetActive(true);
        }

        public void onSkipButtonClick()
        {
            //TODO
        }

        public void onClearAllButtonClick()
        {
            //TODO
        }

        public void onShuffleButtonClick()
        {
            //TODO
        }

        public void onRemoveSongButtonClick(string songName)
        {
            //TODO   
        }

        //Called by Chat controller
        public void SearchAndAddSong(string songName, string sender)
        {
            downloader.DownloadSong(songName).ContinueWith(foundSong =>
            {
                if (foundSong.Result == null)
                {
                    //Error case
                    Debug.Log("Not found!");
                    NotifyClient(sender, false);
                    return;
                }

                Debug.Log("Song Added: " + songName);
                songNames.Add(foundSong.Result);
                HostData.SetDoShowNotificationWindow(true, "The song: " + songName + " is added by " + sender);
                NotifyClient(sender, true);
                //Update can pick it from here and show it in UI 
            });
        }

        public void NotifyClient(string sender, bool result)
        {
            object[] content = new object[] { sender, result };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(27, content, raiseEventOptions, SendOptions.SendUnreliable);
        }
    }
}