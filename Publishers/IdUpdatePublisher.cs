using PoProj.classes;

namespace PoProj.publishers;

public class IdUpdatePublisher
{
    private readonly List<IUpdateID> _subscribers = new ();

    // Notifying all subscribers about id change
    public void NotifySubscribers(ulong oldId, ulong newId)
    {
        foreach (var idUpdate in _subscribers)
        {
            idUpdate.UpdateId(oldId, newId);
        }
    }

    // Adding new subscriber
    public void Subscribe(IUpdateID idUpdate)
    {
        _subscribers.Add(idUpdate);
    }
}