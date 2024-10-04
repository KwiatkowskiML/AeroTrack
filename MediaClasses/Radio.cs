using PoProj.classes;

namespace PoProj.MediaClasses;

public class Radio : IMedia
{
    private readonly string _name;

    public Radio(string name)
    {
        _name = name;
    }
    public string Report(Airport airport)
    {
        string output = "Reporting for " + _name + ". Ladies and gentlemen, we are at the " +
                        airport.Name + " airport.";
        return output;
    }

    public string Report(CargoPlane cargoPlane)
    {
        string output = "Reporting for " + _name + ". Ladies and gentlemen, we are seeing the " +
                        cargoPlane.Serial + " aircraft fly above us.";
        return output;
    }

    public string Report(PassengerPlane passengerPlane)
    {
        string output = "Reporting for " + _name + ". Ladies and gentlemen, we've just witnessed " +
                        passengerPlane.Serial + " take off.";
        return output;
    }
}