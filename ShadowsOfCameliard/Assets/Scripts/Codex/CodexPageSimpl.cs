using System;
using System.Collections;
using UnityEngine;

// -----------------------------------------------------------------------------
// CodexPageSimpl
//
// Responsabilidades:
// - Versión simplificada de CodexPage: genera la malla de página y la anima.
// - Expone el evento TurnCompleted para notificar al códice cuando el giro termina.
// - Aplica efecto de curvatura (curl) a los vértices durante el giro.
//
// Atributos principales:
// - curlAmount: intensidad del curl en los bordes.
// - turnDuration / turnCurve: duración y curva del giro.
//
// Notas:
// - El evento TurnCompleted usa Action<CodexPageSimpl> para identificar la página.
// - A diferencia de CodexPage, no tiene InkRevealController ni flipImage.
// -----------------------------------------------------------------------------
public class CodexPageSimpl : MonoBehaviour
{
    [Header("Page Settings")]
    [SerializeField] int xSegments = 40;
    [SerializeField] int ySegments = 8;
    [SerializeField] float pageWidth = 3f;
    [SerializeField] float pageHeight = 4f;

    [Header("Turn Animation")]
    [SerializeField] float turnDuration = 1.2f;
    [SerializeField] AnimationCurve turnCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Curl Effect")]
    [SerializeField] float curlAmount = 0.35f;

    [Header("Material")]
    [SerializeField] Material frontMaterial;
    [SerializeField] Material backMaterial;

    [Header("Sound Effects")]
    [SerializeField] AudioClip turnSound;

    public event Action<CodexPageSimpl> TurnCompleted;

    public float TurnDuration => turnDuration;
    public float GetPageWidth => pageWidth;
    public float GetPageHeight => pageHeight;
    public bool IsTurning => isTurning;

    Mesh mesh;
    Vector3[] originalVertices;

    bool isTurning = false;
    bool isRightToLeft = true;

    public void SetTurnDuration(float duration)
    {
        turnDuration = duration;
    }

    void Awake()
    {
        CodexSimpl codex = GetComponentInParent<CodexSimpl>();

        if (codex != null)
        {
            pageWidth = codex.Width;
            pageHeight = codex.Height;
        }

        mesh = GeneratePage(xSegments, ySegments);

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshFilter.mesh = mesh;
        }

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.materials = new Material[]
            {
                frontMaterial,
                backMaterial
            };
        }

        originalVertices = mesh.vertices;
    }

    public void TurnPage()
    {
        if (isTurning) return;

        isTurning = true;

        if (turnSound != null)
        {
            // Reproducimos el sonido usando la cámara principal
            AudioSource.PlayClipAtPoint(turnSound, Camera.main.transform.position);
        }

        StartCoroutine(TurnPageCoroutine(isRightToLeft));

        isRightToLeft = !isRightToLeft;
    }

    // -----------------------------------------------------------------------------
    // GeneratePage
    //
    // - Genera la malla procedural con dos sub-meshes (cara frontal y trasera).
    // - La cara trasera usa el orden de vértices invertido para normales correctas.
    // -----------------------------------------------------------------------------
    Mesh GeneratePage(int xSegments, int ySegments)
    {
        Mesh generatedMesh = new Mesh();

        Vector3[] vertices = new Vector3[(xSegments + 1) * (ySegments + 1)];
        Vector2[] uv = new Vector2[vertices.Length];

        int[] frontTriangles = new int[xSegments * ySegments * 6];
        int[] backTriangles = new int[xSegments * ySegments * 6];

        int v = 0;

        for (int y = 0; y <= ySegments; y++)
        {
            for (int x = 0; x <= xSegments; x++)
            {
                float xf = (float)x / xSegments;
                float yf = (float)y / ySegments;

                vertices[v] = new Vector3(
                    xf * pageWidth,
                    0f,
                    yf * pageHeight - pageHeight * 0.5f
                );

                uv[v] = new Vector2(xf, yf);

                v++;
            }
        }

        int t = 0;

        for (int y = 0; y < ySegments; y++)
        {
            for (int x = 0; x < xSegments; x++)
            {
                int i = y * (xSegments + 1) + x;

                int bottomLeft = i;
                int topLeft = i + xSegments + 1;
                int bottomRight = i + 1;
                int topRight = i + xSegments + 2;

                frontTriangles[t] = bottomLeft;
                frontTriangles[t + 1] = topLeft;
                frontTriangles[t + 2] = bottomRight;

                frontTriangles[t + 3] = bottomRight;
                frontTriangles[t + 4] = topLeft;
                frontTriangles[t + 5] = topRight;

                backTriangles[t] = bottomRight;
                backTriangles[t + 1] = topLeft;
                backTriangles[t + 2] = bottomLeft;

                backTriangles[t + 3] = topRight;
                backTriangles[t + 4] = topLeft;
                backTriangles[t + 5] = bottomRight;

                t += 6;
            }
        }

        generatedMesh.vertices = vertices;
        generatedMesh.uv = uv;
        generatedMesh.subMeshCount = 2;
        generatedMesh.SetTriangles(frontTriangles, 0);
        generatedMesh.SetTriangles(backTriangles, 1);

        generatedMesh.RecalculateNormals();
        generatedMesh.RecalculateBounds();

        return generatedMesh;
    }

    // -----------------------------------------------------------------------------
    // TurnPageCoroutine
    //
    // - Interpola la rotación local 180° usando la AnimationCurve y aplica curl.
    // - Al finalizar invoca el evento TurnCompleted para notificar al códice.
    // -----------------------------------------------------------------------------
    IEnumerator TurnPageCoroutine(bool forward = true)
    {
        Quaternion startRotation = transform.localRotation;

        float direction = forward ? -1f : 1f;
        Quaternion endRotation = startRotation * Quaternion.Euler(0f, 0f, direction * 180f);

        float elapsed = 0f;

        while (elapsed < turnDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / turnDuration);
            float curvedT = turnCurve.Evaluate(t);

            transform.localRotation = Quaternion.Slerp(startRotation, endRotation, curvedT);

            ApplyCurl(curvedT, forward);

            yield return null;
        }

        transform.localRotation = endRotation;
        ApplyCurl(1f, forward);

        isTurning = false;

        TurnCompleted?.Invoke(this);
    }

    // -----------------------------------------------------------------------------
    // ApplyCurl
    //
    // - Desplaza el eje Y de cada vértice con la misma fórmula seno doble que CodexPage.
    // -----------------------------------------------------------------------------
    void ApplyCurl(float progress, bool forward = true)
    {
        if (mesh == null || originalVertices == null) return;

        Vector3[] vertices = new Vector3[originalVertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 v = originalVertices[i];

            float normalizedX = v.x / pageWidth;

            float curl =
                Mathf.Sin(normalizedX * Mathf.PI) *
                curlAmount *
                Mathf.Sin(progress * Mathf.PI);

            curl *= forward ? -1f : 1f;

            v.y += curl;

            vertices[i] = v;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}