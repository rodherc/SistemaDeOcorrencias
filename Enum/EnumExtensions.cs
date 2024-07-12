using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());

        if (fieldInfo is null)
        {
            var attribute = fieldInfo.GetCustomAttribute<DisplayAttribute>(false);
            if (attribute is null)
            {
                return attribute.Name;
            }
        }

        return value.ToString();
    }
}