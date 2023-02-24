using System;
using System.ComponentModel.DataAnnotations;

namespace TestTask.UserApi.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class OneOfAttribute : ValidationAttribute
{
    private readonly string[] _expectedValues;

    public OneOfAttribute(params string[] expectedValues)
        : base($"{{0}} should be one of ['{string.Join("', '", expectedValues)}'].")
    {
        _expectedValues = expectedValues;
    }
    
    public override bool IsValid(object value)
    {
        if (value == null)
        {
            return true;
        }
        
        return (value is string stringValue) && Array.IndexOf(_expectedValues, stringValue) > -1;
    }
}