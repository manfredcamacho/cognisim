using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Tooltip("Arrastra aquí el asset de ObjectData que define este objeto.")]
    public ObjectData objectData;

    [Tooltip("Propiedad específica de esta instancia, como su color.")]
    public string instanceColor; // Podría ser un enum también si los colores son fijos.

    // Podríamos añadir más propiedades de instancia aquí, como la ubicación.
    public string instanceLocation;

    void OnValidate()
    {
        // Pequeño truco para que el nombre del GameObject en la jerarquía refleje
        // el tipo de objeto y su color, facilitando la organización.
        if (objectData != null && !string.IsNullOrEmpty(instanceColor))
        {
            gameObject.name = $"{objectData.displayName} ({instanceColor})";
        }
        else if (objectData != null)
        {
            gameObject.name = objectData.displayName;
        }
    }
}
