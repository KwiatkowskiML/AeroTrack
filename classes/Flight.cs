using System.Text.Json.Serialization;
using Mapsui.Projections;
using PoProj.CommandsSupport;
using PoProj.DataStorage;
using PoProj.features;
using PoProj.publishers;
using PoProj.Publishers;
using IProperty = ExCSS.IProperty;

namespace PoProj.classes;

public class Flight : AirTraffic, IUpdateID, IUpdatePosition
{ 
    #region Fields and properties
    public ulong OriginId
    {
        get
        {
            return _originAirport == null ? _originId : _originAirport.ID;
        }
        set
        {
            _originId = value;
        }
    }
    public ulong TargetId
    {
        get
        {
            return _targetAirport == null ? _targetId : _targetAirport.ID;
        }
        set
        {
            _targetId = value;
        }
    }
    public string TakeoffTime { get; set; }
    public string LandingTime { get; set; }
    public float? Longitude { get; set; }
    public float? Latitude { get; set; }
    public float? Amsl { get; set; }
    public ulong PlaneId { get; set; }
    public ulong[] CrewId
    {
        get
        {
            if (_crewList == null)
                return _crewId;
            
            // reconstructing the array
            int crewCount = _crewList.Count;
            var output = new ulong[crewCount];
            int index = 0;
            foreach (var crew in _crewList)
            {
                output[index++] = crew.ID;
            }

            return output;
        }
    }
    public ulong[] LoadId
    {
        get
        {
            if (_loadList == null)
                return _loadId;
            
            // reconstructing the array
            int loadCount = _loadList.Count;
            var output = new ulong[loadCount];
            int index = 0;
            foreach (var load in _loadList)
            {
                output[index++] = load.ID;
            }

            return output;
        }
    }
    
    // private
    private ulong _originId;
    private ulong _targetId;
    private readonly ulong[] _loadId;
    private readonly ulong[] _crewId;
    
    private Airport? _originAirport = null;
    private Airport? _targetAirport = null;
    private List<ILoadable>? _loadList = null;
    private List<Crew>? _crewList = null;

    private readonly TimeSpan _timeSpan;
    private float _longitudeDiff;
    private float _latitudeDiff;
    #endregion

    #region Visitor support

    public override void GetLoaded(IStore visitor, EventManager eventManager)
    {
        visitor.Load(this);
        eventManager.IdUpdate.Subscribe(this);
        eventManager.PosUpdate.Subscribe(this);
    }

    public override bool GetStored(IStore visitor) => visitor.Store(this);
    #endregion
    
    [JsonConstructor]
    public Flight(ulong id, ulong originId, ulong targetId, string takeoffTime, string landingTime, float? longitude,
        float? latitude, float? amsl, ulong planeId, ulong[] crewId, ulong[] loadId)
    {
        ID = id;
        _originId = originId;
        _targetId = targetId;
        TakeoffTime = takeoffTime;
        LandingTime = landingTime;
        Longitude = longitude;
        Latitude = latitude;
        Amsl = amsl;
        PlaneId = planeId;
        _crewId = crewId;
        _loadId = loadId;
        
        // Calculating time span
        var startTime = DateTime.Parse(TakeoffTime);
        var endTime = DateTime.Parse(LandingTime);
        if (endTime < startTime)
            endTime = endTime.AddDays(1);
        _timeSpan = endTime - startTime;
        
        // Initializing flight dictionary
        Properties = new Dictionary<string, CommandsSupport.IProperty>
        {
            { "id", new Property<ulong>(() => this.ID, value => this.ID = ulong.Parse(value)) },
            { "origin", new Property<ulong>(() => this.OriginId, value => this.OriginId = ulong.Parse(value)) },
            { "target", new Property<ulong>(() => this.TargetId, value => this.TargetId = ulong.Parse(value)) },
            { "takeofftime", new Property<string>(() => this.TakeoffTime, value => this.TakeoffTime = value) },
            { "landingtime", new Property<string>(() => this.LandingTime, value => this.LandingTime = value) },
            { "worldposition.lat", new Property<float>(() => this.Latitude.Value, value => this.Latitude = float.Parse(value)) },
            { "worldposition.long", new Property<float>(() => this.Longitude.Value, value => this.Longitude = float.Parse(value)) },
            { "amsl", new Property<float>(() => this.Amsl.Value, value => this.Amsl = float.Parse(value)) },
            { "plane", new Property<ulong>(() => this.PlaneId, value => this.PlaneId = ulong.Parse(value)) }
        };
    }
    
    public new static string[] fieldNames =
    [
        "id",
        "origin",
        "target",
        "takeofftime",
        "landingtime",
        "worldposition.lat",
        "worldposition.long",
        "amsl",
        "plane"
    ];

    // Updates current plane position
    public void UpdatePosition(DateTime currentTime)
    {
        // If plane is not in the air, then there is nothing to update
        if (!IsinTheAir(currentTime))
            return;
        
        // If Airport is not set return
        if (_originAirport == null || _targetAirport == null)
            return;
        
        // Updating position
        Latitude += _latitudeDiff;
        Longitude += _longitudeDiff;
    }

    // Returns rotation in radians
    public double GetRotation()
    {
        // If Airport is not set return
        if (_originAirport == null || _targetAirport == null)
            return 0;
        
        // Next coordinates
        var nextLatitude = Latitude + _latitudeDiff;
        var nextLongitude = Longitude + _longitudeDiff;
        
        // Calculating X and Y coordinates
        var (xStart, yStart) = SphericalMercator.FromLonLat((double)Longitude!, (double)Latitude!);
        var (xEnd, yEnd) = SphericalMercator.FromLonLat((double)nextLongitude!, (double)nextLatitude!);
        
        // Returning the angle
        return Math.Atan2(xEnd - xStart, yEnd - yStart);
    }

    // Checks whether flight is currently in the air
    public bool IsinTheAir(DateTime currentTime)
    {
        // Checking whether flight is currently in the air 
        var takeOffTime = DateTime.Parse(TakeoffTime);
        var landingTime = DateTime.Parse(LandingTime);

        // landingTime is the day after takeOffTime and currentTime is the same day as landingTime and before landingTime
        var cond1 = DateTime.Compare(currentTime, takeOffTime) < 0 &&
                    DateTime.Compare(currentTime, landingTime) < 0 &&
                    DateTime.Compare(takeOffTime, landingTime) > 0;
            
        // landingTime is the same day as takeOffTime and currentTime is between takeOffTime and landingTime
        var cond2 = DateTime.Compare(currentTime, takeOffTime) >= 0 &&
                    DateTime.Compare(currentTime, landingTime) <= 0;
            
        // landingTime is the day after takeOffTime and currentTime is the same day as takeOffTime and after takeOffTime
        var cond3 = DateTime.Compare(currentTime, takeOffTime) > 0 &&
                    DateTime.Compare(currentTime, landingTime) > 0 &&
                    DateTime.Compare(takeOffTime, landingTime) > 0;
        
        return cond1 || cond2 || cond3;
    }

    private double SecondsLeft(DateTime currentTime)
    {
        var startTime = DateTime.Parse(TakeoffTime);
        var endTime = DateTime.Parse(LandingTime);
        if (endTime < startTime)
            endTime = endTime.AddDays(1);

        TimeSpan differnce = endTime - currentTime;
        return differnce.TotalSeconds;
    }

    // If not Initialized then initializing flight's originAirport
    public void SetAirports(Airport originAirport, Airport targetAirport)
    {
        if (_originAirport != null || _targetAirport != null)
            return;
        
        // Setting up airports reference
        _originAirport = originAirport;
        _targetAirport = targetAirport;
        
        // Setting up coordinates
        Latitude ??= _originAirport.Latitude;
        Longitude ??= _originAirport.Longitude;
        
        // Calculating coordinates difference
        _latitudeDiff = (_targetAirport.Latitude - Latitude.Value) / (float)_timeSpan.TotalSeconds;
        _longitudeDiff = (_targetAirport.Longitude - Longitude.Value) / (float)_timeSpan.TotalSeconds;
    }
    
    // Adding new load list reference
    public void SetLoad(List<ILoadable> loadList)
    {
        _loadList ??= loadList;
    }
    
    // Adding Crew list reference
    public void SetCrew(List<Crew> crewList)
    {
        _crewList ??= crewList;
    }
    
    public override void UpdateId(ulong oldId, ulong newId)
    {
        if (ID == oldId && DataSingleton.Instance.FlightIdChange(oldId, newId))
        {
            LogManager.Instance.IdUpdateLog(ID, newId);
            ID = newId;
        }
    }
    public void UpdatePosition(ulong objectId, float longitude, float latitude, float amsl)
    {
        var currentTime = DateTime.Now;
        if (ID == objectId)
        {
            LogManager.Instance.PosUpdateLog(objectId, Longitude == null ? "NULL" : Longitude.Value.ToString(),
                Latitude == null ? "NULL" : Latitude.Value.ToString(),
                Amsl == null ? "NULL" : Amsl.Value.ToString(),
                longitude, latitude, amsl);
            Longitude = longitude;
            Latitude = latitude;
            Amsl = amsl;

            // Calculating coordinates difference
            if (_targetAirport != null && _originAirport != null)
            {
                float seconds = IsinTheAir(currentTime)
                    ? (float)SecondsLeft(currentTime)
                    : (float)_timeSpan.TotalSeconds;
                _latitudeDiff = (_targetAirport.Latitude - Latitude.Value) / seconds;
                _longitudeDiff = (_targetAirport.Longitude - Longitude.Value) / seconds;
            }
        }

        if (_targetAirport != null && _targetAirport.ID == objectId)
        {
            float seconds = IsinTheAir(currentTime)
                ? (float)SecondsLeft(currentTime)
                : (float)_timeSpan.TotalSeconds;
            _latitudeDiff = (_targetAirport.Latitude - Latitude.Value) / seconds;
            _longitudeDiff = (_targetAirport.Longitude - Longitude.Value) / seconds;
        }
    }
    public override void GetRemoved(IRemove visitor) => visitor.Remove(this);

}