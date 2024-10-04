using System.Text.Json.Serialization;
using PoProj.CommandsSupport;
using PoProj.DataStorage;
using PoProj.features;
using PoProj.publishers;
using PoProj.Publishers;

namespace PoProj.classes;

public class Crew: Person, IUpdateID
{
    public ushort Practice { get; set; }
    public string Role { get; set; }
    
    public new static string[] fieldNames =
    [
        "id",
        "name",
        "age",
        "phone",
        "email",
        "practice",
        "role"
    ];
    
    [JsonConstructor]
    public Crew(ulong id, string name, ulong age, string phone, string email, ushort practice, string role)
    {
        ID = id;
        Name = name;
        Age = age;
        Phone = phone;
        Email = email;
        Practice = practice;
        Role = role;
        
        // Initializing crew dictionary
        Properties = new Dictionary<string, IProperty>
        {
            { "id", new Property<ulong>(() => this.ID, value => this.ID = ulong.Parse(value)) },
            { "name", new Property<string>(() => this.Name, value => this.Name = value) },
            { "age", new Property<ulong>(() => this.Age, value => this.Age = ulong.Parse(value)) },
            { "phone", new Property<string>(() => this.Phone, value => this.Phone = value) },
            { "email", new Property<string>(() => this.Email, value => this.Email = value) },
            { "practice", new Property<ushort>(() => this.Practice, value => this.Practice = ushort.Parse(value)) },
            { "role", new Property<string>(() => this.Role, value => this.Role = value) }
        };
    }

    public override void GetLoaded(IStore visitor, EventManager eventManager)
    {
        visitor.Load(this);
        eventManager.IdUpdate.Subscribe(this);
        eventManager.ContactUpdate.Subscribe(this);
    }
    public override bool GetStored(IStore visitor) => visitor.Store(this);
    public override void UpdateId(ulong oldId, ulong newId)
    {
        if (ID == oldId && DataSingleton.Instance.CrewIdChange(oldId, newId))
        {
            LogManager.Instance.IdUpdateLog(ID, newId);
            ID = newId;
        }
    }
    public override void GetRemoved(IRemove visitor) => visitor.Remove(this);

}