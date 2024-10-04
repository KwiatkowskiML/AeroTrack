using PoProj.classes;

namespace PoProj.features;

public class FlightAdapter: FlightGUI
{
    // Flight object reference
    private readonly Flight _flight;
    public FlightAdapter(Flight flight)
    {
        _flight = flight;
        ID = _flight.ID;
        WorldPosition = new WorldPosition((float)_flight.Latitude!, (float)_flight.Longitude!);
        MapCoordRotation = _flight.GetRotation();
    }
}