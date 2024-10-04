namespace PoProj.classes;

public abstract class Plane: AirTraffic
{
    public string Serial { get; set; }
    public string Country { get; set; }
    public string Model { get; set; }

    protected Plane()
    {
        ID = 0;
        Serial = "";
        Country = "";
        Model = "";
    }
}