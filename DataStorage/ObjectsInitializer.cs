using System.Text;
using PoProj.features;
using PoProj.Publishers;

namespace PoProj.DataStorage;

public class ObjectInitializer(EventManager eventManager)
{
    //
    private EventManager _eventManager = eventManager;
    
    // Dictionary of factory methods for each class
    public static readonly Dictionary<string, IFactory> Factories = new()
    {
        { "AI", new AirportFactory()},
        { "CP", new CargoPlaneFactory()},
        { "PP", new PassengerPlaneFactory()},
        { "CA", new CargoFactory()},
        { "C", new CrewFactory()},
        { "P", new PassengerFactory()},
        { "FL", new FlightFactory()}
    };

    // Code translation dictionary
    private static readonly Dictionary<string, string> NcodeTranslation = new()
    {
        { "NAI", "AI" },
        { "NCP", "CP" },
        { "NPP", "PP" },
        { "NCA", "CA" },
        { "NCR", "C" },
        { "NPA", "P" },
        { "NFL", "FL" }
    };

    // Filling database from file
    public void InitFromFile(string relativePath)
    {
        // Opening file with properly formatted lines
        var sr = new StreamReader(relativePath);
        string? line;
        
        // Adding objects to database
        while ((line = sr.ReadLine()) != null)
        {
            // Splitting properly formatted line into strings representing each property of a class
            string[] subStrings = line.Split(',');
            var objectCode = subStrings[0];
            
            // Creating an instance of appropriate class
            var airTraffic = Factories[objectCode].Create(subStrings);
            
            // Adding object to database waitlist
            airTraffic.GetLoaded(DataSingleton.Instance, eventManager);
        }
        sr.Close();
    }
    
    // Method
    public void AddBytes(byte[] bytes)
    {
        // Creating an instance of appropriate class
        var objectCode = Encoding.ASCII.GetString(bytes, 0, 3);
        var airTraffic = Factories[NcodeTranslation[objectCode]].Create(bytes);
        
        // Adding object to database waitlist
        airTraffic.GetLoaded(DataSingleton.Instance, eventManager);
    }
}