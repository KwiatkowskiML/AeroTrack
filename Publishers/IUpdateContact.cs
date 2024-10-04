namespace PoProj.Publishers;

public interface IUpdateContact
{
    void UpdateContactInfo(ulong objectId, string phoneNumber, string emailAddress);
}