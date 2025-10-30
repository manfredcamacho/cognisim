using System;

/// <summary>
/// Interface that must be implemented by all exercise controllers.
/// It acts as a contract that guarantees that each exercise will have an initialization method
/// and an event to notify when it has finished.
/// </summary>
public interface IExerciseController
{
    /// <summary>
    /// Event that will be triggered when the exercise has finished.
    /// The SessionController will subscribe to this event.
    /// </summary>
    event Action OnExerciseComplete;

    /// <summary>
    /// Initializes the exercise with the specific parameters loaded from the JSON.
    /// </summary>
    /// <param name="parameters">The object that contains all the configuration data.</param>
    void Initialize(string exerciseId, ExerciseParameters parameters);

    /// <summary>
    /// Starts the exercise logic.    
    /// </summary>
    void StartExercise();
}