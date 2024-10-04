using System.Globalization;
using System.Text.Json.Serialization;
using PoProj.CommandsSupport;
using PoProj.DataStorage;
using PoProj.features;
using PoProj.MediaClasses;
using PoProj.publishers;
using PoProj.Publishers;

namespace PoProj.classes;

public class Airport : AirTraffic, IReportable, IUpdateID, IUpdatePosition
{
    public string Name { get; set; }
    public string Code { get; set; }
    public float Longitude { get; private set; }
    public float Latitude { get; private set; }
    public float AMSL { get; private set; }
    public string Country { get; set; }

    public new static string[] fieldNames =
    [
        "id",
        "name",
        "code",
        "worldposition.lat",
        "worldposition.long",
        "amsl",
        "countrycode"
    ];

    [JsonConstructor]
    public Airport(ulong id, string name, string code, float longitude, float latitude, float amsl, string country)
    {
        ID = id;
        Name = name;
        Code = code;
        Longitude = longitude;
        Latitude = latitude;
        AMSL = amsl;
        Country = country;
        
        // Initializing airport dictionary
        Properties = new Dictionary<string, IProperty>
        {
            { "id", new Property<ulong>(() => this.ID, value => this.ID = ulong.Parse(value)) },
            { "name", new Property<string>(() => this.Name, value => this.Name = value) },
            { "code", new Property<string>(() => this.Code, value => this.Code = value) },
            { "worldposition.lat", new Property<float>(() => this.Latitude, value => this.Latitude = float.Parse(value)) },
            { "worldposition.long", new Property<float>(() => this.Longitude, value => this.Longitude = float.Parse(value)) },
            { "amsl", new Property<float>(() => this.AMSL, value => this.AMSL = float.Parse(value)) },
            { "countrycode", new Property<string>(() => this.Country, value => this.Country = value) }
        };
    }

    public override void GetLoaded(IStore visitor, EventManager eventManager)
    {
        visitor.Load(this);
        eventManager.IdUpdate.Subscribe(this);
        eventManager.PosUpdate.Subscribe(this);
    }
    public override bool GetStored(IStore visitor) => visitor.Store(this);
    public string GetReported(IMedia media) => media.Report(this);
    public override void UpdateId(ulong oldId, ulong newId)
    {
        if (ID == oldId && DataSingleton.Instance.AirportIdChange(oldId, newId))
        {
            LogManager.Instance.IdUpdateLog(ID, newId);
            ID = newId;
        }
    }
    public void UpdatePosition(ulong objectId, float longitude, float latitude, float amsl)
    {
        if (ID != objectId) return;
        
        LogManager.Instance.PosUpdateLog(objectId, Longitude.ToString(), Latitude.ToString(), AMSL.ToString(),
            longitude, latitude, amsl);
        Longitude = longitude;
        Latitude = latitude;
        AMSL = amsl;
    }
    public override void GetRemoved(IRemove visitor) => visitor.Remove(this);
}