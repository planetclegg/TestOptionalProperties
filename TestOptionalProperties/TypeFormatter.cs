namespace TestOptionalProperties;

public static class TypeFormatter
{
    public static string GetFormattedTypeName(Type? type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            // Handle nullable types
            var underlyingType = Nullable.GetUnderlyingType(type);
            return $"{GetFormattedTypeName(underlyingType)}?";
        }
        else if (type.IsGenericType)
        {
            // Handle generic types (e.g., List<T>)
            var genericTypeName = type.Name.Substring(0, type.Name.IndexOf('`'));
            var genericArgs = type.GetGenericArguments();
            var formattedArgs = string.Join(", ", Array.ConvertAll(genericArgs, GetFormattedTypeName));
            return $"{genericTypeName}<{formattedArgs}>";
        }
        else
        {
            // Apply aliases for common types
            switch (type.FullName)
            {
                case "System.Int32":
                    return "int";
                case "System.String":
                    return "string";
                case "System.Double":
                    return "double";
                case "System.Boolean":
                    return "bool";
                default:
                    // Return the type name for non-alias types
                    return type.Name;
            }
        }
    }
}