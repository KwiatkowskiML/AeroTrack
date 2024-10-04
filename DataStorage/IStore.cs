using PoProj.classes;

namespace PoProj.DataStorage;

public interface IStore
{
    #region Load methods
    void Load(Airport airport);
    void Load(Cargo cargo);
    void Load(CargoPlane cargoPlane);
    void Load(Crew crew);
    void Load(Flight flight);
    void Load(Passenger passenger);
    void Load(PassengerPlane passengerPlane);
    #endregion
    
    #region Store methods
    bool Store(Airport airport);
    bool Store(Cargo cargo);
    bool Store(CargoPlane cargoPlane);
    bool Store(Crew crew);
    bool Store(Flight flight);
    bool Store(Passenger passenger);
    bool Store(PassengerPlane passengerPlane);
    #endregion
}