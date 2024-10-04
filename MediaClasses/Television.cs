using PoProj.classes;

namespace PoProj.MediaClasses;

public class Television: IMedia
{
    private readonly string name;

    public Television(string name)
    {
        this.name = name;
    }
    
    public string Report(Airport airport)
    {
        string output = "<An image of " + airport.Name + " airport>";
        return output;
    }
    
    public string Report(CargoPlane cargoPlane)
    {
        string output = "<An image of " + cargoPlane.Model + " cargo plane>";
        return output;
    }
    
    public string Report(PassengerPlane passengerPlane)
    {
        string output = "<An image of " + passengerPlane.Model + " passenger plane>";
        return output;
    }
}