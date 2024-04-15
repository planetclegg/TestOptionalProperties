using System.Reflection;

namespace TestOptionalProperties;

public static class ReflectionUtils
{
    // finds all the types of all properties of all types in an assembly.  <-- Yes I wrote that correctly.
    // a selector is used to limit the scope of the returned types
    public static IEnumerable<Type> FindPropertyTypes(IEnumerable<Assembly> fromAssemblies,
        Func<Type, bool> selector)
    {
        var props= fromAssemblies
                .SelectMany(a => a.GetTypes())
                .SelectMany(t => t.GetProperties())
                .Where(p => p.PropertyType.IsGenericType)
                .Select(p => p.PropertyType)
                //.Where(t=> !t.FullName.StartsWith("System.")) // hmmm
                .Select(t=>t);
                //.Distinct(); // space/time tradeoff on where you put this distinct. 
        return props.Where(selector).Distinct();
    }

    // a little bit of an issue
    // second arg (t) => t == typeof(PropertyValue<>)
    // second arg (t) => t == typeof(PropertyValue<>)
    // && Nullable.GetUnderlyingType(t.GetGenericArguments())[0] =   =null
    public static IEnumerable<Type> ActivateSurrogateGenericTypes(Type surrogateType, Func<Type, bool> sourceTypeSelector)
    {
        var stuff = FindPropertyTypes(AppDomain.CurrentDomain.GetAssemblies(), sourceTypeSelector);
        return stuff;
    }

}