using System;
using System.Collections.Generic;
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
        public GameObject songsListPanel;
        public Button playButton;
        public Button pauseButton;
        public Text currentSongName;

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
            // UI (scroll panel) update


            // Played songs update
            if (!player.isPlaying)
            {
                if (songNames.Count > 0)
                {
                    //TODO: Not complete, songs will be played from here. Also forgot to take pause case
                    player.source = VideoSource.Url;
                    player.url = MusicDownloader.MUSIC_FOLDER + songNames[0];

                    player.Play();
                    songNames.RemoveAt(0);
                }
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
        public void SearchAndAddSong(string songName)
        {
            downloader.DownloadSong(songName).ContinueWith(foundSong =>
            {
                songNames.Add(foundSong.Result);
                //Update can pick it from here and show it in UI 
            });
        }
    }
}