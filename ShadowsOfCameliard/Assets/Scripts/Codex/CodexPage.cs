using System.Collections;
using UnityEngine;
using UnityEngine.Animations;

// -----------------------------------------------------------------------------
// CodexPage
//
// Responsabilidades:
// - Genera proceduralmente la malla de una página de libro.
// - Anima el volteo de página con una AnimationCurve.
// - Aplica un efecto de curvatura (curl) a los vértices durante el giro.
//
// Atributos principales:
// - xSegments / ySegments: resolución de la malla.
// - curlAmount: intensidad del efecto de curvatura en los bordes.
// - turnDuration / turnCurve: duración y curva de interpolación del giro.
//
// Notas:
// - Cada página usa dos sub-materiales: frente (texto) y dorso (imagen).
// - La curvatura se calcula con una onda senoidal sobre el eje X normalizado.
// - InkRevealController revela la tinta al girar la página.
// -----------------------------------------------------------------------------
public class CodexPage : MonoBehaviour
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
    [SerializeField] Material frontMaterial; // texto
    [SerializeField] bool flipFrontImage;
    [SerializeField] Material backMaterial;  // imagen
    [SerializeField] bool flipBackImage;

    [Header("Sound Effects")]
    [SerializeField] AudioClip turnSound;

    public float TurnDuration => turnDuration;

    Codex codex;

    Mesh mesh;
    Vector3[] originalVertices;
    bool isTurning = false;
    bool isRightToLeft = true;

    InkRevealController inkRevealController;

    void Start()
    {
        if (transform.parent != null)
        {
            codex = transform.parent.GetComponent<Codex>();

            if (codex != null)
            {
                pageWidth = codex.Width;
                pageHeight = codex.Height;
            }
        }

        mesh = GeneratePage(xSegments, ySegments);
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().materials = new Material[]
        {
            frontMaterial,
            backMaterial
        };

        originalVertices = mesh.vertices;


        if (flipFrontImage)
        {
            FlipImage(frontMaterial);
        }

        if (flipBackImage)
        {
            FlipImage(backMaterial);
        }

        inkRevealController = GetComponent<InkRevealController>();
    }

    void FlipImage(Material material)
    {
        if (material != null)
        {
            material.mainTextureScale = new Vector2(-1f, 1f);
            material.mainTextureOffset = new Vector2(1f, 0f);
        }
    }

    void RevealInk()
    {
        if (inkRevealController != null)
        {
            inkRevealController.Reveal();
        }
    }

    // -----------------------------------------------------------------------------
    // TurnPage
    //
    // - Inicia el giro de página si no hay uno en curso, reproduce el sonido
    //   y alterna la dirección del siguiente giro.
    // - Desencadena la revelación de tinta mediante InkRevealController.
    // -----------------------------------------------------------------------------
    public void TurnPage()
    {
        if (isTurning) return;

        isTurning = true;

        if (turnSound != null)
        {
            AudioSource.PlayClipAtPoint(turnSound, transform.position);
        }
        
        StartCoroutine(TurnPageCoroutine(isRightToLeft));
        isRightToLeft = !isRightToLeft;

        RevealInk();
    }

    // -----------------------------------------------------------------------------
    // GeneratePage
    //
    // - Genera una malla procedural con xSegments × ySegments subdivisiones.
    // - Crea los arrays de vértices, UVs y triángulos para la cara frontal y trasera.
    //
    // Notas:
    // - La cara trasera usa el mismo quad pero con orden de vértices invertido
    //   para que las normales apunten hacia el interior.
    // -----------------------------------------------------------------------------
    Mesh GeneratePage(int xSegments, int ySegments)
    {
        Mesh mesh = new Mesh();

        // array de vértices de la nueva malla
        Vector3[] vertices = new Vector3[(xSegments + 1) * (ySegments + 1)];
        // coordenadas UV para mapeo de la textura
        Vector2[] uv = new Vector2[vertices.Length];
        // índices de los 6 vértices por quad (2 triángulos) de ambas caras     
        int[] frontTriangles = new int[xSegments * ySegments * 6];
        int[] backTriangles = new int[xSegments * ySegments * 6];

        // Generar vértices y coordenadas UV
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

        // Generar los índices de los triángulos de los quads de ambas caras
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

                // Cara frontal
                frontTriangles[t] = bottomLeft;
                frontTriangles[t + 1] = topLeft;
                frontTriangles[t + 2] = bottomRight;

                frontTriangles[t + 3] = bottomRight;
                frontTriangles[t + 4] = topLeft;
                frontTriangles[t + 5] = topRight;

                // Cara trasera: mismo quad, pero orden inverso
                backTriangles[t] = bottomRight;
                backTriangles[t + 1] = topLeft;
                backTriangles[t + 2] = bottomLeft;

                backTriangles[t + 3] = topRight;
                backTriangles[t + 4] = topLeft;
                backTriangles[t + 5] = bottomRight;

                t += 6;
            }
        }

        // Asignar los datos a la malla
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.subMeshCount = 2;
        mesh.SetTriangles(frontTriangles, 0);
        mesh.SetTriangles(backTriangles, 1);

        // Recalcular normales para que la iluminación funcione correctamente
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    // -----------------------------------------------------------------------------
    // TurnPageCoroutine
    //
    // - Interpola la rotación local 180° (adelante o atrás) usando la AnimationCurve.
    // - Aplica el efecto de curl en cada frame durante la interpolación.
    // -----------------------------------------------------------------------------
    IEnumerator TurnPageCoroutine(bool forward = true)
    {
        isTurning = true;

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
    }

    // -----------------------------------------------------------------------------
    // ApplyCurl
    //
    // - Desplaza el eje Y de cada vértice con una onda seno doble para simular
    //   el doblado del papel al girar (seno en X × seno en el progreso del giro).
    // - Recalcula normales y bounds tras modificar los vértices.
    // -----------------------------------------------------------------------------
    void ApplyCurl(float progress, bool forward = true)
    {
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

    public float GetPageWidth()
    {
        return pageWidth;
    }
}
