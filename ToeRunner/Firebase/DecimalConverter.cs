using Google.Cloud.Firestore;
using System;

namespace ToeRunner.Firebase;

/// <summary>
/// Converter for decimal to double and vice versa for Firestore
/// </summary>
public class DecimalConverter : IFirestoreConverter<decimal>
{
    public object ToFirestore(decimal value)
    {
        // Convert decimal to double for Firestore storage
        return (double)value;
    }

    public decimal FromFirestore(object value)
    {
        if (value == null)
            return 0m;
            
        if (value is double doubleValue)
        {
            return (decimal)doubleValue;
        }
        else if (value is long longValue)
        {
            return (decimal)longValue;
        }
        else if (value is int intValue)
        {
            return (decimal)intValue;
        }
        else if (value is string stringValue && decimal.TryParse(stringValue, out decimal result))
        {
            return result;
        }
        
        throw new ArgumentException($"Cannot convert {value?.GetType().Name ?? "null"} to decimal");
    }
}
