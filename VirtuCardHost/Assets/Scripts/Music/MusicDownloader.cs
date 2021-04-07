using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VideoLibrary;
using UnityEngine.UI;
using System.Linq;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System.Threading.Tasks;
using System;

public class MusicDownloader : MonoBehaviour
{
    public readonly static string MUSIC_FOLDER = "MusicFiles/";

    /*
    Commenting out Button/Text input for now
    public Button theButton;
    public InputField textInput;
    */
    
    public class VideoReturnInfo
    {
        public string songName;
        public string videoUrl;
    }

    // Start is called before the first frame update
    void Start()
    {
      //  theButton.onClick.AddListener(delegate { DoTheThing(); });
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void DoTheThing()
    {
    //    DownloadSong(textInput.text);
    }

    /// <summary>
    /// This downloads the first youtube video matching the keyword into the music folder.
    /// Returns the name of the file that the video was saved under
    /// </summary>
    /// <param name="keyword"></param>
    /// <returns></returns>
    public async Task<string> DownloadSong(string keyword)
    {
        var returnInfo = await FindVideoUrlAsync(keyword);

        if (String.IsNullOrEmpty(returnInfo.videoUrl))
        {
            Debug.Log("URL not found for " + keyword);
            return null;
        }

        Debug.Log("URL found: " + returnInfo.videoUrl);

        string fileNameToSaveAs = returnInfo.songName + ".mp4";
        SaveVideoToDiskAsync(returnInfo.videoUrl, fileNameToSaveAs);
        return fileNameToSaveAs;
    }

    /// <summary>
    /// This finds the youtube video url for the <paramref name="keywordToSearchFor"/>.
    /// </summary>
    /// <param name="keywordToSearchFor">The youtube keyword or string of keywords to search for</param>
    /// <returns></returns>
    public async Task<VideoReturnInfo> FindVideoUrlAsync(string keywordToSearchFor)
    {
        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = "AIzaSyCSXouAZn244EUDSW-M1iJI2UemWnSEdkI",
            ApplicationName = this.GetType().ToString()
        });

        var searchListRequest = youtubeService.Search.List("snippet");
        searchListRequest.Q = keywordToSearchFor; // Replace with your search term.
        searchListRequest.MaxResults = 1;

        // Call the search.list method to retrieve results matching the specified query term.
        var searchListResponse = await searchListRequest.ExecuteAsync();

        VideoReturnInfo returnInfo = new VideoReturnInfo();
        bool didSucceed = false;

        // Add each result to the appropriate list, and then display the lists of
        // matching videos, channels, and playlists.
        foreach (var searchResult in searchListResponse.Items)
        {
            switch (searchResult.Id.Kind)
            {
                case "youtube#video":
                    // Removing any thing that can cause issues with file IO
                    returnInfo.songName =
                        searchResult.Snippet.Title.Replace(":", "").Replace("*", "").Replace("\"", "");
                    returnInfo.videoUrl = String.Format("https://www.youtube.com/watch?v={0}", searchResult.Id.VideoId);
                    didSucceed = true;
                    break;
            }
        }

        if (didSucceed)
            return returnInfo;
        return null;
    }

    /// <summary>
    /// Retrieves the youtube video from the link and writes it to the music folder
    /// </summary>
    /// <param name="link">The URI to the video</param>
    public async void SaveVideoToDiskAsync(string link, string fileName)
    {
        var youTube = YouTube.Default; // starting point for YouTube actions
        var videoInfo = await youTube.GetVideoAsync(link);
        if (!Directory.Exists(MUSIC_FOLDER))
        {
            Directory.CreateDirectory(MUSIC_FOLDER); // Creates directory, if not present
        }

        File.WriteAllBytes(MUSIC_FOLDER + fileName, videoInfo.GetBytes());
    }

    /// <summary>
    /// This finds the youtube video url for the <paramref name="keywordToSearchFor"/>.
    /// Warning this will be slow due to being synchronous. Use <see cref="FindVideoUrlAsync(string)"/> instead.
    /// </summary>
    /// <param name="keywordToSearchFor"></param>
    /// <returns></returns>
    private string FindVideoUrl(string keywordToSearchFor)
    {
        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = "AIzaSyCSXouAZn244EUDSW-M1iJI2UemWnSEdkI",
            ApplicationName = this.GetType().ToString()
        });

        var searchListRequest = youtubeService.Search.List("snippet");
        searchListRequest.Q = keywordToSearchFor; // Replace with your search term.
        searchListRequest.MaxResults = 1;

        // Call the search.list method to retrieve results matching the specified query term.
        var searchListResponse = searchListRequest.Execute();

        List<string> videoLinks = new List<string>();
        List<string> channels = new List<string>();
        List<string> playlists = new List<string>();

        // Add each result to the appropriate list, and then display the lists of
        // matching videos, channels, and playlists.
        foreach (var searchResult in searchListResponse.Items)
        {
            switch (searchResult.Id.Kind)
            {
                case "youtube#video":
                    videoLinks.Add(String.Format("https://www.youtube.com/watch?v={0}", searchResult.Id.VideoId));
                    break;

                case "youtube#channel":
                    channels.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
                    break;

                case "youtube#playlist":
                    playlists.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.PlaylistId));
                    break;
            }
        }

        if (videoLinks.Count == 0)
        {
            return String.Empty;
        }

        return videoLinks.First();
    }

    /// <summary>
    /// Retrieves the youtube video from the link and writes it to the music folder 
    /// Warning this will be very slow being it is synchronous. Use <see cref="SaveVideoToDiskAsync(string)"/> instead
    /// </summary>
    /// <param name="link">The URI to the video</param>
    public void SaveVideoToDisk(string link, string fileName)
    {
        var youTube = YouTube.Default; // starting point for YouTube actions
        var video = youTube.GetVideo(link); // gets a Video object with info about the video
        File.WriteAllBytes(MUSIC_FOLDER + fileName, video.GetBytes());
    }
}