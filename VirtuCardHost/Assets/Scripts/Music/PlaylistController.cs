using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Video;
using Button = UnityEngine.UI.Button;
using Random = UnityEngine.Random;

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
        private bool songSourceSet;
        private bool isPaused = false;
        private bool justSwappedSongs = false;

        //Downlaoder
        private MusicDownloader downloader;

        private void Start()
        {
            musicPanel.SetActive(false);
            songSourceSet = false;
            isPaused = false;
            justSwappedSongs = false;
            songNames = new List<string>();
            downloader = new MusicDownloader();

            // delete all songs in the folder
            System.IO.DirectoryInfo di = new DirectoryInfo(MusicDownloader.MUSIC_FOLDER);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }

        private void Update()
        {
            // if the song source is not set and the song list is not empty
            if (MusicDownloader.fileBeingWritten == false && songSourceSet == false && songNames.Count > 0 &&
                player.isPlaying == false)
            {
                PlaySong(songNames[0]);
            }
            else if (songSourceSet == true && isPaused == false && player.isPlaying == false &&
                     justSwappedSongs == false)
            {
                // it has finished playing, so play the next one and delete the old one

                // delete song that just finished playing
                File.Delete(MusicDownloader.MUSIC_FOLDER + songNames[0]);
                songNames.RemoveAt(0);


                // play next song
                if (songNames.Count > 0)
                {
                    PlaySong(songNames[0]);
                }
                else
                {
                    // end of playlist
                    songSourceSet = false;
                    justSwappedSongs = false;
                }
            }

            if (player.isPlaying)
            {
                justSwappedSongs = false;
            }

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

        private void PlaySong(string songName)
        {
            currentSongName.text = songName;
            songSourceSet = true;
            justSwappedSongs = true;
            player.source = VideoSource.Url;
            player.url = MusicDownloader.MUSIC_FOLDER + songName;
            StartCoroutine(PlaySong());

            if (!isPaused)
            {
                playButton.gameObject.SetActive(false);
                pauseButton.gameObject.SetActive(true);
            }
            else
            {
                playButton.gameObject.SetActive(true);
                pauseButton.gameObject.SetActive(false);
            }
        }

        IEnumerator PlaySong()
        {
            player.Prepare();
            while (!player.isPrepared)
            {
                yield return null;
            }

            if (!isPaused)
            {
                player.Play();
            }
        }

        public void onMusicButtonClick()
        {
            musicPanel.SetActive(!musicPanel.activeSelf);
        }

        public void onPlayButtonClick()
        {
            player.Play();
            isPaused = false;
            playButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(true);
        }

        public void onPauseButtonClick()
        {
            player.Pause();
            isPaused = true;
            pauseButton.gameObject.SetActive(false);
            playButton.gameObject.SetActive(true);
        }

        public void onSkipButtonClick()
        {
            if (songNames.Count > 0)
            {
                player.Stop();
                songNames.RemoveAt(0);
                if (songNames.Count > 0)
                {
                    PlaySong(songNames[0]);
                }
                else
                {
                    songSourceSet = false;
                    justSwappedSongs = false;
                }
            }
        }

        public void onClearAllButtonClick()
        {
            //TODO
        }

        public void onShuffleButtonClick()
        {
            if (songNames.Count > 1)
            {
                songNames = songNames.OrderBy(x => Random.value).ToList();
                player.Stop();
                PlaySong(songNames[0]);
            }
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

        //Called by Chat controller
        public async void SearchAndAddSongAsync(string songName, string sender)
        {
            string fileName = await downloader.DownloadSong(songName);
            //await Task.Delay(10000);

            if (fileName == null)
            {
                //Error case
                Debug.Log("Not found!");
                NotifyClient(sender, false);
                return;
            }

            Debug.Log("Song Added: " + songName);
            songNames.Add(fileName);
            HostData.SetDoShowNotificationWindow(true, "The song: " + songName + " is added by " + sender);
            NotifyClient(sender, true);
        }

        public void NotifyClient(string sender, bool result)
        {
            object[] content = new object[] {sender, result};
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
            PhotonNetwork.RaiseEvent(27, content, raiseEventOptions, SendOptions.SendUnreliable);
        }
    }
}