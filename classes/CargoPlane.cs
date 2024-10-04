using System.Globalization;
using System.Text.Json.Serialization;
using PoProj.CommandsSupport;
using PoProj.DataStorage;
using PoProj.features;
using PoProj.MediaClasses;
using PoProj.publishers;
using PoProj.Publishers;

namespace PoProj.classes;

public class CargoPlane: Plane, IReportable, IUpdateID
{
    public float MaxLoad { get; set; }
    public new static string[] fieldNames =
    [
        "id",
        "serial",
        "countrycode",
        "model",
        "maxload"
    ];
    
    [JsonConstructor]
    public CargoPlane(ulong id, string serial, string country, string model, float maxLoad)
    {
        ID = id;
        Serial = serial;
        Country = country;
        Model = model;
        MaxLoad = maxLoad;
        
        // Initializing cargoplane dictionary
        Properties = new Dictionary<string, IProperty>
        {
            { "id", new Property<ulong>(() => this.ID, value => this.ID = ulong.Parse(value)) },
            { "serial", new Property<string>(() => this.Serial, value => this.Serial = value) },
            { "countrycode", new Property<string>(() => this.Country, value => this.Country = value) },
            { "model", new Property<string>(() => this.Model, value => this.Model = value) },
            { "maxload", new Property<float>(() => this.MaxLoad, value => this.MaxLoad = float.Parse(value)) }
        };
    }

    public override void GetLoaded(IStore visitor, EventManager eventManager)
    {
        visitor.Load(this);
        eventManager.IdUpdate.Subscribe(this);
    }

    public override bool GetStored(IStore visitor) => visitor.Store(this);
    public string GetReported(IMedia media) => media.Report(this);
    public override void UpdateId(ulong oldId, ulong newId)
    {
        if (ID == oldId && DataSingleton.Instance.CargoPlaneIdChange(oldId, newId))
        {
            LogManager.Instance.IdUpdateLog(ID, newId);
            ID = newId;
        }
    }
    public override void GetRemoved(IRemove visitor) => visitor.Remove(this);

}