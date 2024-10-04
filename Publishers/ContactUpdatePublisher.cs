namespace PoProj.Publishers;

public class ContactUpdatePublisher
{
    private readonly List<IUpdateContact> _subscribers = new ();

    // Notifying all subscribers about contact info update
    public void NotifySubscribers(ulong objectId, string phoneNumber, string emailAddress)
    {
        foreach (var contactUpdate in _subscribers)
        {
            contactUpdate.UpdateContactInfo(objectId, phoneNumber, emailAddress);
        }
    }

    // Adding new subscriber
    public void Subscribe(IUpdateContact updateContact)
    {
        _subscribers.Add(updateContact);
    }
}