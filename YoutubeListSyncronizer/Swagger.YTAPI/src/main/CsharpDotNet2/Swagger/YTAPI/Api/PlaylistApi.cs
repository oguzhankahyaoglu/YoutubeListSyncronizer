using System;
using System.Collections.Generic;
using RestSharp;
using Swagger.YTAPI.Client;
using Swagger.YTAPI.Model;

namespace Swagger.YTAPI.Api
{
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IPlaylistApi
    {
        /// <summary>
        ///  
        /// </summary>
        /// <param name="playlistid"></param>
        /// <returns>YoutubePlaylist</returns>
        YoutubePlaylist Get (string playlistid);
    }
  
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public class PlaylistApi : IPlaylistApi
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistApi"/> class.
        /// </summary>
        /// <param name="apiClient"> an instance of ApiClient (optional)</param>
        /// <returns></returns>
        public PlaylistApi(ApiClient apiClient = null)
        {
            if (apiClient == null) // use the default one in Configuration
                this.ApiClient = Configuration.DefaultApiClient; 
            else
                this.ApiClient = apiClient;
        }
    
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistApi"/> class.
        /// </summary>
        /// <returns></returns>
        public PlaylistApi(String basePath)
        {
            this.ApiClient = new ApiClient(basePath);
        }
    
        /// <summary>
        /// Sets the base path of the API client.
        /// </summary>
        /// <param name="basePath">The base path</param>
        /// <value>The base path</value>
        public void SetBasePath(String basePath)
        {
            this.ApiClient.BasePath = basePath;
        }
    
        /// <summary>
        /// Gets the base path of the API client.
        /// </summary>
        /// <param name="basePath">The base path</param>
        /// <value>The base path</value>
        public String GetBasePath(String basePath)
        {
            return this.ApiClient.BasePath;
        }
    
        /// <summary>
        /// Gets or sets the API client.
        /// </summary>
        /// <value>An instance of the ApiClient</value>
        public ApiClient ApiClient {get; set;}
    
        /// <summary>
        ///  
        /// </summary>
        /// <param name="playlistid"></param> 
        /// <returns>YoutubePlaylist</returns>            
        public YoutubePlaylist Get (string playlistid)
        {
            
            // verify the required parameter 'playlistid' is set
            if (playlistid == null) throw new ApiException(400, "Missing required parameter 'playlistid' when calling Get");
            
    
            var path = "/api/Playlist/{playlistid}";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "playlistid" + "}", ApiClient.ParameterToString(playlistid));
    
            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, FileParameter>();
            String postBody = null;
    
                                                    
            // authentication setting, if any
            String[] authSettings = new String[] {  };
    
            // make the HTTP request
            IRestResponse response = (IRestResponse) ApiClient.CallApi(path, Method.GET, queryParams, postBody, headerParams, formParams, fileParams, authSettings);
    
            if (((int)response.StatusCode) >= 400)
                throw new ApiException ((int)response.StatusCode, "Error calling Get: " + response.Content, response.Content);
            else if (((int)response.StatusCode) == 0)
                throw new ApiException ((int)response.StatusCode, "Error calling Get: " + response.ErrorMessage, response.ErrorMessage);
    
            return (YoutubePlaylist) ApiClient.Deserialize(response.Content, typeof(YoutubePlaylist), response.Headers);
        }
    
    }
}
