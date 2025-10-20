using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The root object for a session's results file.
/// Contains all metadata and the list of all events that occurred.
/// </summary>

public class SessionResults
{
    public string sessionId;
    public string patientId;
    public string sessionStartTime;
    public string sessionEndTime;
    public List<MetricEvent> events;

    public SessionResults(string sessionId, string patientId)
    {
        this.sessionId = sessionId;
        this.patientId = patientId;
        this.events = new List<MetricEvent>();
    }
}

/// <summary>
/// Represents a single, timestamped event that occurred during an exercise.
/// This is the core data point for our process metrics.
/// </summary>

public class MetricEvent
{
    public float timestamp; // Time in seconds since the session started
    public string exerciseId;
    public string eventType; // e.g., "ExerciseStart", "CorrectClick", "DragEnd", "Timeout"
    public SerializableVector2 position; // Screen position of the event, if applicable
    public string details; // Any extra information, e.g., the ID of the clicked object

    public MetricEvent(float timestamp, string exerciseId, string eventType, Vector2? position, string details)
    {
        this.timestamp = timestamp;
        this.exerciseId = exerciseId;
        this.eventType = eventType;
        this.position = position.HasValue? new SerializableVector2(position.Value) : null;
        this.details = details;
    }
}

/// <summary>
/// A simple serializable wrapper for Unity's Vector2, as it is not serializable by default.
/// </summary>

public class SerializableVector2
{
    public float x;
    public float y;

    public SerializableVector2(Vector2 vector)
    {
        x = vector.x;
        y = vector.y;
    }
}