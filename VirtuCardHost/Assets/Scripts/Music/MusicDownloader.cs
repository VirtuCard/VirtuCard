using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VideoLibrary;
using UnityEngine.UI;
using System.Linq;
using System.Runtime.CompilerServices;

public class MusicDownloader : MonoBehaviour
{
    public readonly static string MUSIC_FOLDER = "MusicFiles/";

    public Button theButton;

    // Start is called before the first frame update
    void Start()
    {
        theButton.onClick.AddListener(delegate { DoTheThing(); });
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DoTheThing()
    {
        //SaveVideoToDiskAsync(VIDEO_URL_HERE);
    }

    /// <summary>
    /// Retrieves the youtube video from the link and writes it to the music folder 
    /// Warning this will be very slow being it is synchronous. Use <see cref="SaveVideoToDiskAsync(string)"/> instead
    /// </summary>
    /// <param name="link">The URI to the video</param>
    public void SaveVideoToDisk(string link)
    {
        var youTube = YouTube.Default; // starting point for YouTube actions
        var video = youTube.GetVideo(link); // gets a Video object with info about the video
        File.WriteAllBytes(MUSIC_FOLDER + video.FullName, video.GetBytes());
    }

    /// <summary>
    /// Retrieves the youtube video from the link and writes it to the music folder
    /// </summary>
    /// <param name="link">The URI to the video</param>
    public async void SaveVideoToDiskAsync(string link)
    {
        var youTube = YouTube.Default; // starting point for YouTube actions
        var videoInfo = await youTube.GetVideoAsync(link);
        File.WriteAllBytes(MUSIC_FOLDER + videoInfo.FullName, videoInfo.GetBytes());
    }
}