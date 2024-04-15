namespace TestOptionalProperties;

public interface OptionValue {} // this is a marker interface

public class OptionValue<T> : OptionValue
{
    private T _value;

    public T Value
    {
        get => _value;
        set 
        { 
            _value = value;
            IsSet = true;
        }
    }
    
    public bool IsSet { get; private set; }


    public OptionValue()
    {
    }

    public OptionValue(T value)
    {
        Value = value;
    }

    public static implicit operator OptionValue<T>(T value)
    {
        return new OptionValue<T>(value);
    }

    public static implicit operator T(OptionValue<T> ov)
    {
        if (ov.IsSet)
            return ov.Value;
        else
            throw new Exception("Trying to implicitly evaluate ObjectValue that has not been set");
    }

    public void SetValueIfExists(ref T receiver)
    {
        if (IsSet)
            receiver = this.Value;
    }

    private object? FormatValue()
    {
        if (IsSet)
        {
            if (Value is null) return "null"; // all that for this.
            else if (typeof(T) == typeof(string)) return $"\"{Value}\"";
            else return Value;
        }
        else
        {
            return "UNSET";
        }
    }
    public override string ToString()
    {
        return $"OptionValue<{TypeFormatter.GetFormattedTypeName(typeof(T))}>({FormatValue()})";
    }
}