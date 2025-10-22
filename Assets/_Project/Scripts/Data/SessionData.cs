using System.Collections.Generic;

[System.Serializable]
public class SessionData
{
    public string sessionId;
    public string patientId;
    public string patientName;
    public List<ExerciseData> exercises;
}