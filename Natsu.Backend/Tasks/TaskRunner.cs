using System.Diagnostics;
using Midori.Logging;
using MongoDB.Driver;
using Natsu.Backend.Database;

namespace Natsu.Backend.Tasks;

public static class TaskRunner
{
    private static IMongoCollection<ScheduledTaskInfo> db => MongoDatabase.GetCollection<ScheduledTaskInfo>("scheduled-tasks");
    private static List<(ScheduledTaskInfo info, IScheduledTask task)> tasks { get; } = new();

    private static List<IScheduledTask> current { get; } = new();

    private static bool running = false;

    public static void RegisterTask(IScheduledTask task)
    {
        var info = getInfo(task);
        tasks.Add((info, task));
    }

    public static void Start()
    {
        if (running)
            return;

        running = true;

        // ReSharper disable once UseObjectOrCollectionInitializer
        var thread = new Thread(() =>
        {
            while (running)
            {
                try
                {
                    foreach (var (info, task) in tasks)
                    {
                        var run = info.Triggers.Any(t =>
                        {
                            switch (t.Type)
                            {
                                case TriggerType.Interval:
                                    return DateTimeOffset.Now.ToUnixTimeSeconds() - info.LastRun >= t.Ticks!.Value / TimeSpan.TicksPerSecond;

                                default:
                                    return false;
                            }
                        });

                        if (!run)
                            continue;

                        lock (current)
                        {
                            if (current.Contains(task))
                                return;

                            current.Add(task);
                        }

                        info.LastRun = DateTimeOffset.Now.ToUnixTimeSeconds();
                        updateInfo(info);

                        Logger.Log($"Starting task '{task.ID}' ({task.GetType().Name}).");
                        Task.Run(() =>
                        {
                            var sw = new Stopwatch();
                            sw.Start();

                            try
                            {
                                task.Execute();
                            }
                            finally
                            {
                                sw.Stop();

                                lock (current)
                                    current.Remove(task);

                                info.Duration = (long)sw.Elapsed.TotalSeconds;
                                updateInfo(info);

                                Logger.Log($"Finished task '{task.ID}' ({task.GetType().Name}) took {info.Duration}s to finish.");
                            }
                        });
                    }

                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Failed to loop TaskRunner.");
                }
            }
        });

        thread.Name = "TaskRunner";
        thread.Start();
    }

    private static void updateInfo(ScheduledTaskInfo info) => db.ReplaceOne(x => x.ID == info.ID, info);

    private static ScheduledTaskInfo getInfo(IScheduledTask task)
    {
        var info = db.Find(x => x.ID == task.ID).FirstOrDefault();

        if (info is null)
        {
            info = new ScheduledTaskInfo()
            {
                ID = task.ID,
                Triggers = task.GetDefaultTriggers().ToList()
            };

            db.InsertOne(info);
        }

        return info;
    }
}
