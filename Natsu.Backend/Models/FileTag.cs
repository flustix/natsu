using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Natsu.Backend.Models;

[JsonObject(MemberSerialization.OptIn)]
public class FileTag
{
    /// <summary>
    /// The ID of the tag.
    /// </summary>
    [BsonId]
    [JsonProperty("id")]
    public ObjectId ID { get; set; } = ObjectId.GenerateNewId();

    /// <summary>
    /// The name of the tag.
    /// </summary>
    [BsonElement("name")]
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The color of the tag in RGB separated by commas (e.g., "255,0,0" for red).
    /// </summary>
    [BsonElement("color")]
    [JsonProperty("color")]
    public string RGBColor { get; set; } = "163,163,245";

    /// <summary>
    /// The parents of this tag. When searching the parent tag, file with this tag will also be included.
    /// </summary>
    [BsonElement("parents")]
    [JsonProperty("parents")]
    public List<ObjectId> Parents { get; set; } = new();

    /// <summary>
    /// The Owner of this tag.
    /// </summary>
    [BsonElement("owner")]
    [JsonProperty("owner")]
    public ObjectId Owner { get; set; } = ObjectId.Empty;
}
