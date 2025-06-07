using Google.Cloud.Firestore;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ToeRunner.Firebase;

/// <summary>
/// Converter for object to string (JSON) and vice versa for Firestore
/// </summary>
public class DynamicToStringConverter : IFirestoreConverter<object>
{
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public object ToFirestore(object value)
    {
        if (value == null)
            return string.Empty;
        
        try
        {
            // Explicitly convert to string to ensure Firestore receives a string value
            string jsonString = JsonSerializer.Serialize(value, _jsonOptions);
            return jsonString;
        }
        catch (Exception ex)
        {
            // If serialization fails, return a simple error message as JSON
            return $"{{\"error\":\"Failed to serialize: {ex.Message.Replace("\"", "\\\"")}\"}}";
        }
    }

    public object FromFirestore(object value)
    {
        if (value == null || (value is string s && string.IsNullOrEmpty(s)))
            return new JsonElement();
            
        if (value is string json)
        {
            try
            {
                // Parse the JSON string into a JsonDocument and return its root element
                using JsonDocument doc = JsonDocument.Parse(json);
                return doc.RootElement.Clone();
            }
            catch (JsonException ex)
            {
                // If parsing fails, return an object with error information
                return new { Error = $"Failed to parse JSON: {ex.Message}" };
            }
        }
        
        // If the value is not a string, try to convert it to a string and parse it
        try
        {
            string jsonString = value?.ToString() ?? string.Empty;
            using JsonDocument doc = JsonDocument.Parse(jsonString);
            return doc.RootElement.Clone();
        }
        catch
        {
            // If all else fails, return an object with the original value as a string
            return new { OriginalValue = value?.ToString() ?? "null" };
        }
    }
}
