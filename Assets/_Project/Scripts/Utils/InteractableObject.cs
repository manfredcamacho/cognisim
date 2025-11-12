using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Tooltip("Arrastra aquí el asset de ObjectData que define este objeto.")]
    public ObjectData objectData;

    [Tooltip("Propiedad específica de esta instancia, como su color.")]
    public string instanceColor;

    public string instanceLocation;

#if UNITY_EDITOR
    void OnValidate()
    {
        // Only run this in the Unity Editor, not at runtime
        if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
        {
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
#endif
}
