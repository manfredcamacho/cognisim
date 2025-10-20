[System.Serializable]
public class ExerciseParameters
{
    // --- Parámetros para el ejercicio: Calibración Motora ---
    public int numTargets; // Cantidad de círculos a tocar.

    // --- Parámetros para el ejercicio: Tarea de Cancelación Avanzada ---
    public string gridSize; // Ej: "8x10"
    public string targetSymbol; // El identificador del símbolo a buscar.
    public int durationSeconds; // Duración del ejercicio en segundos.

    // --- Parámetros para el ejercicio: Secuenciación de AVD ---
    public string scenario; // Ej: "preparar_desayuno", "hacer_la_compra"

    // --- Parámetros para el ejercicio: Reconocimiento Visual Personalizado ---
    public string imageSetId; // Identificador del set de imágenes a cargar (ej: "familia_perez")
    public int displayTimeSeconds; // Tiempo que cada imagen se muestra en pantalla.
    public int numDistractors; // Cantidad de imágenes distractoras a usar en la fase de reconocimiento.
}