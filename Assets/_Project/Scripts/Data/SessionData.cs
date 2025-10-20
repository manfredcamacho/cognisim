using System.Collections.Generic;

[System.Serializable]
public class SessionData
{
    public string sessionId;
    public string patientId;
    public List<ExerciseData> exercises;
}