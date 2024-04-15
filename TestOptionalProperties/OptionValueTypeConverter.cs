using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestOptionalProperties;

public class OptionValueTypeConverterFactory
{

    public static JsonConverter CreateFor(Type optionValueType, bool makeNullable)
    {
        if (optionValueType.GetGenericTypeDefinition() != typeof(OptionValue<>))
            throw new ArgumentException($"I can only create json converters for OptionValue<> generic types, not {optionValueType.FullName}");

        var baseOfTypeToInstantiate =
            !makeNullable
                ? typeof(OptionValueTypeConverter<>) // for non-null generic args eg OptionValue<int>
                : typeof(OptionValueTypeConverterNullable<>); // for null generic args eg. OptionValue<int?>

        var genericArgument = optionValueType.GetGenericArguments()[0];
        var actualType = baseOfTypeToInstantiate.MakeGenericType(genericArgument);
        // TODO: verify its a JsonConverter?
        
        var result = Activator.CreateInstance(actualType) as JsonConverter;
        return result;

    }

    public static JsonConverter CreateFor(Type optionValueType)
    {
        if (optionValueType.GetGenericTypeDefinition() != typeof(OptionValue<>))
            throw new ArgumentException($"I can only create json converters for OptionValue<> generic types, not {optionValueType.FullName}");

        var genericArgument = optionValueType.GetGenericArguments()[0];
        var underlyingType = Nullable.GetUnderlyingType(genericArgument);

        var baseOfTypeToInstantiate =
            underlyingType == null
                ? typeof(OptionValueTypeConverter<>) // for non-null generic args eg OptionValue<int>
                : typeof(OptionValueTypeConverterNullable<>); // for null generic args eg. OptionValue<int?>
        
        var actualType = baseOfTypeToInstantiate.MakeGenericType(genericArgument);
        // TODO: verify its a JsonConverter?
        
        var result = Activator.CreateInstance(actualType) as JsonConverter;
        return result;
    }
}


public class OptionValueTypeConverter<T> : JsonConverter<OptionValue<T>>
{
    public override bool HandleNull => false;

    public override OptionValue<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // FIXME: null warnings. probably just throw exception since it should never be null.
        
        T? value = JsonSerializer.Deserialize<T>(ref reader, options);
        if (value is null)
            throw new Exception("didn't expect this to be null");
        
        return new OptionValue<T>(value);
    }

    public override void Write(Utf8JsonWriter writer, OptionValue<T> value, JsonSerializerOptions options)
    {
        if (value.IsSet)
            JsonSerializer.Serialize(writer, value.Value, options);
    }
}

public class OptionValueTypeConverterNullable<T> : JsonConverter<OptionValue<T?>>
{
    public override bool HandleNull => true;

    public override OptionValue<T?>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        T? value = JsonSerializer.Deserialize<T?>(ref reader, options);
        return new OptionValue<T?>(value);
    }

    public override void Write(Utf8JsonWriter writer, OptionValue<T?> value, JsonSerializerOptions options)
    {
        if (value.IsSet)
            JsonSerializer.Serialize(writer, value.Value, options);
    }

}