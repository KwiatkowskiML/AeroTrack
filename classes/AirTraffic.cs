using System.Text.Json.Serialization;
using PoProj.CommandsSupport;
using PoProj.DataStorage;
using PoProj.features;
using PoProj.publishers;
using PoProj.Publishers;

namespace PoProj.classes;

[Serializable]
[JsonDerivedType(typeof(Airport), typeDiscriminator: "Airport")]
[JsonDerivedType(typeof(Cargo), typeDiscriminator: "Cargo")]
[JsonDerivedType(typeof(CargoPlane), typeDiscriminator: "CargoPlane")]
[JsonDerivedType(typeof(Crew), typeDiscriminator: "Crew")]
[JsonDerivedType(typeof(Flight), typeDiscriminator: "Flight")]
[JsonDerivedType(typeof(Passenger), typeDiscriminator: "Passenger")]
[JsonDerivedType(typeof(PassengerPlane), typeDiscriminator: "PassengerPlane")]

public abstract class AirTraffic: IUpdateID
{
    // Dictionary of field references
    public Dictionary<string, IProperty> Properties;
    public ulong ID { get; set; }
    public abstract void GetLoaded(IStore visitor, EventManager eventManager);
    public abstract bool GetStored(IStore visitor);
    public abstract void GetRemoved(IRemove visitor);
    public abstract void UpdateId(ulong oldId, ulong newId);
    public static string[] fieldNames;

}