using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A UI component that fills its rect with a customizable gradient.
/// Attach to a GameObject with a RectTransform (e.g. a Panel). If the GameObject already has a UI Image,
/// remove or disable it (this component renders by itself).
/// </summary>
[AddComponentMenu("UI/Gradient Panel")]
[RequireComponent(typeof(CanvasRenderer))]
[ExecuteAlways]
public class GradientPanel : MaskableGraphic
{
    public enum GradientMode
    {
        TwoColors,
        FourCorners
    }

    [Tooltip("Mode used to paint the gradient.")]
    public GradientMode mode = GradientMode.TwoColors;

    [Header("Two color gradient")]
    public Color color1 = Color.white;
    public Color color2 = Color.black;

    [Tooltip("When true, angle (degrees) is used to drive the gradient direction. When false, use 'direction'.")]
    public bool useAngle = false;

    [Tooltip("Angle in degrees used when Use Angle is true (0 = left->right, 90 = bottom->top).")]
    [Range(0f, 360f)]
    public float angle = 0f;

    public enum Direction
    {
        LeftToRight,
        RightToLeft,
        BottomToTop,
        TopToBottom,
        DiagonalTLBR,
        DiagonalBLTR
    }

    [Tooltip("Predefined gradient directions (used when Use Angle is false).")]
    public Direction direction = Direction.LeftToRight;

    [Header("Four corner colors (used when Mode = FourCorners)")]
    public Color topLeft = Color.white;
    public Color topRight = Color.white;
    public Color bottomLeft = Color.black;
    public Color bottomRight = Color.black;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Rect rect = rectTransform.rect;

        // corners in local space
        Vector2 bl = new Vector2(rect.xMin, rect.yMin);
        Vector2 tl = new Vector2(rect.xMin, rect.yMax);
        Vector2 tr = new Vector2(rect.xMax, rect.yMax);
        Vector2 br = new Vector2(rect.xMax, rect.yMin);

        // normalized positions (0..1) for interpolation math
        Vector2 nBL = new Vector2(0f, 0f);
        Vector2 nTL = new Vector2(0f, 1f);
        Vector2 nTR = new Vector2(1f, 1f);
        Vector2 nBR = new Vector2(1f, 0f);

        Color32 cBL, cTL, cTR, cBR;

        if (mode == GradientMode.FourCorners)
        {
            cBL = MultiplyVertexColor(bottomLeft);
            cTL = MultiplyVertexColor(topLeft);
            cTR = MultiplyVertexColor(topRight);
            cBR = MultiplyVertexColor(bottomRight);
        }
        else
        {
            // compute direction vector
            Vector2 dirVec = GetDirectionVector();

            // find projection min/max across the unit square corners to normalize t in [0,1]
            float p0 = Vector2.Dot(nBL, dirVec);
            float p1 = Vector2.Dot(nTL, dirVec);
            float p2 = Vector2.Dot(nTR, dirVec);
            float p3 = Vector2.Dot(nBR, dirVec);
            float pMin = Mathf.Min(p0, Mathf.Min(p1, Mathf.Min(p2, p3)));
            float pMax = Mathf.Max(p0, Mathf.Max(p1, Mathf.Max(p2, p3)));
            float denom = (pMax - pMin);
            if (Mathf.Approximately(denom, 0f)) denom = 1f;

            // compute t for each corner in [0,1]
            float tBL = Mathf.Clamp01((p0 - pMin) / denom);
            float tTL = Mathf.Clamp01((p1 - pMin) / denom);
            float tTR = Mathf.Clamp01((p2 - pMin) / denom);
            float tBR = Mathf.Clamp01((p3 - pMin) / denom);

            Color colBL = Color.Lerp(color1, color2, tBL);
            Color colTL = Color.Lerp(color1, color2, tTL);
            Color colTR = Color.Lerp(color1, color2, tTR);
            Color colBR = Color.Lerp(color1, color2, tBR);

            cBL = MultiplyVertexColor(colBL);
            cTL = MultiplyVertexColor(colTL);
            cTR = MultiplyVertexColor(colTR);
            cBR = MultiplyVertexColor(colBR);
        }

        // UVs: simple full-rect mapping
        Vector2 uvBL = new Vector2(0f, 0f);
        Vector2 uvTL = new Vector2(0f, 1f);
        Vector2 uvTR = new Vector2(1f, 1f);
        Vector2 uvBR = new Vector2(1f, 0f);

        // Add vertices (order: BL, TL, TR, BR)
        vh.AddVert(new Vector3(bl.x, bl.y), cBL, uvBL);
        vh.AddVert(new Vector3(tl.x, tl.y), cTL, uvTL);
        vh.AddVert(new Vector3(tr.x, tr.y), cTR, uvTR);
        vh.AddVert(new Vector3(br.x, br.y), cBR, uvBR);

        // Two triangles
        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }

    // Helper: multiply gradient vertex color by the Graphic's color (like Image does)
    private Color32 MultiplyVertexColor(Color col)
    {
        Color final = new Color(col.r * color.r, col.g * color.g, col.b * color.b, col.a * color.a);
        return final;
    }

    // Compute a normalized direction vector for the gradient based on settings
    private Vector2 GetDirectionVector()
    {
        if (useAngle)
        {
            float rad = angle * Mathf.Deg2Rad;
            Vector2 v = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            return v.normalized;
        }

        switch (direction)
        {
            case Direction.LeftToRight: return new Vector2(1f, 0f);
            case Direction.RightToLeft: return new Vector2(-1f, 0f);
            case Direction.BottomToTop: return new Vector2(0f, 1f);
            case Direction.TopToBottom: return new Vector2(0f, -1f);
            case Direction.DiagonalTLBR: return new Vector2(1f, -1f).normalized; // top-left to bottom-right
            case Direction.DiagonalBLTR: return new Vector2(1f, 1f).normalized;  // bottom-left to top-right
            default: return new Vector2(1f, 0f);
        }
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        SetVerticesDirty();
        SetMaterialDirty();
    }

    // Preserve the Graphic.color multiplication usage in the inspector
    public override Texture mainTexture
    {
        get
        {
            // UI Graphic expects a texture reference; return white texture if none to avoid warnings
            return s_WhiteTexture;
        }
    }

    // Optionally allow changing gradient from code and forcing refresh
    public void Refresh()
    {
        SetVerticesDirty();
    }
}