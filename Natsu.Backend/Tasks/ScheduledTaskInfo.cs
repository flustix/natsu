using MongoDB.Bson.Serialization.Attributes;

namespace Natsu.Backend.Tasks;

public class ScheduledTaskInfo
{
    [BsonId]
    public string ID { get; set; } = null!;

    [BsonElement("triggers")]
    public List<TaskTrigger> Triggers { get; set; } = new();

    [BsonElement("last")]
    public long LastRun { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();

    [BsonElement("duration")]
    public long Duration { get; set; } = 0;
}
