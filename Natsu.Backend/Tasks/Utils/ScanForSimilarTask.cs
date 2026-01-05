using MongoDB.Bson;
using Natsu.Backend.Components;
using Natsu.Backend.Database.Helpers;

namespace Natsu.Backend.Tasks.Utils;

public class ScanForSimilarTask : IScheduledTask
{
    public string ID => "scan-similar";
    public string Name => "Scan for similar Images";

    public void Execute()
    {
        var files = TaggedFileHelper.All;

        foreach (var x in files)
        {
            foreach (var y in files)
            {
                if (x == y)
                    continue;

                try
                {
                    var sort = new List<ObjectId> { x.ID, y.ID };
                    sort = sort.Order().ToList();

                    var comp = TaggedFileHelper.GetComparison(sort.First(), sort.Last());
                    if (comp != null) continue;

                    var result = ImageComparer.CalculateSimilarity(x, y);
                    TaggedFileHelper.SaveComparison(sort.First(), sort.Last(), result);
                }
                catch (Exception)
                {
                    return;
                }
            }
        }
    }

    public TaskTrigger[] GetDefaultTriggers() => new TaskTrigger[]
    {
        new() { Ticks = new TimeSpan(12, 0, 0).Ticks }
    };
}
