using System.Text.Json.Serialization;
using PoProj.CommandsSupport;
using PoProj.DataStorage;
using PoProj.features;
using PoProj.publishers;
using PoProj.Publishers;

namespace PoProj.classes;

public class Passenger: Person, IUpdateID, ILoadable
{
    public string Class { get; set; }
    public ulong Miles { get; set; }
    public new static string[] fieldNames =
    [
        "id",
        "name",
        "age",
        "phone",
        "email",
        "class",
        "miles"
    ];
    
    [JsonConstructor]
    public Passenger(ulong id, string name, ulong age, string phone, string email, string @class, ulong miles)
    {
        ID = id;
        Name = name;
        Age = age;
        Phone = phone;
        Email = email;
        Class = @class;
        Miles = miles;
        
        // Initializing passenger dictionary
        Properties = new Dictionary<string, IProperty>
        {
            { "id", new Property<ulong>(() => this.ID, value => this.ID = ulong.Parse(value)) },
            { "name", new Property<string>(() => this.Name, value => this.Name = value) },
            { "age", new Property<ulong>(() => this.Age, value => this.Age = ulong.Parse(value)) },
            { "phone", new Property<string>(() => this.Phone, value => this.Phone = value) },
            { "email", new Property<string>(() => this.Email, value => this.Email = value) },
            { "class", new Property<string>(() => this.Class, value => this.Class = value) },
            { "miles", new Property<ulong>(() => this.Miles, value => this.Miles = ulong.Parse(value)) }
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
        if (ID == oldId && DataSingleton.Instance.PassengerIdChange(oldId, newId))
        {
            LogManager.Instance.IdUpdateLog(ID, newId);
            ID = newId;
        }
    }
    public override void GetRemoved(IRemove visitor) => visitor.Remove(this);

}