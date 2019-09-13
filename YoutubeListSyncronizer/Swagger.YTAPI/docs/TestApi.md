# Swagger.YTAPI..TestApi

All URIs are relative to *http://ytdownloader-api.herokuapp.com/swagger/v1/swagger.json*

Method | HTTP request | Description
------------- | ------------- | -------------
[**Index**](TestApi.md#index) | **GET** /api/Test | 


<a name="index"></a>
# **Index**
> DateTime? Index ()



### Example
```csharp
using System;
using System.Diagnostics;
using Swagger.YTAPI.Api;
using Swagger.YTAPI.Client;
using Swagger.YTAPI.Model;

namespace Example
{
    public class IndexExample
    {
        public void main()
        {
            
            var apiInstance = new TestApi();

            try
            {
                DateTime? result = apiInstance.Index();
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TestApi.Index: " + e.Message );
            }
        }
    }
}
```

### Parameters
This endpoint does not need any parameter.

### Return type

**DateTime?**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

