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
    public interface IUserVideosApi
    {
        /// <summary>
        ///  
        /// </summary>
        /// <param name="username"></param>
        /// <returns>YoutubeUserVideos</returns>
        YoutubeUserVideos Get (string username);
    }
  
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public class UserVideosApi : IUserVideosApi
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserVideosApi"/> class.
        /// </summary>
        /// <param name="apiClient"> an instance of ApiClient (optional)</param>
        /// <returns></returns>
        public UserVideosApi(ApiClient apiClient = null)
        {
            if (apiClient == null) // use the default one in Configuration
                this.ApiClient = Configuration.DefaultApiClient; 
            else
                this.ApiClient = apiClient;
        }
    
        /// <summary>
        /// Initializes a new instance of the <see cref="UserVideosApi"/> class.
        /// </summary>
        /// <returns></returns>
        public UserVideosApi(String basePath)
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
        /// <param name="username"></param> 
        /// <returns>YoutubeUserVideos</returns>            
        public YoutubeUserVideos Get (string username)
        {
            
            // verify the required parameter 'username' is set
            if (username == null) throw new ApiException(400, "Missing required parameter 'username' when calling Get");
            
    
            var path = "/api/UserVideos/{Username}";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "Username" + "}", ApiClient.ParameterToString(username));
    
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
    
            return (YoutubeUserVideos) ApiClient.Deserialize(response.Content, typeof(YoutubeUserVideos), response.Headers);
        }
    
    }
}
