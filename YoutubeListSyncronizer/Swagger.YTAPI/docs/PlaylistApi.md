# Swagger.YTAPI..PlaylistApi

All URIs are relative to *http://ytdownloader-api.herokuapp.com/swagger/v1/swagger.json*

Method | HTTP request | Description
------------- | ------------- | -------------
[**Get**](PlaylistApi.md#get) | **GET** /api/Playlist/{playlistid} | 


<a name="get"></a>
# **Get**
> YoutubePlaylist Get (string playlistid)



### Example
```csharp
using System;
using System.Diagnostics;
using Swagger.YTAPI.Api;
using Swagger.YTAPI.Client;
using Swagger.YTAPI.Model;

namespace Example
{
    public class GetExample
    {
        public void main()
        {
            
            var apiInstance = new PlaylistApi();
            var playlistid = playlistid_example;  // string | 

            try
            {
                YoutubePlaylist result = apiInstance.Get(playlistid);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling PlaylistApi.Get: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **playlistid** | **string**|  | 

### Return type

[**YoutubePlaylist**](YoutubePlaylist.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

