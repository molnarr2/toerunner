using Google.Cloud.Firestore;
using System;

namespace ToeRunner.Firebase;

/// <summary>
/// Converter for DateTime to Firestore Timestamp and vice versa
/// </summary>
public class DateTimeConverter : IFirestoreConverter<DateTime>
{
    public object ToFirestore(DateTime value)
    {
        // Ensure the DateTime is properly converted to UTC with the correct Kind
        DateTime utcDateTime = DateTime.SpecifyKind(value.ToUniversalTime(), DateTimeKind.Utc);
        return Timestamp.FromDateTime(utcDateTime);
    }

    public DateTime FromFirestore(object value)
    {
        if (value is Timestamp timestamp)
        {
            // Ensure the returned DateTime has the correct Kind (Utc)
            return DateTime.SpecifyKind(timestamp.ToDateTime(), DateTimeKind.Utc);
        }
        throw new ArgumentException($"Expected Timestamp but got {value?.GetType().Name ?? "null"}");
    }
}
