using PoProj.classes;

namespace PoProj.MediaClasses;

public class Newspaper : IMedia
{
    private readonly string _name;

    public Newspaper(string name)
    {
        _name = name;
    }
    public string Report(Airport airport)
    {
        string output = _name + " - A report from the " + airport.Name + " airport, " + airport.Country + ".";
        return output;
    }

    public string Report(CargoPlane cargoPlane)
    {
        string output = _name + " - An interview with the crew of " + cargoPlane.Serial + ".";
        return output;
    }

    public string Report(PassengerPlane passengerPlane)
    {
        string output = _name + " - Breaking news! " + passengerPlane.Model + " aircraft loses EASA fails" +
                        " certification after inspection of" + passengerPlane.Serial + ".";
        return output;
    }
}