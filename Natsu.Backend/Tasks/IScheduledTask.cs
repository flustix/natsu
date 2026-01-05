namespace Natsu.Backend.Tasks;

public interface IScheduledTask
{
    string ID { get; }
    string Name { get; }

    void Execute();
    TaskTrigger[] GetDefaultTriggers();
}
