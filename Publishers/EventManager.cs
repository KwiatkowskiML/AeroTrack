using PoProj.publishers;
namespace PoProj.Publishers;

public class EventManager
{
    public readonly IdUpdatePublisher IdUpdate = new ();
    public readonly PositionUpdatePublisher PosUpdate = new ();
    public readonly ContactUpdatePublisher ContactUpdate = new();
}