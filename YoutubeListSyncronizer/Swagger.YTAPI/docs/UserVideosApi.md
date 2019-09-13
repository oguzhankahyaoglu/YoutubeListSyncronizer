# Swagger.YTAPI..UserVideosApi

All URIs are relative to *http://ytdownloader-api.herokuapp.com/swagger/v1/swagger.json*

Method | HTTP request | Description
------------- | ------------- | -------------
[**Get**](UserVideosApi.md#get) | **GET** /api/UserVideos/{Username} | 


<a name="get"></a>
# **Get**
> YoutubeUserVideos Get (string username)



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
            
            var apiInstance = new UserVideosApi();
            var username = username_example;  // string | 

            try
            {
                YoutubeUserVideos result = apiInstance.Get(username);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling UserVideosApi.Get: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **username** | **string**|  | 

### Return type

[**YoutubeUserVideos**](YoutubeUserVideos.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

