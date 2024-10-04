using System.Globalization;
using System.Text;
using PoProj.classes;

namespace PoProj.features;

public interface IFactory
{
    public AirTraffic Create(string[] subStrings);
    public AirTraffic Create(byte[] bytes);
    public AirTraffic Create(Dictionary<string, string> keyVal);
}

public class AirportFactory : IFactory
{
    public AirTraffic Create(string[] subStrings)
    {
        // Wrong class indication
        if (subStrings[0] != "AI")
            throw new ArgumentException("Considered line does not refer to Airport class object.");

        // Initializing each property
        var id = ulong.Parse(subStrings[1]);
        var name = subStrings[2];
        var code = subStrings[3];
        var longitude = float.Parse(subStrings[4], CultureInfo.InvariantCulture.NumberFormat);
        var latitude = float.Parse(subStrings[5], CultureInfo.InvariantCulture.NumberFormat);
        var amsl = float.Parse(subStrings[6], CultureInfo.InvariantCulture.NumberFormat);
        var country = subStrings[7];

        // Adding airport to DataStorage class
        return new Airport(id, name, code, longitude, latitude, amsl, country);
    }
    public AirTraffic Create(byte[] bytes)
    {
        int offset = 0;
        
        // Wrong class indication
        string classCode = Parser<string>.GetStringFromBytes(bytes, offset, 3);
        if (classCode != "NAI")
            throw new ArgumentException("Considered bytes do not refer to Airport class object.");
        offset += 3;
        
        // Initializing each property
        var messageLength = BitConverter.ToUInt32(bytes, offset);
        offset += 4;
        var id = BitConverter.ToUInt64(bytes, offset);
        offset += 8;
        var nameLength = BitConverter.ToUInt16(bytes, offset);
        offset += 2;
        var name = Parser<string>.GetStringFromBytes(bytes, offset, nameLength);
        offset += nameLength;
        var code = Parser<string>.GetStringFromBytes(bytes, offset, 3);
        offset += 3;
        var longitude = BitConverter.ToSingle(bytes, offset);
        offset += 4;
        var latitude = BitConverter.ToSingle(bytes, offset);
        offset += 4;
        var amsl = BitConverter.ToSingle(bytes, offset);
        offset += 4;
        var country = Parser<string>.GetStringFromBytes(bytes, offset, 3);

        // Adding airport to DataStorage class
        return new Airport(id, name, code, longitude, latitude, amsl, country);
    }
    public AirTraffic Create(Dictionary<string, string> keyVal)
    {
        if (keyVal.Count != Airport.fieldNames.Length)
            throw new ArgumentException("Not enough arguments to create Airport object");
        return Create(new string[]
        {
            "AI",
            keyVal["id"],
            keyVal["name"],
            keyVal["code"],
            keyVal["worldposition.long"],
            keyVal["worldposition.lat"],
            keyVal["amsl"],
            keyVal["countrycode"]
        });
    }
}   

public class CargoFactory : IFactory
{
    public AirTraffic Create(string[] subStrings)
    {
        // Wrong class indication
        if (subStrings[0] != "CA")
            throw new ArgumentException("Considered line does not refer to Cargo class object.");

        // Initializing each property
        var id = ulong.Parse(subStrings[1]);
        var weight = float.Parse(subStrings[2], CultureInfo.InvariantCulture.NumberFormat);
        var code = subStrings[3];
        var description = subStrings[4];

        return new Cargo(id, weight, code, description);
    }
    
    public AirTraffic Create(byte[] bytes)
    {
        int offset = 0;
        
        // Wrong class indication
        string classCode = Parser<string>.GetStringFromBytes(bytes, offset, 3);
        if (classCode != "NCA")
            throw new ArgumentException("Considered bytes do not refer to Cargo class object.");
        offset += 3;
        
        // Initializing each property
        var messageLength = BitConverter.ToUInt32(bytes, offset);
        offset += 4;
        var id = BitConverter.ToUInt64(bytes, offset);
        offset += 8;
        var weight = BitConverter.ToSingle(bytes, offset);
        offset += 4;
        var code = Parser<string>.GetStringFromBytes(bytes, offset, 6);
        offset += 6;
        var descriptionLength = BitConverter.ToUInt16(bytes, offset);
        offset += 2;
        var description = Parser<string>.GetStringFromBytes(bytes, offset, descriptionLength);

        return new Cargo(id, weight, code, description);
    }

    public AirTraffic Create(Dictionary<string, string> keyVal)
    {
        if (keyVal.Count != Cargo.fieldNames.Length)
            throw new ArgumentException("Not enough arguments to create Cargo object");
        return Create(new string[]
        {
            "CA",
            keyVal["id"],
            keyVal["weight"],
            keyVal["code"],
            keyVal["description"]
        });
    }
}

public class CargoPlaneFactory : IFactory
{
    public AirTraffic Create(string[] subStrings)
    {
        // Wrong class indication
        if (subStrings[0] != "CP")
            throw new ArgumentException("Considered line does not refer to CargoPlane class object.");

        // Initializing each property
        var id = ulong.Parse(subStrings[1]);
        var serial = subStrings[2];
        var country = subStrings[3];
        var model = subStrings[4];
        var maxLoad = float.Parse(subStrings[5], CultureInfo.InvariantCulture.NumberFormat);

        return new CargoPlane(id, serial, country, model, maxLoad);
    }
    public AirTraffic Create(byte[] bytes)
    {
        int offset = 0;
        
        // Wrong class indication
        string classCode = Parser<string>.GetStringFromBytes(bytes, offset, 3);
        if (classCode != "NCP")
            throw new ArgumentException("Considered bytes do not refer to CargoPlane class object.");
        offset += 3;
        
        // Initializing each property
        var messageLength = BitConverter.ToUInt32(bytes, offset);
        offset += 4;
        var id = BitConverter.ToUInt64(bytes, offset);
        offset += 8;
        var serial = Parser<string>.GetStringFromBytes(bytes, offset, 10);
        offset += 10;
        var country = Parser<string>.GetStringFromBytes(bytes, offset, 3);
        offset += 3;
        var modelLength = BitConverter.ToUInt16(bytes, offset);
        offset += 2;
        var model = Parser<string>.GetStringFromBytes(bytes, offset, modelLength);
        offset += modelLength;
        var maxLoad = BitConverter.ToSingle(bytes, offset);

        return new CargoPlane(id, serial, country, model, maxLoad);
    }
    public AirTraffic Create(Dictionary<string, string> keyVal)
    {
        if (keyVal.Count != CargoPlane.fieldNames.Length)
            throw new ArgumentException("Not enough arguments to create CargoPlane object");
        return Create(new string[]
        {
            "CP",
            keyVal["id"],
            keyVal["serial"],
            keyVal["countrycode"],
            keyVal["model"],
            keyVal["maxload"]
        });
    }
}

public class CrewFactory : IFactory
{
    public AirTraffic Create(string[] subStrings)
    {
        // Wrong class indication
        if (subStrings[0] != "C")
            throw new ArgumentException("Considered line does not refer to Crew class object.");

        // Initializing each property
        var id = ulong.Parse(subStrings[1]);
        var name = subStrings[2];
        var age = ulong.Parse(subStrings[3]);
        var phone = subStrings[4];
        var email = subStrings[5];
        var practice = ushort.Parse(subStrings[6]);
        var role = subStrings[7];

        return new Crew(id, name, age, phone, email, practice, role);
    }
    public AirTraffic Create(byte[] bytes)
    {
        int offset = 0;
        
        // Wrong class indication
        string classCode = Parser<string>.GetStringFromBytes(bytes, offset, 3);
        if (classCode != "NCR")
            throw new ArgumentException("Considered bytes do not refer to Crew class object.");
        offset += 3;
        
        // Initializing each property
        var messageLength = BitConverter.ToUInt32(bytes, offset);
        offset += 4;
        var id = BitConverter.ToUInt64(bytes, offset);
        offset += 8;
        var nameLength = BitConverter.ToUInt16(bytes, offset);
        offset += 2;
        var name = Parser<string>.GetStringFromBytes(bytes, offset, nameLength);
        offset += nameLength;
        var age = BitConverter.ToUInt16(bytes, offset);
        offset += 2;
        var phone = Parser<string>.GetStringFromBytes(bytes, offset, 12);
        offset += 12;
        var emailLength = BitConverter.ToUInt16(bytes, offset);
        offset += 2;
        var email = Parser<string>.GetStringFromBytes(bytes, offset, emailLength);
        offset += emailLength;
        var practice = BitConverter.ToUInt16(bytes, offset);
        offset += 2;
        var role = Parser<string>.GetStringFromBytes(bytes, offset, 1);
        
        return new Crew(id, name, age, phone, email, practice, role);
    }
    public AirTraffic Create(Dictionary<string, string> keyVal)
    {
        if (keyVal.Count != Crew.fieldNames.Length)
            throw new ArgumentException("Not enough arguments to create Crew object");
        return Create(new string[]
        {
            "C",
            keyVal["id"],
            keyVal["name"],
            keyVal["age"],
            keyVal["phone"],
            keyVal["email"],
            keyVal["practice"],
            keyVal["role"]
        });
    }
}

public class FlightFactory : IFactory
{
    public AirTraffic Create(string[] subStrings)
    {
        // Wrong class indication
        if (subStrings[0] != "FL")
            throw new ArgumentException("Considered line does not refer to Flight class object.");

        // Initializing each property
        var id = ulong.Parse(subStrings[1]);
        var originId = ulong.Parse(subStrings[2]);
        var targetId = ulong.Parse(subStrings[3]);
        var takeoffTime = subStrings[4];
        var landingTime = subStrings[5];
        var longitude = float.Parse(subStrings[6], CultureInfo.InvariantCulture.NumberFormat);
        var latitude = float.Parse(subStrings[7], CultureInfo.InvariantCulture.NumberFormat);
        var amsl = float.Parse(subStrings[8], CultureInfo.InvariantCulture.NumberFormat);
        var planeId = ulong.Parse(subStrings[9]);
        var crewId = Parser<ulong>.StringToArray(subStrings[10]);
        var loadId = Parser<ulong>.StringToArray(subStrings[11]);

        return new Flight(id, originId, targetId, takeoffTime, landingTime, longitude, latitude, amsl, planeId, crewId,
            loadId);
    }
    public AirTraffic Create(byte[] bytes)
    {
        int offset = 0;
        
        // Wrong class indication
        string classCode = Parser<string>.GetStringFromBytes(bytes, offset, 3);
        if (classCode != "NFL")
            throw new ArgumentException("Considered bytes do not refer to Flight class object.");
        offset += 3;
        
        // Initializing each property
        var messageLength = BitConverter.ToUInt32(bytes, offset);
        offset += 4;
        var id = BitConverter.ToUInt64(bytes, offset);
        offset += 8;
        var originId = BitConverter.ToUInt64(bytes, offset);
        offset += 8;
        var targetId = BitConverter.ToUInt64(bytes, offset);
        offset += 8;
        var msTakeoffTime = BitConverter.ToInt64(bytes, offset);
        offset += 8;
        var msLandingTime = BitConverter.ToInt64(bytes, offset);
        offset += 8;
        var takeoffTime = $"{DateTime.UnixEpoch.AddMilliseconds(msTakeoffTime):HH:mm}";
        var landingTime = $"{DateTime.UnixEpoch.AddMilliseconds(msLandingTime):HH:mm}";
        float? longitude = null;
        float? latitude = null;
        float? amsl = null;
        var planeId = BitConverter.ToUInt64(bytes, offset);
        offset += 8;
        
        var crewCount = BitConverter.ToUInt16(bytes, offset);
        offset += 2;
        var crewId = new ulong[crewCount];
        for (int i = 0; i < crewCount; i++)
        {
            crewId[i] = BitConverter.ToUInt64(bytes, offset);
            offset += 8;
        }

        var loadCount = BitConverter.ToUInt16(bytes, offset);
        offset += 2;
        var loadId = new ulong[loadCount];
        for (int i = 0; i < loadCount; i++)
        {
            loadId[i] = BitConverter.ToUInt64(bytes, offset);
            offset += 8;
        }

        return new Flight(id, originId, targetId, takeoffTime, landingTime, longitude, latitude, amsl, planeId, crewId,
            loadId);
    }
    public AirTraffic Create(Dictionary<string, string> keyVal)
    {
        if (keyVal.Count != Flight.fieldNames.Length)
            throw new ArgumentException("Not enough arguments to create Flight object");
        return Create(new string[]
        {
            "FL",
            keyVal["id"],
            keyVal["origin"],
            keyVal["target"],
            keyVal["takeofftime"],
            keyVal["landingtime"],
            keyVal["worldposition.long"],
            keyVal["worldposition.long"],
            keyVal["amsl"],
            keyVal["plane"],
            "[60]",
            "[351]"
        });
    }
}

public class PassengerFactory : IFactory
{
    public AirTraffic Create(string[] subStrings)
    {
        // Wrong class indication
        if (subStrings[0] != "P")
            throw new ArgumentException("Considered line does not refer to Passenger class object.");

        // Initializing each property
        var id = ulong.Parse(subStrings[1]);
        var name = subStrings[2];
        var age = ulong.Parse(subStrings[3]);
        var phone = subStrings[4];
        var email = subStrings[5];
        var pclass = subStrings[6];
        var miles = ulong.Parse(subStrings[7]);

        return new Passenger(id, name, age, phone, email, pclass, miles);
    }
    public AirTraffic Create(byte[] bytes)
    {
        int offset = 0;
        
        // Wrong class indication
        string classCode = Parser<string>.GetStringFromBytes(bytes, offset, 3);
        if (classCode != "NPA")
            throw new ArgumentException("Considered bytes do not refer to Passenger class object.");
        offset += 3;
        
        // Initializing each property
        var messageLength = BitConverter.ToUInt32(bytes, offset);
        offset += 4;
        var id = BitConverter.ToUInt64(bytes, offset);
        offset += 8;
        var nameLength = BitConverter.ToUInt16(bytes, offset);
        offset += 2;
        var name = Parser<string>.GetStringFromBytes(bytes, offset, nameLength);
        offset += nameLength;
        var age = BitConverter.ToUInt16(bytes, offset);
        offset += 2;
        var phone = Parser<string>.GetStringFromBytes(bytes, offset, 12);
        offset += 12;
        var emailLength = BitConverter.ToUInt16(bytes, offset);
        offset += 2;
        var email = Parser<string>.GetStringFromBytes(bytes, offset, emailLength);
        offset += emailLength;
        var pclass = Parser<string>.GetStringFromBytes(bytes, offset, 1);
        offset += 1;
        var miles = BitConverter.ToUInt64(bytes, offset);

        return new Passenger(id, name, age, phone, email, pclass, miles);
    }
    public AirTraffic Create(Dictionary<string, string> keyVal)
    {
        if (keyVal.Count != Passenger.fieldNames.Length)
            throw new ArgumentException("Not enough arguments to create Passenger object");
        return Create(new string[]
        {
            "P",
            keyVal["id"],
            keyVal["name"],
            keyVal["age"],
            keyVal["phone"],
            keyVal["email"],
            keyVal["class"],
            keyVal["miles"]
        });
    }
}

public class PassengerPlaneFactory : IFactory
{
    public AirTraffic Create(string[] subStrings)
    {
        // Wrong class indication
        if (subStrings[0] != "PP")
            throw new ArgumentException("Considered line does not refer to PassengerPlane class object.");

        // Initializing each property
        var id = ulong.Parse(subStrings[1]);
        var serial = subStrings[2];
        var country = subStrings[3];   
        var model = subStrings[4];
        var firstClassSize = ushort.Parse(subStrings[5]);
        var businessClassSize = ushort.Parse(subStrings[6]);
        var economyClassSize = ushort.Parse(subStrings[7]);

        return new PassengerPlane(id, serial, country, model, firstClassSize, businessClassSize, economyClassSize);
    }
    public AirTraffic Create(byte[] bytes)
    {
        int offset = 0;
        
        // Wrong class indication
        string classCode = Parser<string>.GetStringFromBytes(bytes, offset, 3);
        if (classCode != "NPP")
            throw new ArgumentException("Considered bytes do not refer to PassengerPlane class object.");
        offset += 3;
        
        // Initializing each property
        var messageLength = BitConverter.ToUInt32(bytes, offset);
        offset += 4;
        var id = BitConverter.ToUInt64(bytes, offset);
        offset += 8;
        var serial = Parser<string>.GetStringFromBytes(bytes, offset, 10);
        offset += 10;
        var country = Parser<string>.GetStringFromBytes(bytes, offset, 3);
        offset += 3;
        var modelLength = BitConverter.ToUInt16(bytes, offset);
        offset += 2;
        var model = Parser<string>.GetStringFromBytes(bytes, offset, modelLength);
        offset += modelLength;
        var firstClassSize = BitConverter.ToUInt16(bytes, offset);
        offset += 2;
        var businessClassSize = BitConverter.ToUInt16(bytes, offset);
        offset += 2;
        var economyClassSize = BitConverter.ToUInt16(bytes, offset);

        return new PassengerPlane(id, serial, country, model, firstClassSize, businessClassSize, economyClassSize);
    }
    public AirTraffic Create(Dictionary<string, string> keyVal)
    {
        if (keyVal.Count != PassengerPlane.fieldNames.Length)
            throw new ArgumentException("Not enough arguments to create PassengerPlane object");
        return Create(new string[]
        {
            "PP",
            keyVal["id"],
            keyVal["serial"],
            keyVal["countrycode"],
            keyVal["firstclasssize"],
            keyVal["businessclasssize"],
            keyVal["economyclasssize"]
        });
    }
}