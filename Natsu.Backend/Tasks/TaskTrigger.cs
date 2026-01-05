using MongoDB.Bson.Serialization.Attributes;

namespace Natsu.Backend.Tasks;

public class TaskTrigger
{
    [BsonElement("type")]
    public TriggerType Type { get; set; }

    [BsonElement("ticks")]
    public long? Ticks { get; set; }
}

public enum TriggerType
{
    Interval
}
