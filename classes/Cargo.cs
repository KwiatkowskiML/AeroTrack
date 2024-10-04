using System.Globalization;
using System.Text.Json.Serialization;
using PoProj.CommandsSupport;
using PoProj.DataStorage;
using PoProj.features;
using PoProj.publishers;
using PoProj.Publishers;

namespace PoProj.classes;

public class Cargo: AirTraffic, IUpdateID, ILoadable
{
    public float Weight { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    
    public new static string[] fieldNames =
    [
        "id",
        "weight",
        "code",
        "description"
    ];
    
    [JsonConstructor]
    public Cargo(ulong id, float weight, string code, string description)
    {
        ID = id;
        Weight = weight;
        Code = code;
        Description = description;
        
        // Initializing cargo dictionary
        Properties = new Dictionary<string, IProperty>
        {
            { "id", new Property<ulong>(() => this.ID, value => this.ID = ulong.Parse(value)) },
            { "weight", new Property<float>(() => this.Weight, value => this.Weight = float.Parse(value)) },
            { "code", new Property<string>(() => this.Code, value => this.Code = value) },
            { "description", new Property<string>(() => this.Description, value => this.Description = value) }
        };
    }
    public override void GetLoaded(IStore visitor, EventManager eventManager)
    {
        visitor.Load(this);
        eventManager.IdUpdate.Subscribe(this);
    }
    public override bool GetStored(IStore visitor) => visitor.Store(this);
    public override void UpdateId(ulong oldId, ulong newId)
    {
        if (ID == oldId && DataSingleton.Instance.CargoIdChange(oldId, newId))
        {
            LogManager.Instance.IdUpdateLog(ID, newId);
            ID = newId;
        }
    }
    public override void GetRemoved(IRemove visitor) => visitor.Remove(this);

}