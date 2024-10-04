namespace PoProj.Publishers;

public sealed class LogManager
{
    private static volatile LogManager? _instance;
    private static readonly object SyncRoot = new();
    private readonly string _path;

    // Initializing log file path
    private LogManager()
    {
        _path = $"update_logs/log_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.txt";
    }
    
    // Initializing LogManager Instance
    public static LogManager Instance
    {
        get
        {
            if (_instance != null) return _instance;
            lock (SyncRoot)
            {
                if (_instance != null) return _instance;
                _instance = new LogManager();
            }
            return _instance;
        }
    }

    public void IdUpdateLog(ulong oldId, ulong newId)
    {
        using (var sw = new StreamWriter(_path, append: true))
        {
            sw.WriteLine($"ID_UPDATE: OLD_ID={oldId}, NEW_ID={newId}");
        }
    }

    public void PosUpdateLog(ulong objectId, string oldLongitude, string oldLatitude, string oldAmsl, 
        float newLongitude, float newLatitude, float newAmsl)
    {
        using (var sw = new StreamWriter(_path, append: true))
        {
            sw.WriteLine($"POSITION_UPDATE: OBJECT_ID={objectId}, OLD_LONGITUDE=" + oldLongitude +
                         $", NEW_LONGITUDE={newLongitude}, OLD_LATITUDE=" + oldLatitude + $", NEW_LATITUDE={newLatitude}" +
                         $", OLD_AMSL=" + oldAmsl + $", NEW_AMSL={newAmsl}");
        }
    }

    public void ContactUpdateLog(ulong objectId, string oldPhone, string oldEmail, string newPhone, string newEmail)
    {
        using (var sw = new StreamWriter(_path, append: true))
        {
            sw.WriteLine($"CONTACT_INFO_UPDATE: OBJECT_ID={objectId}, OLD_PHONE_NUMBER={oldPhone}," +
                         $" NEW_PHONE_NUMBER={newPhone}, OLD_EMAIL_ADDRESS={oldEmail}, NEW_EMAIL_ADDRESS={newEmail}");
        }
    }
}