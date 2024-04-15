using System.Diagnostics;
using System.Xml.Schema;
using TestOptionalProperties;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    // there's a lot here that we'd want to refactor out.
    //var assembliesToSearch = AppDomain.CurrentDomain.GetAssemblies(); // too expansive.
    var assembliesToSearch = new[] {typeof(OptionValue<>).Assembly};
    
    var optionValueTypes = ReflectionUtils.FindPropertyTypes(assembliesToSearch,
        (t) => t.GetGenericTypeDefinition() == typeof(OptionValue<>)).ToList();
    
    var typesToCreate = optionValueTypes.Distinct().Select(t =>
    {
        var genericArgument = t.GetGenericArguments()[0];
        var underlyingType = Nullable.GetUnderlyingType(genericArgument);
        return new
        {
            // need check for IsClass because OptionValue<SomeClass?> doesn't actually exist as a type under the hood
            // so we may register an extra typeconverter that we don't need, but the alternative is that
            // nullability configuration won't work right, i.e.  OptionValue<string?> should accept a null
            // but OptionValue<string> should not.
            Type = t,
            MakeNullable = (underlyingType != null || genericArgument.IsClass),
            MakeNonNullable = underlyingType == null
        };
    }).ToList();
    
    
    // there is an issue here where reference types like string that are marked nullable are actually returned
    // with out the nullable attribute, which makes sense when you realize its somewhat a compiler hint :-(.
        
    typesToCreate.ForEach(ovt =>
        {
            Debug.WriteLine($"Creating converter for {ovt.Type.FullName} {ovt.MakeNullable} {ovt.MakeNonNullable}");
            if (ovt.MakeNonNullable)
                options.JsonSerializerOptions.Converters.Add(OptionValueTypeConverterFactory.CreateFor(ovt.Type, false));
            if (ovt.MakeNullable)
                options.JsonSerializerOptions.Converters.Add(OptionValueTypeConverterFactory.CreateFor(ovt.Type, true));
        });

    // this first one doesn't get found.
    // options.JsonSerializerOptions.Converters.Add(new OptionValueTypeConverterNullable<string?>());
    // options.JsonSerializerOptions.Converters.Add(new OptionValueTypeConverter<string>());
    // options.JsonSerializerOptions.Converters.Add(new OptionValueTypeConverterNullable<int?>());
    // options.JsonSerializerOptions.Converters.Add(new OptionValueTypeConverter<int>());
    // options.JsonSerializerOptions.Converters.Add(new OptionValueTypeConverter<DateOnly>());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


