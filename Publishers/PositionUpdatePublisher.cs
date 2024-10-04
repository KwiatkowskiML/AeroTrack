namespace PoProj.Publishers;

public class PositionUpdatePublisher
{
    private readonly List<IUpdatePosition> _subscribers = new ();

    // Notifying all subscribers about position update
    public void NotifySubscribers(ulong oldId, float longitude, float latitude, float amsl)
    {
        foreach (var iUpdatePosition in _subscribers)
        {
            iUpdatePosition.UpdatePosition(oldId, longitude, latitude, amsl);
        }
    }

    // Adding new subscriber
    public void Subscribe(IUpdatePosition idUpdate)
    {
        _subscribers.Add(idUpdate);
    }
}