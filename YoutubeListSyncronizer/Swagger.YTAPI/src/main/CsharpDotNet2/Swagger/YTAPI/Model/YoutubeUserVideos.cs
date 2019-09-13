using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Swagger.YTAPI.Model {

  /// <summary>
  /// 
  /// </summary>
  [DataContract]
  public class YoutubeUserVideos {
    /// <summary>
    /// Gets or Sets PlaylistName
    /// </summary>
    [DataMember(Name="playlistName", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "playlistName")]
    public string PlaylistName { get; set; }

    /// <summary>
    /// Gets or Sets VideoIDsDictionary
    /// </summary>
    [DataMember(Name="videoIDsDictionary", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "videoIDsDictionary")]
    public Dictionary<String, string> VideoIDsDictionary { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()  {
      var sb = new StringBuilder();
      sb.Append("class YoutubeUserVideos {\n");
      sb.Append("  PlaylistName: ").Append(PlaylistName).Append("\n");
      sb.Append("  VideoIDsDictionary: ").Append(VideoIDsDictionary).Append("\n");
      sb.Append("}\n");
      return sb.ToString();
    }

    /// <summary>
    /// Get the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public string ToJson() {
      return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

}
}
