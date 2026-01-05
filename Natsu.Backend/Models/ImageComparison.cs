using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Natsu.Backend.Models;

public class ImageComparison
{
    [BsonId]
    public ObjectId ID { get; init; } = ObjectId.GenerateNewId();

    [BsonElement("one")]
    public ObjectId File1 { get; init; }

    [BsonElement("two")]
    public ObjectId File2 { get; init; }

    [BsonElement("similarity")]
    public float Similarity { get; init; }
}
