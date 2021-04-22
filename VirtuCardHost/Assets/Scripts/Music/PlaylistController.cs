using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using RenderHeads.Media.AVProVideo;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Video;
using Button = UnityEngine.UI.Button;
using Random = UnityEngine.Random;

namespace Music
{
    public class PlaylistController : MonoBehaviour
    {
        private readonly static string EMPTY_MESSAGE = "Add a song to the queue using !play <song>";

        public float initialVolumeLevel;

        //      public VideoPlayer player;
        public MediaPlayer mediaPlayer;
        public GameObject musicPanel;
        public Button playButton;
        public Button pauseButton;
        public UnityEngine.UI.Slider volumeControlSlider;
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

            volumeControlSlider.SetValueWithoutNotify(initialVolumeLevel);
            volumeControlSlider.onValueChanged.AddListener(delegate { SetVolume(volumeControlSlider.value); });
            mediaPlayer.Control.SetVolume(initialVolumeLevel);

            // delete all songs in the folder
            System.IO.DirectoryInfo di = new DirectoryInfo(MusicDownloader.MUSIC_FOLDER);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }

        private void Update()
        {
            if (songNames.Count == 0)
            {
                currentSongName.text = EMPTY_MESSAGE;
                // Should i return here? If there's noticeable lag while running maybe add a return here 
            }

            // if the song source is not set and the song list is not empty
            if (MusicDownloader.fileBeingWritten == false && songSourceSet == false && songNames.Count > 0 &&
                (mediaPlayer.Control.IsPlaying() == false || mediaPlayer.Control.IsFinished() == true))
            {
                PlaySong(songNames[0]);
            }
            else if (songSourceSet == true && isPaused == false && mediaPlayer.Control.IsFinished() == true &&
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

                ReformatPlaylist();
            }

            if (mediaPlayer.Control.IsPlaying())
            {
                justSwappedSongs = false;
            }

            // UI (scroll panel) update
            // Remove all old children
            /*
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
            */
        }

        public void SetVolume(float volume)
        {
            mediaPlayer.Control.SetVolume(volume);
        }

        private void PlaySong(string songName)
        {
            currentSongName.text = songName;
            songSourceSet = true;
            justSwappedSongs = true;

            bool isOpening = mediaPlayer.OpenMedia(
                new MediaPath(MusicDownloader.MUSIC_FOLDER + songName + ".mp4",
                    MediaPathType.AbsolutePathOrURL), autoPlay: true);

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
            mediaPlayer.Control.SetVolume(volumeControlSlider.value);
        }

        public void onMusicButtonClick()
        {
            musicPanel.SetActive(!musicPanel.activeSelf);
            ReformatPlaylist();
        }

        public void onPlayButtonClick()
        {
            if (songNames.Count > 0)
            {
                mediaPlayer.Play();
                isPaused = false;
                playButton.gameObject.SetActive(false);
                pauseButton.gameObject.SetActive(true);
            }
        }

        public void onPauseButtonClick()
        {
            mediaPlayer.Pause();
            isPaused = true;
            pauseButton.gameObject.SetActive(false);
            playButton.gameObject.SetActive(true);
        }

        public void onSkipButtonClick()
        {
            if (songNames.Count > 0)
            {
                mediaPlayer.Stop();
                string storeName = songNames[0];
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

                ReformatPlaylist();
                // Keep file deletion for the end
                File.Delete(MusicDownloader.MUSIC_FOLDER + storeName);
            }
        }

        public void onClearAllButtonClick()
        {
            if (songNames.Count > 0)
            {
                songNames.Clear();
                mediaPlayer.Stop();
                playButton.gameObject.SetActive(true);
                pauseButton.gameObject.SetActive(false);
                ReformatPlaylist();
                mediaPlayer.CloseMedia();
                songSourceSet = false;
                justSwappedSongs = false;
            }
        }

        public void onMuteButtonClick()
        {
            mediaPlayer.AudioVolume = mediaPlayer.AudioVolume == 0.0f ? volumeControlSlider.value : 0.0f;
        }

        public void onShuffleButtonClick()
        {
            if (songNames.Count > 1)
            {
                songNames = songNames.OrderBy(x => Random.value).ToList();
                mediaPlayer.Stop();
                PlaySong(songNames[0]);
            }

            ReformatPlaylist();
        }

        public void onRemoveSongButtonClick(string songName)
        {
            if (currentSongName.text.Equals(songName))
            {
                mediaPlayer.Stop();
                // if it is the current song playing
                songNames.RemoveAt(0);
                File.Delete(MusicDownloader.MUSIC_FOLDER + songName);
                if (songNames.Count > 0)
                {
                    PlaySong(songNames[0]);
                }
                else
                {
                    currentSongName.text = String.Empty;
                    songSourceSet = false;
                    justSwappedSongs = false;
                    playButton.gameObject.SetActive(true);
                    pauseButton.gameObject.SetActive(false);
                }
            }
            else
            {
                songNames.Remove(songName);
                File.Delete(MusicDownloader.MUSIC_FOLDER + songName);
            }

            ReformatPlaylist();
        }

        public void ReformatPlaylist()
        {
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

            // add the UI panel for the new song
            GameObject songView = Instantiate(songTemplate, songsListPanel.transform);
            songView.transform.Find("SongName").gameObject.GetComponent<Text>().text = fileName;
            songView.transform.Find("DeleteSong").gameObject.GetComponent<Button>().onClick
                .AddListener(delegate { onRemoveSongButtonClick(fileName); });
            songView.SetActive(true);

            // send out notifications
            HostData.SetDoShowNotificationWindow(true, "The song: \"" + songName + "\" was added by " + sender);
            NotifyClient(sender, true);
        }

        public void NotifyClient(string sender, bool result)
        {
            object[] content = new object[] {sender, result};
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
            PhotonNetwork.RaiseEvent((int)NetworkEventCodes.SongVerification, content, raiseEventOptions, SendOptions.SendUnreliable);
        }
    }
}