using PoProj.classes;

namespace PoProj.CommandsSupport;

public interface IRemove
{
    void Remove(Airport airport);
    void Remove(Cargo cargo);
    void Remove(CargoPlane cargoPlane);
    void Remove(Crew crew);
    void Remove(Flight flight);
    void Remove(Passenger passenger);
    void Remove(PassengerPlane passengerPlane);
}