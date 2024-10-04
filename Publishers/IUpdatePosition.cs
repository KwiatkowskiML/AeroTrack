namespace PoProj.Publishers;

public interface IUpdatePosition
{
    void UpdatePosition(ulong objectId, float longitude, float latitude, float amsl);
}