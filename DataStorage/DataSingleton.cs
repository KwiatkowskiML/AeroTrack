using System.Text.Json;
using PoProj.CommandsSupport;
using PoProj.DataStorage;

namespace PoProj.features;
using classes;

public sealed class DataSingleton: IStore, IRemove
{
    // Singleton instance
    private static volatile DataSingleton? _instance;
    
    // Waitlist containing loaded objects
    private readonly Queue<AirTraffic> _waitList = new();
    
    #region Lock variables used during objects dictionaries synchronization
    // Lock variable used during creation of singleton instance
    private static readonly object SyncRoot = new();
    
    // Lock variable used during _waitlist synchronization
    private static readonly object QueueSync = new();
    
    // Lock variables for each dictionary
    private static readonly object AirportSync = new();
    private static readonly object CargoSync = new();
    private static readonly object CargoPlaneSync = new();
    private static readonly object CrewSync = new();
    private static readonly object FlightSync = new();
    private static readonly object PassengerSync = new();
    private static readonly object PassengerPlaneSync = new();
    #endregion
    
    #region Dictionaries for each AirTraffic class
    private readonly Dictionary<ulong, Airport> _airportDictionary = new();
    private readonly Dictionary<ulong, Cargo> _cargoDictionary = new();
    private readonly Dictionary<ulong, CargoPlane> _cargoPlaneDictionary = new();
    private readonly Dictionary<ulong, Crew> _crewDictionary = new();
    private readonly Dictionary<ulong, Flight> _flightDictionary = new();
    private readonly Dictionary<ulong, Passenger> _passengerDictionary = new();
    private readonly Dictionary<ulong, PassengerPlane> _passengerPlaneDictionary = new();
    #endregion
    
    // Parameterless constructor
    private DataSingleton() { }
    
    // Instance property initializing the singleton
    public static DataSingleton Instance
    {
        get
        {
            if (_instance != null) return _instance;
            lock (SyncRoot)
            {
                if (_instance != null) return _instance;
                _instance = new DataSingleton();
            }
            return _instance;
        }
    }
    
    // Returning list of flights in the air at currentTime
    public List<FlightGUI> GetCurrentFlightsGui(DateTime currentTime)
    {
        // Updaing dictionaries content
        Consolidate();
        
        var outputList = new List<FlightGUI>();
        lock (FlightSync)
        {
            foreach (var flight in _flightDictionary.Values)
            {
                // Checking whether the flight is currently in air
                if (!flight.IsinTheAir(currentTime))
                    continue;
                
                // Updateing flight's position
                flight.UpdatePosition(currentTime);
                
                // Adding flight to list
                outputList.Add(new FlightAdapter(flight));
            }
        }
        return outputList;
    }
    
    // Database consolidation
    private void Consolidate()
    {
        lock (QueueSync)
        {
            var queueLength = _waitList.Count;
            for (int i = 0; i < queueLength; i++)
            {
                // Getting first item from waitlist and storing it
                var airTraffic = _waitList.Dequeue();
                
                // If objevt couldn't be stored it is enqueued once again
                if (!airTraffic.GetStored(this))
                    _waitList.Enqueue(airTraffic);
            }
            
            // Second run of consolidation in case of adding neccesary objects to database in the first run
            queueLength = _waitList.Count;
            for (int i = 0; i < queueLength; i++)
            {
                // Getting first item from waitlist and storing it
                var airTraffic = _waitList.Dequeue();
                
                // If objevt couldn't be stored it is enqueued once again
                if (!airTraffic.GetStored(this))
                    _waitList.Enqueue(airTraffic);
            }
        }
    }
    
    // Serializing stored data
    public void JsonSerialization(string path)
    {
        // Creating list containing data to serialize
        var toSerialize = new List<AirTraffic>();
        Consolidate();
        
        lock (AirportSync)
            toSerialize.AddRange(_airportDictionary.Values.ToList());
        lock (CargoPlaneSync)
            toSerialize.AddRange(_cargoPlaneDictionary.Values.ToList());
        lock (PassengerPlaneSync)
            toSerialize.AddRange(_passengerPlaneDictionary.Values.ToList());
        lock (CargoSync)
            toSerialize.AddRange(_cargoDictionary.Values.ToList());
        lock (CrewSync)
            toSerialize.AddRange(_crewDictionary.Values.ToList());
        lock (PassengerSync)
            toSerialize.AddRange(_passengerDictionary.Values.ToList());
        lock (FlightSync)
            toSerialize.AddRange(_flightDictionary.Values.ToList());
        
        // Serializing the list
        var serializedData = JsonSerializer.Serialize(toSerialize,
            new JsonSerializerOptions{ WriteIndented = true});
        File.WriteAllText(path, serializedData);
    }

    // Returning a list of objects implementing IReportable interface
    public List<IReportable> GetReportableList()
    {
        var reportableList = new List<IReportable>();
        Consolidate();
        
        lock(AirportSync)
            reportableList.AddRange(_airportDictionary.Values.ToList());
        lock(CargoPlaneSync)
            reportableList.AddRange(_cargoPlaneDictionary.Values.ToList());
        lock(PassengerPlaneSync)
            reportableList.AddRange(_passengerPlaneDictionary.Values.ToList());

        return reportableList;
    }
    
    // ID change methodsS
    #region ID change
    public bool AirportIdChange(ulong oldId, ulong newId)
    {
        // Update of database
        Consolidate();
        lock (AirportSync)
        {
            // Checking whether there is already an airport with desired id
            if (_airportDictionary.ContainsKey(newId))
                return false;
            var airport = _airportDictionary[oldId];
            _airportDictionary.Remove(oldId);
            _airportDictionary.Add(newId, airport);
        }
        return true;
    }
    public bool CargoIdChange(ulong oldId, ulong newId)
    {
        // Update of database
        Consolidate();
        lock (CargoSync)
        {
            // Checking whether there is already a cargo object with desired id
            if (_cargoDictionary.ContainsKey(newId))
                return false;
            var cargo = _cargoDictionary[oldId];
            _cargoDictionary.Remove(oldId);
            _cargoDictionary.Add(newId, cargo);
        }
        return true;
    }
    public bool CargoPlaneIdChange(ulong oldId, ulong newId)
    {
        // Update of database
        Consolidate();
        lock (CargoPlaneSync)
        {
            // Checking whether there is already a CargoPlane object with desired id
            if (_cargoPlaneDictionary.ContainsKey(newId))
                return false;
            var cargoPlane = _cargoPlaneDictionary[oldId];
            _cargoPlaneDictionary.Remove(oldId);
            _cargoPlaneDictionary.Add(newId, cargoPlane);
        }
        return true;
    }
    public bool CrewIdChange(ulong oldId, ulong newId)
    {
        // Update of database
        Consolidate();
        lock (CrewSync)
        {
            // Checking whether there is already a Crew object with desired id
            if (_crewDictionary.ContainsKey(newId))
                return false;
            var crew = _crewDictionary[oldId];
            _crewDictionary.Remove(oldId);
            _crewDictionary.Add(newId, crew);
        }
        return true;
    }
    public bool PassengerIdChange(ulong oldId, ulong newId)
    {
        // Update of database
        Consolidate();
        lock (PassengerSync)
        {
            // Checking whether there is already a Passenger object with desired id
            if (_passengerDictionary.ContainsKey(newId))
                return false;
            var passenger = _passengerDictionary[oldId];
            _passengerDictionary.Remove(oldId);
            _passengerDictionary.Add(newId, passenger);
        }
        return true;
    }
    public bool PassengerPlaneIdChange(ulong oldId, ulong newId)
    {
        // Update of database
        Consolidate();
        lock (PassengerPlaneSync)
        {
            // Checking whether there is already a PassengerPlane object with desired id
            if (_passengerPlaneDictionary.ContainsKey(newId))
                return false;
            var passengerPlane = _passengerPlaneDictionary[oldId];
            _passengerPlaneDictionary.Remove(oldId);
            _passengerPlaneDictionary.Add(newId, passengerPlane);
        }
        return true;
    }
    public bool FlightIdChange(ulong oldId, ulong newId)
    {
        // Update of database
        Consolidate();
        lock (FlightSync)
        {
            // Checking whether there is already a flight with desired id
            if (_flightDictionary.ContainsKey(newId))
            {
                return false;
            }
            // Checking whether there is any flight object with newId stuck in the _waitList
            lock (QueueSync)
            {
                foreach (var airTraffic in _waitList)
                {
                    if (airTraffic.ID == newId)
                        return false;
                }
            }
            var flight = _flightDictionary[oldId];
            _flightDictionary.Remove(oldId);
            _flightDictionary.Add(newId, flight);
        }
        return true;
    }
    #endregion
    
    // IStore Load methods used to load data into waitlist
    #region IStore load methods
    public void Load(Airport airport)
    {
        lock (QueueSync)
            _waitList.Enqueue(airport);
    }
    public void Load(Cargo cargo)
    {
        lock (QueueSync)
            _waitList.Enqueue(cargo);
    }
    public void Load(CargoPlane cargoPlane)
    {
        lock (QueueSync)
            _waitList.Enqueue(cargoPlane);
    }
    public void Load(Crew crew)
    {
        lock (QueueSync)
            _waitList.Enqueue(crew);
    }
    public void Load(Flight flight)
    {
        lock (QueueSync)
            _waitList.Enqueue(flight);
    }
    public void Load(Passenger passenger)
    {
        lock (QueueSync)
            _waitList.Enqueue(passenger);
    }
    public void Load(PassengerPlane passengerPlane)
    {
        lock (QueueSync)
            _waitList.Enqueue(passengerPlane);
    }
    #endregion
    
    // IStore Store methods used to store data from waitlist in appropiate dictionaries
    #region IStore store methods
    public bool Store(Airport airport)
    {
        lock (AirportSync)
            return _airportDictionary.TryAdd(airport.ID, airport);
    }
    public bool Store(Cargo cargo)
    {
        lock (CargoSync)
            return _cargoDictionary.TryAdd(cargo.ID, cargo);
    }
    public bool Store(CargoPlane cargoPlane)
    {
        lock (CargoPlaneSync)
            return _cargoPlaneDictionary.TryAdd(cargoPlane.ID, cargoPlane);
    }
    public bool Store(Crew crew)
    {
        lock (CrewSync)
            return _crewDictionary.TryAdd(crew.ID, crew);
    }
    public bool Store(Flight flight)
    {
        Airport? originAirport;
        Airport? targetAirport;
        
        // Checking whether all proper airports are in the database
        lock (AirportSync)
        {
            if (!_airportDictionary.TryGetValue(flight.OriginId, out originAirport))
                return false;
            if (!_airportDictionary.TryGetValue(flight.TargetId, out targetAirport))
                return false;
        }
        
        // Checking whether all crew members are in the database
        var crewList = new List<Crew>();
        lock (CrewSync)
        {
            foreach (var crewId in flight.CrewId)
            {
                Crew? currentCrew;
                if (!_crewDictionary.TryGetValue(crewId, out currentCrew))
                    return false;
                crewList.Add(currentCrew);
            }
        }

        // Checking type of the plane
        bool isCargoPlane = false;
        bool isPassengerPlane = false;
        lock (CargoPlaneSync)
        {
            isCargoPlane = _cargoPlaneDictionary.ContainsKey(flight.PlaneId);
        }
        lock (PassengerPlaneSync)
        {
            isPassengerPlane = _passengerPlaneDictionary.ContainsKey(flight.PlaneId);
        }

        if (!isPassengerPlane && !isCargoPlane)
            return false;
        
        // Checking whether all loads are in the database
        var loadList = new List<ILoadable>();
        if (isPassengerPlane)
        {
            lock (PassengerSync)
            {
                foreach (var loadId in flight.LoadId)
                {
                    Passenger? passenger;
                    if (!_passengerDictionary.TryGetValue(loadId, out passenger))
                        return false;
                    loadList.Add(passenger);
                }
            }
        }

        if (isCargoPlane)
        {
            lock (CargoSync)
            {
                foreach (var loadId in flight.LoadId)
                {
                    Cargo? cargo;
                    if (!_cargoDictionary.TryGetValue(loadId, out cargo))
                        return false;
                    loadList.Add(cargo);
                }
            }
        }
        
        flight.SetAirports(originAirport, targetAirport);
        flight.SetCrew(crewList);
        flight.SetLoad(loadList);
        
        // Adding flight to flightDictionary
        lock (FlightSync)
            return _flightDictionary.TryAdd(flight.ID, flight);
    }
    public bool Store(Passenger passenger)
    {
        lock (PassengerSync)
            return _passengerDictionary.TryAdd(passenger.ID, passenger);
    }
    public bool Store(PassengerPlane passengerPlane)
    {
        lock (PassengerPlaneSync)
            return _passengerPlaneDictionary.TryAdd(passengerPlane.ID, passengerPlane);
    }
    #endregion
    
    // IReceiver methods
    #region IReciever methods
    public void Execute(DisplayQuery displayQuery)
    {
        (List<AirTraffic> classList, var fieldNames) = GetAirTrafficInfo(displayQuery.ObjectClass);
        if (displayQuery.ObjectFields.Length == 1 && displayQuery.ObjectFields[0] == "*")
        {
            displayQuery.ObjectFields = fieldNames!;
        }
        
        // executing conditions
        classList = ComparerHelpers.ConditionExecution(classList, displayQuery.Conditions);
        
        // init
        var headers = displayQuery.ObjectFields;
        string[,] values = null;
        int fieldsCount = headers.Length;
        int[] columnWidths = new int[fieldsCount];
        
        // filling values array and calculating max width
        values = new string[classList.Count, fieldsCount];
        int row = 0;
        foreach (var airTraffic in classList)
        {
            for (int column = 0; column < fieldsCount; column++)
            {
                values[row, column] = airTraffic.Properties[displayQuery.ObjectFields[column]].Value.ToString();
                columnWidths[column] = Math.Max(columnWidths[column], values[row, column].Length);
            }
            row++;
        }
        for (int column = 0; column < fieldsCount; column++)
        {
            columnWidths[column] = Math.Max(columnWidths[column], headers[column].Length);
        }
        
        // writing headers
        Console.Write("| ");
        for (int column = 0; column < headers.Length; column++)
        {
            Console.Write(headers[column].PadRight(columnWidths[column]));
            Console.Write(" | ");
        }
        Console.WriteLine();
        Console.WriteLine(new string('-', columnWidths.Sum() + headers.Length * 3 + 1));
       
        // writing array content
        for (int i = 0; i < values.GetLength(0); i++)
        {
            Console.Write("| ");
            for (int j = 0; j < values.GetLength(1); j++)
            {
                Console.Write(values[i, j].PadLeft(columnWidths[j]));
                Console.Write(" | ");
            }
            Console.WriteLine();
        }
    }
    public void Execute(DeleteQuery deleteQuery)
    {
        // init
        (var classList, _) = GetAirTrafficInfo(deleteQuery.ObjectClass);
        
        // executing conditions
        classList = ComparerHelpers.ConditionExecution(classList, deleteQuery.Conditions);
        
        foreach (var airtraffic in classList)
        {
            airtraffic.GetRemoved(this);
        }
    }
    public void Execute(AddQuery addQuery)
    {
        var factory = ObjectInitializer.Factories[AddQuery.ClassToFactoryCode[addQuery.ObjectClass]];
        var obj = factory.Create(addQuery.KeyValueDict);
        obj.GetStored(this); 
    }
    public void Execute(UpdateQuery updateQuery)
    {
        // init
        (var classList, _) = GetAirTrafficInfo(updateQuery.ObjectClass);
        
        // executing conditions
        classList = ComparerHelpers.ConditionExecution(classList, updateQuery.Conditions);
        foreach (var airTraffic in classList)
        {
            foreach (var keyVal in updateQuery.KeyValueDict)
            {
                airTraffic.Properties[keyVal.Key].Value = keyVal.Value;
            }
        }
    }
    private (List<AirTraffic> objectList, string[]? fieldNames) GetAirTrafficInfo(string objectClass)
    {
        // init
        List<AirTraffic> classList = new List<AirTraffic>();
        string[]? fieldNames = null;
        
        // database update
        Consolidate();
        
        // setting up classList
        switch (objectClass)
        {
            case "airport":
            {
                lock (AirportSync)
                    classList.AddRange(_airportDictionary.Values.ToList());
                fieldNames = Airport.fieldNames;
                break;
            }
            case "flight":
            {
                lock (FlightSync)
                    classList.AddRange(_flightDictionary.Values.ToList());
                fieldNames = Flight.fieldNames;
                break;
            }
            case "passengerplane":
            {
                lock (PassengerPlaneSync)
                    classList.AddRange(_passengerPlaneDictionary.Values.ToList());
                fieldNames = PassengerPlane.fieldNames;
                break;
            }
            case "cargoplane":
            {
                lock (CargoPlaneSync)
                    classList.AddRange(_cargoPlaneDictionary.Values.ToList());
                fieldNames = CargoPlane.fieldNames;
                break;
            }
            case "cargo":
            {
                lock (CargoSync)
                    classList.AddRange(_cargoDictionary.Values.ToList());
                fieldNames = Cargo.fieldNames;
                break;
            }
            case "passenger":
            {
                lock (PassengerSync)
                    classList.AddRange(_passengerDictionary.Values.ToList());
                fieldNames = Passenger.fieldNames;
                break;
            }
            case "crew":
            {
                lock (CrewSync)
                    classList.AddRange(_crewDictionary.Values.ToList());
                fieldNames = Crew.fieldNames;
                break;
            }
        }
        return (classList, fieldNames);
    }
    #endregion
    
    #region IRemove methods
    public void Remove(Airport airport)
    {
        lock (AirportSync)
            _airportDictionary.Remove(airport.ID);
    }
    public void Remove(Cargo cargo)
    {
        lock (CargoSync)
            _cargoDictionary.Remove(cargo.ID);
    }
    public void Remove(CargoPlane cargoPlane)
    {
        lock (CargoPlaneSync)
            _cargoPlaneDictionary.Remove(cargoPlane.ID);
    }
    public void Remove(Crew crew)
    {
        lock (CrewSync)
            _crewDictionary.Remove(crew.ID);
    }
    public void Remove(Flight flight)
    {
        lock (FlightSync)
            _flightDictionary.Remove(flight.ID);
    }
    public void Remove(Passenger passenger)
    {
        lock (PassengerSync)
            _passengerDictionary.Remove(passenger.ID);
    }
    public void Remove(PassengerPlane passengerPlane)
    {
        lock (PassengerPlaneSync)
            _passengerPlaneDictionary.Remove(passengerPlane.ID);
    }
    #endregion
}