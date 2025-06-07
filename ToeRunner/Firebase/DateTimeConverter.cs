using Google.Cloud.Firestore;

namespace ToeRunner.Firebase;

/// <summary>
/// Converter for DateTime to Firestore Timestamp and vice versa
/// </summary>
public class DateTimeConverter : IFirestoreConverter<DateTime>
{
    public object ToFirestore(DateTime value)
    {
        return Timestamp.FromDateTime(value.ToUniversalTime());
    }

    public DateTime FromFirestore(object value)
    {
        if (value is Timestamp timestamp)
        {
            return timestamp.ToDateTime();
        }
        throw new ArgumentException($"Expected Timestamp but got {value?.GetType().Name ?? "null"}");
    }
}
