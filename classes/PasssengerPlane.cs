using System.Text.Json.Serialization;
using PoProj.CommandsSupport;
using PoProj.DataStorage;
using PoProj.features;
using PoProj.MediaClasses;
using PoProj.publishers;
using PoProj.Publishers;

namespace PoProj.classes;

public class PassengerPlane: Plane, IReportable, IUpdateID
{
    public ushort FirstClassSize { get; set; }
    public ushort BusinessClassSize { get; set; }
    public ushort EconomyClassSize { get; set; }
    
    public new static string[] fieldNames =
    [
        "id",
        "serial",
        "countrycode",
        "model",
        "firstclasssize",
        "businessclasssize",
        "economyclasssize"
    ];
    
    [JsonConstructor]
    public PassengerPlane(ulong id, string serial, string country, string model, ushort firstClassSize,
        ushort businessClassSize, ushort economyClassSize)
    {
        ID = id;
        Serial = serial;
        Country = country;
        Model = model;
        FirstClassSize = firstClassSize;
        BusinessClassSize = businessClassSize;
        EconomyClassSize = economyClassSize;
        
        // Initializing passengerplane dictionary
        Properties = new Dictionary<string, IProperty>
        {
            { "id", new Property<ulong>(() => this.ID, value => this.ID = ulong.Parse(value)) },
            { "serial", new Property<string>(() => this.Serial, value => this.Serial = value) },
            { "countrycode", new Property<string>(() => this.Country, value => this.Country = value) },
            { "model", new Property<string>(() => this.Model, value => this.Model = value) },
            { "firstclasssize", new Property<ushort>(() => this.FirstClassSize, value => this.FirstClassSize = ushort.Parse(value)) },
            { "businessclasssize", new Property<ushort>(() => this.BusinessClassSize, value => this.BusinessClassSize = ushort.Parse(value)) },
            { "economyclasssize", new Property<ushort>(() => this.EconomyClassSize, value => this.EconomyClassSize = ushort.Parse(value)) }
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
        if (ID == oldId && DataSingleton.Instance.PassengerPlaneIdChange(oldId, newId))
        {
            LogManager.Instance.IdUpdateLog(ID, newId);
            ID = newId;
        }
    }
    public override void GetRemoved(IRemove visitor) => visitor.Remove(this);

}