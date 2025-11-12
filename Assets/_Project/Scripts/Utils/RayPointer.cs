using UnityEngine;

public class RayPointer : MonoBehaviour
{
    [Header("Visual Configuration")]
    [Tooltip("Ray width")]
    [Range(0.005f, 0.05f)]
    public float rayWidth = 0.015f;

    [Tooltip("Ray transparency (0 = invisible, 1 = opaque)")]
    [Range(0f, 1f)]
    public float transparency = 0.7f;

    [Header("Hand Position")]
    [Tooltip("Offset from camera to simulate hand position")]
    public Vector3 handOffset = new Vector3(0.3f, -0.3f, 0.5f);

    [Header("Object Detection")]
    [Tooltip("Maximum raycast distance")]
    public float maxDistance = 100f;

    [Tooltip("Layers that the raycast can detect")]
    public LayerMask detectableLayers = ~0; // All layers by default

    // Private references
    private LineRenderer lineRenderer;
    private RaycastHit hitInfo;
    private bool isPointingAtObject = false;

    void Start()
    {
        SetupLineRenderer();
    }

    void SetupLineRenderer()
    {
        // Create LineRenderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        // Configure basic properties
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = rayWidth;
        lineRenderer.endWidth = rayWidth * 0.7f;
        lineRenderer.useWorldSpace = true;

        // Create transparent material
        Material rayMaterial = new Material(Shader.Find("Particles/Standard Unlit"));
        rayMaterial.SetFloat("_Mode", 2); // Fade mode for transparency
        rayMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        rayMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        rayMaterial.SetInt("_ZWrite", 0);
        rayMaterial.DisableKeyword("_ALPHATEST_ON");
        rayMaterial.EnableKeyword("_ALPHABLEND_ON");
        rayMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        rayMaterial.renderQueue = 3000;

        // White color with transparency
        Color rayColor = new Color(1f, 1f, 1f, transparency);
        rayMaterial.SetColor("_Color", rayColor);

        lineRenderer.material = rayMaterial;
        lineRenderer.startColor = rayColor;
        lineRenderer.endColor = rayColor;

        // Disable shadows
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
    }

    void Update()
    {
        UpdateRay();
        UpdateTransparency();
    }

    void UpdateRay()
    {
        // 1. Calcular el origen VISUAL (virtual hand)
        Vector3 visualOrigin = transform.position +
                               transform.right * handOffset.x +
                               transform.up * handOffset.y +
                               transform.forward * handOffset.z;

        // 2. Dirección donde la cámara está apuntando
        Vector3 direction = transform.forward;

        // --- ¡EL CAMBIO CLAVE! ---
        // 3. El Raycast FÍSICO (la detección) debe salir del CENTRO de la cámara (transform.position).
        //    Esto asegura que lo que ves (WYSIWYG) es lo que golpeas.
        isPointingAtObject = Physics.Raycast(transform.position, direction, out hitInfo, maxDistance, detectableLayers);

        Vector3 endPoint;

        if (isPointingAtObject)
        {
            // Si detecta un objeto, el rayo (físico y visual) termina en el punto de colisión
            endPoint = hitInfo.point;
        }
        else
        {
            // Si no detecta nada, el rayo se extiende a la distancia máxima DESDE EL CENTRO DE LA CÁMARA
            endPoint = transform.position + (direction * maxDistance);
        }

        // 4. Actualizar las posiciones del rayo VISUAL (LineRenderer)
        //    La línea SÍ sale de la mano (visualOrigin)
        lineRenderer.SetPosition(0, visualOrigin);
        //    Y termina donde el rayo FÍSICO golpeó (endPoint)
        lineRenderer.SetPosition(1, endPoint);
    }

    void UpdateTransparency()
    {
        // Update transparency if changed in Inspector
        Color currentColor = lineRenderer.startColor;
        if (Mathf.Abs(currentColor.a - transparency) > 0.01f)
        {
            Color newColor = new Color(1f, 1f, 1f, transparency);
            lineRenderer.startColor = newColor;
            lineRenderer.endColor = newColor;
            lineRenderer.material.SetColor("_Color", newColor);
        }
    }

    // Public method to check if pointing at an object
    public bool IsPointingAtObject()
    {
        return isPointingAtObject;
    }

    // Public method to get the detected object
    public GameObject GetDetectedObject()
    {
        if (isPointingAtObject)
        {
            return hitInfo.collider.gameObject;
        }
        return null;
    }

    // Public method to get full raycast information
    public RaycastHit GetRaycastInfo()
    {
        return hitInfo;
    }

    // Public method to check if pointing at object with specific tag
    public bool IsPointingAtTag(string tag)
    {
        if (isPointingAtObject)
        {
            return hitInfo.collider.CompareTag(tag);
        }
        return false;
    }
}
