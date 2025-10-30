using System.Collections.Generic;

[System.Serializable]
public class ExerciseParameters
{
    // --- Parameters for the exercise: Motor Calibration ---
    public int numTargets; // Number of circles to touch.

    // --- Parameters for the exercise: Advanced Cancellation Task ---
    public string gridSize; // Example: "8x10"
    public string targetShape; // The identifier of the symbol to search for.
    public string targetColor; // The color of the target shape.
    public int durationSeconds; // Duration of the exercise in seconds.
    public List<string> distractorShapes;
    public string distractorsColor; // The color of the distractor shapes. In the future, we might want to support multiple colors.

    // --- Parameters for the exercise: Executive Sequencing ---
    public string scenario; // Example: "prepare_breakfast", "make_the_purchase"

    // --- Parameters for the exercise: Visual Recognition ---
    public string imageSetId; // Identifier of the image set to load (example: "family_perez")
    public int displayTimeSeconds; // Time each image is displayed on the screen in seconds.
    public int numDistractors; // Number of distractor images to use in the recognition phase.
}