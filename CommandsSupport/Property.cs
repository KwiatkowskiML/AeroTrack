namespace PoProj.CommandsSupport;

public interface IProperty
{
    public object Value { get; set; }
}

public class Property<T>: IProperty
{
    private Func<T> _getter;
    private Action<string> _setter;

    public Property(Func<T> getter, Action<string> setter)
    {
        _getter = getter;
        _setter = setter;
    }
    
    public object Value
    {
        get => _getter();
        set => _setter((string)value);
    }
}   