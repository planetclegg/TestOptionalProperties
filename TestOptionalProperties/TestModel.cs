using System.ComponentModel.DataAnnotations;

namespace TestOptionalProperties;

public class TestModel
{
    // this one is optional but can't pass in null 
    public OptionValue<int> Age { get; set; } = new();
    
    
    // this one can be set to null
    public OptionValue<int?> NullableAge { get; set; } = new();
    
    //[StringLength(10)] // the default validators do NOT work.
    public OptionValue<string> UserName { get; set; } = new();
    
    // this one can be set to null but isn't working.
    public OptionValue<string?> NullableUserName { get; set; } = new();

    public OptionValue<string> Address { get; set; } = new();
    public OptionValue<DateOnly> SomeDate { get; set; } = new();
    
    public override string ToString()
    {
        return $"TestModel(\nAge={Age}, \nNullableAge={NullableAge}, \nUserName={UserName}, "+
               $"\nNullableUserName={NullableUserName}, \nSomeDate={SomeDate})";
    }
}