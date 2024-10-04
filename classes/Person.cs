using PoProj.Publishers;

namespace PoProj.classes;

public abstract class Person: AirTraffic, IUpdateContact
{
    public string Name { get; set; }
    public ulong Age { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }

    protected Person()
    {
        ID = 0;
        Name = "";
        Age = 0;
        Phone = "";
        Email = "";
    }
    
    public void UpdateContactInfo(ulong objectId, string phoneNumber, string emailAddress)
    {
        if (ID != objectId)
            return;

        LogManager.Instance.ContactUpdateLog(objectId, Phone, Email, phoneNumber, emailAddress);
        Phone = phoneNumber;
        Email = emailAddress;
    }
}