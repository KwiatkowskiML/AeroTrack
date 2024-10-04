using PoProj.classes;
using PoProj.features;

namespace PoProj.MediaClasses;

public interface IMedia
{
    string Report(Airport airport);
    string Report(CargoPlane cargoPlane);
    string Report(PassengerPlane passengerPlane);
}