using Google.Cloud.Firestore;
using System;

namespace ToeRunner.Firebase;

/// <summary>
/// Converter for TimeSpan to seconds (double) and vice versa for Firestore
/// </summary>
public class TimeSpanToSecondsConverter : IFirestoreConverter<TimeSpan>
{
    public object ToFirestore(TimeSpan value)
    {
        return value.TotalSeconds;
    }

    public TimeSpan FromFirestore(object value)
    {
        if (value is double seconds)
        {
            return TimeSpan.FromSeconds(seconds);
        }
        else if (value is long longSeconds)
        {
            return TimeSpan.FromSeconds(longSeconds);
        }
        throw new ArgumentException($"Expected double or long but got {value?.GetType().Name ?? "null"}");
    }
}
